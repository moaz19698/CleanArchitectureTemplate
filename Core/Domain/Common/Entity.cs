using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Common
{
    public abstract class Entity
    {
        #region Domain Events
        private readonly List<BaseDomainEvent> domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<BaseDomainEvent> DomainEvents => domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseDomainEvent domainEvent)
        {
            domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseDomainEvent domainEvent)
        {
            domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }
        #endregion
    }
}
