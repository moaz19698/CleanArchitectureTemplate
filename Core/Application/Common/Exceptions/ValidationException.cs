using FluentValidation.Results;

namespace CleanArchitecture.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        #region Properties
        public Dictionary<string, string> Errors { get; }
        #endregion

        #region Constructors
        public ValidationException()
           : base("One or more validation failures have occurred.")
        {
            Errors = new();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures.Select(f =>
               {
                   return new { f.PropertyName, f.ErrorMessage };
               })
                .ToDictionary(s => s.PropertyName, s => s.ErrorMessage);
        }
        #endregion

    }
}
