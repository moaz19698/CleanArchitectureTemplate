using CleanArchitecture.Application.Common.Abstracts.Account;
using CleanArchitecture.Domain.Common;
using Common.DependencyInjection.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Abstracts.DomainEvent
{
    public abstract class BaseDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : BaseDomainEvent
    {
        #region Dependencies
        private IServiceProvider ServiceProvider { get; }
        private ICurrentUser CurrentUserService => ServiceProvider.GetInstance<ICurrentUser>();
        protected ILogger<BaseDomainEventHandler<TDomainEvent>> Logger { get; }
        #endregion

        #region Properties
        public string UserId => CurrentUserService.UserId;
        public string Username => CurrentUserService.Username ?? "Anonymous";
        #endregion

        #region Constructor
        protected BaseDomainEventHandler(ILogger<BaseDomainEventHandler<TDomainEvent>> logger,
                                      IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }
        #endregion

        #region Handle
        public Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
        {
            Logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);

            return HandleEvent(notification, cancellationToken);
        }
        public abstract Task HandleEvent(TDomainEvent notification, CancellationToken cancellationToken);
        #endregion
    }
}
