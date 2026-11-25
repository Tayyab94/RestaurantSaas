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

// FR-M05-03: payment methods (name, type, opens the drawer, needs a reference).
// A method is switched off, never deleted, because payments (M11) will point to it.

[RequirePermission(Permissions.SettingsView)]
public sealed record GetPaymentMethodsQuery : IRequest<IReadOnlyList<PaymentMethodDto>>;

internal sealed class GetPaymentMethodsHandler(IApplicationDbContext db)
    : IRequestHandler<GetPaymentMethodsQuery, IReadOnlyList<PaymentMethodDto>>
{
    public async Task<IReadOnlyList<PaymentMethodDto>> HandleAsync(GetPaymentMethodsQuery request, CancellationToken ct)
    {
        var rows = await db.Set<PaymentMethod>().AsNoTracking()
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
            .ToListAsync(ct);

        return rows
            .Select(p => new PaymentMethodDto(p.Id, p.Name, p.MethodType.ToString(), p.OpensDrawer, p.RequiresReference, p.IsActive, p.SortOrder))
            .ToList();
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SavePaymentMethodCommand(
    Guid? Id,
    string Name,
    string MethodType,
    bool OpensDrawer,
    bool RequiresReference,
    bool IsActive,
    int SortOrder) : IRequest<Guid>;

internal sealed class SavePaymentMethodValidator : AbstractValidator<SavePaymentMethodCommand>
{
    public SavePaymentMethodValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.MethodType)
            .Must(value => Enum.TryParse<PaymentMethodMethodType>(value, out _))
            .WithMessage("Choose a payment type.");

        RuleFor(x => x.SortOrder).InclusiveBetween(0, 1000);
    }
}

internal sealed class SavePaymentMethodHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SavePaymentMethodCommand, Guid>
{
    public async Task<Guid> HandleAsync(SavePaymentMethodCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Payment methods belong to a business.");

        var name = request.Name.Trim();
        var isNew = request.Id is null;

        if (await db.Set<PaymentMethod>().AnyAsync(p => p.Name == name && p.Id != request.Id, ct))
        {
            throw new RequestRejectedException("payment_method_exists", "A payment method with this name already exists.");
        }

        PaymentMethod method;

        if (isNew)
        {
            var last = await db.Set<PaymentMethod>().Select(p => (int?)p.SortOrder).MaxAsync(ct) ?? 0;

            method = new PaymentMethod { TenantId = tenantId, SortOrder = request.SortOrder > 0 ? request.SortOrder : last + 1 };
            db.Set<PaymentMethod>().Add(method);
        }
        else
        {
            method = await db.Set<PaymentMethod>().FirstOrDefaultAsync(p => p.Id == request.Id, ct)
                ?? throw new RequestRejectedException("payment_method_not_found", "The payment method does not exist.");

            if (request.SortOrder > 0)
            {
                method.SortOrder = request.SortOrder;
            }
        }

        // The shop must always be able to take a payment.
        if (!request.IsActive && !isNew
            && !await db.Set<PaymentMethod>().AnyAsync(p => p.IsActive && p.Id != method.Id, ct))
        {
            throw new RequestRejectedException("last_payment_method", "At least one payment method must stay active.");
        }

        method.Name = name;
        method.MethodType = Enum.Parse<PaymentMethodMethodType>(request.MethodType);
        method.OpensDrawer = request.OpensDrawer;
        method.RequiresReference = request.RequiresReference;
        method.IsActive = request.IsActive;

        audit.Add(
            isNew ? "payment_method.created" : "payment_method.updated",
            "PaymentMethod",
            method.Id,
            newValues: new { method.Name, request.MethodType, method.OpensDrawer, method.RequiresReference, method.IsActive });

        await db.SaveChangesAsync(ct);
        return method.Id;
    }
}
