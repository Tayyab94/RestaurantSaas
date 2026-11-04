using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.OrderNumber).HasMaxLength(200);
        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.Property(x => x.CustomerPhone).HasMaxLength(30);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.ServiceCharge).HasPrecision(18, 2);
        builder.Property(x => x.DeliveryCharge).HasPrecision(18, 2);
        builder.Property(x => x.TipAmount).HasPrecision(18, 2);
        builder.Property(x => x.RoundOff).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAmount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CancelReasonNote).HasMaxLength(1000);
        builder.Property(x => x.PriorityReason).HasMaxLength(1000);
        builder.HasOne(x => x.Table).WithMany().HasForeignKey(x => x.TableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.QrSession).WithMany().HasForeignKey(x => x.QrSessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Waiter).WithMany().HasForeignKey(x => x.WaiterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Cashier).WithMany().HasForeignKey(x => x.CashierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CancelReason).WithMany().HasForeignKey(x => x.CancelReasonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ParentOrder).WithMany().HasForeignKey(x => x.ParentOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PriceList).WithMany().HasForeignKey(x => x.PriceListId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.OrderNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
        builder.HasIndex(x => new { x.BranchId, x.BusinessDate, x.Status });
    }
}

public sealed class OrderTableConfiguration : IEntityTypeConfiguration<OrderTable>
{
    public void Configure(EntityTypeBuilder<OrderTable> builder)
    {
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Table).WithMany().HasForeignKey(x => x.TableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.OrderId, x.TableId }).IsUnique();
    }
}

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(x => x.ItemName).HasMaxLength(200);
        builder.Property(x => x.VariantName).HasMaxLength(200);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxRatePercent).HasPrecision(9, 4);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.VoidReasonNote).HasMaxLength(1000);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Deal).WithMany().HasForeignKey(x => x.DealId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TaxRate).WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.KitchenTicket).WithMany().HasForeignKey(x => x.KitchenTicketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.VoidReason).WithMany().HasForeignKey(x => x.VoidReasonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RemakeOfOrderItem).WithMany().HasForeignKey(x => x.RemakeOfOrderItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OrderItemModifierConfiguration : IEntityTypeConfiguration<OrderItemModifier>
{
    public void Configure(EntityTypeBuilder<OrderItemModifier> builder)
    {
        builder.Property(x => x.ModifierName).HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Modifier).WithMany().HasForeignKey(x => x.ModifierId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OrderDiscountConfiguration : IEntityTypeConfiguration<OrderDiscount>
{
    public void Configure(EntityTypeBuilder<OrderDiscount> builder)
    {
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Promotion).WithMany().HasForeignKey(x => x.PromotionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Coupon).WithMany().HasForeignKey(x => x.CouponId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.Property(x => x.FromStatus).HasMaxLength(50);
        builder.Property(x => x.ToStatus).HasMaxLength(50);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OrderNoteConfiguration : IEntityTypeConfiguration<OrderNote>
{
    public void Configure(EntityTypeBuilder<OrderNote> builder)
    {
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OrderTransferConfiguration : IEntityTypeConfiguration<OrderTransfer>
{
    public void Configure(EntityTypeBuilder<OrderTransfer> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromTable).WithMany().HasForeignKey(x => x.FromTableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToTable).WithMany().HasForeignKey(x => x.ToTableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromUser).WithMany().HasForeignKey(x => x.FromUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToUser).WithMany().HasForeignKey(x => x.ToUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class DiscountPresetConfiguration : IEntityTypeConfiguration<DiscountPreset>
{
    public void Configure(EntityTypeBuilder<DiscountPreset> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.MaxValue).HasPrecision(18, 2);
    }
}

public sealed class ReasonCodeConfiguration : IEntityTypeConfiguration<ReasonCode>
{
    public void Configure(EntityTypeBuilder<ReasonCode> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.HasIndex(x => new { x.TenantId, x.ReasonType, x.Name }).IsUnique();
    }
}

public sealed class RemakeRecordConfiguration : IEntityTypeConfiguration<RemakeRecord>
{
    public void Configure(EntityTypeBuilder<RemakeRecord> builder)
    {
        builder.Property(x => x.ReasonNote).HasMaxLength(1000);
        builder.Property(x => x.CostValue).HasPrecision(18, 2);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.KitchenTicket).WithMany().HasForeignKey(x => x.KitchenTicketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.NewKitchenTicket).WithMany().HasForeignKey(x => x.NewKitchenTicketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReasonCode).WithMany().HasForeignKey(x => x.ReasonCodeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.WastageRecord).WithMany().HasForeignKey(x => x.WastageRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
        builder.HasIndex(x => new { x.BranchId, x.OccurredAt });
    }
}

public sealed class ItemAvailabilityLogConfiguration : IEntityTypeConfiguration<ItemAvailabilityLog>
{
    public void Configure(EntityTypeBuilder<ItemAvailabilityLog> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.MenuItemId, x.OccurredAt });
    }
}

public sealed class KdsScreenConfiguration : IEntityTypeConfiguration<KdsScreen>
{
    public void Configure(EntityTypeBuilder<KdsScreen> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Theme).HasMaxLength(20);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class KitchenTicketConfiguration : IEntityTypeConfiguration<KitchenTicket>
{
    public void Configure(EntityTypeBuilder<KitchenTicket> builder)
    {
        builder.Property(x => x.TicketNumber).HasMaxLength(200);
        builder.Property(x => x.TableName).HasMaxLength(200);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Waiter).WithMany().HasForeignKey(x => x.WaiterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReprintOf).WithMany().HasForeignKey(x => x.ReprintOfId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RemakeRecord).WithMany().HasForeignKey(x => x.RemakeRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.TicketNumber }).IsUnique();
    }
}

public sealed class KitchenTicketItemConfiguration : IEntityTypeConfiguration<KitchenTicketItem>
{
    public void Configure(EntityTypeBuilder<KitchenTicketItem> builder)
    {
        builder.Property(x => x.ItemName).HasMaxLength(200);
        builder.Property(x => x.VariantName).HasMaxLength(200);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.ModifiersText).HasMaxLength(1000);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.KitchenTicket).WithMany().HasForeignKey(x => x.KitchenTicketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.KitchenTicketId, x.ReadyAt });
    }
}

public sealed class KitchenPerformanceDailyConfiguration : IEntityTypeConfiguration<KitchenPerformanceDaily>
{
    public void Configure(EntityTypeBuilder<KitchenPerformanceDaily> builder)
    {
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.BusinessDate, x.StationId, x.MenuItemId }).IsUnique();
    }
}
