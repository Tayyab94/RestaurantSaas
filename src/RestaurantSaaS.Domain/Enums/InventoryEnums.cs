namespace RestaurantSaaS.Domain.Enums;

public enum StockLocationLocationType
{
    DryStore,
    Freezer,
    Chiller,
    Bar,
    Kitchen,
    Other,
}

public enum StockMovementMovementType
{
    Purchase,
    Sale,
    Return,
    Wastage,
    Adjustment,
    TransferOut,
    TransferIn,
    Opening,
    Production,
    Issue,
    Packaging,
}

public enum WastageReason
{
    Expired,
    Spoiled,
    Spilled,
    Returned,
    Staff,
    Remake,
}

public enum StockTransferStatus
{
    Requested,
    InTransit,
    Received,
    Cancelled,
}

public enum PackagingOrderType
{
    DineIn,
    Takeaway,
    Delivery,
    All,
}

public enum PackagingAppliesTo
{
    Order,
    Item,
}

public enum StockRequisitionRequisitionType
{
    InternalIssue,
    BranchRequest,
}

public enum RequisitionStatus
{
    Requested,
    Approved,
    Issued,
    Received,
    Rejected,
    Cancelled,
}

public enum ReorderSuggestionReason
{
    BelowReorder,
    BelowPar,
}

public enum ReorderSuggestionStatus
{
    Open,
    Ordered,
    Dismissed,
}

public enum RecipeCostSnapshotTrigger
{
    PurchasePrice,
    RecipeChange,
    Daily,
}

public enum PrepListStatus
{
    Draft,
    Published,
    InProgress,
    Done,
}

public enum PrepListSource
{
    Par,
    SalesAverage,
    Manual,
}

public enum PrepListItemStatus
{
    Todo,
    InProgress,
    Done,
    Skipped,
}

public enum ProductionPlanStatus
{
    Draft,
    Approved,
    InProduction,
    Done,
    Cancelled,
}

public enum ProductionPlanSource
{
    BranchRequests,
    ParLevels,
    Manual,
}

public enum ProductionPlanItemStatus
{
    Planned,
    InProduction,
    Done,
}

public enum ProductionDemandStatus
{
    Planned,
    Dispatched,
    Received,
}
