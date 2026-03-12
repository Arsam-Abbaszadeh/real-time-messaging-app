namespace realTimeMessagingWebApp.DTOs;

public sealed record class PaginatedChatHistoryOptionsDto
{
    public Guid ChatId { get; init; }
    public int  StartMessageSequence { get; init; }
    public int EndMessageSequence { get; init; }
    public bool EndFallBackToMaxInt { get; init; }
}
