namespace realTimeMessagingWebApp.Services.ArgumentOptions.Validators;

public static class ChatHistoryOptionsValidators
{
    public static class ChatHistoryOptionsValidator
    {
        public static bool IsValid(ChatHistoryOptions options)
            => options.ChatId != Guid.Empty
                && (options.EndMessageSequence!= -1 || options.EndFallBackToMaxInt)
                && options.StartMessageSequence >= 0
                && (options.StartMessageSequence == -1 || options.EndMessageSequence > options.StartMessageSequence);
    }
}
