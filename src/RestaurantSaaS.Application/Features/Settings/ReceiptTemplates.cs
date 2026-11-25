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

// FR-M05-04: receipt design (header, footer, logo, QR, tax breakdown) per language.
// One template per document type (Invoice or Kot) and language. Printing itself comes in M10.

[RequirePermission(Permissions.SettingsView)]
public sealed record GetReceiptTemplatesQuery : IRequest<IReadOnlyList<ReceiptTemplateDto>>;

internal sealed class GetReceiptTemplatesHandler(IApplicationDbContext db)
    : IRequestHandler<GetReceiptTemplatesQuery, IReadOnlyList<ReceiptTemplateDto>>
{
    public async Task<IReadOnlyList<ReceiptTemplateDto>> HandleAsync(GetReceiptTemplatesQuery request, CancellationToken ct)
    {
        var rows = await db.Set<ReceiptTemplate>().AsNoTracking()
            .OrderBy(t => t.DocumentType).ThenBy(t => t.Language)
            .ToListAsync(ct);

        return rows
            .Select(t => new ReceiptTemplateDto(
                t.Id, t.Name, t.DocumentType.ToString(), t.Language, t.HeaderText, t.FooterText,
                t.ShowLogo, t.ShowQr, t.ShowTaxBreakdown, t.ShowTrialMark))
            .ToList();
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SaveReceiptTemplateCommand(
    Guid? Id,
    string Name,
    string DocumentType,
    string Language,
    string? HeaderText,
    string? FooterText,
    bool ShowLogo,
    bool ShowQr,
    bool ShowTaxBreakdown,
    bool ShowTrialMark) : IRequest<Guid>;

internal sealed class SaveReceiptTemplateValidator : AbstractValidator<SaveReceiptTemplateCommand>
{
    public SaveReceiptTemplateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.DocumentType)
            .Must(value => value is nameof(DocumentType.Invoice) or nameof(DocumentType.Kot))
            .WithMessage("Choose Invoice or Kitchen ticket.");

        RuleFor(x => x.Language)
            .Must(value => value is "en" or "ur")
            .WithMessage("Choose English or Urdu.");

        RuleFor(x => x.HeaderText).MaximumLength(1000);
        RuleFor(x => x.FooterText).MaximumLength(1000);
    }
}

internal sealed class SaveReceiptTemplateHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SaveReceiptTemplateCommand, Guid>
{
    public async Task<Guid> HandleAsync(SaveReceiptTemplateCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Receipt templates belong to a business.");

        var type = Enum.Parse<DocumentType>(request.DocumentType);
        var isNew = request.Id is null;

        if (await db.Set<ReceiptTemplate>().AnyAsync(t => t.DocumentType == type && t.Language == request.Language && t.Id != request.Id, ct))
        {
            throw new RequestRejectedException("template_exists", "There is already a template for this document and language. Edit that one.");
        }

        ReceiptTemplate template;

        if (isNew)
        {
            template = new ReceiptTemplate { TenantId = tenantId };
            db.Set<ReceiptTemplate>().Add(template);
        }
        else
        {
            template = await db.Set<ReceiptTemplate>().FirstOrDefaultAsync(t => t.Id == request.Id, ct)
                ?? throw new RequestRejectedException("template_not_found", "The template does not exist.");
        }

        template.Name = request.Name.Trim();
        template.DocumentType = type;
        template.Language = request.Language;
        template.HeaderText = string.IsNullOrWhiteSpace(request.HeaderText) ? null : request.HeaderText.Trim();
        template.FooterText = string.IsNullOrWhiteSpace(request.FooterText) ? null : request.FooterText.Trim();
        template.ShowLogo = request.ShowLogo;
        template.ShowQr = request.ShowQr;
        template.ShowTaxBreakdown = request.ShowTaxBreakdown;
        template.ShowTrialMark = request.ShowTrialMark;

        audit.Add(
            isNew ? "receipt_template.created" : "receipt_template.updated",
            "ReceiptTemplate",
            template.Id,
            newValues: new { template.Name, request.DocumentType, template.Language });

        await db.SaveChangesAsync(ct);
        return template.Id;
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record DeleteReceiptTemplateCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteReceiptTemplateHandler(IApplicationDbContext db, AuditTrail audit)
    : IRequestHandler<DeleteReceiptTemplateCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteReceiptTemplateCommand request, CancellationToken ct)
    {
        var template = await db.Set<ReceiptTemplate>().FirstOrDefaultAsync(t => t.Id == request.Id, ct)
            ?? throw new RequestRejectedException("template_not_found", "The template does not exist.");

        db.Set<ReceiptTemplate>().Remove(template);
        audit.Add("receipt_template.deleted", "ReceiptTemplate", template.Id, oldValues: new { template.Name });

        await db.SaveChangesAsync(ct);
        return true;
    }
}
