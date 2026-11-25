using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Settings;

// FR-M05-06: module switches and business-type templates.

/// <summary>Works out which modules are on: a saved switch, else the template default of the business type, else on.</summary>
internal static class ModuleResolver
{
    public static async Task<Dictionary<string, bool>> ResolveAsync(IApplicationDbContext db, Guid tenantId, CancellationToken ct)
    {
        var businessType = await db.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => t.BusinessType)
            .FirstOrDefaultAsync(ct);

        var template = ModuleCatalog.Templates.FirstOrDefault(t => t.Code == businessType.ToString());

        var saved = (await db.Set<TenantFeature>().AsNoTracking()
                .Select(f => new { f.Feature.Key, f.IsEnabled })
                .ToListAsync(ct))
            .GroupBy(f => f.Key)
            .ToDictionary(g => g.Key, g => g.First().IsEnabled);

        var result = new Dictionary<string, bool>();

        foreach (var module in ModuleCatalog.All)
        {
            if (saved.TryGetValue(module.Key, out var on))
            {
                result[module.Key] = on;
            }
            else if (template is not null && template.Modules.TryGetValue(module.Key, out var byDefault))
            {
                result[module.Key] = byDefault;
            }
            else
            {
                result[module.Key] = true;
            }
        }

        return result;
    }

    /// <summary>Saves the given switches as TenantFeature rows (adds or changes; the caller saves).</summary>
    public static async Task WriteAsync(IApplicationDbContext db, Guid tenantId, IEnumerable<ModuleSwitchChangeDto> changes, CancellationToken ct)
    {
        var keys = changes.Select(c => c.Key).ToArray();

        var features = await db.Set<Feature>().Where(f => keys.Contains(f.Key)).ToDictionaryAsync(f => f.Key, ct);
        var rows = await db.Set<TenantFeature>().Include(f => f.Feature).Where(f => keys.Contains(f.Feature.Key)).ToListAsync(ct);

        foreach (var change in changes)
        {
            if (!features.TryGetValue(change.Key, out var feature))
            {
                throw new RequestRejectedException("module_not_ready", "The module list is not ready. Restart the application and try again.");
            }

            var row = rows.FirstOrDefault(r => r.Feature.Key == change.Key);

            if (row is null)
            {
                db.Set<TenantFeature>().Add(new TenantFeature { TenantId = tenantId, FeatureId = feature.Id, IsEnabled = change.IsEnabled });
            }
            else
            {
                row.IsEnabled = change.IsEnabled;
            }
        }
    }
}

/// <summary>No permission is needed besides signing in: the menu of every user hides switched-off modules.</summary>
public sealed record GetModuleSwitchesQuery : IRequest<IReadOnlyList<ModuleSwitchDto>>;

internal sealed class GetModuleSwitchesHandler(IApplicationDbContext db, ICurrentUser current)
    : IRequestHandler<GetModuleSwitchesQuery, IReadOnlyList<ModuleSwitchDto>>
{
    public async Task<IReadOnlyList<ModuleSwitchDto>> HandleAsync(GetModuleSwitchesQuery request, CancellationToken ct)
    {
        // The platform administrator has no business: every module is shown.
        var state = current.TenantId is { } tenantId
            ? await ModuleResolver.ResolveAsync(db, tenantId, ct)
            : ModuleCatalog.All.ToDictionary(m => m.Key, _ => true);

        return ModuleCatalog.All
            .Select(m => new ModuleSwitchDto(m.Key, m.Name, m.Description, state.GetValueOrDefault(m.Key, true)))
            .ToList();
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SaveModuleSwitchesCommand(IReadOnlyList<ModuleSwitchChangeDto> Items) : IRequest<bool>;

internal sealed class SaveModuleSwitchesValidator : AbstractValidator<SaveModuleSwitchesCommand>
{
    public SaveModuleSwitchesValidator() =>
        RuleFor(x => x.Items)
            .NotNull()
            .Must(items => items is not null && items.All(i => ModuleCatalog.All.Any(m => m.Key == i.Key)))
            .WithMessage("Unknown module.");
}

internal sealed class SaveModuleSwitchesHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SaveModuleSwitchesCommand, bool>
{
    public async Task<bool> HandleAsync(SaveModuleSwitchesCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Module switches belong to a business.");

        var before = await ModuleResolver.ResolveAsync(db, tenantId, ct);

        await ModuleResolver.WriteAsync(db, tenantId, request.Items, ct);

        audit.Add(
            "modules.updated",
            "Tenant",
            tenantId,
            oldValues: before,
            newValues: request.Items.ToDictionary(i => i.Key, i => i.IsEnabled));

        await db.SaveChangesAsync(ct);
        return true;
    }
}

[RequirePermission(Permissions.SettingsView)]
public sealed record GetBusinessTemplatesQuery : IRequest<IReadOnlyList<BusinessTemplateDto>>;

internal sealed class GetBusinessTemplatesHandler(IApplicationDbContext db, ICurrentUser current)
    : IRequestHandler<GetBusinessTemplatesQuery, IReadOnlyList<BusinessTemplateDto>>
{
    public async Task<IReadOnlyList<BusinessTemplateDto>> HandleAsync(GetBusinessTemplatesQuery request, CancellationToken ct)
    {
        string? currentCode = null;

        if (current.TenantId is { } tenantId)
        {
            currentCode = (await db.Tenants.AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => t.BusinessType)
                .FirstOrDefaultAsync(ct)).ToString();
        }

        return ModuleCatalog.Templates
            .Select(t => new BusinessTemplateDto(
                t.Code,
                t.Name,
                t.Description,
                t.Code == currentCode,
                t.Modules.Select(m => new ModuleSwitchChangeDto(m.Key, m.Value)).ToList()))
            .ToList();
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record ApplyBusinessTemplateCommand(string Code) : IRequest<bool>;

internal sealed class ApplyBusinessTemplateValidator : AbstractValidator<ApplyBusinessTemplateCommand>
{
    public ApplyBusinessTemplateValidator() =>
        RuleFor(x => x.Code)
            .Must(code => ModuleCatalog.Templates.Any(t => t.Code == code))
            .WithMessage("Choose a business type from the list.");
}

internal sealed class ApplyBusinessTemplateHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<ApplyBusinessTemplateCommand, bool>
{
    public async Task<bool> HandleAsync(ApplyBusinessTemplateCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Templates belong to a business.");

        var template = ModuleCatalog.Templates.First(t => t.Code == request.Code);

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new RequestRejectedException("not_found", "The business was not found.");

        // What the business had before, so the audit log can show how to go back.
        var oldType = tenant.BusinessType.ToString();
        var before = await ModuleResolver.ResolveAsync(db, tenantId, ct);

        tenant.SetBusinessType(Enum.Parse<BusinessType>(template.Code));

        await ModuleResolver.WriteAsync(
            db,
            tenantId,
            template.Modules.Select(m => new ModuleSwitchChangeDto(m.Key, m.Value)).ToList(),
            ct);

        audit.Add(
            "template.applied",
            "Tenant",
            tenantId,
            oldValues: new { BusinessType = oldType, Modules = before },
            newValues: new { BusinessType = template.Code, Modules = template.Modules });

        await db.SaveChangesAsync(ct);
        return true;
    }
}
