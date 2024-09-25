using CleanArchitecture.Application.Common.Abstracts.Account;
using CleanArchitecture.Domain.Common;
using Common.SystemTypes.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Persistence.EF.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        #region Dependencies
        private ICurrentUser CurrentUserService { get; }
        #endregion

        #region Constructor
        public AuditableEntityInterceptor(ICurrentUser currentUser)
        {
            CurrentUserService = currentUser;
        }
        #endregion

        #region Save Changes
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntitiesShadowProperties(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntitiesShadowProperties(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        #endregion

        private void UpdateEntitiesShadowProperties(DbContext? context)
        {
            if (context == null) return;

            context.ChangeTracker.Entries<AuditableEntity>()
                              .Where(e => e.State is EntityState.Added or EntityState.Modified || e.HasChangedOwnedEntities())
                              .ToList()
                              .ForEach(entry => UpdateEntityShadowProperties(entry));
        }

        private void UpdateEntityShadowProperties(EntityEntry<AuditableEntity> entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = CurrentUserService.UserId;
                    entry.Entity.CreatedOn = DateTime.Now;
                    const string IdProperty = "Id";

                    if (entry.Entity.GetType().GetProperty(IdProperty) != null)
                    {
                        SetNewGuidEntityId(IdProperty, entry);
                    }
                    break;
                case EntityState.Modified:
                    entry.Entity.LastUpdatedBy = CurrentUserService.UserId;
                    entry.Entity.LastUpdatedOn = DateTime.Now;
                    break;
            }
        }

        private static void SetNewGuidEntityId(string IdProperty, EntityEntry<AuditableEntity> entry)
        {
            if (Guid.TryParse(entry.Property(IdProperty).CurrentValue?.ToString(), out Guid id) && id == Guid.Empty)
            {
                entry.Property(IdProperty).CurrentValue = Guid.NewGuid().AsSequentialGuid();
            }
        }
    }
    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
