using CleanArchitecture.Application.Common.Messaging;
using System.Net;

namespace CleanArchitecture.Application.Common.Models
{
    public record Error(int Code, string Message, Dictionary<string, string>? SubErrors = default)
    {
        public Error(HttpStatusCode code, string Message, Dictionary<string, string>? SubErrors = default)
            : this(Convert.ToInt32(code), Message, SubErrors)
        {

        }
        public static Error ThrowException(Exception exception) => new(HttpStatusCode.InternalServerError, "Internal Server Error", GetSubErrorFromException(exception));
        public static Error InternalServerError => new(HttpStatusCode.InternalServerError, "Internal Server Error");
        public static Error ItemNotFound(string item) => new(HttpStatusCode.NotFound, $"{item} not Found");
        public static Error NullArgument => new(HttpStatusCode.BadRequest, "Null Argument");

        private static Dictionary<string, string> GetSubErrorFromException(Exception? exception)
        {
            Dictionary<string, string> subErrors = [];

            return MapExceptionToDictionary(exception, subErrors);
        }

        private static Dictionary<string, string> MapExceptionToDictionary(Exception? exception,
                                                                           Dictionary<string, string> subErrors)
        {
            if (exception == null)
            {
                return subErrors;
            }
            subErrors.Add(exception.Message, $"TargetSite: {exception.TargetSite},\r\nSource:{exception.Source}, \r\nStack Trace:{exception.StackTrace}");

            return MapExceptionToDictionary(exception.InnerException, subErrors);
        }

        public static implicit operator Response<bool>(Error error)
        {
            return Response.Failure(error);
        }
    }








}
