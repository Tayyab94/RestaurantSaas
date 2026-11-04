using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.Property(x => x.ShiftNumber).HasMaxLength(200);
        builder.Property(x => x.OpeningCash).HasPrecision(18, 2);
        builder.Property(x => x.ExpectedCash).HasPrecision(18, 2);
        builder.Property(x => x.ClosingCash).HasPrecision(18, 2);
        builder.Property(x => x.CashDifference).HasPrecision(18, 2);
        builder.Property(x => x.TotalSales).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Cashier).WithMany().HasForeignKey(x => x.CashierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.ShiftNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class CashMovementConfiguration : IEntityTypeConfiguration<CashMovement>
{
    public void Configure(EntityTypeBuilder<CashMovement> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.Reference).HasMaxLength(200);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class CashDrawerLogConfiguration : IEntityTypeConfiguration<CashDrawerLog>
{
    public void Configure(EntityTypeBuilder<CashDrawerLog> builder)
    {
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class DayClosingConfiguration : IEntityTypeConfiguration<DayClosing>
{
    public void Configure(EntityTypeBuilder<DayClosing> builder)
    {
        builder.Property(x => x.ZReportNumber).HasMaxLength(200);
        builder.Property(x => x.TotalSales).HasPrecision(18, 2);
        builder.Property(x => x.TotalTax).HasPrecision(18, 2);
        builder.Property(x => x.TotalDiscount).HasPrecision(18, 2);
        builder.Property(x => x.TotalReturns).HasPrecision(18, 2);
        builder.Property(x => x.TotalCash).HasPrecision(18, 2);
        builder.Property(x => x.TotalCard).HasPrecision(18, 2);
        builder.Property(x => x.TotalOnline).HasPrecision(18, 2);
        builder.Property(x => x.TotalCredit).HasPrecision(18, 2);
        builder.Property(x => x.TotalExpenses).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.BranchId, x.BusinessDate }).IsUnique();
    }
}
