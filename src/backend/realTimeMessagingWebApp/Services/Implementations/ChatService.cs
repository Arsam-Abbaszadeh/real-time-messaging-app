using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Services.ArgumentOptions;
using realTimeMessagingWebApp.Services.Interfaces;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Data;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.Services.Implementations;

public class ChatService(
    Context dbContext)
    : IChatService
{
    readonly Context _context = dbContext;

    public async Task<ServiceResult<IList<Message>>> GetPaginatedChatHistory(ChatHistoryOptions options)
    {
        if (options.EndMessageSequence.HasValue && options.EndMessageSequence.Value < options.StartMessageSequence)
        {
            return new ServiceResult<IList<Message>>
            {
                IsSuccess = false,
                Message = "EndMessageSequence must be greater than or equal to StartMessageSequence."
            };
        }

        if (!(!options.EndFallBackToMaxInt && !options.EndMessageSequence.HasValue))
        {
            try
            {
                var messages = await _context.Messages
                       .FromSqlInterpolated($@"
                       EXEC GetPaginatedChatHistoryWithEndAsLast 
                               {options.ChatId},
                               {options.StartMessageSequence}, 
                               {options!.EndMessageSequence},
                               {options.EndFallBackToMaxInt}")
                       .ToListAsync();

                return new ServiceResult<IList<Message>>
                {
                    IsSuccess = true,
                    Data = messages
                };
            }

            // TODO, verify if sql call throws error on fail read and if so make catch more specific
            catch (Exception ex) // probs should be specific ef core sql exception
            {
                return new ServiceResult<IList<Message>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while fetching chat history: {ex.Message}"
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
            .ToListAsync();

        return new ServiceResult<IList<Message>>
        {
            IsSuccess = true,
            Data = messages
        };
    }
        
    public Task<ServiceResult<IList<Chat>>> GetUserChats(Guid userId)
    {
        throw new NotImplementedException();
    }
}
