using CleanArchitecture.Application.Common.Abstracts;
using CleanArchitecture.Application.Common.Errors;
using CleanArchitecture.Application.Common.Messaging;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using System.Reflection;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IRequestResponsePipeline<TRequest, TResponse>
        where TRequest : IBaseRequest<Response<TResponse>>
    {
        #region Dependencies
        private IIdentityService IdentityService { get; }
        #endregion

        #region Constructor
        public AuthorizationBehaviour(IIdentityService identityService)
        {
            IdentityService = identityService;
        }
        #endregion

        #region Handle
        public async Task<Response<TResponse>> Handle(TRequest request, MyRequestResponseHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (authorizeAttributes.Any())
            {
                // Must be authenticated user
                var user = await IdentityService.GetCurrentUserAsync();

                if (user is null)
                {
                    return Response.Failure<TResponse>(SecurityAccessErrors.NotAuthenticatedUser);
                }

                // Role-based authorization
                var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

                if (authorizeAttributesWithRoles.Any())
                {
                    var authorized = false;

                    foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (var role in roles)
                        {
                            var isInRole = await IdentityService.IsInRoleAsync(user.Id.ToString(), role.Trim());

                            if (isInRole)
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }

                    // Must be a member of at least one role in roles
                    if (!authorized)
                    {
                        return Response.Failure<TResponse>(SecurityAccessErrors.ForbiddenAccess);
                    }
                }

                // Policy-based authorization
                var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

                if (authorizeAttributesWithPolicies.Any())
                {
                    foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        var IsPermission = policy.Contains(Permissions.CLAIM_TYPE);

                        if (IsPermission)
                        {
                            var IsUserHasPermission = await IdentityService.HasUserPermissonAsync(user.Id.ToString(), policy);

                            if (!IsUserHasPermission)
                            {
                                return Response.Failure<TResponse>(SecurityAccessErrors.ForbiddenAccess);
                            }
                        }
                        else
                        {
                            var authorized = await IdentityService.AuthorizeAsync(user.Id.ToString(), policy);

                            if (!authorized)
                            {
                                return Response.Failure<TResponse>(SecurityAccessErrors.ForbiddenAccess);
                            }
                        }
                    }
                }
            }

            // User is authorized / authorization not required
            return await next();
        }
        #endregion
    }
}
