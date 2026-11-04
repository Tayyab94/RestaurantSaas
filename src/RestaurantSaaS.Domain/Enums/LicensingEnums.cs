namespace RestaurantSaaS.Domain.Enums;

public enum LicenseSigningKeyAlgorithm
{
    EcdsaP256,
    Rsa2048,
}

public enum LicenseStatus
{
    Unused,
    Active,
    Expired,
    Revoked,
    Suspended,
    Superseded,
}

public enum LicenseKeyPurpose
{
    New,
    Renewal,
    Upgrade,
}

public enum LicenseActivationActivationMethod
{
    Online,
    OfflineFile,
    Trial,
    Web,
}

public enum LicenseActivationStatus
{
    Active,
    Deactivated,
    Revoked,
}

public enum LicenseTransferStatus
{
    Requested,
    Approved,
    Rejected,
    Completed,
}

public enum LicenseEventType
{
    Generated,
    ActivationAttempt,
    Activated,
    ActivationFailed,
    Validated,
    Renewed,
    Expired,
    Revoked,
    Transferred,
    TamperDetected,
    ClockRollback,
    TrialStarted,
    OfflineRequestReceived,
    OfflineLicenseIssued,
    KeyBruteForce,
    Upgraded,
    Downgraded,
    Extended,
}

public enum LicenseRecordSource
{
    Online,
    OfflineFile,
    Trial,
    Renewal,
    Repair,
    Upgrade,
    Downgrade,
    Amendment,
}

public enum LicenseClockSource
{
    Local,
    LicenseServer,
}

public enum LicenseCheckTrigger
{
    Startup,
    Periodic,
    ShiftOpen,
    DayClose,
    ServerRequest,
}

public enum LicenseCheckResult
{
    Ok,
    Repaired,
    NotActivated,
    Expired,
    Tampered,
    Mismatch,
    FingerprintMismatch,
    ClockError,
    Revoked,
}

public enum LicenseRequestType
{
    Activation,
    Trial,
    Renewal,
    Transfer,
}

public enum LicenseRequestStatus
{
    Created,
    Sent,
    Imported,
    Failed,
}

public enum NotificationChannel
{
    InApp,
    Email,
    Sms,
    WhatsApp,
}

public enum LicenseChangeType
{
    UpgradeType,
    UpgradePlan,
    AddLimits,
    Downgrade,
    Extend,
    ConvertMode,
}

public enum LicenseChangeEffectiveMode
{
    Immediate,
    EndOfPeriod,
}

public enum LicenseChangeRequestedByRole
{
    Tenant,
    SuperAdmin,
}

public enum LicenseChangeStatus
{
    Requested,
    Approved,
    Rejected,
    Scheduled,
    Applied,
    Cancelled,
}
