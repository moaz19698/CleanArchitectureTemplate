using CleanArchitecture.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private const string ISSUER = "Jwt:Issuer";

        private IConfiguration Configuration { get; }
        public PermissionAuthorizationHandler(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User is null)
            {
                return Task.CompletedTask;
            }
            var Issuer = Configuration.GetValue<string>(ISSUER);

            var hasPermission = context.User.Claims.Any(c => c.Type == Permissions.CLAIM_TYPE
                                                                      && c.Value == requirement.Permission
                                                                     && c.Issuer == Issuer);

            if (hasPermission)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }

}
