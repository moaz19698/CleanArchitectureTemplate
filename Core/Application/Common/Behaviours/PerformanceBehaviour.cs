using CleanArchitecture.Application.Common.Messaging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IRequestResponsePipeline<TRequest, TResponse>
        where TRequest : IBaseRequest<Response<TResponse>>
    {

        #region Dependencies
        private Stopwatch Timer { get; }
        private ILogger<PerformanceBehaviour<TRequest, TResponse>> Logger { get; }
        #endregion

        #region Constructor
        public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
        {
            Timer = new Stopwatch();
            Logger = logger;
        }
        #endregion

        #region Handle
        public async Task<Response<TResponse>> Handle(TRequest request,
                                                 MyRequestResponseHandlerDelegate<TResponse> next,
                                                 CancellationToken cancellationToken)
        {
            Timer.Start();

            var response = await next();

            Timer.Stop();

            var elapsedMilliseconds = Timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                Logger.LogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                    request.RequestType, elapsedMilliseconds, request.UserId, request.UserName, request);
            }
            Logger.LogInformation("request {Request} Process Take Time: {ElapsedMilliseconds}", request.RequestName, elapsedMilliseconds);

            return response;
        }
        #endregion
    }
}
