using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Rider: tenant table. Part of the Delivery module. See Database Schema.</summary>
public sealed class Rider : TenantEntity
{
    public Guid? UserId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? CnicNumber { get; set; }
    public string? VehicleType { get; set; }
    public string? VehicleNumber { get; set; }
    public bool IsOwnRider { get; set; }
    public RiderCommissionType CommissionType { get; set; }
    public decimal CommissionValue { get; set; }
    public RiderPayModel PayModel { get; set; }
    public decimal? CashLimit { get; set; }
    public RiderStatus Status { get; set; }
    public decimal CashInHand { get; set; }

    public User? User { get; set; }
    public Branch? Branch { get; set; }
    public Employee? Employee { get; set; }
}
