using CleanArchitecture.Application.Common.Abstracts;
using CleanArchitecture.Application.Common.Abstracts.Account;
using CleanArchitecture.Application.Common.Errors;
using CleanArchitecture.Application.Common.Messaging;
using CleanArchitecture.Application.Users.Commands.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Dependencies
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtProvider _jwtProvider;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrentUser CurrentUser;
        #endregion

        #region Constructor
        public IdentityService(UserManager<ApplicationUser> userManager,
                              IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
                              IAuthorizationService authorizationService,
                              JwtProvider jwtProvider,
                              ICurrentUser currentUser,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
            this.CurrentUser = currentUser;
            _roleManager = roleManager;
        }
        #endregion

        #region Mehtods
        public async Task<string?> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user?.UserName;
        }
        public async Task<UserDto?> GetCurrentUserAsync()
        {
            var user = await _userManager.FindByNameAsync(CurrentUser.Username);

            return user is not null
                ? new UserDto
                {
                    Id = Guid.Parse(user.Id),
                    Email = user.Email,
                    UserName = user.UserName,
                }
                : null;
        }
        public async Task<string?> GetUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user?.Id;
        }
        public async Task<(Response<bool> Result, string UserId)> CreateUserAsync(UserDto userDto, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                MiddleName = userDto.MiddleName,
                ThirdName = userDto.ThirdName,
                FamilyName = userDto.FamilyName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }
        public async Task<(Response<bool> Result, string UserId)> CreateUserAsync(string userName, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }
        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            IList<string> roles = [];
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            if (user is not null)
            {
                roles = await _userManager.GetRolesAsync(user);
            }

            return roles;
        }
        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);

            return user != null && await _userManager.IsInRoleAsync(user, role);
        }
        public async Task<IList<string>> GetUserRolesPermissionsAsync(string userId)
        {
            var userRoles = await GetUserRolesAsync(userId);

            return await GetRolesPermissionsAsync(userRoles);
        }
        private async Task<IList<string>> GetRolesPermissionsAsync(IList<string> roles)
        {
            var permissions = new List<string>();

            foreach (var userRole in roles)
            {
                var rolePermissions = await _roleManager.GetRolePermissionsAsync(userRole);
                permissions.AddRange(rolePermissions);
            }

            return permissions;
        }
        public async Task<bool> HasUserPermissonAsync(string userId, string permission)
        {
            var userPermission = await GetUserRolesPermissionsAsync(userId);

            return userPermission.Any(p => p == permission);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                return false;
            }

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            if (principal is null)
            {
                return false;
            }
            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;

        }
        public async Task<Response<bool>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);

            return user != null ? await DeleteUserAsync(user) : Response.Success(true);
        }
        public async Task<Response<bool>> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }
        public async Task<Response<TokenResponse>> GetTokenAsync(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);

            return appUser is null ? Response.Failure<TokenResponse>(SecurityAccessErrors.NotAuthenticatedUser) : Response.Success(await _jwtProvider.GenerateAsync(appUser));
        }
        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user is not null && await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task UpdateSecurityStampAsync()
        {
            var user = await _userManager.FindByNameAsync(CurrentUser.Username);

            if (user is null)
            {
                return;
            }
            await _userManager.UpdateSecurityStampAsync(user);

        }
        #endregion
    }
}
