namespace realTimeMessagingWebApp.DTOs;

public record LoginResponseDto // might need to add user ID
{
    public bool IsSuccessful { get; init; }
    public string Message { get; init; }
    public Guid? UserId { get; init; }
    public string? AccessToken { get; init; } = null;
    public DateTime? AccessTokenExpiration { get; init; } = null;
}
