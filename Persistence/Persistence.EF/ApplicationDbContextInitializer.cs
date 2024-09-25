using CleanArchitecture.Application.Common.Abstracts.Persistence;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CleanArchitecture.Persistence.EF
{
    public static class InitializerExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
            await initializer.InitializeAsync();
            await initializer.SeedAsync();
        }
    }
    public class ApplicationDbContextInitializer
    {
        public ILogger<ApplicationDbContextInitializer> Logger { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }
        public IApplicationDbContext DbContext { get; }
        public ApplicationDbContext Context { get; }

        public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
                                               UserManager<ApplicationUser> userManager,
                                               RoleManager<IdentityRole> roleManager,
                                               ApplicationDbContext context)
        {
            Logger = logger;
            UserManager = userManager;
            RoleManager = roleManager;
            Context = context;
            DbContext = context;
        }

        public async Task InitializeAsync()
        {
            await Context.Database.MigrateAsync();
            await Context.Database.EnsureCreatedAsync();
        }

        public async Task SeedAsync()
        {
            IdentityRole administratorRole = await AddDefaultRoles();

            await AddDefaultUser(administratorRole);

            await AddAdminRolePermissiom(administratorRole);
        }

        private async Task AddAdminRolePermissiom(IdentityRole administratorRole)
        {
            var allClaims = await RoleManager.GetClaimsAsync(administratorRole);
            var allPermission = Permissions.GetAllPermissions();

            foreach (var permission in allPermission)
            {
                if (!allClaims.Any(c => c.Type == Permissions.CLAIM_TYPE && c.Value == permission))
                {
                    await RoleManager.AddClaimAsync(administratorRole, new Claim(Permissions.CLAIM_TYPE, permission));
                }
            }
        }

        private async Task AddDefaultUser(IdentityRole administratorRole)
        {
            // Default users
            var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

            if (await UserManager.Users.AllAsync(u => u.UserName!.ToLower() != administrator.UserName.ToLower()))
            {
                await UserManager.CreateAsync(administrator, "P@ssw0rd@123");

                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await UserManager.AddToRolesAsync(administrator, [administratorRole.Name]);
                }
            }
        }

        private async Task<IdentityRole> AddDefaultRoles()
        {
            // Default roles
            var administratorRole = new IdentityRole(Roles.Administrator);

            if (!await RoleManager.Roles.AnyAsync(r => r.Name == administratorRole.Name))
            {
                await RoleManager.CreateAsync(administratorRole);
            }
            else
            {
                administratorRole = await RoleManager.Roles.FirstOrDefaultAsync(r => r.Name == administratorRole.Name);
            }

            return administratorRole!;
        }


    }
}
