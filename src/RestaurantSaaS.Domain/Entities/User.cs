using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>
/// A person who signs in. TenantId is null only for platform users (the SuperAdmin).
/// Passwords are stored only as a salted hash.
/// </summary>
public sealed class User : BaseEntity, IMayHaveTenant
{
    private User()
    {
    }

    public Guid? TenantId { get; private set; }

    public string UserName { get; private set; } = string.Empty;

    public string NormalizedUserName { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public bool IsSuperAdmin { get; private set; }

    public bool MustChangePassword { get; private set; }

    public DateTime? PasswordChangedAt { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    public int FailedLoginCount { get; private set; }

    public DateTime? LockoutEnd { get; private set; }

    /// <summary>Hash of the 4 to 6 digit PIN for quick sign-in (staff on a shared screen). Null when the user has no PIN.</summary>
    public string? PinHash { get; private set; }

    public DateTime? PinSetAt { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    public static string Normalize(string userName) => userName.Trim().ToUpperInvariant();

    public static User Create(
        Guid? tenantId,
        string userName,
        string fullName,
        string passwordHash,
        bool isSuperAdmin = false,
        bool mustChangePassword = false,
        string? email = null,
        string? phone = null,
        Guid? id = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var user = new User
        {
            TenantId = tenantId,
            UserName = userName.Trim(),
            NormalizedUserName = Normalize(userName),
            FullName = fullName.Trim(),
            Email = email?.Trim(),
            Phone = phone?.Trim(),
            PasswordHash = passwordHash,
            IsSuperAdmin = isSuperAdmin,
            MustChangePassword = mustChangePassword,
        };

        if (id.HasValue)
        {
            user.Id = id.Value;
        }

        return user;
    }

    public bool IsLockedOut(DateTime now) => LockoutEnd.HasValue && LockoutEnd.Value > now;

    public void RegisterFailedLogin(DateTime now, int maxAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginCount++;

        if (FailedLoginCount >= maxAttempts)
        {
            LockoutEnd = now.Add(lockoutDuration);
            FailedLoginCount = 0;
        }
    }

    public void RegisterSuccessfulLogin(DateTime now)
    {
        LastLoginAt = now;
        FailedLoginCount = 0;
        LockoutEnd = null;
    }

    public void SetPassword(string passwordHash, DateTime now, bool mustChangePassword = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        PasswordChangedAt = now;
        MustChangePassword = mustChangePassword;
        FailedLoginCount = 0;
        LockoutEnd = null;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    /// <summary>Clears the wrong-attempt counter after a correct PIN was used for an approval (it is not a sign-in).</summary>
    public void ResetFailedLogins() => FailedLoginCount = 0;

    public void SetPin(string pinHash, DateTime now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pinHash);
        PinHash = pinHash;
        PinSetAt = now;
    }

    public void ClearPin()
    {
        PinHash = null;
        PinSetAt = null;
    }
}
