using CleanArchitecture.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? ThirdName { get; set; }
        public string? FamilyName { get; set; }
        public bool? Otpactivate { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool Gender { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "Anonymous";
        public DateTime? LastUpdatedOn { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
