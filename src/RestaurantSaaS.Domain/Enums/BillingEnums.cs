namespace RestaurantSaaS.Domain.Enums;

public enum InvoiceInvoiceType
{
    Sale,
    CreditNote,
}

public enum InvoiceStatus
{
    Open,
    Paid,
    PartiallyPaid,
    Credit,
    Void,
}

public enum PaymentPaymentType
{
    Sale,
    Refund,
    LedgerReceipt,
    AdvanceReceipt,
}

public enum PaymentStatus
{
    Completed,
    Pending,
    Failed,
    Reversed,
}

public enum PrintJobStatus
{
    Queued,
    Printing,
    Done,
    Failed,
}

public enum FbrSubmissionStatus
{
    Pending,
    Submitted,
    Failed,
    Skipped,
}
