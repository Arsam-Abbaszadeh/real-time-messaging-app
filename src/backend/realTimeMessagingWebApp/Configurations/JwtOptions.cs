namespace realTimeMessagingWebApp.Configurations;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt:JwtOptions";

    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ClockSkewSeconds { get; init; }
}