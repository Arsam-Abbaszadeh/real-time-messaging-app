namespace realTimeMessagingWebApp.DTOs;

public record ChatSummaryDto
{
    public string ChatName { get; init; } = string.Empty;
    public DateTime ChatCreationDate { get; init; }
    public string ChatImageUrl { get; init; } = string.Empty;

    public Guid ChatId { get; init; }
}
