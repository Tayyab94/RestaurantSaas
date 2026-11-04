using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.ContactPerson).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.WhatsApp).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.TaxNumber).HasMaxLength(200);
        builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
        builder.Property(x => x.OpeningBalance).HasPrecision(18, 2);
        builder.Property(x => x.CurrentBalance).HasPrecision(18, 2);
        builder.Property(x => x.AdvanceBalance).HasPrecision(18, 2);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.BankAccountNo).HasMaxLength(200);
        builder.Property(x => x.Rating).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class SupplierContactConfiguration : IEntityTypeConfiguration<SupplierContact>
{
    public void Configure(EntityTypeBuilder<SupplierContact> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Designation).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.WhatsApp).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SupplierItemConfiguration : IEntityTypeConfiguration<SupplierItem>
{
    public void Configure(EntityTypeBuilder<SupplierItem> builder)
    {
        builder.Property(x => x.SupplierSku).HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.MinOrderQuantity).HasPrecision(18, 3);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PurchaseUnit).WithMany().HasForeignKey(x => x.PurchaseUnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SupplierItemPriceHistoryConfiguration : IEntityTypeConfiguration<SupplierItemPriceHistory>
{
    public void Configure(EntityTypeBuilder<SupplierItemPriceHistory> builder)
    {
        builder.Property(x => x.OldPrice).HasPrecision(18, 2);
        builder.Property(x => x.NewPrice).HasPrecision(18, 2);
        builder.HasOne(x => x.SupplierItem).WithMany().HasForeignKey(x => x.SupplierItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SupplierAdvanceConfiguration : IEntityTypeConfiguration<SupplierAdvance>
{
    public void Configure(EntityTypeBuilder<SupplierAdvance> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.UsedAmount).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceNo).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SupplierPayment).WithMany().HasForeignKey(x => x.SupplierPaymentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SupplierEvaluationConfiguration : IEntityTypeConfiguration<SupplierEvaluation>
{
    public void Configure(EntityTypeBuilder<SupplierEvaluation> builder)
    {
        builder.Property(x => x.OnTimePercent).HasPrecision(9, 4);
        builder.Property(x => x.RejectionPercent).HasPrecision(9, 4);
        builder.Property(x => x.PriceScore).HasPrecision(9, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SupplierLedgerEntryConfiguration : IEntityTypeConfiguration<SupplierLedgerEntry>
{
    public void Configure(EntityTypeBuilder<SupplierLedgerEntry> builder)
    {
        builder.Property(x => x.DebitAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAfter).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.SupplierId, x.EntryDate });
    }
}

public sealed class SupplierPaymentConfiguration : IEntityTypeConfiguration<SupplierPayment>
{
    public void Configure(EntityTypeBuilder<SupplierPayment> builder)
    {
        builder.Property(x => x.PaymentNumber).HasMaxLength(200);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceNo).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GoodsReceipt).WithMany().HasForeignKey(x => x.GoodsReceiptId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.PaymentNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.Property(x => x.PoNumber).HasMaxLength(200);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SourceSuggestion).WithMany().HasForeignKey(x => x.SourceSuggestionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.PoNumber }).IsUnique();
    }
}

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.TaxRatePercent).HasPrecision(9, 4);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SupplierItem).WithMany().HasForeignKey(x => x.SupplierItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.Property(x => x.GrnNumber).HasMaxLength(200);
        builder.Property(x => x.SupplierInvoiceNo).HasMaxLength(200);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.GrnNumber }).IsUnique();
    }
}

public sealed class GoodsReceiptItemConfiguration : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.RejectedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.TaxRatePercent).HasPrecision(9, 4);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.Property(x => x.BatchNumber).HasMaxLength(200);
        builder.HasOne(x => x.GoodsReceipt).WithMany().HasForeignKey(x => x.GoodsReceiptId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SupplierItem).WithMany().HasForeignKey(x => x.SupplierItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.Property(x => x.ReturnNumber).HasMaxLength(200);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GoodsReceipt).WithMany().HasForeignKey(x => x.GoodsReceiptId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.ReturnNumber }).IsUnique();
    }
}

public sealed class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.HasOne(x => x.PurchaseReturn).WithMany().HasForeignKey(x => x.PurchaseReturnId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchId).OnDelete(DeleteBehavior.Restrict);
    }
}
