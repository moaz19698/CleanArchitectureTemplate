using CleanArchitecture.Application.Common.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IRequestResponsePipeline<TRequest, TResponse>
         where TRequest : IBaseRequest<Response<TResponse>>
    {
        #region Dependencies
        public ILogger<LoggingBehaviour<TRequest, TResponse>> Logger { get; }
        #endregion

        #region Constructor
        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            Logger = logger;
        }
        #endregion

        #region Process

        public async Task<Response<TResponse>> Handle(TRequest request, MyRequestResponseHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            Logger.LogInformation("CleanArchitecture Processing request  Request: {Name} {@RequestType} {@UserId} {@UserName} {@Request}",
                request.RequestName, request.RequestType, request.UserId, request.UserName, request);

            var response = await next();

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            if (response.IsSuccess)
            {
                Logger.LogInformation("CleanArchitecture Completed Response: Request Name: {RequestName} Response:{@Response}",
                                      request.RequestName,
                                      JsonConvert.SerializeObject(response, options));
            }
            else
            {
                Logger.LogError("Completed request {RequestName} with error:{Error}",
                                request.RequestName,
                               JsonConvert.SerializeObject(response.Error));
            }

            return response;
        }
        #endregion
    }
}
