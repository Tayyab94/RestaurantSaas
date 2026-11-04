using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Message).HasMaxLength(1000);
        builder.Property(x => x.TargetRole).HasMaxLength(200);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.HasOne(x => x.TargetUser).WithMany().HasForeignKey(x => x.TargetUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder.HasOne(x => x.Notification).WithMany().HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.IsRead });
    }
}

public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.Property(x => x.EntityName).HasMaxLength(100);
        builder.Property(x => x.FileName).HasMaxLength(200);
        builder.Property(x => x.ContentType).HasMaxLength(100);
        builder.Property(x => x.StorageUrl).HasMaxLength(500);
    }
}

public sealed class MessageLogConfiguration : IEntityTypeConfiguration<MessageLog>
{
    public void Configure(EntityTypeBuilder<MessageLog> builder)
    {
        builder.Property(x => x.Recipient).HasMaxLength(200);
        builder.Property(x => x.TemplateName).HasMaxLength(200);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.ProviderReference).HasMaxLength(200);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
    }
}

public sealed class MessageTemplateConfiguration : IEntityTypeConfiguration<MessageTemplate>
{
    public void Configure(EntityTypeBuilder<MessageTemplate> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Language).HasMaxLength(10);
        builder.Property(x => x.Subject).HasMaxLength(200);
    }
}

public sealed class BackupLogConfiguration : IEntityTypeConfiguration<BackupLog>
{
    public void Configure(EntityTypeBuilder<BackupLog> builder)
    {
        builder.Property(x => x.FilePath).HasMaxLength(200);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
    }
}

public sealed class OwnerReportScheduleConfiguration : IEntityTypeConfiguration<OwnerReportSchedule>
{
    public void Configure(EntityTypeBuilder<OwnerReportSchedule> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Recipient).HasMaxLength(200);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PushTokenConfiguration : IEntityTypeConfiguration<PushToken>
{
    public void Configure(EntityTypeBuilder<PushToken> builder)
    {
        builder.Property(x => x.Token).HasMaxLength(500);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.IsActive });
    }
}

public sealed class IntegrationLogConfiguration : IEntityTypeConfiguration<IntegrationLog>
{
    public void Configure(EntityTypeBuilder<IntegrationLog> builder)
    {
        builder.Property(x => x.EntityType).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.RequestExcerpt).HasMaxLength(1000);
        builder.Property(x => x.ResponseExcerpt).HasMaxLength(1000);
    }
}

public sealed class ExternalMenuMappingConfiguration : IEntityTypeConfiguration<ExternalMenuMapping>
{
    public void Configure(EntityTypeBuilder<ExternalMenuMapping> builder)
    {
        builder.Property(x => x.ExternalName).HasMaxLength(200);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
    {
        builder.Property(x => x.EventType).HasMaxLength(200);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.HasOne(x => x.WebhookSubscription).WithMany().HasForeignKey(x => x.WebhookSubscriptionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.Status, x.NextRetryAt });
    }
}
