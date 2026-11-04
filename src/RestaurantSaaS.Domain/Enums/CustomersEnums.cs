namespace RestaurantSaaS.Domain.Enums;

public enum CustomerCustomerType
{
    Individual,
    Corporate,
}

public enum CustomerBillingCycle
{
    None,
    Weekly,
    Monthly,
}

public enum CustomerLedgerEntryEntryType
{
    CreditSale,
    Payment,
    Advance,
    AdvanceAdjust,
    Refund,
    Opening,
    Adjustment,
}

public enum CustomerAdvancePurpose
{
    Catering,
    Booking,
    Deposit,
}

public enum CustomerAdvanceStatus
{
    Open,
    Partly,
    Used,
    Refunded,
}

public enum LoyaltyTransactionType
{
    Earn,
    Redeem,
    Expire,
    Adjust,
}

public enum GiftCardStatus
{
    Active,
    Used,
    Expired,
    Blocked,
}

public enum GiftCardTransactionType
{
    Issue,
    Redeem,
    Refund,
    Adjust,
}

public enum PromotionPromoType
{
    HappyHour,
    Percent,
    BuyXGetY,
    Bundle,
}

public enum CouponDiscountType
{
    Percent,
    Amount,
}

public enum ConsolidatedInvoiceStatus
{
    Draft,
    Issued,
    PartlyPaid,
    Paid,
    Void,
}

public enum ImportJobImportType
{
    Menu,
    Customers,
    RawMaterials,
    Suppliers,
    OpeningBalances,
    Employees,
}

public enum ImportJobStatus
{
    Uploaded,
    Validated,
    Importing,
    Done,
    Failed,
}
