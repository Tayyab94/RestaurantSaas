using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Promotion: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class Promotion : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public PromotionPromoType PromoType { get; set; }
    public string RulesJson { get; set; } = string.Empty;
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public string? DaysOfWeek { get; set; }
    public TimeOnly? TimeFrom { get; set; }
    public TimeOnly? TimeTo { get; set; }
    public string? BranchScope { get; set; }
    public bool IsActive { get; set; }
}
