using RestaurantSaaS.Application.Features.Settings;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalSettingsService(LocalExecutor executor) : ISettingsService
{
    public Task<BusinessProfileDto> GetBusinessProfileAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetBusinessProfileQuery(), ct);

    public async Task SaveBusinessProfileAsync(SaveBusinessProfileRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(
            new SaveBusinessProfileCommand(
                request.Name, request.OwnerName, request.Email, request.Phone, request.BusinessType,
                request.Address, request.TaxNumber, request.TimeZone, request.DefaultLanguage),
            ct);

    public async Task SetLogoAsync(string? dataUri, CancellationToken ct = default) =>
        await executor.SendAsync(new SetBusinessLogoCommand(dataUri), ct);

    public Task<IReadOnlyList<BranchDetailDto>> GetBranchesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetBranchDetailsQuery(), ct);

    public Task<Guid> SaveBranchAsync(SaveBranchRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new SaveBranchCommand(request.Id, request.Code, request.Name, request.City, request.Address, request.Phone, request.OpeningHours),
            ct);

    public Task<IReadOnlyList<TaxRateDto>> GetTaxRatesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetTaxRatesQuery(), ct);

    public Task<Guid> SaveTaxRateAsync(SaveTaxRateRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new SaveTaxRateCommand(
                request.Id, request.Name, request.Rate, request.TaxType, request.AppliesTo,
                request.IsInclusive, request.IsDefault, request.FbrTaxCode),
            ct);

    public async Task DeleteTaxRateAsync(Guid id, CancellationToken ct = default) =>
        await executor.SendAsync(new DeleteTaxRateCommand(id), ct);

    public Task<BillingSettingsDto> GetBillingSettingsAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetBillingSettingsQuery(), ct);

    public async Task SaveBillingSettingsAsync(BillingSettingsDto request, CancellationToken ct = default) =>
        await executor.SendAsync(
            new SaveBillingSettingsCommand(
                request.CurrencyCode, request.ServiceChargeEnabled, request.ServiceChargePercent,
                request.RoundingMode, request.RoundingStep),
            ct);

    public Task<IReadOnlyList<CurrencyDto>> GetCurrenciesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetCurrenciesQuery(), ct);

    public Task<TestBillDto> PreviewBillAsync(decimal subtotal, CancellationToken ct = default) =>
        executor.SendAsync(new PreviewTestBillQuery(subtotal), ct);

    public Task<IReadOnlyList<PaymentMethodDto>> GetPaymentMethodsAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetPaymentMethodsQuery(), ct);

    public Task<Guid> SavePaymentMethodAsync(SavePaymentMethodRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new SavePaymentMethodCommand(
                request.Id, request.Name, request.MethodType, request.OpensDrawer,
                request.RequiresReference, request.IsActive, request.SortOrder),
            ct);

    public Task<IReadOnlyList<ReceiptTemplateDto>> GetReceiptTemplatesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetReceiptTemplatesQuery(), ct);

    public Task<Guid> SaveReceiptTemplateAsync(SaveReceiptTemplateRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new SaveReceiptTemplateCommand(
                request.Id, request.Name, request.DocumentType, request.Language, request.HeaderText, request.FooterText,
                request.ShowLogo, request.ShowQr, request.ShowTaxBreakdown, request.ShowTrialMark),
            ct);

    public async Task DeleteReceiptTemplateAsync(Guid id, CancellationToken ct = default) =>
        await executor.SendAsync(new DeleteReceiptTemplateCommand(id), ct);

    public Task<IReadOnlyList<NumberSequenceDto>> GetNumberSequencesAsync(Guid branchId, CancellationToken ct = default) =>
        executor.SendAsync(new GetNumberSequencesQuery(branchId), ct);

    public async Task SaveNumberSequencesAsync(Guid branchId, SaveNumberSequencesRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(new SaveNumberSequencesCommand(branchId, request.Items), ct);

    public Task<IReadOnlyList<ModuleSwitchDto>> GetModuleSwitchesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetModuleSwitchesQuery(), ct);

    public async Task SaveModuleSwitchesAsync(SaveModuleSwitchesRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(new SaveModuleSwitchesCommand(request.Items), ct);

    public Task<IReadOnlyList<BusinessTemplateDto>> GetBusinessTemplatesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetBusinessTemplatesQuery(), ct);

    public async Task ApplyBusinessTemplateAsync(string code, CancellationToken ct = default) =>
        await executor.SendAsync(new ApplyBusinessTemplateCommand(code), ct);
}
