using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Identity
{
    internal static class RoleMangerExtension
    {
        internal static async Task<IList<string>> GetRolePermissionsAsync(this RoleManager<IdentityRole> roleManager, string roleName)
        {
            var permissions = new List<string>();
            var role = await roleManager.FindByNameAsync(roleName);

            if (role is not null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(role);
                permissions.AddRange(roleClaims.Select(c => c.Value).ToList());
            }

            return permissions;
        }
    }
}
