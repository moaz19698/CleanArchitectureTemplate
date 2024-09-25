using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.Common.Messaging
{
    #region Class BaseRequest 
    public record BaseRequest<TResponse> : IBaseRequest<TResponse>
    {
        #region Properties
        [JsonIgnore]
        public string? UserName { get; set; }
        [JsonIgnore]
        public string? UserId { get; set; }
        [JsonIgnore]
        public string RequestName => this.GetType().Name;
        [JsonIgnore]
        public RequestType RequestType { get; init; }

        #endregion

        #region Constructors
        public BaseRequest()
        {
            RequestType = RequestType.BaseRequest;
        }
        #endregion
    }
    #endregion

    #region Class AppRequest
    public record AppRequest<TResponse> : BaseRequest<Response<TResponse>>
    {
        #region Constructor
        public AppRequest()
        {
            RequestType = RequestType.AppRequest;
        }
        #endregion
    }
    #endregion

    #region Class BaseCommand
    public record BaseCommand<TResponse> : AppRequest<TResponse>, IBaseCommand
    {
        #region Constructor
        public BaseCommand()
        {
            RequestType = RequestType.Command;
        }
        #endregion
    }
    #endregion

    #region Class BaseQuery
    public record BaseQuery<TResponse> : AppRequest<TResponse>, IBaseQuery
    {
        #region Constructor
        public BaseQuery()
        {
            RequestType = RequestType.Query;
        }
        #endregion
    }
    #endregion

    #region Class PagedListQuery
    public record PagedListQuery<TResponse> : BaseQuery<TResponse>, IBaseQuery
    {
        #region Properites
        public int PageIndex { get; set; } = 0;

        public int PageSize { get; set; }

        public int PagePerPages { get; set; }

        public string? OrderByPropertyName { get; set; }

        public string? SortDirection { get; set; } = "desc";
        #endregion

        #region Constructors
        public PagedListQuery()
        {
            RequestType = RequestType.PagedListQuery;
        }

        #endregion
    }
    #endregion
}