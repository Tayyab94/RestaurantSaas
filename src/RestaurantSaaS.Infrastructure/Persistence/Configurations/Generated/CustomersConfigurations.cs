using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class CustomerGroupConfiguration : IEntityTypeConfiguration<CustomerGroup>
{
    public void Configure(EntityTypeBuilder<CustomerGroup> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.DiscountPercent).HasPrecision(9, 4);
        builder.HasOne(x => x.PriceList).WithMany().HasForeignKey(x => x.PriceListId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Phone2).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
        builder.Property(x => x.CurrentBalance).HasPrecision(18, 2);
        builder.Property(x => x.AdvanceBalance).HasPrecision(18, 2);
        builder.Property(x => x.TaxNumber).HasMaxLength(200);
        builder.Property(x => x.CompanyName).HasMaxLength(200);
        builder.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PriceList).WithMany().HasForeignKey(x => x.PriceListId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MergedIntoCustomer).WithMany().HasForeignKey(x => x.MergedIntoCustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.Phone }).IsUnique();
    }
}

public sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.Property(x => x.Label).HasMaxLength(50);
        builder.Property(x => x.AddressLine).HasMaxLength(500);
        builder.Property(x => x.Landmark).HasMaxLength(200);
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Designation).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CustomerLedgerEntryConfiguration : IEntityTypeConfiguration<CustomerLedgerEntry>
{
    public void Configure(EntityTypeBuilder<CustomerLedgerEntry> builder)
    {
        builder.Property(x => x.DebitAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAfter).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CustomerId, x.EntryDate });
    }
}

public sealed class CustomerAdvanceConfiguration : IEntityTypeConfiguration<CustomerAdvance>
{
    public void Configure(EntityTypeBuilder<CustomerAdvance> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.UsedAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Payment).WithMany().HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class LoyaltyProgramConfiguration : IEntityTypeConfiguration<LoyaltyProgram>
{
    public void Configure(EntityTypeBuilder<LoyaltyProgram> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.PointsPerAmount).HasPrecision(18, 3);
        builder.Property(x => x.AmountUnit).HasPrecision(18, 2);
        builder.Property(x => x.RedeemValuePerPoint).HasPrecision(18, 2);
    }
}

public sealed class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class GiftCardConfiguration : IEntityTypeConfiguration<GiftCard>
{
    public void Configure(EntityTypeBuilder<GiftCard> builder)
    {
        builder.Property(x => x.CardNumber).HasMaxLength(200);
        builder.Property(x => x.Pin).HasMaxLength(200);
        builder.Property(x => x.InitialAmount).HasPrecision(18, 2);
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.HasOne(x => x.IssuedToCustomer).WithMany().HasForeignKey(x => x.IssuedToCustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.CardNumber }).IsUnique();
    }
}

public sealed class GiftCardTransactionConfiguration : IEntityTypeConfiguration<GiftCardTransaction>
{
    public void Configure(EntityTypeBuilder<GiftCardTransaction> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.GiftCard).WithMany().HasForeignKey(x => x.GiftCardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.DaysOfWeek).HasMaxLength(20);
        builder.Property(x => x.BranchScope).HasMaxLength(50);
    }
}

public sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.MinOrder).HasPrecision(18, 2);
        builder.HasOne(x => x.Promotion).WithMany().HasForeignKey(x => x.PromotionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class CouponRedemptionConfiguration : IEntityTypeConfiguration<CouponRedemption>
{
    public void Configure(EntityTypeBuilder<CouponRedemption> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.Coupon).WithMany().HasForeignKey(x => x.CouponId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.Property(x => x.Comment).HasMaxLength(1000);
        builder.Property(x => x.Source).HasMaxLength(50);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.DiscountPercent).HasPrecision(9, 4);
        builder.HasOne(x => x.PriceList).WithMany().HasForeignKey(x => x.PriceListId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ConsolidatedInvoiceConfiguration : IEntityTypeConfiguration<ConsolidatedInvoice>
{
    public void Configure(EntityTypeBuilder<ConsolidatedInvoice> builder)
    {
        builder.Property(x => x.InvoiceNumber).HasMaxLength(200);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.InvoiceNumber }).IsUnique();
    }
}

public sealed class ConsolidatedInvoiceLineConfiguration : IEntityTypeConfiguration<ConsolidatedInvoiceLine>
{
    public void Configure(EntityTypeBuilder<ConsolidatedInvoiceLine> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.ConsolidatedInvoice).WithMany().HasForeignKey(x => x.ConsolidatedInvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CustomerMergeLogConfiguration : IEntityTypeConfiguration<CustomerMergeLog>
{
    public void Configure(EntityTypeBuilder<CustomerMergeLog> builder)
    {
        builder.HasOne(x => x.KeptCustomer).WithMany().HasForeignKey(x => x.KeptCustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MergedCustomer).WithMany().HasForeignKey(x => x.MergedCustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.Property(x => x.FileName).HasMaxLength(200);
        builder.Property(x => x.ErrorFileUrl).HasMaxLength(500);
    }
}
