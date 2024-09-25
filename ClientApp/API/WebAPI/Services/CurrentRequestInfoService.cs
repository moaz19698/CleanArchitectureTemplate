using CleanArchitecture.Application.Common.Abstracts.ClinetInfo;

namespace CleanArchitecture.WebAPI.Services
{
    public class CurrentRequestInfoService : ICurrentRequestInfoService
    {
        #region Dependencies
        private IHttpContextAccessor HttpAccessor { get; set; }
        public HttpRequest? CurrentRequest { get; set; }
        #endregion

        #region Constructor
        public CurrentRequestInfoService(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpAccessor = httpContextAccessor;
            this.CurrentRequest = HttpAccessor.HttpContext?.Request;
            this.Host = CurrentRequest?.Host.Host;
            this.Method = CurrentRequest?.Method;
            this.Path = CurrentRequest?.Path;
            this.PathBase = CurrentRequest?.PathBase.ToString();
            this.Protocol = CurrentRequest?.Protocol;
        }
        #endregion

        #region Properties
        public string? Host { get; init; }
        public string? Method { get; init; }
        public string? Path { get; init; }
        public string? PathBase { get; init; }
        public string? Protocol { get; init; }
        #endregion
    }
}
