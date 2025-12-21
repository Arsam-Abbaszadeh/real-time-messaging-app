using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppData;
using realTimeMessagingWebAppData.Repository;
using realTimeMessagingWebAppData.Entities;
using realTimeMessagingWebAppData.Enums;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public class GroupChatService(
        Context dBcontext,
        ICustomRepository<GroupChat> customRepository,
        RelationShipService relationShipService)
        : IGroupChatService
    {
        readonly Context _context = dBcontext ?? throw new InvalidOperationException($"Could not inject {nameof(Context)}");
        readonly ICustomRepository<GroupChat> _customRepository = customRepository ?? throw new InvalidOperationException($"Could not inject {nameof(ICustomRepository<GroupChat>)}");
        readonly RelationShipService _relationShipService = relationShipService ?? throw new InvalidOperationException($"Could not inject {nameof(RelationShipService)}");

        public async Task<ServiceResult> AddUserToGroupChat(Guid groupChatId, Guid memberId) 
            => await AddUsersToGroupChat(groupChatId, [memberId]);

        public async Task<ServiceResult> AddUsersToGroupChat(Guid groupChatId, ICollection<Guid> memberIds)
        {
            var actualGroupChat = await _customRepository.GetFullEntityAsync(_context, groupChatId, true, gc => gc.GroupChatMembers);
            if (actualGroupChat is null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"Group chat with ID {groupChatId} not found",
                };
            }

            var existingMembers = actualGroupChat.GroupChatMembers.Select(m => m.UserId).Intersect(memberIds).ToList();
            if (existingMembers.Count != 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"Some users are already members of the group chat {actualGroupChat.GroupChatName}",
                };
            }

            var areAllFriends = memberIds.Count > 1 ?
                await _relationShipService.AreAllFriendsOfUser(actualGroupChat.GroupChatAdminId, memberIds) :
                await _relationShipService.AreFriends(actualGroupChat.GroupChatAdminId, memberIds.First());

            if (!areAllFriends.IsSuccess)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Not all users to be added are friends of the group chat creator",
                    Data = null
                };
            }   

            foreach (var memberId in memberIds)
            {
                var areFriends = await _context.FriendShips.AnyAsync(fs =>
                    ((fs.UserBId == actualGroupChat.GroupChatAdminId && fs.UserAId == memberId) ||
                    (fs.UserAId == actualGroupChat.GroupChatAdminId && fs.UserBId == memberId)) &&
                    fs.Status == FriendShipStatus.Friends);

                if (!areFriends)
                {
                    _context.ChangeTracker.Clear(); // to avoid tracking issues

                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = $"User with ID {memberId} is not a friend of the group chat creator and cannot be added",
                        Data = null
                    };
                }

                var connector = new GroupChatConnector
                {
                    GroupChatConnectorId = Guid.NewGuid(),
                    GroupChatId = groupChatId,
                    UserId = memberId,
                    JoinDate = DateTime.UtcNow
                };

                await _context.GroupChatConnectors.AddAsync(connector);
            }

            await _context.SaveChangesAsync();
            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"All users added to group chat {actualGroupChat.GroupChatName} successfully",
                Data = actualGroupChat // not sure if we need to send this
            };
        }

        public async Task<ServiceResult> ChangeGroupChatAdmin(Guid groupChat, Guid memberId)
        {
            // assumes admin auth has been done

            var isMember = await UserIsGroupChatMember(memberId, groupChat);
            if (!isMember)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with ID {memberId} is not a member of the group chat with ID {groupChat}",
                };
            }

            var rowsAffected = await _context.GroupChats
                .Where(gc => gc.GroupChatId == groupChat)
                .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.GroupChatAdminId, memberId));

            if (rowsAffected == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"Group chat with ID {groupChat} not found",
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with ID {memberId} assigned as admin of group chat with ID {groupChat} successfully",
                };
            }
        }

        public async Task<ServiceResult> RemoveUserFromGroupChat(Guid groupChatId, Guid memberId, Guid selfId) // TODO should I be wrapping this in a try catch?
        {
            // assumes admin authentication is done elsewhere
            var countMemebers = await _context.GroupChatConnectors.CountAsync(gcc => gcc.GroupChatId == groupChatId);

            if (countMemebers == 1)
            {
                var deletionResult = await DeleteGroupChat(groupChatId);
                if (deletionResult.IsSuccess)
                {
                    return new ServiceResult
                    {
                        IsSuccess = true,
                        Message = $"User with ID {memberId} removed from group chat with ID {groupChatId} successfully. Group chat deleted as it had no more members.",
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = $"Failed to delete group chat with ID {groupChatId} after removing the last member.",
                    };
                }
            }

            var isSelf = memberId == selfId;
            if (isSelf)
            {
                var nextAdminId = await RandomlySelectOtherMember(groupChatId, memberId);

                await _context.GroupChats
                    .Where(gc => gc.GroupChatId == groupChatId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.GroupChatAdminId, nextAdminId));
            }

            await _context.GroupChatConnectors
                .Where(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == memberId)
                .ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {memberId} removed from group chat with ID {groupChatId} successfully",
            };
        }

        public async Task<ServiceResult> RemoveSelfFromGroupChat(Guid groupChatId, Guid userId, bool? isAdmin = null)
        {
            var countMemebers = await _context.GroupChatConnectors.CountAsync(gcc => gcc.GroupChatId == groupChatId);

            if (countMemebers == 1)
            {
                var deletionResult = await DeleteGroupChat(groupChatId);
                if (deletionResult.IsSuccess)
                {
                    return new ServiceResult
                    {
                        IsSuccess = true,
                        Message = $"User with ID {userId} removed from group chat with ID {groupChatId} successfully. Group chat deleted as it had no more members.",
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = $"Failed to delete group chat with ID {groupChatId} after removing the last member.",
                    };
                }
            }

            isAdmin = isAdmin ?? (await GetGroupChatAdmin(groupChatId)) == userId;
            if (isAdmin == true)
            {
                var nextAdminId = await RandomlySelectOtherMember(groupChatId, userId);
                await _context.GroupChats
                    .Where(gc => gc.GroupChatId == groupChatId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.GroupChatAdminId, nextAdminId));
            }

            await _context.GroupChatConnectors
                .Where(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == userId)
                .ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {userId} removed from group chat with ID {groupChatId} successfully",
            };
        }


        public async Task<ServiceResult> RemoveOtherUserFromGroupChat(Guid groupChatId, Guid memberId)
        {
            // assumes admin perms and are not removing self
            var rowsAffected = await _context.GroupChatConnectors
                .Where(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == memberId)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with ID {memberId} is not a member of the group chat or has already been removed"
                };
            }

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {memberId} removed from group chat successfully"
            };
        }

        public async Task<ServiceResult> CreateAndAddMembersToGroupChat(GroupChat groupChat, Guid creator, Guid? admin, ICollection<Guid> memberIds)
        {

            if (memberIds.Count > 1 && groupChat.ChatType == GroupChatType.DirectMessage)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Direct Message chats can only have one member",
                };
            }

            var groupChatCreationResult = await CreateNewGroupChat(groupChat, creator);
            if (!groupChatCreationResult.IsSuccess)
            {
                return groupChatCreationResult; // return the failure result
            }

            var actualAdmin = admin ?? creator;
            var adminChangeResult = await AssignGroupChatAdmin(groupChat.GroupChatId, actualAdmin);
            if (!adminChangeResult.IsSuccess)
            {
                return adminChangeResult;
            }

            var trackedGroupChar = (GroupChat)groupChatCreationResult.Data!; // should not be null if result creation result was succesful

            if (memberIds.Count > 0) // idk if this will every equal 0 but just in case
            {
                var addMemeberResult = await AddUsersToGroupChat(trackedGroupChar.GroupChatId, memberIds);

                if (!addMemeberResult.IsSuccess)
                {
                    return addMemeberResult;
                }
            }

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Group chat: {trackedGroupChar.GroupChatName} created and members added successfully",
                Data = trackedGroupChar // is reference type so should 
            };
        }

        public async Task<ServiceResult> DeleteGroupChat(Guid groupChatId)
        {
            // assumes admin authentication is done elsewhere and therfore you can just delete the group chat

            await _context.GroupChatConnectors.Where(gcc => gcc.GroupChatId == groupChatId).ExecuteDeleteAsync();
            await _context.Messages.Where(m => m.GroupChatId == groupChatId).ExecuteDeleteAsync();
            await _context.GroupChats.Where(gc => gc.GroupChatId == groupChatId).ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Group chat with ID {groupChatId} deleted successfully",
            };
        }

        public async Task<ServiceResult> AssignGroupChatAdmin(Guid groupChatId, Guid newAdminId)
        {

            var isMember = UserIsGroupChatMember(newAdminId, groupChatId);
            if (!isMember.Result)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with Id {newAdminId} is not a member of group chat with Id {groupChatId} and cannot be assigned as admin"
                };
            }

            // assumes admin authentication is done elsewhere
            var result = await _context.GroupChats // if result is greater 1 we should probs do a roll back or throw an error or something, idk even if its worth trying to do validation like though
                .Where(gc => gc.GroupChatId == groupChatId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.GroupChatAdminId, newAdminId));

            if (result == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with Id {newAdminId} was not assigned as admin of group chat with Id {groupChatId}"
                };
            }
            else // not great logic, doesnt account for more than 
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with ID {newAdminId} assigned as admin of group chat with ID {groupChatId} successfully",
                };
            }
        }

        #region Helpers

        async Task<ServiceResult> CreateNewGroupChat(GroupChat groupChat, Guid creatorId)
        {
            // assumes many values are already set in the group chat entity
            groupChat.CreationDate = DateTime.UtcNow;
            groupChat.GroupChatAdminId = groupChat.GroupChatCreatorId;
            groupChat.GroupChatId = Guid.NewGuid();
            groupChat.GroupChatCreatorId = creatorId;
            await _context.GroupChats.AddAsync(groupChat);

            var connector = new GroupChatConnector
            {
                GroupChatConnectorId = Guid.NewGuid(),
                GroupChatId = groupChat.GroupChatId,
                UserId = creatorId,
                JoinDate = groupChat.CreationDate
            };
            await _context.GroupChatConnectors.AddAsync(connector);

            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Group chat {groupChat.GroupChatName} created successfully",
                Data = groupChat // do we need to return the whole group chat entity here?
            };
        }

        async Task<Guid> GetGroupChatAdmin(Guid groupChatId)
            => await _context.GroupChats
                        .Where(gc => gc.GroupChatId == groupChatId)
                        .Select(gc => gc.GroupChatAdminId)
                        .FirstAsync();

        async Task<bool> UserIsGroupChatMember(Guid memberId, Guid groupChatId)
            => await _context.GroupChatConnectors
                .AsNoTracking()
                .AnyAsync(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == memberId);

        async Task<Guid> RandomlySelectOtherMember(Guid groupChatId, Guid excludingUserId)
        {
            var possibleAdmins = await _context.GroupChatConnectors
                .Where(gcc => gcc.GroupChatId == groupChatId && gcc.UserId != excludingUserId)
                .Select(gcc => gcc.UserId)
                .ToListAsync();

            var random = new Random();
            int index = random.Next(possibleAdmins.Count);
            return possibleAdmins[index];
        }


        # endregion
    }
}
