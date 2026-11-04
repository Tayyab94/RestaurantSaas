using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Auth;

/// <summary>
/// A new business signs up (cloud), or the one business of a desktop or on-premise installation is set up for the first time.
/// It creates the tenant (14-day trial), its main branch, the system roles, the starting data and the Owner user.
/// The phone number is required and can be used by one business only.
/// (A verification code for the phone or email is added in a later step.)
/// </summary>
[AllowAnonymousRequest]
public sealed record RegisterBusinessCommand(
    string BusinessName,
    string BusinessType,
    string OwnerFullName,
    string UserName,
    string? Email,
    string? Phone,
    string Password) : IRequest<RegisterBusinessResultDto>;

internal sealed class RegisterBusinessValidator : AbstractValidator<RegisterBusinessCommand>
{
    public RegisterBusinessValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MinimumLength(3).MaximumLength(150);
        RuleFor(x => x.BusinessType).Must(v => Enum.TryParse<BusinessType>(v, true, out _)).WithMessage("Choose a valid business type.");
        RuleFor(x => x.OwnerFullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(50);
          //  .Matches("^[A-Za-z0-9._-]+$").WithMessage("Use letters, digits, dot, dash or underscore only.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("A phone number is required.")
            .Must(p => PhoneNumber.TryNormalize(p, out _)).WithMessage("Enter a valid phone number, for example 0300 1234567.");
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Password).MustBeStrongPassword();
    }
}

internal sealed class RegisterBusinessHandler(IApplicationDbContext db, TenantProvisioner provisioner, IDeploymentInfo deployment)
    : IRequestHandler<RegisterBusinessCommand, RegisterBusinessResultDto>
{
    public async Task<RegisterBusinessResultDto> HandleAsync(RegisterBusinessCommand request, CancellationToken ct)
    {
        // A desktop or on-premise installation holds exactly one business, which is set up once.
        if (deployment.Mode != DeploymentMode.Cloud
            && await db.Tenants.IgnoreQueryFilters().AnyAsync(t => !t.IsDeleted, ct))
        {
            throw new RequestRejectedException("already_configured", "This installation is already set up. Please sign in.");
        }

        PhoneNumber.TryNormalize(request.Phone, out var phone);
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLowerInvariant();

        if (await db.Tenants.IgnoreQueryFilters().AnyAsync(t => !t.IsDeleted && t.Phone == phone, ct))
        {
            throw new RequestRejectedException("phone_in_use", "A business is already registered with this phone number. Please sign in, or use another number.");
        }

        if (email is not null && await db.Tenants.IgnoreQueryFilters().AnyAsync(t => !t.IsDeleted && t.Email == email, ct))
        {
            throw new RequestRejectedException("email_in_use", "A business is already registered with this email address. Please sign in, or use another address.");
        }

        var result = await provisioner.ProvisionAsync(
            new ProvisionTenantRequest(
                request.BusinessName,
                Enum.Parse<BusinessType>(request.BusinessType, true),
                deployment.Mode,
                request.OwnerFullName,
                request.UserName,
                request.Password,
                email,
                phone),
            ct);

        return new RegisterBusinessResultDto(result.Tenant.Code, result.Owner.UserName, result.Tenant.TrialEndsAt ?? DateTime.UtcNow);
    }
}
