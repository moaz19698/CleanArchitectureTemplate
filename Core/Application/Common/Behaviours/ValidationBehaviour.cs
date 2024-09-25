using CleanArchitecture.Application.Common.Errors;
using CleanArchitecture.Application.Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IRequestResponsePipeline<TRequest, TResponse>
          where TRequest : IBaseRequest<Response<TResponse>>
    {
        #region Dependencies
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        #endregion

        #region Constructors
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        #endregion

        #region Handle
        public async Task<Response<TResponse>> Handle(TRequest request,
                                                         MyRequestResponseHandlerDelegate<TResponse> next,
                                                       CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    return Response.Failure<TResponse>(ValidationErrors.FluentValidationErrors(ConvertFailuresToDictionary(failures)),
                                                       $"at {request.GetType().AssemblyQualifiedName}");
                }
            }
            return await next();
        }

        private static Dictionary<string, string> ConvertFailuresToDictionary(List<ValidationFailure> failures) =>
            failures.GroupJoin(failures,
                               propertyName => propertyName.PropertyName,
                               errorMessage => errorMessage.PropertyName,
                               (propertyName, errorMessages) =>
                               new
                               {
                                   propertyName.PropertyName,
                                   errorMessage = errorMessages.Select(s => s.ErrorMessage)
                               })
                    .DistinctBy(s => s.PropertyName)
                    .ToDictionary(s => $"[{s.PropertyName}]:", s => $"{{{string.Join(',', s.errorMessage)}}}");
        #endregion
    }
}
