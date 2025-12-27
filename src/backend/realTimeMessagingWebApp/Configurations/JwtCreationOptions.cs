namespace realTimeMessagingWebApp.Configurations;

public sealed class JwtCreationOptions
{
    public required int RefreshExpiration { get; set; }
    public required int AccessExpiration { get; set; }
}
