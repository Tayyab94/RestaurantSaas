using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class UserBranchConfiguration : IEntityTypeConfiguration<UserBranch>
{
    public void Configure(EntityTypeBuilder<UserBranch> builder)
    {
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.BranchId }).IsUnique();
    }
}

public sealed class UserPermissionOverrideConfiguration : IEntityTypeConfiguration<UserPermissionOverride>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverride> builder)
    {
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.PermissionId }).IsUnique();
    }
}

public sealed class ApprovalLimitConfiguration : IEntityTypeConfiguration<ApprovalLimit>
{
    public void Configure(EntityTypeBuilder<ApprovalLimit> builder)
    {
        builder.Property(x => x.MaxDiscountPercent).HasPrecision(9, 4);
        builder.Property(x => x.MaxDiscountAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.RoleId).IsUnique();
    }
}

public sealed class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.Property(x => x.TargetHash).HasMaxLength(200);
        builder.Property(x => x.CodeHash).HasMaxLength(200);
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.HasIndex(x => new { x.TargetHash, x.Purpose });
    }
}

public sealed class UserTwoFactorConfiguration : IEntityTypeConfiguration<UserTwoFactor>
{
    public void Configure(EntityTypeBuilder<UserTwoFactor> builder)
    {
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}

public sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.Property(x => x.Language).HasMaxLength(10);
        builder.Property(x => x.DefaultScreen).HasMaxLength(200);
        builder.Property(x => x.NumberFormat).HasMaxLength(200);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DefaultBranch).WithMany().HasForeignKey(x => x.DefaultBranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.EndReason).HasMaxLength(1000);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.IsActive });
    }
}

public sealed class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.Property(x => x.UserNameTried).HasMaxLength(200);
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(x => x.Action).HasMaxLength(50);
        builder.Property(x => x.EntityName).HasMaxLength(100);
        builder.Property(x => x.IpAddress).HasMaxLength(200);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.OccurredAt });
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.EntityId });
    }
}

public sealed class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.Property(x => x.Source).HasMaxLength(100);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ApprovalLogConfiguration : IEntityTypeConfiguration<ApprovalLog>
{
    public void Configure(EntityTypeBuilder<ApprovalLog> builder)
    {
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.Device).WithMany().HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
    }
}
