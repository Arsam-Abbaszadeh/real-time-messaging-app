namespace realTimeMessagingWebApp.DTOs;

public record ImageAccessDetailsDto
{
    public string ObjectKey { get; init; }
    public string PresignedUrl { get; init; }
}
