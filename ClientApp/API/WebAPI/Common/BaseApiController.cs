using CleanArchitecture.Application.Common.Messaging;
using Common.DependencyInjection.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebAPI.Common
{
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        #region Dependencies

        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetInstance<IMediator>();
        #endregion

        #region Constructors
        public BaseApiController()
        {

        }
        #endregion

        #region Properties

        #endregion

        #region Method
        [NonAction]
        public ObjectResult Result<T>(IResponse<T> response)
        {
            return response.IsSuccess
                ? Ok(response)
                : Problem(response);
        }

        public ObjectResult Problem<T>(IResponse<T> response)
        {
            return Problem($"{response.Error?.Message} {GetDetailsSubErrors(response)}",
                          $"{response.Source} Path:{HttpContext.Request.Path}/{HttpContext.Request.Method} Host:{HttpContext.Request.Host} Protocol:{HttpContext.Request.Protocol}",
                          response.StatusCode,
                          response.Error!.Message);

            static string GetDetailsSubErrors(IResponse<T> response)
            {
                string detailsSubErrors = string.Empty;

                if (response.Error != null && response.Error.SubErrors != null && response.Error.SubErrors.Count > 0)
                {
                    detailsSubErrors = $"Inner Errors: {string.Join(',', response.Error.SubErrors.Select(s => $"{s.Key} - {s.Value}").ToList())}";
                }

                return detailsSubErrors;
            }
        }


        #endregion
    }
}
