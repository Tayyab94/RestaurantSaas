using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.UserName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.NormalizedUserName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.Phone).HasMaxLength(30);
        builder.Property(u => u.PasswordHash).HasMaxLength(300).IsRequired();
        builder.Property(u => u.PinHash).HasMaxLength(300);

        builder.HasOne<Tenant>().WithMany().HasForeignKey(u => u.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.TenantId);
        builder.HasIndex(u => new { u.TenantId, u.NormalizedUserName }).IsUnique();
    }
}

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(300);

        builder.HasIndex(r => new { r.TenantId, r.Name }).IsUnique();
    }
}

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.Property(p => p.Key).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Module).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(200).IsRequired();

        builder.HasIndex(p => p.Key).IsUnique();
    }
}

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
    }
}

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasOne(rp => rp.Role).WithMany().HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(rp => rp.Permission).WithMany().HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();
    }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(t => t.TokenHash).HasMaxLength(100).IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.TokenHash).IsUnique();
    }
}
