using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

/// <summary>Business settings (M05): profile and logo, branches, tax, service charge, currency and rounding.</summary>
public interface ISettingsService
{
    Task<BusinessProfileDto> GetBusinessProfileAsync(CancellationToken ct = default);

    Task SaveBusinessProfileAsync(SaveBusinessProfileRequest request, CancellationToken ct = default);

    /// <summary>Sets the logo (data URI of a PNG or JPEG up to about 200 KB) or removes it when null.</summary>
    Task SetLogoAsync(string? dataUri, CancellationToken ct = default);

    Task<IReadOnlyList<BranchDetailDto>> GetBranchesAsync(CancellationToken ct = default);

    /// <summary>Creates a branch (Id null) or changes an existing one.</summary>
    Task<Guid> SaveBranchAsync(SaveBranchRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<TaxRateDto>> GetTaxRatesAsync(CancellationToken ct = default);

    Task<Guid> SaveTaxRateAsync(SaveTaxRateRequest request, CancellationToken ct = default);

    Task DeleteTaxRateAsync(Guid id, CancellationToken ct = default);

    Task<BillingSettingsDto> GetBillingSettingsAsync(CancellationToken ct = default);

    Task SaveBillingSettingsAsync(BillingSettingsDto request, CancellationToken ct = default);

    Task<IReadOnlyList<CurrencyDto>> GetCurrenciesAsync(CancellationToken ct = default);

    /// <summary>Works out a sample bill for the given items total from the saved tax, service charge and rounding.</summary>
    Task<TestBillDto> PreviewBillAsync(decimal subtotal, CancellationToken ct = default);

    // Payment methods (FR-M05-03)
    Task<IReadOnlyList<PaymentMethodDto>> GetPaymentMethodsAsync(CancellationToken ct = default);

    Task<Guid> SavePaymentMethodAsync(SavePaymentMethodRequest request, CancellationToken ct = default);

    // Receipt design (FR-M05-04)
    Task<IReadOnlyList<ReceiptTemplateDto>> GetReceiptTemplatesAsync(CancellationToken ct = default);

    Task<Guid> SaveReceiptTemplateAsync(SaveReceiptTemplateRequest request, CancellationToken ct = default);

    Task DeleteReceiptTemplateAsync(Guid id, CancellationToken ct = default);

    // Document numbers (FR-M05-05)
    Task<IReadOnlyList<NumberSequenceDto>> GetNumberSequencesAsync(Guid branchId, CancellationToken ct = default);

    Task SaveNumberSequencesAsync(Guid branchId, SaveNumberSequencesRequest request, CancellationToken ct = default);

    // Module switches and business-type templates (FR-M05-06)

    /// <summary>The switches of the signed-in business (any signed-in user may read them, so the menu can hide modules).</summary>
    Task<IReadOnlyList<ModuleSwitchDto>> GetModuleSwitchesAsync(CancellationToken ct = default);

    Task SaveModuleSwitchesAsync(SaveModuleSwitchesRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<BusinessTemplateDto>> GetBusinessTemplatesAsync(CancellationToken ct = default);

    /// <summary>Sets the business type and the module switches of the template (Restaurant, Cafe, Bakery, FastFood or Retail).</summary>
    Task ApplyBusinessTemplateAsync(string code, CancellationToken ct = default);
}
