using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Services.ArgumentOptions;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Data;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.Constants;
using Npgsql;

namespace realTimeMessagingWebApp.Services;

public class ChatService(
    Context dbContext)
    : IChatService
{
    readonly Context _context = dbContext;

    public async Task<ServiceResult<IList<Message>>> GetPaginatedChatHistory(ChatHistoryOptions options)
    {
        if (options.EndMessageSequence < options.StartMessageSequence && options.EndMessageSequence != ServiceConstants.ChatService.EndmessageSequence)
        {
            return new ServiceResult<IList<Message>>
            {
                IsSuccess = false,
                Message = "EndMessageSequence must be greater than or equal to StartMessageSequence."
            };
        }

        if (options.EndFallBackToMaxInt || options.EndMessageSequence != ServiceConstants.ChatService.EndmessageSequence)
        {
            try
            {
                // TODO .include makes life easier but also changing postgres function to use form DTO and reutns message attachements as JSON objects
                var messages = await _context.Messages
                       .FromSqlInterpolated($@"
                       EXEC GetPaginatedChatHistory
                               {options.ChatId},
                               {options.StartMessageSequence}, 
                               {options.EndMessageSequence},
                               {options.EndFallBackToMaxInt}")
                       .Include(m => m.Attachments)
                       .ToListAsync();

                return new ServiceResult<IList<Message>>
                {
                    IsSuccess = true,
                    Data = messages
                };
            }

            catch (PostgresException ex)
            {
                return new ServiceResult<IList<Message>>
                {
                    IsSuccess = false,
                    Message = $"Postgres error occured: {ex.Message}"
                };
            }
        }

        return new ServiceResult<IList<Message>>
        {
            IsSuccess = false,
            Message = $"Need to provide an {nameof(options.EndMessageSequence)} or set {nameof(options.EndFallBackToMaxInt)} to true"
        };
        
    }

    public async Task<ServiceResult<IList<Message>>> GetTopNChatMessages(Guid chatId, int range)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.SequenceNumber)
            .Take(range)
            .Include(m => m.Attachments)
            .ToListAsync();

        return new ServiceResult<IList<Message>>
        {
            IsSuccess = true,
            Data = messages
        };
    }
        
    public async Task<ServiceResult<IList<Chat>>> GetUserChats(Guid userId)
    {
        var chats = await _context.Chats
            .Where(cc => cc.ChatMembers.Any(cm => cm.UserId == userId)).ToListAsync();

        return new ServiceResult<IList<Chat>>
        {
            IsSuccess = true,
            Data = chats
        };
    }
}
