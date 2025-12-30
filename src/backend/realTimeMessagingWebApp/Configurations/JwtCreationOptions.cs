namespace realTimeMessagingWebApp.Configurations;

public sealed class JwtCreationOptions : IConfigModel
{
    public static string SectionName => "Jwt:JwtCreationOptions";

    public required int RefreshExpiration { get; init; }
    public required int AccessExpiration { get; init; }
}
