using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Plan: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public int MaxBranches { get; set; }
    public int MaxUsers { get; set; }
    public int MaxDevices { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
