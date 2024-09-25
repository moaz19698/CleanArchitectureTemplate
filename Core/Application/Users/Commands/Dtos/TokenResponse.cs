namespace CleanArchitecture.Application.Users.Commands.Dtos
{
    public record TokenResponse
    {
        public string Token { get; init; }
        public DateTime Expiration { get; init; }

        public TokenResponse(string token, DateTime expiration)
        {
            Token = token;
            Expiration = expiration;
        }
    }
}
