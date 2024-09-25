using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.EF.Configurations
{
    internal static class IdentityConfiguration
    {
        internal static void ConfigureIdentity(this ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                   .ToTable("User", "Identity");
            builder.Entity<ApplicationUser>()
                .Property(e => e.FirstName);

            builder.Entity<ApplicationUser>()
               .Property(e => e.MiddleName);

            builder.Entity<ApplicationUser>()
               .Property(e => e.ThirdName);

            builder.Entity<ApplicationUser>()
               .Property(e => e.FamilyName);

            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Identity");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Identity");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Identity");
            builder.Entity<IdentityRole>().HasKey(r => r.Id);
            builder.Entity<IdentityRole>().ToTable("Role", "Identity");
            builder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Identity");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Identity");
        }
    }
}
