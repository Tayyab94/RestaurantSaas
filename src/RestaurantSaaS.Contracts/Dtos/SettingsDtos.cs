namespace RestaurantSaaS.Contracts.Dtos;

/// <summary>The business profile (M05). BusinessType is one of Restaurant, Cafe, Bakery, FastFood, Retail.</summary>
public sealed record BusinessProfileDto(
    string Code,
    string Name,
    string OwnerName,
    string? Email,
    string? Phone,
    string BusinessType,
    string? Address,
    string? TaxNumber,
    string TimeZone,
    string DefaultLanguage,
    string? LogoDataUri);

public sealed record SaveBusinessProfileRequest(
    string Name,
    string OwnerName,
    string? Email,
    string? Phone,
    string BusinessType,
    string? Address,
    string? TaxNumber,
    string TimeZone,
    string DefaultLanguage);

/// <summary>A PNG or JPEG logo as a data URI (data:image/png;base64,...). Null removes the logo.</summary>
public sealed record SetLogoRequest(string? DataUri);

public sealed record BranchDetailDto(
    Guid Id,
    string Code,
    string Name,
    string? City,
    string? Address,
    string? Phone,
    bool IsMain,
    string? OpeningHours);

/// <summary>Id null creates a branch. The code of an existing branch cannot change.</summary>
public sealed record SaveBranchRequest(
    Guid? Id,
    string Code,
    string Name,
    string? City,
    string? Address,
    string? Phone,
    string? OpeningHours);

/// <summary>TaxType is Percent or Fixed. AppliesTo is Food, Service, Retail or All.</summary>
public sealed record TaxRateDto(
    Guid Id,
    string Name,
    decimal Rate,
    string TaxType,
    string AppliesTo,
    bool IsInclusive,
    bool IsDefault,
    string? FbrTaxCode);

public sealed record SaveTaxRateRequest(
    Guid? Id,
    string Name,
    decimal Rate,
    string TaxType,
    string AppliesTo,
    bool IsInclusive,
    bool IsDefault,
    string? FbrTaxCode);

/// <summary>RoundingMode is None, Nearest, Up or Down. RoundingStep is 1, 5 or 10 (the amount is rounded to a multiple of it).</summary>
public sealed record BillingSettingsDto(
    string CurrencyCode,
    bool ServiceChargeEnabled,
    decimal ServiceChargePercent,
    string RoundingMode,
    int RoundingStep);

public sealed record CurrencyDto(string Code, string Name, string Symbol, int DecimalPlaces);

public sealed record TestBillLineDto(string Label, decimal Amount, bool IsTotal);

/// <summary>A sample bill worked out from the saved settings, so the owner can check them.</summary>
public sealed record TestBillDto(IReadOnlyList<TestBillLineDto> Lines, decimal Total, string CurrencyCode);

// ----- Payment methods (FR-M05-03) -----

/// <summary>MethodType is Cash, Card, Online, Credit, GiftCard or Wallet.</summary>
public sealed record PaymentMethodDto(
    Guid Id,
    string Name,
    string MethodType,
    bool OpensDrawer,
    bool RequiresReference,
    bool IsActive,
    int SortOrder);

/// <summary>Id null creates a method. SortOrder 0 or less puts a new method at the end.</summary>
public sealed record SavePaymentMethodRequest(
    Guid? Id,
    string Name,
    string MethodType,
    bool OpensDrawer,
    bool RequiresReference,
    bool IsActive,
    int SortOrder);

// ----- Receipt design (FR-M05-04) -----

/// <summary>DocumentType is Invoice or Kot. Language is en or ur. One template per document type and language.</summary>
public sealed record ReceiptTemplateDto(
    Guid Id,
    string Name,
    string DocumentType,
    string Language,
    string? HeaderText,
    string? FooterText,
    bool ShowLogo,
    bool ShowQr,
    bool ShowTaxBreakdown,
    bool ShowTrialMark);

public sealed record SaveReceiptTemplateRequest(
    Guid? Id,
    string Name,
    string DocumentType,
    string Language,
    string? HeaderText,
    string? FooterText,
    bool ShowLogo,
    bool ShowQr,
    bool ShowTaxBreakdown,
    bool ShowTrialMark);

// ----- Document numbers (FR-M05-05) -----

/// <summary>DocumentType is Order, Invoice, Payment, Ticket or Shift. ResetPolicy is Never, Daily, Monthly or Yearly.</summary>
public sealed record NumberSequenceDto(
    string DocumentType,
    string Label,
    string Prefix,
    string? Suffix,
    int Padding,
    string ResetPolicy,
    long CurrentValue,
    string NextNumber);

public sealed record NumberSequenceInputDto(
    string DocumentType,
    string Prefix,
    string? Suffix,
    int Padding,
    string ResetPolicy);

public sealed record SaveNumberSequencesRequest(IReadOnlyList<NumberSequenceInputDto> Items);

// ----- Module switches and business-type templates (FR-M05-06) -----

public sealed record ModuleSwitchDto(string Key, string Name, string Description, bool IsEnabled);

public sealed record ModuleSwitchChangeDto(string Key, bool IsEnabled);

public sealed record SaveModuleSwitchesRequest(IReadOnlyList<ModuleSwitchChangeDto> Items);

public sealed record BusinessTemplateDto(
    string Code,
    string Name,
    string Description,
    bool IsCurrent,
    IReadOnlyList<ModuleSwitchChangeDto> Modules);
