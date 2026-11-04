namespace RestaurantSaaS.Domain.Common;

/// <summary>A row that belongs to one branch of a tenant.</summary>
public abstract class BranchEntity : TenantEntity
{
    public Guid BranchId { get; set; }
}
