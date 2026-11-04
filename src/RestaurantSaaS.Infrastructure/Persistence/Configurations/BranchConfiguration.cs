using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.Property(b => b.Code).HasMaxLength(30).IsRequired();
        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
        builder.Property(b => b.City).HasMaxLength(100);
        builder.Property(b => b.Address).HasMaxLength(400);
        builder.Property(b => b.Phone).HasMaxLength(30);

        builder.HasIndex(b => new { b.TenantId, b.Code }).IsUnique();
    }
}
