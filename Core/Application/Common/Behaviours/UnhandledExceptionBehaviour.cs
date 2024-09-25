using CleanArchitecture.Application.Common.Messaging;
using CleanArchitecture.Application.Common.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IRequestResponsePipeline<TRequest, TResponse>
          where TRequest : IBaseRequest<Response<TResponse>>
    {
        #region Dependencies
        private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;
        #endregion

        #region Constructor

        public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;

        }
        #endregion

        #region Handel
        public async Task<Response<TResponse>> Handle(TRequest request,
                                                       MyRequestResponseHandlerDelegate<TResponse> next,
                                                       CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Unhandled Exception");
            var requestName = typeof(TRequest).Name;

            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: "CleanArchitecture Request: Unhandled Exception for Request {@Name} {@Request} {@Message}", requestName, request, ex.Message);

                return Response.Failure<TResponse>(Error.ThrowException(ex));
            }
        }
        #endregion
    }
}
