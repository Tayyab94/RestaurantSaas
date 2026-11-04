namespace RestaurantSaaS.Domain.Enums;

public enum SupplierSupplierType
{
    Vegetables,
    Meat,
    Dairy,
    Packaging,
    Utilities,
    Other,
}

public enum SupplierItemPriceHistorySource
{
    Manual,
    GoodsReceipt,
}

public enum SupplierAdvanceStatus
{
    Open,
    Partly,
    Used,
    Refunded,
}

public enum SupplierLedgerEntryType
{
    Purchase,
    Payment,
    Return,
    Adjustment,
    Advance,
}

public enum PurchaseOrderStatus
{
    Draft,
    Sent,
    Partial,
    Received,
    Cancelled,
}

public enum PurchaseOrderApprovalStatus
{
    NotRequired,
    Pending,
    Approved,
    Rejected,
}
