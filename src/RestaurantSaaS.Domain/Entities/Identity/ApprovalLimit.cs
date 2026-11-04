using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ApprovalLimit: tenant table. Part of the Identity module. See Database Schema.</summary>
public sealed class ApprovalLimit : TenantEntity
{
    public Guid RoleId { get; set; }
    public decimal MaxDiscountPercent { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public bool CanApproveVoid { get; set; }
    public bool CanApproveReturn { get; set; }

    public Role Role { get; set; } = null!;
}
