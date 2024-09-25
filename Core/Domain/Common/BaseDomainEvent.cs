using MediatR;

namespace CleanArchitecture.Domain.Common
{
    public abstract record BaseDomainEvent : INotification
    {
        #region Constructor
        protected BaseDomainEvent()
        {
            DateOccurred = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties
        public bool IsPublished { get; set; }

        public DateTimeOffset DateOccurred { get; protected set; } = DateTime.UtcNow;
        #endregion
    }
}
