using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Common.Paging;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Licensing;

// ================= client side =================

[AllowAnonymousRequest]
public sealed record GetLicenseStatusQuery : IRequest<LicenseStatusDto>;

internal sealed class GetLicenseStatusHandler(ILicenseGate gate, ICurrentUser user)
    : IRequestHandler<GetLicenseStatusQuery, LicenseStatusDto>
{
    public async Task<LicenseStatusDto> HandleAsync(GetLicenseStatusQuery request, CancellationToken ct)
    {
        var status = await gate.GetStatusAsync(ct);
        return user.IsAuthenticated ? status : status with { LicensedTo = null, KeyHint = null };
    }
}

[AllowAnonymousRequest]
public sealed record ActivateLicenseCommand(string Key, string UserName) : IRequest<LicenseStatusDto>;

internal sealed class ActivateLicenseValidator : AbstractValidator<ActivateLicenseCommand>
{
    public ActivateLicenseValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(60);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(200);
    }
}

internal sealed class ActivateLicenseHandler(
    IDeploymentInfo deployment, ICurrentUser user, ILicenseGate gate, ILicenseServerClient server,
    ILicenseFileStore files, ILicenseKeyRing keys, IMachineFingerprint machine,
    LicenseServerService serverService, IApplicationDbContext db, AuditTrail audit)
    : IRequestHandler<ActivateLicenseCommand, LicenseStatusDto>
{
    public async Task<LicenseStatusDto> HandleAsync(ActivateLicenseCommand request, CancellationToken ct)
    {
        if (deployment.Mode == DeploymentMode.Cloud)
        {
            return await ActivateCloudAsync(request, ct);
        }

        var current = await gate.RefreshAsync(LicenseCheckTrigger.Startup, ct);
        var openToAnyone = current.State is LicenseState.NotActivated or LicenseState.Expired;

        if (!openToAnyone)
        {
            RequireSignedInWithPermission();
        }

        var oldContent = files.Read();

        var me = machine.Get();
        var issue = await server.ActivateAsync(
            new ServerActivateRequest(
                request.Key, request.UserName.Trim(), me.Hash, me.Parts,
                Environment.MachineName, Environment.OSVersion.VersionString,
                typeof(ActivateLicenseHandler).Assembly.GetName().Version?.ToString(), deployment.Mode.ToString()),
            ct);

        //return await StoreAndCheckAsync(issue, ct);
        var status = await StoreAndCheckAsync(issue, ct);
        await FreeReplacedKeyAsync(oldContent, issue, ct);
        return status;
    }

    /// The owner changed the key. The new key is already active, so the old paid key is freed
    /// on the License Server (it can be used on another PC). A trial needs nothing: the server
    /// marks it as replaced. The same key again needs nothing.
    private async Task FreeReplacedKeyAsync(string? oldContent, LicenseIssueDto issue, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(oldContent)
            || !LicenseFileCodec.TryOpen(oldContent, keys, out var old, out _, out _)
            || !LicenseFileCodec.TryOpen(issue.LicenseFile, keys, out var fresh, out _, out _))
        {
            return;
        }

        if (old.LicenseId == fresh.LicenseId
            || string.Equals(old.LicenseType, nameof(LicenseType.Trial), StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            await server.DeactivateAsync(new ServerDeactivateRequest(oldContent, "key_changed"), ct);
        }
        catch (RequestRejectedException)
        {
            // The new key works already. If the old key stays marked as used,
            // the administrator can free it with "Free PCs".
        }
    }

    private async Task<LicenseStatusDto> ActivateCloudAsync(ActivateLicenseCommand request, CancellationToken ct)
    {
        RequireSignedInWithPermission();

        if (user.TenantId is not { } tenantId)
        {
            throw new RequestRejectedException("no_tenant", "A license key is entered by a business.");
        }

        await serverService.ActivateForTenantAsync(request.Key, request.UserName.Trim(), tenantId, user.UserId, ct);

        audit.Add("License.Activated", nameof(Subscription), tenantId);
        await db.SaveChangesAsync(ct);

        return await gate.RefreshAsync(LicenseCheckTrigger.ServerRequest, ct);
    }

    private async Task<LicenseStatusDto> StoreAndCheckAsync(LicenseIssueDto issue, CancellationToken ct)
    {
        if (!LicenseFileCodec.TryOpen(issue.LicenseFile, keys, out var payload, out _, out _))
        {
            throw new RequestRejectedException("license_file_invalid", "The license received is not valid. Please contact support.");
        }

        var me = machine.Get();
        if (!FingerprintMatcher.Matches(payload.Fingerprint, payload.FingerprintParts, me.Hash, me.Parts))
        {
            throw new RequestRejectedException("license_file_other_pc", "The license received is for another PC. Please contact support.");
        }

        files.Write(issue.LicenseFile);
        var status = await gate.RefreshAsync(LicenseCheckTrigger.Startup, ct);

        if (user.TenantId is not null)
        {
            audit.Add("License.Activated", nameof(LicenseRecord), payload.LicenseId, null, new { payload.KeyHint, payload.LicenseType, payload.ExpiresAt });
            await db.SaveChangesAsync(ct);
        }

        return status;
    }

    private void RequireSignedInWithPermission()
    {
        if (!user.IsAuthenticated)
        {
            throw new UnauthorizedException("unauthenticated", "Please sign in to change the license.");
        }

        if (!user.HasPermission(Permissions.LicenseActivate))
        {
            throw new ForbiddenException($"You do not have the permission '{Permissions.LicenseActivate}'.");
        }
    }
}

[AllowAnonymousRequest]
public sealed record StartTrialCommand(string UserName, string Phone, string? Email) : IRequest<LicenseStatusDto>;

internal sealed class StartTrialValidator : AbstractValidator<StartTrialCommand>
{
    public StartTrialValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().WithMessage("A phone number is required.")
            .Must(p => PhoneNumber.TryNormalize(p, out _)).WithMessage("Enter a valid phone number, for example 0300 1234567.");
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

internal sealed class StartTrialHandler(
    IDeploymentInfo deployment, ILicenseGate gate, ILicenseServerClient server,
    ILicenseFileStore files, ILicenseKeyRing keys, IMachineFingerprint machine)
    : IRequestHandler<StartTrialCommand, LicenseStatusDto>
{
    public async Task<LicenseStatusDto> HandleAsync(StartTrialCommand request, CancellationToken ct)
    {
        if (deployment.Mode == DeploymentMode.Cloud)
        {
            throw new RequestRejectedException("trial_automatic", "A cloud business starts with a 14-day free trial when it signs up.");
        }

        var current = await gate.RefreshAsync(LicenseCheckTrigger.Startup, ct);
        if (current.State != LicenseState.NotActivated)
        {
            throw new RequestRejectedException("already_licensed", "This installation already has a license.");
        }

        var me = machine.Get();
        var issue = await server.StartTrialAsync(
            new ServerTrialRequest(
                request.UserName.Trim(), request.Phone, request.Email, me.Hash, me.Parts,
                Environment.MachineName, Environment.OSVersion.VersionString,
                typeof(StartTrialHandler).Assembly.GetName().Version?.ToString(), deployment.Mode.ToString()),
            ct);

        if (!LicenseFileCodec.TryOpen(issue.LicenseFile, keys, out var payload, out _, out _)
            || !FingerprintMatcher.Matches(payload.Fingerprint, payload.FingerprintParts, me.Hash, me.Parts))
        {
            throw new RequestRejectedException("license_file_invalid", "The license received is not valid. Please contact support.");
        }

        files.Write(issue.LicenseFile);
        return await gate.RefreshAsync(LicenseCheckTrigger.Startup, ct);
    }
}


/// The owner deactivates this installation. The License Server frees the PC, then the local
/// license file is removed. The same key can be entered again later (here or on another PC).
/// Not for trials, and not for the cloud (a cloud license belongs to the business).
[RequirePermission(Permissions.LicenseDeactivate)]
public sealed record DeactivateLicenseCommand : IRequest<LicenseStatusDto>;

internal sealed class DeactivateLicenseHandler(
    IDeploymentInfo deployment,
    ILicenseGate gate,
    ILicenseServerClient server,
    ILicenseFileStore files,
    ILicenseKeyRing keys,
    IApplicationDbContext db,
    AuditTrail audit)
    : IRequestHandler<DeactivateLicenseCommand, LicenseStatusDto>
{
    public async Task<LicenseStatusDto> HandleAsync(DeactivateLicenseCommand request, CancellationToken ct)
    {
        if (deployment.Mode == DeploymentMode.Cloud)
        {
            throw new RequestRejectedException("cloud_not_supported", "A cloud license belongs to the business. Please contact support to change it.");
        }

        var content = files.Read();
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new RequestRejectedException("not_activated", "This installation is not activated.");
        }

        if (!LicenseFileCodec.TryOpen(content, keys, out var payload, out _, out _))
        {
            throw new RequestRejectedException("license_file_invalid", "The license file is not valid, so it cannot be deactivated. Please contact support.");
        }

        //if (string.Equals(payload.LicenseType, nameof(LicenseType.Trial), StringComparison.OrdinalIgnoreCase))
        //{
        //    throw new RequestRejectedException("trial_cannot_deactivate", "A free trial cannot be deactivated. Enter a license key to continue.");
        //}

        // The server must confirm first. If it cannot be reached, nothing changes here.
        await server.DeactivateAsync(new ServerDeactivateRequest(content, "owner"), ct);

        try
        {
            files.Delete();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new RequestRejectedException("file_delete_failed", "The license was freed on the server, but the file license.lic could not be deleted. Delete it by hand and restart the app.");
        }

        if (await db.Tenants.IgnoreQueryFilters().AnyAsync(t => !t.IsDeleted, ct))
        {
            audit.Add("License.Deactivated", nameof(LicenseRecord), payload.LicenseId, null, new { payload.KeyHint, payload.LicenseType });
            await db.SaveChangesAsync(ct);
        }

        return await gate.RefreshAsync(LicenseCheckTrigger.Startup, ct);
    }
}

// ================= License Server side =================

[RequirePermission(Permissions.LicenseGenerate)]
public sealed record GenerateLicenseKeysCommand(GenerateLicenseKeysRequest Request) : IRequest<GeneratedLicenseKeysDto>;

internal sealed class GenerateLicenseKeysValidator : AbstractValidator<GenerateLicenseKeysCommand>
{
    public GenerateLicenseKeysValidator()
    {
        RuleFor(x => x.Request.LicenseType).Must(v => Enum.TryParse<LicenseType>(v, true, out var t) && t != LicenseType.Trial)
            .WithMessage("Choose 6 months, 12 months or lifetime.");
        RuleFor(x => x.Request.Quantity).InclusiveBetween(1, 500);
        RuleFor(x => x.Request.MaxActivations).InclusiveBetween(1, 10);
        RuleFor(x => x.Request.MaxDevices).InclusiveBetween(1, 100);
        RuleFor(x => x.Request.MaxBranches).InclusiveBetween(1, 100);
        RuleFor(x => x.Request.Notes).MaximumLength(1000);
        RuleFor(x => x.Request.SoldPrice).GreaterThanOrEqualTo(0).When(x => x.Request.SoldPrice.HasValue);
    }
}

internal sealed class GenerateLicenseKeysHandler(LicenseServerService server, ICurrentUser user)
    : IRequestHandler<GenerateLicenseKeysCommand, GeneratedLicenseKeysDto>
{
    public Task<GeneratedLicenseKeysDto> HandleAsync(GenerateLicenseKeysCommand request, CancellationToken ct) =>
        server.GenerateKeysAsync(request.Request, user.UserId, ct);
}

[RequirePermission(Permissions.LicenseGenerate)]
public sealed record GetLicenseKeysPageQuery(PageRequest Page) : IRequest<PagedResult<LicenseKeyListItemDto>>;

internal sealed class GetLicenseKeysPageHandler(IApplicationDbContext db)
    : IRequestHandler<GetLicenseKeysPageQuery, PagedResult<LicenseKeyListItemDto>>
{
    public async Task<PagedResult<LicenseKeyListItemDto>> HandleAsync(GetLicenseKeysPageQuery request, CancellationToken ct)
    {
        var page = request.Page;
        var query = db.Set<License>().AsNoTracking().Where(l => l.LicenseType != LicenseType.Trial);

        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var term = page.Search.Trim();
            query = query.Where(l => l.KeyHint.Contains(term) || (l.LicensedUserName != null && l.LicensedUserName.Contains(term)));
        }

        var sorted = (page.Sort?.ToLowerInvariant(), page.Desc) switch
        {
            (null, _) => query.OrderByDescending(l => l.CreatedAt).ThenByDescending(l => l.Id),
            ("createdat", false) => query.OrderBy(l => l.CreatedAt).ThenBy(l => l.Id),
            ("createdat", true) => query.OrderByDescending(l => l.CreatedAt).ThenByDescending(l => l.Id),
            ("status", false) => query.OrderBy(l => l.Status).ThenBy(l => l.Id),
            ("status", true) => query.OrderByDescending(l => l.Status).ThenByDescending(l => l.Id),
            ("expiresat", false) => query.OrderBy(l => l.ExpiresAt).ThenBy(l => l.Id),
            ("expiresat", true) => query.OrderByDescending(l => l.ExpiresAt).ThenByDescending(l => l.Id),
            _ => throw new RequestRejectedException("invalid_sort", $"Sorting by '{page.Sort}' is not allowed."),
        };

        var rows = await sorted
            .Select(l => new Row(l.Id, l.KeyHint, l.LicenseType, l.Plan.Name, l.Status, l.ActivationCount, l.MaxActivations, l.LicensedUserName, l.ExpiresAt, l.CreatedAt))
            .ToPagedResultAsync(page, ct);

        var items = rows.Items
            .Select(r => new LicenseKeyListItemDto(
                r.Id, r.KeyHint, LicensePolicy.TypeName(r.Type), r.PlanName, r.Status.ToString(), r.ActivationCount, r.MaxActivations, r.LicensedUserName, r.ExpiresAt, r.CreatedAt))
            .ToList();

        return new PagedResult<LicenseKeyListItemDto>(items, rows.Page, rows.PageSize, rows.TotalCount);
    }

    private sealed record Row(
        Guid Id, string KeyHint, LicenseType Type, string PlanName, LicenseStatus Status,
        int ActivationCount, int MaxActivations, string? LicensedUserName, DateTime? ExpiresAt, DateTime CreatedAt);
}

[RequirePermission(Permissions.LicenseGenerate)]
public sealed record GetPlanOptionsQuery : IRequest<IReadOnlyList<PlanOptionDto>>;

internal sealed class GetPlanOptionsHandler(IApplicationDbContext db) : IRequestHandler<GetPlanOptionsQuery, IReadOnlyList<PlanOptionDto>>
{
    public async Task<IReadOnlyList<PlanOptionDto>> HandleAsync(GetPlanOptionsQuery request, CancellationToken ct) =>
        await db.Set<Plan>().AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder)
            .Select(p => new PlanOptionDto(p.Id, p.Code, p.Name))
            .ToListAsync(ct);
}

[AllowAnonymousRequest]
public sealed record ServerActivateCommand(ServerActivateRequest Request) : IRequest<LicenseIssueDto>;

internal sealed class ServerActivateValidator : AbstractValidator<ServerActivateCommand>
{
    public ServerActivateValidator()
    {
        RuleFor(x => x.Request.Key).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Request.UserName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Fingerprint).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.FingerprintParts).NotNull().Must(p => p is { Count: >= 1 and <= 10 });
    }
}

internal sealed class ServerActivateHandler(LicenseOptions options, LicenseServerService server)
    : IRequestHandler<ServerActivateCommand, LicenseIssueDto>
{
    public Task<LicenseIssueDto> HandleAsync(ServerActivateCommand request, CancellationToken ct) =>
        options.ServerEnabled
            ? server.ActivateAsync(request.Request, ct)
            : throw new RequestRejectedException("not_a_license_server", "This host is not a License Server.");
}

[AllowAnonymousRequest]
public sealed record ServerTrialCommand(ServerTrialRequest Request) : IRequest<LicenseIssueDto>;

internal sealed class ServerTrialValidator : AbstractValidator<ServerTrialCommand>
{
    public ServerTrialValidator()
    {
        RuleFor(x => x.Request.UserName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Phone).NotEmpty().Must(p => PhoneNumber.TryNormalize(p, out _)).WithMessage("Enter a valid phone number, for example 0300 1234567.");
        RuleFor(x => x.Request.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Request.Email));
        RuleFor(x => x.Request.Fingerprint).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.FingerprintParts).NotNull().Must(p => p is { Count: >= 1 and <= 10 });
    }
}

internal sealed class ServerTrialHandler(LicenseOptions options, LicenseServerService server)
    : IRequestHandler<ServerTrialCommand, LicenseIssueDto>
{
    public Task<LicenseIssueDto> HandleAsync(ServerTrialCommand request, CancellationToken ct) =>
        options.ServerEnabled
            ? server.StartTrialAsync(request.Request, ct)
            : throw new RequestRejectedException("not_a_license_server", "This host is not a License Server.");
}


/// SuperAdmin: frees every PC of a key (lost or broken PC). The key can be activated again.
[RequirePermission(Permissions.LicenseGenerate)]
public sealed record ResetLicenseKeyCommand(Guid LicenseId) : IRequest<bool>;

internal sealed class ResetLicenseKeyHandler(LicenseServerService server, ICurrentUser user)
    : IRequestHandler<ResetLicenseKeyCommand, bool>
{
    public async Task<bool> HandleAsync(ResetLicenseKeyCommand request, CancellationToken ct)
    {
        await server.ResetKeyAsync(request.LicenseId, user.UserId, ct);
        return true;
    }
}

/// An app asks this License Server to free its activation. The signed license file is the proof.
[AllowAnonymousRequest]
public sealed record ServerDeactivateCommand(ServerDeactivateRequest Request) : IRequest<bool>;

internal sealed class ServerDeactivateValidator : AbstractValidator<ServerDeactivateCommand>
{
    public ServerDeactivateValidator()
    {
        RuleFor(x => x.Request.LicenseFile).NotEmpty().MaximumLength(20000);
        RuleFor(x => x.Request.Reason).MaximumLength(200);
    }
}

internal sealed class ServerDeactivateHandler(LicenseOptions options, LicenseServerService server)
    : IRequestHandler<ServerDeactivateCommand, bool>
{
    public async Task<bool> HandleAsync(ServerDeactivateCommand request, CancellationToken ct)
    {
        if (!options.ServerEnabled)
        {
            throw new RequestRejectedException("not_a_license_server", "This host is not a License Server.");
        }

        await server.DeactivateAsync(request.Request, ct);
        return true;
    }
}
