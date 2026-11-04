using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PaymentMethod: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class PaymentMethod : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public PaymentMethodMethodType MethodType { get; set; }
    public bool OpensDrawer { get; set; }
    public bool RequiresReference { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
