using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppInfra.Persistence.Data.Repository;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebAppInfra.Persistence.Enums;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Data;

namespace realTimeMessagingWebApp.Services
{
    public class ChatService(
        Context dBcontext,
        ICustomRepository<Chat> customRepository,
        RelationShipService relationShipService)
        : IChatService
    {
        readonly Context _context = dBcontext ?? throw new InvalidOperationException($"Could not inject {nameof(Context)}");
        readonly ICustomRepository<Chat> _customRepository = customRepository ?? throw new InvalidOperationException($"Could not inject {nameof(ICustomRepository<Chat>)}");
        readonly RelationShipService _relationShipService = relationShipService ?? throw new InvalidOperationException($"Could not inject {nameof(RelationShipService)}");

        public async Task<ServiceResult<Chat>> AddUserToChat(Guid chatId, Guid memberId) 
            => await AddUsersToChat(chatId, [memberId]);

        public async Task<ServiceResult<Chat>> AddUsersToChat(Guid chatId, ICollection<Guid> memberIds)
        {
            var actualChat = await _customRepository.GetFullEntityAsync(_context, chatId, true, gc => gc.ChatMembers);
            if (actualChat is null)
            {
                return new ServiceResult<Chat>
                {
                    IsSuccess = false,
                    Message = $"Chat with ID {chatId} not found",
                };
            }

            var existingMembers = actualChat.ChatMembers.Select(m => m.UserId).Intersect(memberIds).ToList();
            if (existingMembers.Count != 0)
            {
                return new ServiceResult<Chat>
                {
                    IsSuccess = false,
                    Message = $"Some users are already members of the chat {actualChat.ChatName}",
                };
            }

            var areAllFriends = memberIds.Count > 1 ?
                await _relationShipService.AreAllFriendsOfUser(actualChat.ChatAdminId, memberIds) :
                await _relationShipService.AreFriends(actualChat.ChatAdminId, memberIds.First());

            if (!areAllFriends.IsSuccess)
            {
                return new ServiceResult<Chat>
                {
                    IsSuccess = false,
                    Message = "Not all users to be added are friends of the chat creator",
                    Data = null
                };
            }   

            foreach (var memberId in memberIds)
            {
                var areFriends = await _context.FriendShips.AnyAsync(fs =>
                    ((fs.UserBId == actualChat.ChatAdminId && fs.UserAId == memberId) ||
                    (fs.UserAId == actualChat.ChatAdminId && fs.UserBId == memberId)) &&
                    fs.Status == FriendShipStatus.Friends);

                if (!areFriends)
                {
                    _context.ChangeTracker.Clear(); // to avoid tracking issues

                    return new ServiceResult<Chat>
                    {
                        IsSuccess = false,
                        Message = $"User with ID {memberId} is not a friend of the chat creator and cannot be added",
                        Data = null
                    };
                }

                var connector = new ChatConnector
                {
                    ChatConnectorId = Guid.NewGuid(),
                    ChatId = chatId,
                    UserId = memberId,
                    JoinDate = DateTime.UtcNow
                };

                await _context.ChatConnectors.AddAsync(connector);
            }

            await _context.SaveChangesAsync();
            return new ServiceResult<Chat>
            {
                IsSuccess = true,
                Message = $"All users added to chat {actualChat.ChatName} successfully",
                Data = actualChat // not sure if we need to send this
            };
        }

        public async Task<ServiceResult> ChangeChatAdmin(Guid chatId, Guid memberId)
        {
            // assumes admin auth has been done

            var isMember = await UserIsChatMember(memberId, chatId);
            if (!isMember)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with ID {memberId} is not a member of the chat with ID {chatId}",
                };
            }

            var rowsAffected = await _context.Chats
                .Where(gc => gc.ChatId == chatId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.ChatAdminId, memberId));

            if (rowsAffected == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"Chat with ID {chatId} not found",
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with ID {memberId} assigned as admin of chat with ID {chatId} successfully",
                };
            }
        }

        public async Task<ServiceResult> RemoveUserFromChat(Guid chatId, Guid memberId, Guid selfId) // TODO should I be wrapping this in a try catch?
        {
            // assumes admin authentication is done elsewhere
            var countMemebers = await _context.ChatConnectors.CountAsync(gcc => gcc.ChatId == chatId);

            if (countMemebers == 1)
            {
                var deletionResult = await DeleteChat(chatId);
                if (deletionResult.IsSuccess)
                {
                    return new ServiceResult
                    {
                        IsSuccess = true,
                        Message = $"User with ID {memberId} removed from chat with ID {chatId} successfully. Chat deleted as it had no more members.",
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = $"Failed to delete chat with ID {chatId} after removing the last member.",
                    };
                }
            }

            var isSelf = memberId == selfId;
            if (isSelf)
            {
                var nextAdminId = await RandomlySelectOtherMember(chatId, memberId);

                await _context.Chats
                    .Where(gc => gc.ChatId == chatId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.ChatAdminId, nextAdminId));
            }

            await _context.ChatConnectors
                .Where(gcc => gcc.ChatId == chatId && gcc.UserId == memberId)
                .ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {memberId} removed from chat with ID {chatId} successfully",
            };
        }

        public async Task<ServiceResult> RemoveSelfFromChat(Guid chatId, Guid userId, bool? isAdmin = null)
        {
            var countMemebers = await _context.ChatConnectors.CountAsync(gcc => gcc.ChatId == chatId);

            if (countMemebers == 1)
            {
                var deletionResult = await DeleteChat(chatId);
                if (deletionResult.IsSuccess)
                {
                    return new ServiceResult
                    {
                        IsSuccess = true,
                        Message = $"User with ID {userId} removed from chat with ID {chatId} successfully. Chat deleted as it had no more members.",
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = $"Failed to delete chat with ID {chatId} after removing the last member.",
                    };
                }
            }

            isAdmin = isAdmin ?? (await GetChatAdmin(chatId)) == userId;
            if (isAdmin == true)
            {
                var nextAdminId = await RandomlySelectOtherMember(chatId, userId);
                await _context.Chats
                    .Where(gc => gc.ChatId == chatId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.ChatAdminId, nextAdminId));
            }

            await _context.ChatConnectors
                .Where(gcc => gcc.ChatId == chatId && gcc.UserId == userId)
                .ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {userId} removed from chat with ID {chatId} successfully",
            };
        }


        public async Task<ServiceResult> RemoveOtherUserFromChat(Guid chatId, Guid memberId)
        {
            // assumes admin perms and are not removing self
            var rowsAffected = await _context.ChatConnectors
                .Where(gcc => gcc.ChatId == chatId && gcc.UserId == memberId)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with ID {memberId} is not a member of the chat or has already been removed"
                };
            }

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with ID {memberId} removed from chat successfully"
            };
        }

        public async Task<ServiceResult<Chat>> CreateAndAddMembersToChat(Chat chat, Guid creator, Guid? admin, ICollection<Guid> memberIds)
        {

            if (memberIds.Count > 1 && chat.ChatKind == ChatType.DirectMessage)
            {
                return new ServiceResult<Chat>
                {
                    IsSuccess = false,
                    Message = "Direct Message chats can only have one member",
                };
            }

            var chatCreationResult = await CreateNewChat(chat, creator);
            if (!chatCreationResult.IsSuccess)
            {
                return chatCreationResult; // return the failure result
            }

            var actualAdmin = admin ?? creator;
            var adminChangeResult = await AssignChatAdmin(chat.ChatId, actualAdmin);
            if (!adminChangeResult.IsSuccess)
            {
                return new ServiceResult<Chat>
                {
                    IsSuccess = false,
                    Message = adminChangeResult.Message
                };
            }

            var trackedChat = chatCreationResult.Data!; // should not be null if result creation result was succesful

            if (memberIds.Count > 0) // idk if this will every equal 0 but just in case
            {
                var addMemeberResult = await AddUsersToChat(trackedChat.ChatId, memberIds);

                if (!addMemeberResult.IsSuccess)
                {
                    return addMemeberResult;
                }
            }

            return new ServiceResult<Chat>
            {
                IsSuccess = true,
                Message = $"Chat: {trackedChat.ChatName} created and members added successfully",
                Data = trackedChat // is reference type so should 
            };
        }

        public async Task<ServiceResult> DeleteChat(Guid chatId)
        {
            // assumes admin authentication is done elsewhere and therfore you can just delete the chat

            await _context.ChatConnectors.Where(gcc => gcc.ChatId == chatId).ExecuteDeleteAsync();
            await _context.Messages.Where(m => m.ChatId == chatId).ExecuteDeleteAsync();
            await _context.Chats.Where(gc => gc.ChatId == chatId).ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Chat with ID {chatId} deleted successfully",
            };
        }

        public async Task<ServiceResult> AssignChatAdmin(Guid chatId, Guid newAdminId)
        {

            var isMember = UserIsChatMember(newAdminId, chatId);
            if (!isMember.Result)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with Id {newAdminId} is not a member of chat with Id {chatId} and cannot be assigned as admin"
                };
            }

            // assumes admin authentication is done elsewhere
            var result = await _context.Chats // if result is greater 1 we should probs do a roll back or throw an error or something, idk even if its worth trying to do validation like though
                .Where(gc => gc.ChatId == chatId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(gc => gc.ChatAdminId, newAdminId));

            if (result == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with Id {newAdminId} was not assigned as admin of chat with Id {chatId}"
                };
            }
            else // not great logic, doesnt account for more than 
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with ID {newAdminId} assigned as admin of chat with ID {chatId} successfully",
                };
            }
        }

        public Task<ServiceResult<ICollection<Chat>>> GetUserChats(Guid userId)
        {
            throw new NotImplementedException();
        }


        #region Helpers

        async Task<ServiceResult<Chat>> CreateNewChat(Chat chat, Guid creatorId)
        {
            // assumes many values are already set in the chat entity
            chat.CreationDate = DateTime.UtcNow;
            chat.ChatAdminId = chat.ChatCreatorId;
            chat.ChatId = Guid.NewGuid();
            chat.ChatCreatorId = creatorId;
            await _context.Chats.AddAsync(chat);

            var connector = new ChatConnector
            {
                ChatConnectorId = Guid.NewGuid(),
                ChatId = chat.ChatId,
                UserId = creatorId,
                JoinDate = chat.CreationDate
            };
            await _context.ChatConnectors.AddAsync(connector);

            await _context.SaveChangesAsync();

            return new ServiceResult<Chat>
            {
                IsSuccess = true,
                Message = $"Chat {chat.ChatName} created successfully",
                Data = chat // do we need to return the whole chat entity here?
            };
        }

        async Task<Guid> GetChatAdmin(Guid chatId)
            => await _context.Chats
                        .Where(gc => gc.ChatId == chatId)
                        .Select(gc => gc.ChatAdminId)
                        .FirstAsync();

        async Task<bool> UserIsChatMember(Guid memberId, Guid chatId)
            => await _context.ChatConnectors
                .AsNoTracking()
                .AnyAsync(gcc => gcc.ChatId == chatId && gcc.UserId == memberId);

        async Task<Guid> RandomlySelectOtherMember(Guid chatId, Guid excludingUserId)
        {
            var possibleAdmins = await _context.ChatConnectors
                .Where(gcc => gcc.ChatId == chatId && gcc.UserId != excludingUserId)
                .Select(gcc => gcc.UserId)
                .ToListAsync();

            var random = new Random();
            int index = random.Next(possibleAdmins.Count);
            return possibleAdmins[index];
        }
        #endregion
    }
}
