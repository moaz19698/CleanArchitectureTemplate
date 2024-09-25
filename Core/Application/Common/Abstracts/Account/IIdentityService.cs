using CleanArchitecture.Application.Common.Messaging;
using CleanArchitecture.Application.Users.Commands.Dtos;

namespace CleanArchitecture.Application.Common.Abstracts
{
    public interface IIdentityService
    {
        Task<string?> GetUserNameAsync(string userId);
        Task<string?> GetUserAsync(string userName);
        Task<UserDto?> GetCurrentUserAsync();
        Task<(Response<bool> Result, string UserId)> CreateUserAsync(string userName, string email, string password);
        Task<(Response<bool> Result, string UserId)> CreateUserAsync(UserDto userDto, string password);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<string>> GetUserRolesPermissionsAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> HasUserPermissonAsync(string userId, string permission);
        Task<Response<TokenResponse>> GetTokenAsync(string userName);
        Task<bool> AuthorizeAsync(string userId, string policyName);
        Task<bool> CheckPasswordAsync(string userName, string password);
        Task<Response<bool>> DeleteUserAsync(string userId);
        Task UpdateSecurityStampAsync();
    }
}
