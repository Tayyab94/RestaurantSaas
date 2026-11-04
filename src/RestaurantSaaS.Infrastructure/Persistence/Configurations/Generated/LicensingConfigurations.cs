using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class LicenseSigningKeyConfiguration : IEntityTypeConfiguration<LicenseSigningKey>
{
    public void Configure(EntityTypeBuilder<LicenseSigningKey> builder)
    {
        builder.Property(x => x.KeyId).HasMaxLength(50);
        builder.Property(x => x.PrivateKeyVaultRef).HasMaxLength(200);
        builder.HasIndex(x => x.KeyId).IsUnique();
    }
}

public sealed class LicenseBatchConfiguration : IEntityTypeConfiguration<LicenseBatch>
{
    public void Configure(EntityTypeBuilder<LicenseBatch> builder)
    {
        builder.Property(x => x.BatchNumber).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.BatchNumber).IsUnique();
    }
}

public sealed class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.Property(x => x.KeyHash).HasMaxLength(200);
        builder.Property(x => x.KeyHint).HasMaxLength(10);
        builder.Property(x => x.KeyChecksum).HasMaxLength(20);
        builder.Property(x => x.LicensedUserName).HasMaxLength(200);
        builder.Property(x => x.LicensedPhone).HasMaxLength(30);
        builder.Property(x => x.RevokeReason).HasMaxLength(1000);
        builder.Property(x => x.SoldPrice).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReplacesLicense).WithMany().HasForeignKey(x => x.ReplacesLicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.KeyHash).IsUnique();
    }
}

public sealed class LicenseActivationConfiguration : IEntityTypeConfiguration<LicenseActivation>
{
    public void Configure(EntityTypeBuilder<LicenseActivation> builder)
    {
        builder.Property(x => x.MachineFingerprintHash).HasMaxLength(200);
        builder.Property(x => x.MachineName).HasMaxLength(200);
        builder.Property(x => x.OsVersion).HasMaxLength(200);
        builder.Property(x => x.AppVersion).HasMaxLength(50);
        builder.Property(x => x.ActivatedUserName).HasMaxLength(200);
        builder.Property(x => x.LastIpAddress).HasMaxLength(200);
        builder.Property(x => x.IssuedSignatureHash).HasMaxLength(200);
        builder.Property(x => x.IssuedKeyId).HasMaxLength(50);
        builder.Property(x => x.DeactivationReason).HasMaxLength(1000);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ActivatedByUser).WithMany().HasForeignKey(x => x.ActivatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.MachineFingerprintHash);
    }
}

public sealed class LicenseTransferConfiguration : IEntityTypeConfiguration<LicenseTransfer>
{
    public void Configure(EntityTypeBuilder<LicenseTransfer> builder)
    {
        builder.Property(x => x.ToFingerprintHash).HasMaxLength(200);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromActivation).WithMany().HasForeignKey(x => x.FromActivationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class TrialRegistryConfiguration : IEntityTypeConfiguration<TrialRegistry>
{
    public void Configure(EntityTypeBuilder<TrialRegistry> builder)
    {
        builder.Property(x => x.MachineFingerprintHash).HasMaxLength(200);
        builder.Property(x => x.PhoneHash).HasMaxLength(200);
        builder.Property(x => x.EmailHash).HasMaxLength(200);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ConvertedToLicense).WithMany().HasForeignKey(x => x.ConvertedToLicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.MachineFingerprintHash).IsUnique();
        builder.HasIndex(x => x.PhoneHash).IsUnique();
        builder.HasIndex(x => x.EmailHash).IsUnique();
    }
}

public sealed class LicenseEventConfiguration : IEntityTypeConfiguration<LicenseEvent>
{
    public void Configure(EntityTypeBuilder<LicenseEvent> builder)
    {
        builder.Property(x => x.Result).HasMaxLength(50);
        builder.Property(x => x.ReasonCode).HasMaxLength(100);
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.Property(x => x.FingerprintHash).HasMaxLength(200);
        builder.Property(x => x.AppVersion).HasMaxLength(50);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Activation).WithMany().HasForeignKey(x => x.ActivationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.LicenseId, x.OccurredAt });
    }
}

public sealed class LicenseRecordConfiguration : IEntityTypeConfiguration<LicenseRecord>
{
    public void Configure(EntityTypeBuilder<LicenseRecord> builder)
    {
        builder.Property(x => x.KeyHash).HasMaxLength(200);
        builder.Property(x => x.KeyHint).HasMaxLength(10);
        builder.Property(x => x.PlanCode).HasMaxLength(50);
        builder.Property(x => x.LicensedUserName).HasMaxLength(200);
        builder.Property(x => x.MachineFingerprintHash).HasMaxLength(200);
        builder.Property(x => x.SignatureHash).HasMaxLength(200);
        builder.Property(x => x.SignatureKeyId).HasMaxLength(50);
        builder.Property(x => x.PreviousRecordHash).HasMaxLength(200);
        builder.Property(x => x.RecordHash).HasMaxLength(200);
        builder.Property(x => x.RecordHmac).HasMaxLength(200);
        builder.HasIndex(x => x.Sequence).IsUnique();
    }
}

public sealed class LicenseClockMarkConfiguration : IEntityTypeConfiguration<LicenseClockMark>
{
    public void Configure(EntityTypeBuilder<LicenseClockMark> builder)
    {
        builder.Property(x => x.Hmac).HasMaxLength(200);
    }
}

public sealed class LicenseCheckLogConfiguration : IEntityTypeConfiguration<LicenseCheckLog>
{
    public void Configure(EntityTypeBuilder<LicenseCheckLog> builder)
    {
        builder.Property(x => x.ReasonCode).HasMaxLength(100);
        builder.Property(x => x.FileHash).HasMaxLength(200);
        builder.Property(x => x.AppVersion).HasMaxLength(50);
    }
}

public sealed class LicenseRequestConfiguration : IEntityTypeConfiguration<LicenseRequest>
{
    public void Configure(EntityTypeBuilder<LicenseRequest> builder)
    {
        builder.Property(x => x.KeyHint).HasMaxLength(10);
        builder.Property(x => x.UserName).HasMaxLength(200);
        builder.Property(x => x.FingerprintHash).HasMaxLength(200);
        builder.Property(x => x.RequestFilePath).HasMaxLength(200);
        builder.Property(x => x.ResponseFilePath).HasMaxLength(200);
    }
}

public sealed class LicenseReminderConfiguration : IEntityTypeConfiguration<LicenseReminder>
{
    public void Configure(EntityTypeBuilder<LicenseReminder> builder)
    {
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.LicenseId, x.MilestoneDays, x.Channel }).IsUnique();
    }
}

public sealed class LicenseAttemptConfiguration : IEntityTypeConfiguration<LicenseAttempt>
{
    public void Configure(EntityTypeBuilder<LicenseAttempt> builder)
    {
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.Property(x => x.KeyHashTried).HasMaxLength(200);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.IpAddress, x.AttemptedAt });
    }
}

public sealed class LicenseChangeConfiguration : IEntityTypeConfiguration<LicenseChange>
{
    public void Configure(EntityTypeBuilder<LicenseChange> builder)
    {
        builder.Property(x => x.PriceDifference).HasPrecision(18, 2);
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.NewLicense).WithMany().HasForeignKey(x => x.NewLicenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Activation).WithMany().HasForeignKey(x => x.ActivationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromPlan).WithMany().HasForeignKey(x => x.FromPlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToPlan).WithMany().HasForeignKey(x => x.ToPlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.UpgradeKeyLicense).WithMany().HasForeignKey(x => x.UpgradeKeyLicenseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LegacyLicenseConfiguration : IEntityTypeConfiguration<LegacyLicense>
{
    public void Configure(EntityTypeBuilder<LegacyLicense> builder)
    {
        builder.Property(x => x.UserName).HasMaxLength(200);
        builder.Property(x => x.MacAddress).HasMaxLength(200);
    }
}
