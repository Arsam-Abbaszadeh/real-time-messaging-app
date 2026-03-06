namespace realTimeMessagingWebApp.Services.ArgumentOptions;

public sealed record ChatHistoryOptions
{
    public Guid ChatId { get; init; }
    public uint StartMessageSequence { get; init; }
    public uint? EndMessageSequence { get; init; }

    // to get the last message instead of failing if endMessageSequence overshoots
    public bool EndFallBackToMaxInt { get; init; } = false;
    public bool EndMessageIsLast { get; init; } = false;
}
