using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace x99AssessmentByTva.Infrastructure.Data.Interceptors;

public sealed class AuditableEntityInterceptor(
    IUser user,
    TimeProvider dateTime) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(
            eventData,
            result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) 
            return;

        var utcNow = dateTime.GetUtcNow();

        //INFO: Add Timestamps.
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified)
                && !entry.HasChangedOwnedEntities())
                continue;

            if (entry.State == EntityState.Added)
                entry.Entity.Created = utcNow;

            entry.Entity.LastModified = utcNow;
        }

        //INFO: Add User audit.
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified)
                && !entry.HasChangedOwnedEntities())
                continue;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = user.Id;
                entry.Entity.Created = utcNow;
            }
            entry.Entity.LastModifiedBy = user.Id;
            entry.Entity.LastModified = utcNow;
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null
            && r.TargetEntry.Metadata.IsOwned()
            && r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}
