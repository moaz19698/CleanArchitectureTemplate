namespace CleanArchitecture.Application.Common.Abstracts.Account
{
    public interface ICurrentUser
    {
        string UserId { get; }
        string Username { get; }
    }
}
