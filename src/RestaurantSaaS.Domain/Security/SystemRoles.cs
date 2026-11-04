namespace RestaurantSaaS.Domain.Security;

public sealed record RoleDefinition(string Name, string Description, IReadOnlyList<string> Permissions);

/// <summary>Roles created for every new tenant. The tenant Owner can later add custom roles.</summary>
public static class SystemRoles
{
    public const string Owner = "Owner";
    public const string Manager = "Manager";
    public const string Cashier = "Cashier";
    public const string Waiter = "Waiter";
    public const string Kitchen = "Kitchen";

    public static IReadOnlyList<RoleDefinition> All { get; } =
    [
        new(Owner, "Full control of the business",
            Permissions.All.Where(p => p.Module != Permissions.PlatformModule).Select(p => p.Key).ToList()),

        new(Manager, "Runs a branch: staff, menu, tables, orders, reports",
        [
            Permissions.DashboardView, Permissions.UserView, Permissions.BranchView,
            Permissions.SettingsView, Permissions.MenuView, Permissions.MenuManage,
            Permissions.TableView, Permissions.TableManage, Permissions.OrderView,
            Permissions.OrderCreate, Permissions.OrderVoid, Permissions.DiscountApply,
            Permissions.ReportView,
        ]),

        new(Cashier, "Takes payments and bills",
        [
            Permissions.DashboardView, Permissions.MenuView, Permissions.TableView,
            Permissions.OrderView, Permissions.OrderCreate,
        ]),

        new(Waiter, "Takes orders at the table",
        [
            Permissions.DashboardView, Permissions.MenuView, Permissions.TableView,
            Permissions.OrderView, Permissions.OrderCreate,
        ]),

        new(Kitchen, "Sees and prepares orders",
        [
            Permissions.DashboardView, Permissions.MenuView, Permissions.OrderView,
        ]),
    ];
}
