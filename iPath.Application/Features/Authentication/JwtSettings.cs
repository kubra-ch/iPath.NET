namespace iPath.Application.Areas.Authentication;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int TokenLifetimeInMinutes { get; set; } = 60;
}
