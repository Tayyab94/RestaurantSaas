namespace RestaurantSaaS.Domain.Enums;

public enum ChartOfAccountAccountType
{
    Asset,
    Liability,
    Equity,
    Income,
    Expense,
}

public enum AccountMappingEventType
{
    Sale,
    SalesTax,
    Payment,
    Purchase,
    SupplierPayment,
    Expense,
    Payroll,
    Return,
    Tips,
}

public enum AccountingPeriodStatus
{
    Open,
    Closed,
    Locked,
}

public enum JournalEntryEntryType
{
    Auto,
    CashReceipt,
    CashPayment,
    BankReceipt,
    BankPayment,
    Journal,
    Opening,
}

public enum JournalEntryStatus
{
    Draft,
    Posted,
    Reversed,
}

public enum PartyType
{
    Customer,
    Supplier,
    Employee,
    Other,
}

public enum ChequeDirection
{
    Issued,
    Received,
}

public enum ChequeStatus
{
    Pending,
    Deposited,
    Cleared,
    Bounced,
    Cancelled,
}
