namespace RestaurantSaaS.Domain.Enums;

public enum NotificationNotificationType
{
    NewOrder,
    OrderReady,
    LowStock,
    LateOrder,
    ShiftAlert,
    LicenseExpiring,
    LicenseExpired,
}

public enum NotificationPriority
{
    Normal,
    High,
}

public enum NotificationTargetType
{
    User,
    Role,
    BranchGroup,
}

public enum BackupLogBackupType
{
    Auto,
    Manual,
}

public enum BackupLogLocation
{
    Local,
    Cloud,
}

public enum OwnerReportScheduleReportType
{
    DailySummary,
    WeeklySummary,
    CashPosition,
}

public enum PushTokenPlatform
{
    Android,
    Windows,
    Web,
}

public enum IntegrationLogDirection
{
    Out,
    In,
}
