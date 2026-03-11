namespace realTimeMessagingWebApp.Services.ArgumentOptions;

public sealed record ChatHistoryOptions
{
    public Guid ChatId { get; init; }
    public int StartMessageSequence { get; init; }
    public int EndMessageSequence { get; init; } = -1;
    public bool EndFallBackToMaxInt { get; init; } = false;
}
