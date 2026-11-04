using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class ReturnReasonConfiguration : IEntityTypeConfiguration<ReturnReason>
{
    public void Configure(EntityTypeBuilder<ReturnReason> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class ReturnOrderConfiguration : IEntityTypeConfiguration<ReturnOrder>
{
    public void Configure(EntityTypeBuilder<ReturnOrder> builder)
    {
        builder.Property(x => x.ReturnNumber).HasMaxLength(200);
        builder.Property(x => x.ReasonNote).HasMaxLength(1000);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.RefundAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Reason).WithMany().HasForeignKey(x => x.ReasonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CreditNoteInvoice).WithMany().HasForeignKey(x => x.CreditNoteInvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.ReturnNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
{
    public void Configure(EntityTypeBuilder<ReturnItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasOne(x => x.ReturnOrder).WithMany().HasForeignKey(x => x.ReturnOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.InvoiceItem).WithMany().HasForeignKey(x => x.InvoiceItemId).OnDelete(DeleteBehavior.Restrict);
    }
}
