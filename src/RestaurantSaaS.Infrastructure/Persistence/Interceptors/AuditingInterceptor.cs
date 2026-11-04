using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Runs on every SaveChanges: fills CreatedAt/UpdatedAt and the user columns, turns Delete into soft delete,
/// stamps the tenant on new rows and refuses a row that belongs to another tenant.
/// </summary>
public sealed class AuditingInterceptor(IClock clock, ICurrentUser currentUser, ITenantProvider tenantProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = clock.UtcNow;
        var userId = currentUser.UserId;
        var tenantId = tenantProvider.TenantId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    StampTenant(entry.Entity, tenantId);
                    break;

                case EntityState.Modified:
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }

    private static void StampTenant(BaseEntity entity, Guid? tenantId)
    {
        if (entity is not TenantEntity tenantEntity)
        {
            return;
        }

        if (tenantEntity.TenantId == Guid.Empty)
        {
            tenantEntity.TenantId = tenantId
                ?? throw new InvalidOperationException("A tenant-owned row cannot be saved without a current tenant.");
        }
        else if (tenantId.HasValue && tenantEntity.TenantId != tenantId.Value)
        {
            throw new InvalidOperationException("A row of another tenant cannot be saved in this tenant's context.");
        }
    }
}
