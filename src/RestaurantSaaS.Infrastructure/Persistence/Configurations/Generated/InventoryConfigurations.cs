using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Symbol).HasMaxLength(10);
    }
}

public sealed class UnitConversionConfiguration : IEntityTypeConfiguration<UnitConversion>
{
    public void Configure(EntityTypeBuilder<UnitConversion> builder)
    {
        builder.Property(x => x.Factor).HasPrecision(18, 6);
        builder.HasOne(x => x.FromUnit).WithMany().HasForeignKey(x => x.FromUnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToUnit).WithMany().HasForeignKey(x => x.ToUnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RawMaterialCategoryConfiguration : IEntityTypeConfiguration<RawMaterialCategory>
{
    public void Configure(EntityTypeBuilder<RawMaterialCategory> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RawMaterialConfiguration : IEntityTypeConfiguration<RawMaterial>
{
    public void Configure(EntityTypeBuilder<RawMaterial> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.PurchaseToBaseFactor).HasPrecision(18, 6);
        builder.Property(x => x.Barcode).HasMaxLength(200);
        builder.Property(x => x.AverageCost).HasPrecision(18, 2);
        builder.Property(x => x.LastPurchasePrice).HasPrecision(18, 2);
        builder.Property(x => x.ReorderLevel).HasPrecision(18, 3);
        builder.Property(x => x.YieldPercent).HasPrecision(9, 4);
        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BaseUnit).WithMany().HasForeignKey(x => x.BaseUnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PurchaseUnit).WithMany().HasForeignKey(x => x.PurchaseUnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DefaultLocation).WithMany().HasForeignKey(x => x.DefaultLocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.YieldQuantity).HasPrecision(18, 3);
        builder.Property(x => x.MethodSummary).HasMaxLength(1000);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Variant).WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RecipeItemConfiguration : IEntityTypeConfiguration<RecipeItem>
{
    public void Configure(EntityTypeBuilder<RecipeItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.WastagePercent).HasPrecision(9, 4);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SubRecipe).WithMany().HasForeignKey(x => x.SubRecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.Property(x => x.TemperatureC).HasPrecision(18, 2);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.VideoUrl).HasMaxLength(500);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.RecipeId, x.StepNumber }).IsUnique();
    }
}

public sealed class StockLocationConfiguration : IEntityTypeConfiguration<StockLocation>
{
    public void Configure(EntityTypeBuilder<StockLocation> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class BranchStockConfiguration : IEntityTypeConfiguration<BranchStock>
{
    public void Configure(EntityTypeBuilder<BranchStock> builder)
    {
        builder.Property(x => x.QuantityOnHand).HasPrecision(18, 3);
        builder.Property(x => x.AverageCost).HasPrecision(18, 2);
        builder.Property(x => x.ReorderLevel).HasPrecision(18, 3);
        builder.Property(x => x.ParLevel).HasPrecision(18, 3);
        builder.Property(x => x.MaxLevel).HasPrecision(18, 3);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.LocationId, x.RawMaterialId }).IsUnique();
    }
}

public sealed class StockBatchConfiguration : IEntityTypeConfiguration<StockBatch>
{
    public void Configure(EntityTypeBuilder<StockBatch> builder)
    {
        builder.Property(x => x.BatchNumber).HasMaxLength(200);
        builder.Property(x => x.QuantityRemaining).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GoodsReceiptItem).WithMany().HasForeignKey(x => x.GoodsReceiptItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAfter).HasPrecision(18, 3);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Requisition).WithMany().HasForeignKey(x => x.RequisitionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.RawMaterialId, x.MovementDate });
    }
}

public sealed class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.Property(x => x.AdjustmentNumber).HasMaxLength(200);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.HasIndex(x => new { x.BranchId, x.AdjustmentNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class StockAdjustmentItemConfiguration : IEntityTypeConfiguration<StockAdjustmentItem>
{
    public void Configure(EntityTypeBuilder<StockAdjustmentItem> builder)
    {
        builder.Property(x => x.SystemQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ActualQuantity).HasPrecision(18, 3);
        builder.Property(x => x.Difference).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.HasOne(x => x.StockAdjustment).WithMany().HasForeignKey(x => x.StockAdjustmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class WastageRecordConfiguration : IEntityTypeConfiguration<WastageRecord>
{
    public void Configure(EntityTypeBuilder<WastageRecord> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.TotalCost).HasPrecision(18, 2);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RemakeRecord).WithMany().HasForeignKey(x => x.RemakeRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.Property(x => x.TransferNumber).HasMaxLength(200);
        builder.HasOne(x => x.FromBranch).WithMany().HasForeignKey(x => x.FromBranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToBranch).WithMany().HasForeignKey(x => x.ToBranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.TransferNumber }).IsUnique();
    }
}

public sealed class StockTransferItemConfiguration : IEntityTypeConfiguration<StockTransferItem>
{
    public void Configure(EntityTypeBuilder<StockTransferItem> builder)
    {
        builder.Property(x => x.SentQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.HasOne(x => x.StockTransfer).WithMany().HasForeignKey(x => x.StockTransferId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StockCountConfiguration : IEntityTypeConfiguration<StockCount>
{
    public void Configure(EntityTypeBuilder<StockCount> builder)
    {
        builder.Property(x => x.CountNumber).HasMaxLength(200);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.CountNumber }).IsUnique();
    }
}

public sealed class StockCountItemConfiguration : IEntityTypeConfiguration<StockCountItem>
{
    public void Configure(EntityTypeBuilder<StockCountItem> builder)
    {
        builder.Property(x => x.SystemQuantity).HasPrecision(18, 3);
        builder.Property(x => x.CountedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.Difference).HasPrecision(18, 3);
        builder.HasOne(x => x.StockCount).WithMany().HasForeignKey(x => x.StockCountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ProductionRunConfiguration : IEntityTypeConfiguration<ProductionRun>
{
    public void Configure(EntityTypeBuilder<ProductionRun> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.TotalCost).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OutputRawMaterial).WithMany().HasForeignKey(x => x.OutputRawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PrepListItem).WithMany().HasForeignKey(x => x.PrepListItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ProductionPlanItem).WithMany().HasForeignKey(x => x.ProductionPlanItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PackagingRuleConfiguration : IEntityTypeConfiguration<PackagingRule>
{
    public void Configure(EntityTypeBuilder<PackagingRule> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StockRequisitionConfiguration : IEntityTypeConfiguration<StockRequisition>
{
    public void Configure(EntityTypeBuilder<StockRequisition> builder)
    {
        builder.Property(x => x.RequisitionNumber).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.FromLocation).WithMany().HasForeignKey(x => x.FromLocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToLocation).WithMany().HasForeignKey(x => x.ToLocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromBranch).WithMany().HasForeignKey(x => x.FromBranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.RequisitionNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class StockRequisitionItemConfiguration : IEntityTypeConfiguration<StockRequisitionItem>
{
    public void Configure(EntityTypeBuilder<StockRequisitionItem> builder)
    {
        builder.Property(x => x.RequestedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ApprovedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.IssuedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 3);
        builder.HasOne(x => x.StockRequisition).WithMany().HasForeignKey(x => x.StockRequisitionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class VarianceAnalysisConfiguration : IEntityTypeConfiguration<VarianceAnalysis>
{
    public void Configure(EntityTypeBuilder<VarianceAnalysis> builder)
    {
        builder.Property(x => x.OpeningQuantity).HasPrecision(18, 3);
        builder.Property(x => x.PurchasedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.NetTransferQuantity).HasPrecision(18, 3);
        builder.Property(x => x.TheoreticalUsage).HasPrecision(18, 3);
        builder.Property(x => x.ClosingQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ActualUsage).HasPrecision(18, 3);
        builder.Property(x => x.VarianceQuantity).HasPrecision(18, 3);
        builder.Property(x => x.VarianceValue).HasPrecision(18, 2);
        builder.Property(x => x.VariancePercent).HasPrecision(9, 4);
        builder.HasOne(x => x.StockCount).WithMany().HasForeignKey(x => x.StockCountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ReorderSuggestionConfiguration : IEntityTypeConfiguration<ReorderSuggestion>
{
    public void Configure(EntityTypeBuilder<ReorderSuggestion> builder)
    {
        builder.Property(x => x.CurrentQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ParLevel).HasPrecision(18, 3);
        builder.Property(x => x.SuggestedQuantity).HasPrecision(18, 3);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SuggestedSupplier).WithMany().HasForeignKey(x => x.SuggestedSupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.Status, x.GeneratedAt });
    }
}

public sealed class RecipeCostSnapshotConfiguration : IEntityTypeConfiguration<RecipeCostSnapshot>
{
    public void Configure(EntityTypeBuilder<RecipeCostSnapshot> builder)
    {
        builder.Property(x => x.CostPerPortion).HasPrecision(18, 2);
        builder.Property(x => x.SellingPrice).HasPrecision(18, 2);
        builder.Property(x => x.FoodCostPercent).HasPrecision(9, 4);
        builder.Property(x => x.TargetFoodCostPercent).HasPrecision(9, 4);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MenuItem).WithMany().HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PrepListConfiguration : IEntityTypeConfiguration<PrepList>
{
    public void Configure(EntityTypeBuilder<PrepList> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasIndex(x => new { x.BranchId, x.BusinessDate });
    }
}

public sealed class PrepListItemConfiguration : IEntityTypeConfiguration<PrepListItem>
{
    public void Configure(EntityTypeBuilder<PrepListItem> builder)
    {
        builder.Property(x => x.ItemName).HasMaxLength(200);
        builder.Property(x => x.TargetQuantity).HasPrecision(18, 3);
        builder.Property(x => x.OnHandQuantity).HasPrecision(18, 3);
        builder.Property(x => x.PreparedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.PrepList).WithMany().HasForeignKey(x => x.PrepListId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Station).WithMany().HasForeignKey(x => x.StationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OutputRawMaterial).WithMany().HasForeignKey(x => x.OutputRawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedEmployee).WithMany().HasForeignKey(x => x.AssignedEmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ProductionRun).WithMany().HasForeignKey(x => x.ProductionRunId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ProductionPlanConfiguration : IEntityTypeConfiguration<ProductionPlan>
{
    public void Configure(EntityTypeBuilder<ProductionPlan> builder)
    {
        builder.HasOne(x => x.CentralBranch).WithMany().HasForeignKey(x => x.CentralBranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CentralBranchId, x.PlanDate });
    }
}

public sealed class ProductionPlanItemConfiguration : IEntityTypeConfiguration<ProductionPlanItem>
{
    public void Configure(EntityTypeBuilder<ProductionPlanItem> builder)
    {
        builder.Property(x => x.PlannedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ProducedQuantity).HasPrecision(18, 3);
        builder.HasOne(x => x.ProductionPlan).WithMany().HasForeignKey(x => x.ProductionPlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OutputRawMaterial).WithMany().HasForeignKey(x => x.OutputRawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ProductionRun).WithMany().HasForeignKey(x => x.ProductionRunId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ProductionPlanDemandConfiguration : IEntityTypeConfiguration<ProductionPlanDemand>
{
    public void Configure(EntityTypeBuilder<ProductionPlanDemand> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.HasOne(x => x.ProductionPlanItem).WithMany().HasForeignKey(x => x.ProductionPlanItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RequestingBranch).WithMany().HasForeignKey(x => x.RequestingBranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.StockRequisition).WithMany().HasForeignKey(x => x.StockRequisitionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DispatchTransfer).WithMany().HasForeignKey(x => x.DispatchTransferId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PrepLabelConfiguration : IEntityTypeConfiguration<PrepLabel>
{
    public void Configure(EntityTypeBuilder<PrepLabel> builder)
    {
        builder.Property(x => x.ItemName).HasMaxLength(200);
        builder.Property(x => x.QuantityText).HasMaxLength(200);
        builder.Property(x => x.DiscardReason).HasMaxLength(1000);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.StockBatch).WithMany().HasForeignKey(x => x.StockBatchId).OnDelete(DeleteBehavior.Restrict);
    }
}
