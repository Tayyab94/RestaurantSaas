namespace RestaurantSaaS.Domain.Security;

public sealed record PermissionInfo(string Key, string Module, string Description);

/// <summary>
/// The catalog of permissions. Every new screen or action gets a key here (Module "Platform" is for SuperAdmin only).
/// The catalog is synced to the Permissions table at start-up.
/// </summary>
public static class Permissions
{
    public const string DashboardView = "Dashboard.View";

    public const string TenantView = "Tenant.View";
    public const string TenantManage = "Tenant.Manage";

    public const string UserView = "User.View";
    public const string UserManage = "User.Manage";
    public const string RoleView = "Role.View";
    public const string RoleManage = "Role.Manage";
    public const string BranchView = "Branch.View";
    public const string BranchManage = "Branch.Manage";
    public const string SettingsView = "Settings.View";
    public const string SettingsManage = "Settings.Manage";
    public const string AuditLogView = "AuditLog.View";

    public const string MenuView = "Menu.View";
    public const string MenuManage = "Menu.Manage";
    public const string TableView = "Table.View";
    public const string TableManage = "Table.Manage";

    public const string OrderView = "Order.View";
    public const string OrderCreate = "Order.Create";
    public const string OrderVoid = "Order.Void";
    public const string DiscountApply = "Discount.Apply";

    public const string ReportView = "Report.View";

    public const string LicenseView = "License.View";
    public const string LicenseActivate = "License.Activate";

    public const string PlatformModule = "Platform";

    // ADD after LicenseActivate constant:
public const string LicenseGenerate = "License.Generate";

    public const string LicenseDeactivate = "License.Deactivate";
    public static IReadOnlyList<PermissionInfo> All { get; } =
    [
        new(DashboardView, "Dashboard", "View the dashboard"),
        new(TenantView, PlatformModule, "View all tenants"),
        new(TenantManage, PlatformModule, "Create, suspend and change tenants"),
        new(UserView, "Administration", "View users and roles"),
        new(UserManage, "Administration", "Create and change users"),
        new(RoleView, "Administration", "View roles"),
        new(RoleManage, "Administration", "Create and change roles"),
        new(BranchView, "Administration", "View branches"),
        new(BranchManage, "Administration", "Create and change branches"),
        new(SettingsView, "Administration", "View business settings"),
        new(SettingsManage, "Administration", "Change business settings"),
        new(AuditLogView, "Administration", "View the audit log"),
        new(MenuView, "Menu", "View the menu"),
        new(MenuManage, "Menu", "Create and change menu items"),
        new(TableView, "Tables", "View tables and floors"),
        new(TableManage, "Tables", "Create and change tables"),
        new(OrderView, "Orders", "View orders"),
        new(OrderCreate, "Orders", "Take orders"),
        new(OrderVoid, "Orders", "Void or cancel orders and items"),
        new(DiscountApply, "Orders", "Give discounts"),
        new(ReportView, "Reports", "View reports"),
        new(LicenseView, "License", "View the license"),
        new(LicenseActivate, "License", "Activate or renew the license"),
        // ADD in the All catalog list after the LicenseActivate line:
        new(LicenseGenerate, PlatformModule, "Make and list license keys"),
        new(LicenseDeactivate, "License", "Deactivate the license of this PC"),
    ];
}
