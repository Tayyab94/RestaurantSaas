using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.MinTempC).HasPrecision(18, 2);
        builder.Property(x => x.MaxTempC).HasPrecision(18, 2);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class FoodSafetyChecklistConfiguration : IEntityTypeConfiguration<FoodSafetyChecklist>
{
    public void Configure(EntityTypeBuilder<FoodSafetyChecklist> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.Property(x => x.Text).HasMaxLength(1000);
        builder.Property(x => x.TextUrdu).HasMaxLength(1000);
        builder.Property(x => x.MinValue).HasPrecision(18, 2);
        builder.Property(x => x.MaxValue).HasPrecision(18, 2);
        builder.HasOne(x => x.Checklist).WithMany().HasForeignKey(x => x.ChecklistId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ChecklistRunConfiguration : IEntityTypeConfiguration<ChecklistRun>
{
    public void Configure(EntityTypeBuilder<ChecklistRun> builder)
    {
        builder.Property(x => x.AssignedTo).HasMaxLength(200);
        builder.HasOne(x => x.Checklist).WithMany().HasForeignKey(x => x.ChecklistId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ChecklistId, x.RunDate });
    }
}

public sealed class ChecklistRunItemConfiguration : IEntityTypeConfiguration<ChecklistRunItem>
{
    public void Configure(EntityTypeBuilder<ChecklistRunItem> builder)
    {
        builder.Property(x => x.ResultNumber).HasPrecision(18, 2);
        builder.Property(x => x.ResultText).HasMaxLength(200);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasOne(x => x.ChecklistRun).WithMany().HasForeignKey(x => x.ChecklistRunId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ChecklistItem).WithMany().HasForeignKey(x => x.ChecklistItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Attachment).WithMany().HasForeignKey(x => x.AttachmentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class TemperatureLogConfiguration : IEntityTypeConfiguration<TemperatureLog>
{
    public void Configure(EntityTypeBuilder<TemperatureLog> builder)
    {
        builder.Property(x => x.TemperatureC).HasPrecision(18, 2);
        builder.Property(x => x.MinC).HasPrecision(18, 2);
        builder.Property(x => x.MaxC).HasPrecision(18, 2);
        builder.Property(x => x.CorrectiveAction).HasMaxLength(200);
        builder.HasOne(x => x.Equipment).WithMany().HasForeignKey(x => x.EquipmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Incident).WithMany().HasForeignKey(x => x.IncidentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EquipmentId, x.RecordedAt });
    }
}

public sealed class FoodSafetyIncidentConfiguration : IEntityTypeConfiguration<FoodSafetyIncident>
{
    public void Configure(EntityTypeBuilder<FoodSafetyIncident> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.ActionTaken).HasMaxLength(200);
        builder.HasOne(x => x.Attachment).WithMany().HasForeignKey(x => x.AttachmentId).OnDelete(DeleteBehavior.Restrict);
    }
}
