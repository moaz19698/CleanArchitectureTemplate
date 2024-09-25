namespace CleanArchitecture.Domain.Common
{
    public abstract class AuditableEntity : Entity, IAuditableEntity
    {
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = "Anonymous";

        public DateTime? LastUpdatedOn { get; set; }

        public string? LastUpdatedBy { get; set; }

    }
}
