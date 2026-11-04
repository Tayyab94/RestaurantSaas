using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class FloorConfiguration : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class RestaurantTableConfiguration : IEntityTypeConfiguration<RestaurantTable>
{
    public void Configure(EntityTypeBuilder<RestaurantTable> builder)
    {
        builder.Property(x => x.TableNumber).HasMaxLength(20);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Shape).HasMaxLength(20);
        builder.Property(x => x.QrToken).HasMaxLength(200);
        builder.HasOne(x => x.Floor).WithMany().HasForeignKey(x => x.FloorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CurrentOrder).WithMany().HasForeignKey(x => x.CurrentOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MergedIntoTable).WithMany().HasForeignKey(x => x.MergedIntoTableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.TableNumber }).IsUnique();
    }
}

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.AdvanceAmount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Table).WithMany().HasForeignKey(x => x.TableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class WaitingListEntryConfiguration : IEntityTypeConfiguration<WaitingListEntry>
{
    public void Configure(EntityTypeBuilder<WaitingListEntry> builder)
    {
        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
    }
}

public sealed class QrOrderSessionConfiguration : IEntityTypeConfiguration<QrOrderSession>
{
    public void Configure(EntityTypeBuilder<QrOrderSession> builder)
    {
        builder.Property(x => x.SessionTokenHash).HasMaxLength(200);
        builder.Property(x => x.CustomerPhone).HasMaxLength(30);
        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.HasOne(x => x.Table).WithMany().HasForeignKey(x => x.TableId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.SessionTokenHash);
    }
}
