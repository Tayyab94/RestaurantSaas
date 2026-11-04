using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
        builder.Property(x => x.YearlyPrice).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(200);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => x.Key).IsUnique();
    }
}

public sealed class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Feature).WithMany().HasForeignKey(x => x.FeatureId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PlanId, x.FeatureId }).IsUnique();
    }
}

public sealed class BusinessTypeTemplateConfiguration : IEntityTypeConfiguration<BusinessTypeTemplate>
{
    public void Configure(EntityTypeBuilder<BusinessTypeTemplate> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class TemplateFeatureConfiguration : IEntityTypeConfiguration<TemplateFeature>
{
    public void Configure(EntityTypeBuilder<TemplateFeature> builder)
    {
        builder.HasOne(x => x.Template).WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Feature).WithMany().HasForeignKey(x => x.FeatureId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class TenantFeatureConfiguration : IEntityTypeConfiguration<TenantFeature>
{
    public void Configure(EntityTypeBuilder<TenantFeature> builder)
    {
        builder.HasOne(x => x.Feature).WithMany().HasForeignKey(x => x.FeatureId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.FeatureId }).IsUnique();
    }
}

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.Property(x => x.CancelReason).HasMaxLength(1000);
        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.License).WithMany().HasForeignKey(x => x.LicenseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SubscriptionInvoiceConfiguration : IEntityTypeConfiguration<SubscriptionInvoice>
{
    public void Configure(EntityTypeBuilder<SubscriptionInvoice> builder)
    {
        builder.Property(x => x.InvoiceNumber).HasMaxLength(200);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.Total).HasPrecision(18, 2);
        builder.HasOne(x => x.Subscription).WithMany().HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.InvoiceNumber }).IsUnique();
    }
}

public sealed class SubscriptionPaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Method).HasMaxLength(50);
        builder.Property(x => x.Reference).HasMaxLength(200);
        builder.HasOne(x => x.SubscriptionInvoice).WithMany().HasForeignKey(x => x.SubscriptionInvoiceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.Property(x => x.Group).HasMaxLength(100);
        builder.Property(x => x.Key).HasMaxLength(100);
        builder.Property(x => x.ValueType).HasMaxLength(50);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.BranchId, x.Key });
    }
}

public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Identifier).HasMaxLength(200);
        builder.Property(x => x.AppVersion).HasMaxLength(200);
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.Property(x => x.BlockedReason).HasMaxLength(1000);
        builder.HasIndex(x => new { x.TenantId, x.Identifier }).IsUnique();
    }
}

public sealed class PrinterConfiguration : IEntityTypeConfiguration<Printer>
{
    public void Configure(EntityTypeBuilder<Printer> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PrinterRouteConfiguration : IEntityTypeConfiguration<PrinterRoute>
{
    public void Configure(EntityTypeBuilder<PrinterRoute> builder)
    {
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Printer).WithMany().HasForeignKey(x => x.PrinterId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ReceiptTemplateConfiguration : IEntityTypeConfiguration<ReceiptTemplate>
{
    public void Configure(EntityTypeBuilder<ReceiptTemplate> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Language).HasMaxLength(10);
        builder.Property(x => x.HeaderText).HasMaxLength(1000);
        builder.Property(x => x.FooterText).HasMaxLength(1000);
    }
}

public sealed class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Rate).HasPrecision(9, 4);
        builder.Property(x => x.FbrTaxCode).HasMaxLength(100);
    }
}

public sealed class NumberSequenceConfiguration : IEntityTypeConfiguration<NumberSequence>
{
    public void Configure(EntityTypeBuilder<NumberSequence> builder)
    {
        builder.Property(x => x.DocumentType).HasMaxLength(50);
        builder.Property(x => x.Prefix).HasMaxLength(20);
        builder.Property(x => x.Suffix).HasMaxLength(20);
        builder.HasIndex(x => new { x.BranchId, x.DocumentType }).IsUnique();
    }
}

public sealed class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class IntegrationSettingConfiguration : IEntityTypeConfiguration<IntegrationSetting>
{
    public void Configure(EntityTypeBuilder<IntegrationSetting> builder)
    {
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.KeyHash).HasMaxLength(200);
        builder.Property(x => x.Scopes).HasMaxLength(1000);
    }
}

public sealed class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.Property(x => x.EventType).HasMaxLength(200);
        builder.Property(x => x.TargetUrl).HasMaxLength(500);
        builder.Property(x => x.SecretHash).HasMaxLength(200);
        builder.Property(x => x.LastStatus).HasMaxLength(200);
    }
}

public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(10);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Symbol).HasMaxLength(10);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class PlatformSettingConfiguration : IEntityTypeConfiguration<PlatformSetting>
{
    public void Configure(EntityTypeBuilder<PlatformSetting> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(200);
        builder.Property(x => x.ValueType).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => x.Key).IsUnique();
    }
}

public sealed class AppReleaseConfiguration : IEntityTypeConfiguration<AppRelease>
{
    public void Configure(EntityTypeBuilder<AppRelease> builder)
    {
        builder.Property(x => x.Version).HasMaxLength(50);
        builder.Property(x => x.MinSupportedVersion).HasMaxLength(50);
        builder.Property(x => x.PackageUrl).HasMaxLength(500);
        builder.Property(x => x.PackageHash).HasMaxLength(200);
        builder.HasIndex(x => new { x.Channel, x.Version }).IsUnique();
    }
}

public sealed class ImpersonationSessionConfiguration : IEntityTypeConfiguration<ImpersonationSession>
{
    public void Configure(EntityTypeBuilder<ImpersonationSession> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.HasOne(x => x.SuperAdminUser).WithMany().HasForeignKey(x => x.SuperAdminUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.City).HasMaxLength(200);
        builder.HasOne(x => x.ParentArea).WithMany().HasForeignKey(x => x.ParentAreaId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OnboardingProgressConfiguration : IEntityTypeConfiguration<OnboardingProgress>
{
    public void Configure(EntityTypeBuilder<OnboardingProgress> builder)
    {
        builder.Property(x => x.StepKey).HasMaxLength(50);
        builder.HasIndex(x => new { x.TenantId, x.StepKey }).IsUnique();
    }
}

public sealed class DataExportConfiguration : IEntityTypeConfiguration<DataExport>
{
    public void Configure(EntityTypeBuilder<DataExport> builder)
    {
        builder.Property(x => x.ExportType).HasMaxLength(50);
        builder.Property(x => x.Format).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.FilePath).HasMaxLength(200);
    }
}

public sealed class RestoreLogConfiguration : IEntityTypeConfiguration<RestoreLog>
{
    public void Configure(EntityTypeBuilder<RestoreLog> builder)
    {
        builder.Property(x => x.SourceFile).HasMaxLength(200);
        builder.Property(x => x.Result).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.BackupLog).WithMany().HasForeignKey(x => x.BackupLogId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AppUpdateLogConfiguration : IEntityTypeConfiguration<AppUpdateLog>
{
    public void Configure(EntityTypeBuilder<AppUpdateLog> builder)
    {
        builder.Property(x => x.FromVersion).HasMaxLength(50);
        builder.Property(x => x.ToVersion).HasMaxLength(50);
        builder.Property(x => x.BackupPath).HasMaxLength(200);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
    }
}

public sealed class InstallationInfoConfiguration : IEntityTypeConfiguration<InstallationInfo>
{
    public void Configure(EntityTypeBuilder<InstallationInfo> builder)
    {
        builder.Property(x => x.InstalledVersion).HasMaxLength(50);
        builder.Property(x => x.ServerAddress).HasMaxLength(200);
        builder.Property(x => x.MachineName).HasMaxLength(200);
        builder.Property(x => x.ServiceName).HasMaxLength(200);
    }
}

public sealed class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.Property(x => x.EntityName).HasMaxLength(100);
        builder.Property(x => x.FieldName).HasMaxLength(100);
        builder.Property(x => x.Language).HasMaxLength(10);
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.EntityId, x.FieldName, x.Language }).IsUnique();
    }
}
