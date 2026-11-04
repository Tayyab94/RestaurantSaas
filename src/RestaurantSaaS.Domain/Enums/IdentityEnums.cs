namespace RestaurantSaaS.Domain.Enums;

public enum VerificationCodePurpose
{
    SignUp,
    PasswordReset,
    Login2Fa,
    PhoneChange,
}

public enum TwoFactorMethod
{
    Totp,
    Sms,
}

public enum UserPreferenceTheme
{
    Light,
    Dark,
    Auto,
}

public enum ApprovalLogApprovalType
{
    Void,
    Discount,
    Return,
    CashOut,
    PriceOverride,
}

public enum ApprovalMethod
{
    Pin,
    Remote,
}
