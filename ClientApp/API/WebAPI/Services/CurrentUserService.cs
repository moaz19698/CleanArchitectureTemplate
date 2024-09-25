using CleanArchitecture.Application.Common.Abstracts.Account;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace CleanArchitecture.WebAPI.Services
{
    public class CurrentUser : ICurrentUser
    {
        #region Dependencies
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Properties
        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.NameId) ?? "Anonymous";
        public string Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Anonymous";
        #endregion

    }
}
