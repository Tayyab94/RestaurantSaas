namespace RestaurantSaaS.Domain.Enums;

public enum LicenseType
{
    Trial=0,
    Months12=1,
    Months6=2,
    Lifetime=3,
}

public enum SubscriptionBillingCycle
{
    Monthly,
    Yearly,
    OneTime,
}

public enum SubscriptionStatus
{
    Trial,
    Active,
    PastDue,
    Cancelled,
    Expired,
}

public enum SubscriptionInvoiceStatus
{
    Draft,
    Issued,
    Paid,
    Overdue,
    Void,
}

public enum DeviceDeviceType
{
    Pos,
    Kitchen,
    Waiter,
    Rider,
    Expo,
}

public enum DeviceMode
{
    Server,
    Client,
}

public enum PrinterPrinterType
{
    Receipt,
    Kot,
    Label,
}

public enum PrinterConnectionType
{
    Usb,
    Network,
    Browser,
}

public enum DocumentType
{
    Kot,
    Invoice,
    Report,
}

public enum TaxRateTaxType
{
    Percent,
    Fixed,
}

public enum TaxRateAppliesTo
{
    Food,
    Service,
    Retail,
    All,
}

public enum NumberSequenceResetPolicy
{
    Never,
    Daily,
    Monthly,
    Yearly,
}

public enum PaymentMethodMethodType
{
    Cash,
    Card,
    Online,
    Credit,
    GiftCard,
    Wallet,
}

public enum IntegrationProvider
{
    Fbr,
    Foodpanda,
    JazzCash,
    Easypaisa,
    Stripe,
    WhatsApp,
    Sms,
    Email,
}

public enum AppReleaseChannel
{
    Stable,
    Beta,
}

public enum RestoreLogLicenseOutcome
{
    Ok,
    Repaired,
    ReactivationNeeded,
}

public enum AppUpdateLogResult
{
    Success,
    Failed,
    RolledBack,
}

public enum InstallationRole
{
    Server,
    Client,
}
