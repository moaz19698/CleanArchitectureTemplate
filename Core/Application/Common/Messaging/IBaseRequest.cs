using MediatR;

namespace CleanArchitecture.Application.Common.Messaging
{
    public enum RequestType
    {
        BaseRequest,
        AppRequest,
        Command,
        Query,
        PagedListQuery

    }
    public interface IBaseRequest<out TResponse> : IRequest<TResponse>
    {
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string RequestName => this.GetType().Name;
        public RequestType RequestType { get; protected init; }
    }

    public interface IBaseCommand
    {
    }

    public interface IBaseQuery
    {

    }
}
