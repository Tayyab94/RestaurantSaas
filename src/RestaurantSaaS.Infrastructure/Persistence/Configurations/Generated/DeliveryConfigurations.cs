using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class DeliveryAreaConfiguration : IEntityTypeConfiguration<DeliveryArea>
{
    public void Configure(EntityTypeBuilder<DeliveryArea> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.DeliveryCharge).HasPrecision(18, 2);
        builder.Property(x => x.MinOrderAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RiderConfiguration : IEntityTypeConfiguration<Rider>
{
    public void Configure(EntityTypeBuilder<Rider> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.CnicNumber).HasMaxLength(200);
        builder.Property(x => x.VehicleType).HasMaxLength(200);
        builder.Property(x => x.VehicleNumber).HasMaxLength(200);
        builder.Property(x => x.CommissionValue).HasPrecision(18, 2);
        builder.Property(x => x.CashLimit).HasPrecision(18, 2);
        builder.Property(x => x.CashInHand).HasPrecision(18, 2);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(1000);
        builder.Property(x => x.Landmark).HasMaxLength(200);
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.Property(x => x.DeliveryCharge).HasPrecision(18, 2);
        builder.Property(x => x.RiderCommission).HasPrecision(18, 2);
        builder.Property(x => x.FailReason).HasMaxLength(1000);
        builder.Property(x => x.CashToCollect).HasPrecision(18, 2);
        builder.Property(x => x.CashCollected).HasPrecision(18, 2);
        builder.Property(x => x.ThirdPartyName).HasMaxLength(200);
        builder.Property(x => x.ExternalReference).HasMaxLength(200);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DeliveryArea).WithMany().HasForeignKey(x => x.DeliveryAreaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Rider).WithMany().HasForeignKey(x => x.RiderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RiderSettlement).WithMany().HasForeignKey(x => x.RiderSettlementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.OrderId).IsUnique();
    }
}

public sealed class DeliveryStatusHistoryConfiguration : IEntityTypeConfiguration<DeliveryStatusHistory>
{
    public void Configure(EntityTypeBuilder<DeliveryStatusHistory> builder)
    {
        builder.Property(x => x.FromStatus).HasMaxLength(50);
        builder.Property(x => x.ToStatus).HasMaxLength(50);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasOne(x => x.Delivery).WithMany().HasForeignKey(x => x.DeliveryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RiderSettlementConfiguration : IEntityTypeConfiguration<RiderSettlement>
{
    public void Configure(EntityTypeBuilder<RiderSettlement> builder)
    {
        builder.Property(x => x.CashCollected).HasPrecision(18, 2);
        builder.Property(x => x.CashHandedOver).HasPrecision(18, 2);
        builder.Property(x => x.Commission).HasPrecision(18, 2);
        builder.Property(x => x.Difference).HasPrecision(18, 2);
        builder.Property(x => x.ShortageAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Rider).WithMany().HasForeignKey(x => x.RiderId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RiderEarningConfiguration : IEntityTypeConfiguration<RiderEarning>
{
    public void Configure(EntityTypeBuilder<RiderEarning> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Rider).WithMany().HasForeignKey(x => x.RiderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Delivery).WithMany().HasForeignKey(x => x.DeliveryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RiderSettlement).WithMany().HasForeignKey(x => x.RiderSettlementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayrollItem).WithMany().HasForeignKey(x => x.PayrollItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.RiderId, x.Status });
    }
}
