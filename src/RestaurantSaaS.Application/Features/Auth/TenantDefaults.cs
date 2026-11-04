using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Auth;

public sealed record PaymentMethodDefault(string Name, PaymentMethodMethodType Type, bool OpensDrawer, bool RequiresReference);

public sealed record ReasonDefault(ReasonCodeReasonType Type, string Name, bool RequiresNote = false);

public sealed record LimitDefault(decimal Percent, decimal Amount, bool CanVoid, bool CanReturn);

/// <summary>What every new business starts with, so it can sell on the first day. The owner can change all of it later.</summary>
public static class TenantDefaults
{
    public static IReadOnlyList<PaymentMethodDefault> PaymentMethods { get; } =
    [
        new("Cash", PaymentMethodMethodType.Cash, OpensDrawer: true, RequiresReference: false),
        new("Card", PaymentMethodMethodType.Card, OpensDrawer: false, RequiresReference: true),
        new("Online transfer", PaymentMethodMethodType.Online, OpensDrawer: false, RequiresReference: true),
        new("Customer credit", PaymentMethodMethodType.Credit, OpensDrawer: false, RequiresReference: false),
    ];

    /// <summary>Starting approval limits of the system roles (the Owner has no limit). The Owner can change them.</summary>
    public static IReadOnlyDictionary<string, LimitDefault> Limits { get; } = new Dictionary<string, LimitDefault>
    {
        ["Manager"] = new(25, 5000, CanVoid: true, CanReturn: true),
        ["Cashier"] = new(5, 500, CanVoid: false, CanReturn: false),
        ["Waiter"] = new(0, 0, CanVoid: false, CanReturn: false),
        ["Kitchen"] = new(0, 0, CanVoid: false, CanReturn: false),
    };

    public static IReadOnlyList<ReasonDefault> ReasonCodes { get; } =
    [
        new(ReasonCodeReasonType.Void, "Wrong item entered"),
        new(ReasonCodeReasonType.Void, "Customer changed mind"),
        new(ReasonCodeReasonType.Void, "Item not available"),
        new(ReasonCodeReasonType.Void, "Other", RequiresNote: true),
        new(ReasonCodeReasonType.Cancel, "Customer left"),
        new(ReasonCodeReasonType.Cancel, "Duplicate order"),
        new(ReasonCodeReasonType.Cancel, "Order mistake"),
        new(ReasonCodeReasonType.Cancel, "Other", RequiresNote: true),
        new(ReasonCodeReasonType.Wastage, "Expired"),
        new(ReasonCodeReasonType.Wastage, "Spoiled"),
        new(ReasonCodeReasonType.Wastage, "Spilled or dropped"),
        new(ReasonCodeReasonType.Adjustment, "Stock count correction"),
        new(ReasonCodeReasonType.Adjustment, "Damaged goods"),
        new(ReasonCodeReasonType.Discount, "Regular customer"),
        new(ReasonCodeReasonType.Discount, "Staff meal"),
        new(ReasonCodeReasonType.Discount, "Manager discount"),
        new(ReasonCodeReasonType.Discount, "Customer complaint"),
        new(ReasonCodeReasonType.CashOut, "Supplier payment"),
        new(ReasonCodeReasonType.CashOut, "Staff advance"),
        new(ReasonCodeReasonType.CashOut, "Petty expense"),
        new(ReasonCodeReasonType.CashOut, "Other", RequiresNote: true),
        new(ReasonCodeReasonType.Remake, "Wrong preparation"),
        new(ReasonCodeReasonType.Remake, "Customer complaint"),
        new(ReasonCodeReasonType.Remake, "Quality problem"),
        new(ReasonCodeReasonType.Refire, "Order delayed"),
        new(ReasonCodeReasonType.Refire, "Dropped or spilled"),
        new(ReasonCodeReasonType.Refire, "Sent to the wrong station"),
    ];
}
