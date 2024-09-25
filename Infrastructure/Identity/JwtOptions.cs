namespace CleanArchitecture.Infrastructure.Identity
{
    public class JwtOptions
    {
        public string? Issuer { get; init; }
        public string? Audience { get; init; }
        public string? Key { get; init; }
        public string? Subject { get; init; }
        public int ExpirationInMinutes { get; init; }

    }
}
