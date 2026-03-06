namespace realTimeMessagingWebApp.Services.ArgumentOptions.Validators;

public static class ChatHistoryOptionsValidators
{
    public static class ChatHistoryOptionsValidator
    {
        public static bool IsValid(ChatHistoryOptions options)
            => options.ChatId != Guid.Empty
                && (options.EndMessageIsLast || options.EndMessageSequence.HasValue)
                && (!options.EndMessageSequence.HasValue || options.EndMessageSequence >= options.StartMessageSequence);
    }
}
