using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Enums;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public class GroupChatService(Context dBcontext) : IGroupChatService
    {
        private readonly Context _context = dBcontext ?? throw new InvalidOperationException($"Could not inject {nameof(Context)}");

        public async Task<ServiceResult> AddUserToGroupChat(GroupChat groupChat, Guid memberId) // expects a valid group chat and user entity
        {
            var isExistingMember = await _context.GroupChatConnectors
            .AnyAsync(gcc => gcc.GroupChatId == groupChat.GroupChatId && gcc.UserId == memberId);

            if (isExistingMember)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User {memberId} is already a member of group chat {groupChat.GroupChatName}",
                };
            }

            var areFriends = await _context.FriendShips.AnyAsync(fs =>
                (fs.UserBId == groupChat.GroupChatAdminId && memberId == fs.UserAId) ||
                (fs.UserAId == groupChat.GroupChatAdminId && memberId == fs.UserBId));

            if (areFriends)
            {
                // must use connector table for this
                var connector = new GroupChatConnector
                {
                    GroupChatConnectorId = Guid.NewGuid(),
                    GroupChatId = groupChat.GroupChatId,
                    UserId = memberId,
                    JoinDate = DateTime.UtcNow
                };

                await _context.GroupChatConnectors.AddAsync(connector);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with ID {memberId} added to group chat {groupChat.GroupChatName} successfully",
                    Data = groupChat // do we need to return the whole group chat entity here?
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with ID {memberId} is not a friend of the group chat creator and cannot be added",
                };
            }
        }

        public async Task<ServiceResult> AddUsersToGroupChat(GroupChat groupChat, ICollection<Guid> memberIds)
        {
            await _context.Entry(groupChat).Collection(gc => gc.GroupChatMembers).LoadAsync();
            
            var existingMembers = groupChat.GroupChatMembers.Select(m => m.UserId).Intersect(memberIds).ToList();
            if (existingMembers.Count != 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"Some users are already members of the group chat {groupChat.GroupChatName}",
                };
            }

            foreach (var memberId in memberIds)
            {
                var areFriends = await _context.FriendShips.AnyAsync(fs =>
                    (fs.UserBId == groupChat.GroupChatCreatorId && memberId == fs.UserAId) ||
                    (fs.UserAId == groupChat.GroupChatCreatorId && memberId == fs.UserBId));
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
                    GroupChatId = groupChat.GroupChatId,
                    UserId = memberId,
                    JoinDate = DateTime.UtcNow
                };

                await _context.GroupChatConnectors.AddAsync(connector);
            }

            await _context.SaveChangesAsync();
            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"All users added to group chat {groupChat.GroupChatName} successfully",
                Data = groupChat // nave properties not updated here
            };
        }

        public async Task<ServiceResult> ChangeGroupChatAdmin(Guid groupChat, Guid memberId)
        {
            // assumes admin auth has been done
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

        async Task<Guid> RandomlyDecideNewAdmin(Guid groupChatId, Guid excludingUserId)
        {
            var possibleAdmins = await _context.GroupChatConnectors
                .Where(gcc => gcc.GroupChatId == groupChatId && gcc.UserId != excludingUserId)
                .Select(gcc => gcc.UserId)
                .ToListAsync();

            var random = new Random();
            int index = random.Next(possibleAdmins.Count);
            return possibleAdmins[index];
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
                var nextAdminId = await RandomlyDecideNewAdmin(groupChatId, memberId);

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
                var nextAdminId = await RandomlyDecideNewAdmin(groupChatId, userId);
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

        async Task<Guid> GetGroupChatAdmin(Guid groupChatId)
            => await _context.GroupChats
                        .Where(gc => gc.GroupChatId == groupChatId)
                        .Select(gc => gc.GroupChatAdminId)
                        .FirstAsync();

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

        async Task<ServiceResult> CreateNewGroupChat(GroupChat groupChat)
        {
            // assumes many values are already set in the group chat entity
            groupChat.CreationDate = DateTime.UtcNow;
            groupChat.GroupChatAdminId = groupChat.GroupChatCreatorId;
            groupChat.GroupChatId = Guid.NewGuid();

            await _context.GroupChats.AddAsync(groupChat);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Group chat {groupChat.GroupChatName} created successfully",
                Data = groupChat // do we need to return the whole group chat entity here?
            };

        }

        public async Task<ServiceResult> CreateAndAddMembersToGroupChat(GroupChat groupChat, Guid admin, ICollection<Guid> memberIds)
        {
            var groupChatCreationResult = await CreateNewGroupChat(groupChat);
            if (!groupChatCreationResult.IsSuccess)
            {
                return groupChatCreationResult; // return the failure result
            }

            var adminChangeResult = await AssignGroupChatAdmin(groupChat.GroupChatId, admin);
            if (!adminChangeResult.IsSuccess)
            {
                return adminChangeResult;
            }


            if (memberIds.Count > 0) // idk if this will every equal 0 but just in case
            {
                var addMemeberResult = groupChat.ChatType == GroupChatType.DirectMessage
                    ? await AddUserToGroupChat(groupChat, memberIds.First())
                    : await AddUsersToGroupChat(groupChat, memberIds);

                if (!addMemeberResult.IsSuccess)
                {
                    return addMemeberResult;
                }
            }

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Group chat {groupChat.GroupChatName} created and members added successfully",
                Data = groupChat // is reference type so should 
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
            // assumes admin authentication is done elsewhere
            await _context.GroupChats
                .Where(gc => gc.GroupChatId == groupChatId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.GroupChatAdminId, newAdminId));

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {newAdminId} assigned as admin of group chat with ID {groupChatId} successfully",
            };
        }
    }
}
