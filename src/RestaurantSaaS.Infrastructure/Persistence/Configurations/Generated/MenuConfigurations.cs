using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.ColorHex).HasMaxLength(9);
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.HasOne(x => x.Printer).WithMany().HasForeignKey(x => x.PrinterId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.KitchenName).HasMaxLength(200);
        builder.Property(x => x.KitchenNameUrdu).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.BasePrice).HasPrecision(18, 2);
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.Property(x => x.Barcode).HasMaxLength(200);
        builder.Property(x => x.ColorHex).HasMaxLength(9);
        builder.Property(x => x.TargetFoodCostPercent).HasPrecision(9, 4);
        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TaxRate).WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.StockItem).WithMany().HasForeignKey(x => x.StockItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class ItemVariantConfiguration : IEntityTypeConfiguration<ItemVariant>
{
    public void Configure(EntityTypeBuilder<ItemVariant> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.Property(x => x.Sku).HasMaxLength(200);
        builder.Property(x => x.Barcode).HasMaxLength(200);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
{
    public void Configure(EntityTypeBuilder<ModifierGroup> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class ModifierConfiguration : IEntityTypeConfiguration<Modifier>
{
    public void Configure(EntityTypeBuilder<Modifier> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.HasOne(x => x.ModifierGroup).WithMany().HasForeignKey(x => x.ModifierGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ItemModifierGroupConfiguration : IEntityTypeConfiguration<ItemModifierGroup>
{
    public void Configure(EntityTypeBuilder<ItemModifierGroup> builder)
    {
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ModifierGroup).WithMany().HasForeignKey(x => x.ModifierGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.MenuItemId, x.ModifierGroupId }).IsUnique();
    }
}

public sealed class BranchMenuItemConfiguration : IEntityTypeConfiguration<BranchMenuItem>
{
    public void Configure(EntityTypeBuilder<BranchMenuItem> builder)
    {
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.OutReason).HasMaxLength(1000);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.MenuItemId });
    }
}

public sealed class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.DaysOfWeek).HasMaxLength(20);
    }
}

public sealed class DealItemConfiguration : IEntityTypeConfiguration<DealItem>
{
    public void Configure(EntityTypeBuilder<DealItem> builder)
    {
        builder.Property(x => x.ChoiceGroupName).HasMaxLength(200);
        builder.Property(x => x.ExtraPrice).HasPrecision(18, 2);
        builder.HasOne(x => x.Deal).WithMany().HasForeignKey(x => x.DealId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ItemBarcodeConfiguration : IEntityTypeConfiguration<ItemBarcode>
{
    public void Configure(EntityTypeBuilder<ItemBarcode> builder)
    {
        builder.Property(x => x.Barcode).HasMaxLength(200);
        builder.Property(x => x.PackSize).HasPrecision(18, 3);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Barcode }).IsUnique();
    }
}

public sealed class MenuItemPriceHistoryConfiguration : IEntityTypeConfiguration<MenuItemPriceHistory>
{
    public void Configure(EntityTypeBuilder<MenuItemPriceHistory> builder)
    {
        builder.Property(x => x.OldPrice).HasPrecision(18, 2);
        builder.Property(x => x.NewPrice).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AllergenConfiguration : IEntityTypeConfiguration<Allergen>
{
    public void Configure(EntityTypeBuilder<Allergen> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.IconKey).HasMaxLength(50);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class MenuItemAllergenConfiguration : IEntityTypeConfiguration<MenuItemAllergen>
{
    public void Configure(EntityTypeBuilder<MenuItemAllergen> builder)
    {
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Allergen).WithMany().HasForeignKey(x => x.AllergenId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.MenuItemId, x.AllergenId }).IsUnique();
    }
}

public sealed class RawMaterialAllergenConfiguration : IEntityTypeConfiguration<RawMaterialAllergen>
{
    public void Configure(EntityTypeBuilder<RawMaterialAllergen> builder)
    {
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Allergen).WithMany().HasForeignKey(x => x.AllergenId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.RawMaterialId, x.AllergenId }).IsUnique();
    }
}
