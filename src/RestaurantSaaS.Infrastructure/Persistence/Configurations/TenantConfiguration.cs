using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.Property(t => t.Code).HasMaxLength(30).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.OwnerName).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Email).HasMaxLength(256);
        builder.Property(t => t.Phone).HasMaxLength(30);
        builder.Property(t => t.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(t => t.TimeZone).HasMaxLength(64).IsRequired();
        builder.Property(t => t.DefaultLanguage).HasMaxLength(10).IsRequired();
        builder.Ignore(t => t.CanSignIn);

        builder.HasIndex(t => t.Code).IsUnique();

        // One business per phone number and per email (the phone is stored in international form, for example +923001234567).
        builder.HasIndex(t => t.Phone).IsUnique();
        builder.HasIndex(t => t.Email).IsUnique();
    }
}
