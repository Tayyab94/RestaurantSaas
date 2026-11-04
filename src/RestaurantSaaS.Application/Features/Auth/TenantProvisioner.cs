using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Features.Onboarding;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Auth;

public sealed record ProvisionTenantRequest(
    string BusinessName,
    BusinessType BusinessType,
    DeploymentMode DeploymentMode,
    string OwnerFullName,
    string UserName,
    string Password,
    string? Email = null,
    string? Phone = null,
    string? FixedCode = null,
    Guid? TenantId = null);

public sealed record ProvisionedTenant(Tenant Tenant, User Owner);

/// <summary>
/// Creates everything a new business needs, in one save: tenant, main branch, the system roles with their
/// permissions, and the Owner user. Used by registration, by the installer and by development seeding.
/// </summary>
public sealed class TenantProvisioner(IApplicationDbContext db, IPasswordHasher hasher, IClock clock)
{
    public async Task<ProvisionedTenant> ProvisionAsync(ProvisionTenantRequest request, CancellationToken ct = default)
    {
        var code = request.FixedCode ?? await GenerateUniqueCodeAsync(request.BusinessName, ct);

        var tenant = Tenant.Create(
            code,
            request.BusinessName,
            request.OwnerFullName,
            request.BusinessType,
            request.DeploymentMode,
            request.TenantId,
            clock.UtcNow.AddDays(AuthDefaults.TrialDays),
            request.Email,
            request.Phone);

        db.Tenants.Add(tenant);
        var mainBranch = Branch.Create(tenant.Id, "MAIN", "Main Branch", isMain: true);
        db.Branches.Add(mainBranch);

        var permissionIds = await db.Permissions.ToDictionaryAsync(p => p.Key, p => p.Id, ct);
        Role? ownerRole = null;

        foreach (var definition in SystemRoles.All)
        {
            var role = Role.Create(tenant.Id, definition.Name, definition.Description, isSystem: true);
            db.Roles.Add(role);

            if (TenantDefaults.Limits.TryGetValue(definition.Name, out var limit))
            {
                db.Set<ApprovalLimit>().Add(new ApprovalLimit
                {
                    TenantId = tenant.Id,
                    RoleId = role.Id,
                    MaxDiscountPercent = limit.Percent,
                    MaxDiscountAmount = limit.Amount,
                    CanApproveVoid = limit.CanVoid,
                    CanApproveReturn = limit.CanReturn,
                });
            }

            foreach (var key in definition.Permissions)
            {
                if (permissionIds.TryGetValue(key, out var permissionId))
                {
                    db.RolePermissions.Add(RolePermission.Create(tenant.Id, role.Id, permissionId));
                }
            }

            if (definition.Name == SystemRoles.Owner)
            {
                ownerRole = role;
            }
        }

        var owner = User.Create(
            tenant.Id,
            request.UserName,
            request.OwnerFullName,
            hasher.Hash(request.Password),
            email: request.Email,
            phone: request.Phone);

        db.Users.Add(owner);
        db.UserRoles.Add(UserRole.Create(tenant.Id, owner.Id, ownerRole!.Id));

        db.Set<UserBranch>().Add(new UserBranch { TenantId = tenant.Id, UserId = owner.Id, BranchId = mainBranch.Id, IsDefault = true });

        AddStartingData(tenant.Id);

        await db.SaveChangesAsync(ct);

        return new ProvisionedTenant(tenant, owner);
    }

    // Payment methods and reasons to start selling on the first day, and the onboarding checklist.
    private void AddStartingData(Guid tenantId)
    {
        var order = 0;

        foreach (var method in TenantDefaults.PaymentMethods)
        {
            db.Set<PaymentMethod>().Add(new PaymentMethod
            {
                TenantId = tenantId,
                Name = method.Name,
                MethodType = method.Type,
                OpensDrawer = method.OpensDrawer,
                RequiresReference = method.RequiresReference,
                IsActive = true,
                SortOrder = ++order,
            });
        }

        order = 0;

        foreach (var reason in TenantDefaults.ReasonCodes)
        {
            db.Set<ReasonCode>().Add(new ReasonCode
            {
                TenantId = tenantId,
                ReasonType = reason.Type,
                Name = reason.Name,
                RequiresNote = reason.RequiresNote,
                IsActive = true,
                SortOrder = ++order,
            });
        }

        foreach (var step in OnboardingSteps.All)
        {
            db.Set<OnboardingProgress>().Add(new OnboardingProgress { TenantId = tenantId, StepKey = step.Key });
        }
    }

    private async Task<string> GenerateUniqueCodeAsync(string businessName, CancellationToken ct)
    {
        var letters = new string(businessName.Where(char.IsLetterOrDigit).Take(6).ToArray()).ToUpperInvariant();

        if (letters.Length < 3)
        {
            letters = "BIZ";
        }

        for (var attempt = 0; attempt < 30; attempt++)
        {
            var code = $"{letters}{RandomNumberGenerator.GetInt32(10, 100)}{(attempt > 10 ? RandomNumberGenerator.GetInt32(10, 100).ToString() : string.Empty)}";

            if (!await db.Tenants.IgnoreQueryFilters().AnyAsync(t => t.Code == code, ct))
            {
                return code;
            }
        }

        throw new InvalidOperationException("A free business code could not be generated.");
    }
}
