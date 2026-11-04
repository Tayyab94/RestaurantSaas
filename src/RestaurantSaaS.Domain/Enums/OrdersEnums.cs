namespace RestaurantSaaS.Domain.Enums;

public enum OrderOrderType
{
    DineIn,
    Takeaway,
    Delivery,
}

public enum OrderSource
{
    WalkIn,
    Phone,
    Qr,
    Website,
    Foodpanda,
    Other,
}

public enum OrderStatus
{
    Draft,
    Held,
    PendingApproval,
    Confirmed,
    InKitchen,
    Ready,
    Served,
    Completed,
    Cancelled,
}

public enum OrderPaymentStatus
{
    Unpaid,
    Partial,
    Paid,
    Credit,
}

public enum OrderPriority
{
    Normal,
    Rush,
    VIP,
}

public enum OrderItemStatus
{
    New,
    Sent,
    Preparing,
    Ready,
    Served,
    Voided,
}

public enum OrderDiscountType
{
    Percent,
    Amount,
    Coupon,
    Promotion,
    Loyalty,
    GiftCard,
}

public enum OrderTransferTransferType
{
    Table,
    Waiter,
}

public enum DiscountPresetType
{
    Percent,
    Amount,
}

public enum DiscountAppliesTo
{
    Order,
    Item,
}

public enum ReasonCodeReasonType
{
    Void,
    Cancel,
    Wastage,
    Adjustment,
    Discount,
    CashOut,
    Remake,
    Refire,
}

public enum RemakeRecordRemakeType
{
    Remake,
    Refire,
}

public enum RemakeRecordChargeTo
{
    Kitchen,
    Waiter,
    Restaurant,
    Customer,
}

public enum ItemAvailabilityLogAction
{
    MarkedOut,
    MarkedIn,
    AutoOut,
    AutoIn,
    Expired,
}

public enum ItemAvailabilitySource
{
    Kitchen,
    Inventory,
    Admin,
    Schedule,
}

public enum KdsScreenScreenType
{
    Station,
    Expo,
}

public enum KitchenTicketStatus
{
    New,
    Preparing,
    Ready,
    Served,
    Cancelled,
}

public enum KitchenTicketItemStatus
{
    New,
    Preparing,
    Ready,
    Served,
    Cancelled,
}
