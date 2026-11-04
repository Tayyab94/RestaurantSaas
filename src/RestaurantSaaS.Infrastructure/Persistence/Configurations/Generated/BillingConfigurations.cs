using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(x => x.InvoiceNumber).HasMaxLength(200);
        builder.Property(x => x.SplitLabel).HasMaxLength(50);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.ServiceCharge).HasPrecision(18, 2);
        builder.Property(x => x.DeliveryCharge).HasPrecision(18, 2);
        builder.Property(x => x.RoundOff).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAmount).HasPrecision(18, 2);
        builder.Property(x => x.FbrInvoiceNumber).HasMaxLength(200);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OriginalInvoice).WithMany().HasForeignKey(x => x.OriginalInvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Cashier).WithMany().HasForeignKey(x => x.CashierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ConsolidatedInvoice).WithMany().HasForeignKey(x => x.ConsolidatedInvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.InvoiceNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
        builder.HasIndex(x => new { x.BranchId, x.BusinessDate, x.IssuedAt });
    }
}

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.Property(x => x.ItemName).HasMaxLength(200);
        builder.Property(x => x.VariantName).HasMaxLength(200);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxRatePercent).HasPrecision(9, 4);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class InvoiceTaxConfiguration : IEntityTypeConfiguration<InvoiceTax>
{
    public void Configure(EntityTypeBuilder<InvoiceTax> builder)
    {
        builder.Property(x => x.TaxName).HasMaxLength(200);
        builder.Property(x => x.Rate).HasPrecision(9, 4);
        builder.Property(x => x.TaxableAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TaxRate).WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.PaymentNumber).HasMaxLength(200);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.TenderedAmount).HasPrecision(18, 2);
        builder.Property(x => x.ChangeAmount).HasPrecision(18, 2);
        builder.Property(x => x.TipAmount).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceNo).HasMaxLength(200);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.PaymentNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
        builder.HasIndex(x => new { x.BranchId, x.PaidAt });
    }
}

public sealed class TipConfiguration : IEntityTypeConfiguration<Tip>
{
    public void Configure(EntityTypeBuilder<Tip> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Payment).WithMany().HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Waiter).WithMany().HasForeignKey(x => x.WaiterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TipPool).WithMany().HasForeignKey(x => x.TipPoolId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PrintLogConfiguration : IEntityTypeConfiguration<PrintLog>
{
    public void Configure(EntityTypeBuilder<PrintLog> builder)
    {
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.ReprintReason).HasMaxLength(1000);
        builder.HasOne(x => x.PrintJob).WithMany().HasForeignKey(x => x.PrintJobId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Printer).WithMany().HasForeignKey(x => x.PrinterId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PrintJobConfiguration : IEntityTypeConfiguration<PrintJob>
{
    public void Configure(EntityTypeBuilder<PrintJob> builder)
    {
        builder.Property(x => x.LastError).HasMaxLength(200);
        builder.HasOne(x => x.Printer).WithMany().HasForeignKey(x => x.PrinterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.Status, x.QueuedAt });
    }
}

public sealed class FbrSubmissionConfiguration : IEntityTypeConfiguration<FbrSubmission>
{
    public void Configure(EntityTypeBuilder<FbrSubmission> builder)
    {
        builder.Property(x => x.FbrInvoiceNumber).HasMaxLength(200);
        builder.Property(x => x.LastError).HasMaxLength(200);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.Status, x.NextRetryAt });
    }
}

public sealed class DailySalesSummaryConfiguration : IEntityTypeConfiguration<DailySalesSummary>
{
    public void Configure(EntityTypeBuilder<DailySalesSummary> builder)
    {
        builder.Property(x => x.GrossSales).HasPrecision(18, 2);
        builder.Property(x => x.Discounts).HasPrecision(18, 2);
        builder.Property(x => x.Tax).HasPrecision(18, 2);
        builder.Property(x => x.ServiceCharge).HasPrecision(18, 2);
        builder.Property(x => x.NetSales).HasPrecision(18, 2);
        builder.Property(x => x.Returns).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.BranchId, x.BusinessDate }).IsUnique();
    }
}
