using System.Globalization;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteSettingsService(RemoteApiClient api) : ISettingsService
{
    public Task<BusinessProfileDto> GetBusinessProfileAsync(CancellationToken ct = default) =>
        api.GetAsync<BusinessProfileDto>("api/v1/settings/business", ct);

    public Task SaveBusinessProfileAsync(SaveBusinessProfileRequest request, CancellationToken ct = default) =>
        api.PutAsync("api/v1/settings/business", request, ct);

    public Task SetLogoAsync(string? dataUri, CancellationToken ct = default) =>
        api.PutAsync("api/v1/settings/business/logo", new SetLogoRequest(dataUri), ct);

    public async Task<IReadOnlyList<BranchDetailDto>> GetBranchesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<BranchDetailDto>>("api/v1/settings/branches", ct);

    public async Task<Guid> SaveBranchAsync(SaveBranchRequest request, CancellationToken ct = default) =>
        (await api.PostAsync<Created>("api/v1/settings/branches", request, authenticated: true, ct)).Id;

    public async Task<IReadOnlyList<TaxRateDto>> GetTaxRatesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<TaxRateDto>>("api/v1/settings/tax-rates", ct);

    public async Task<Guid> SaveTaxRateAsync(SaveTaxRateRequest request, CancellationToken ct = default) =>
        (await api.PostAsync<Created>("api/v1/settings/tax-rates", request, authenticated: true, ct)).Id;

    public Task DeleteTaxRateAsync(Guid id, CancellationToken ct = default) =>
        api.DeleteAsync($"api/v1/settings/tax-rates/{id}", ct);

    public Task<BillingSettingsDto> GetBillingSettingsAsync(CancellationToken ct = default) =>
        api.GetAsync<BillingSettingsDto>("api/v1/settings/billing", ct);

    public Task SaveBillingSettingsAsync(BillingSettingsDto request, CancellationToken ct = default) =>
        api.PutAsync("api/v1/settings/billing", request, ct);

    public async Task<IReadOnlyList<CurrencyDto>> GetCurrenciesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<CurrencyDto>>("api/v1/settings/currencies", ct);

    public Task<TestBillDto> PreviewBillAsync(decimal subtotal, CancellationToken ct = default) =>
        api.GetAsync<TestBillDto>(
            $"api/v1/settings/billing/preview?subtotal={subtotal.ToString(CultureInfo.InvariantCulture)}",
            ct);

    private sealed record Created(Guid Id);

    public async Task<IReadOnlyList<PaymentMethodDto>> GetPaymentMethodsAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<PaymentMethodDto>>("api/v1/settings/payment-methods", ct);

    public async Task<Guid> SavePaymentMethodAsync(SavePaymentMethodRequest request, CancellationToken ct = default) =>
        (await api.PostAsync<Created>("api/v1/settings/payment-methods", request, authenticated: true, ct)).Id;

    public async Task<IReadOnlyList<ReceiptTemplateDto>> GetReceiptTemplatesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<ReceiptTemplateDto>>("api/v1/settings/receipt-templates", ct);

    public async Task<Guid> SaveReceiptTemplateAsync(SaveReceiptTemplateRequest request, CancellationToken ct = default) =>
        (await api.PostAsync<Created>("api/v1/settings/receipt-templates", request, authenticated: true, ct)).Id;

    public Task DeleteReceiptTemplateAsync(Guid id, CancellationToken ct = default) =>
        api.DeleteAsync($"api/v1/settings/receipt-templates/{id}", ct);

    public async Task<IReadOnlyList<NumberSequenceDto>> GetNumberSequencesAsync(Guid branchId, CancellationToken ct = default) =>
        await api.GetAsync<List<NumberSequenceDto>>($"api/v1/settings/number-sequences/{branchId}", ct);

    public Task SaveNumberSequencesAsync(Guid branchId, SaveNumberSequencesRequest request, CancellationToken ct = default) =>
        api.PutAsync($"api/v1/settings/number-sequences/{branchId}", request, ct);

    public async Task<IReadOnlyList<ModuleSwitchDto>> GetModuleSwitchesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<ModuleSwitchDto>>("api/v1/settings/modules", ct);

    public Task SaveModuleSwitchesAsync(SaveModuleSwitchesRequest request, CancellationToken ct = default) =>
        api.PutAsync("api/v1/settings/modules", request, ct);

    public async Task<IReadOnlyList<BusinessTemplateDto>> GetBusinessTemplatesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<BusinessTemplateDto>>("api/v1/settings/business-templates", ct);

    public Task ApplyBusinessTemplateAsync(string code, CancellationToken ct = default) =>
        api.PostAsync($"api/v1/settings/business-templates/{Uri.EscapeDataString(code)}/apply", null, authenticated: true, ct);
}
