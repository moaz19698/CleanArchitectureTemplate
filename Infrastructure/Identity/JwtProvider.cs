using CleanArchitecture.Application.Users.Commands.Dtos;
using CleanArchitecture.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class JwtProvider
    {
        #region Dependencies
        private JwtOptions Options { get; }
        private UserManager<ApplicationUser> UserManager { get; }
        private RoleManager<IdentityRole> RoleManager { get; }
        #endregion

        #region Constructor
        public JwtProvider(IOptions<JwtOptions> options,
                          UserManager<ApplicationUser> userManager,
                          RoleManager<IdentityRole> roleManager)
        {
            Options = options.Value;
            UserManager = userManager;
            RoleManager = roleManager;
        }
        #endregion

        #region Methods
        public async Task<TokenResponse> GenerateAsync(ApplicationUser user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(Options.ExpirationInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = await CreateClaimsAsync(user),
                Expires = expiration,
                SigningCredentials = CreateSigningCredentials(),
                Issuer = Options.Issuer,
                Audience = Options.Audience,
            };
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return new TokenResponse(token, expiration);
        }

        private SigningCredentials CreateSigningCredentials() => new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.Key!)),
                                                                        SecurityAlgorithms.HmacSha256);

        private async Task<ClaimsIdentity> CreateClaimsAsync(ApplicationUser user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);
            var permissions = await GetUserRolesPermissions(userRoles);
            var claimsIdentity = new ClaimsIdentity(new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, Options.Subject!),
                new(JwtRegisteredClaimNames.NameId, user.Id),
                new(JwtRegisteredClaimNames.Name, user.UserName!),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            });
            claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            claimsIdentity.AddClaims(permissions.Select(permission => new Claim(Permissions.CLAIM_TYPE, permission)));

            return claimsIdentity;
        }

        private async Task<IList<string>> GetUserRolesPermissions(IList<string> userRoles)
        {
            var permissions = new List<string>();

            foreach (var userRole in userRoles)
            {
                var rolePermissions = await RoleManager.GetRolePermissionsAsync(userRole);
                permissions.AddRange(rolePermissions);
            }

            return permissions;
        }
        #endregion
    }
}
