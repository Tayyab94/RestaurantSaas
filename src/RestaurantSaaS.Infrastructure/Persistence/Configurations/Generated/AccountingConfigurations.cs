using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class AccountMappingConfiguration : IEntityTypeConfiguration<AccountMapping>
{
    public void Configure(EntityTypeBuilder<AccountMapping> builder)
    {
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DebitAccount).WithMany().HasForeignKey(x => x.DebitAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CreditAccount).WithMany().HasForeignKey(x => x.CreditAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AccountingPeriodConfiguration : IEntityTypeConfiguration<AccountingPeriod>
{
    public void Configure(EntityTypeBuilder<AccountingPeriod> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.Property(x => x.EntryNumber).HasMaxLength(200);
        builder.Property(x => x.SourceType).HasMaxLength(50);
        builder.Property(x => x.Narration).HasMaxLength(1000);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Period).WithMany().HasForeignKey(x => x.PeriodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReversalOf).WithMany().HasForeignKey(x => x.ReversalOfId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.EntryNumber }).IsUnique();
        builder.HasIndex(x => new { x.PeriodId, x.EntryDate });
    }
}

public sealed class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.Property(x => x.Debit).HasPrecision(18, 2);
        builder.Property(x => x.Credit).HasPrecision(18, 2);
        builder.Property(x => x.Narration).HasMaxLength(1000);
        builder.HasOne(x => x.JournalEntry).WithMany().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.AccountId, x.JournalEntryId });
    }
}

public sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.AccountNumber).HasMaxLength(200);
        builder.Property(x => x.BranchName).HasMaxLength(200);
        builder.Property(x => x.OpeningBalance).HasPrecision(18, 2);
        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Debit).HasPrecision(18, 2);
        builder.Property(x => x.Credit).HasPrecision(18, 2);
        builder.Property(x => x.Reference).HasMaxLength(200);
        builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JournalEntry).WithMany().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ChequeConfiguration : IEntityTypeConfiguration<Cheque>
{
    public void Configure(EntityTypeBuilder<Cheque> builder)
    {
        builder.Property(x => x.ChequeNumber).HasMaxLength(200);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JournalEntry).WithMany().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.Status, x.ChequeDate });
    }
}

public sealed class AccountingExportConfiguration : IEntityTypeConfiguration<AccountingExport>
{
    public void Configure(EntityTypeBuilder<AccountingExport> builder)
    {
        builder.Property(x => x.ExportType).HasMaxLength(50);
        builder.Property(x => x.Format).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.FilePath).HasMaxLength(200);
    }
}
