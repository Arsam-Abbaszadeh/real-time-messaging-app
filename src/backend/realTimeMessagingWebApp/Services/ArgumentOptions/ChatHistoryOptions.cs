namespace realTimeMessagingWebApp.Services.ArgumentOptions;

public sealed record ChatHistoryOptions
{
    public Guid ChatId { get; init; }
    public int StartMessageSequence { get; init; }
    public int EndMessageSequence { get; init; } = -1;

    // to get the last message instead of failing if endMessageSequence overshoots
    public bool EndFallBackToMaxInt { get; init; } = false;
}
