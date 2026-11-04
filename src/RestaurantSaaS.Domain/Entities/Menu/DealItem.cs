using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DealItem: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class DealItem : TenantEntity
{
    public Guid DealId { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public int Quantity { get; set; }
    public bool IsChoiceGroup { get; set; }
    public string? ChoiceGroupName { get; set; }
    public decimal ExtraPrice { get; set; }

    public Deal Deal { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
}
