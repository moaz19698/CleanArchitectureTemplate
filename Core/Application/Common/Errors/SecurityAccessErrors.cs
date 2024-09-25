using CleanArchitecture.Application.Common.Models;
using System.Net;

namespace CleanArchitecture.Application.Common.Errors
{
    public static class SecurityAccessErrors
    {
        public static Error NotAuthenticatedUser => new(HttpStatusCode.Unauthorized, "Invaild user");
        public static Error ForbiddenAccess => new(HttpStatusCode.Forbidden, "Forbidden Access");
        public static Error CreationUserFailed(Dictionary<string, string>? subErrors) => new(HttpStatusCode.Conflict, "User can't be created", subErrors);
    }


}
