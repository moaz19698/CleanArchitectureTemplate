using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Common.Abstracts.DomainEvent
{
    public interface IDomainEventService
    {
        Task Publish(BaseDomainEvent domainEvent);
    }
}
