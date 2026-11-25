using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Settings;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Web.Endpoints.Modules;

/// <summary>M05 Business Settings. Thin endpoints: the handlers check the permissions.</summary>
public static class SettingsEndpoints
{
    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/settings/business", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetBusinessProfileQuery(), ct)))
            .WithTags("Settings");

        app.MapPut("/settings/business", async (SaveBusinessProfileRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(
                    new SaveBusinessProfileCommand(r.Name, r.OwnerName, r.Email, r.Phone, r.BusinessType, r.Address, r.TaxNumber, r.TimeZone, r.DefaultLanguage),
                    ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapPut("/settings/business/logo", async (SetLogoRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SetBusinessLogoCommand(r.DataUri), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapGet("/settings/branches", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetBranchDetailsQuery(), ct)))
            .WithTags("Settings");

        app.MapPost("/settings/branches", async (SaveBranchRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.SendAsync(
                    new SaveBranchCommand(r.Id, r.Code, r.Name, r.City, r.Address, r.Phone, r.OpeningHours), ct);
                return Results.Ok(new { id });
            })
            .WithTags("Settings");

        app.MapGet("/settings/tax-rates", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetTaxRatesQuery(), ct)))
            .WithTags("Settings");

        app.MapPost("/settings/tax-rates", async (SaveTaxRateRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.SendAsync(
                    new SaveTaxRateCommand(r.Id, r.Name, r.Rate, r.TaxType, r.AppliesTo, r.IsInclusive, r.IsDefault, r.FbrTaxCode), ct);
                return Results.Ok(new { id });
            })
            .WithTags("Settings");

        app.MapDelete("/settings/tax-rates/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new DeleteTaxRateCommand(id), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapGet("/settings/billing", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetBillingSettingsQuery(), ct)))
            .WithTags("Settings");

        app.MapPut("/settings/billing", async (BillingSettingsDto r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(
                    new SaveBillingSettingsCommand(r.CurrencyCode, r.ServiceChargeEnabled, r.ServiceChargePercent, r.RoundingMode, r.RoundingStep), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapGet("/settings/billing/preview", async (decimal subtotal, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new PreviewTestBillQuery(subtotal), ct)))
            .WithTags("Settings");

        app.MapGet("/settings/currencies", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetCurrenciesQuery(), ct)))
            .WithTags("Settings");

        app.MapGet("/settings/payment-methods", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetPaymentMethodsQuery(), ct)))
            .WithTags("Settings");

        app.MapPost("/settings/payment-methods", async (SavePaymentMethodRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.SendAsync(
                    new SavePaymentMethodCommand(r.Id, r.Name, r.MethodType, r.OpensDrawer, r.RequiresReference, r.IsActive, r.SortOrder), ct);
                return Results.Ok(new { id });
            })
            .WithTags("Settings");

        app.MapGet("/settings/receipt-templates", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetReceiptTemplatesQuery(), ct)))
            .WithTags("Settings");

        app.MapPost("/settings/receipt-templates", async (SaveReceiptTemplateRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.SendAsync(
                    new SaveReceiptTemplateCommand(
                        r.Id, r.Name, r.DocumentType, r.Language, r.HeaderText, r.FooterText,
                        r.ShowLogo, r.ShowQr, r.ShowTaxBreakdown, r.ShowTrialMark),
                    ct);
                return Results.Ok(new { id });
            })
            .WithTags("Settings");

        app.MapDelete("/settings/receipt-templates/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new DeleteReceiptTemplateCommand(id), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapGet("/settings/number-sequences/{branchId:guid}", async (Guid branchId, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetNumberSequencesQuery(branchId), ct)))
            .WithTags("Settings");

        app.MapPut("/settings/number-sequences/{branchId:guid}", async (Guid branchId, SaveNumberSequencesRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SaveNumberSequencesCommand(branchId, r.Items), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapGet("/settings/modules", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetModuleSwitchesQuery(), ct)))
            .WithTags("Settings");

        app.MapPut("/settings/modules", async (SaveModuleSwitchesRequest r, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SaveModuleSwitchesCommand(r.Items), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        app.MapGet("/settings/business-templates", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetBusinessTemplatesQuery(), ct)))
            .WithTags("Settings");

        app.MapPost("/settings/business-templates/{code}/apply", async (string code, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new ApplyBusinessTemplateCommand(code), ct);
                return Results.NoContent();
            })
            .WithTags("Settings");

        return app;
    }
}
