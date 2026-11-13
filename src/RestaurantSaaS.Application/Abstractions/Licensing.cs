using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Abstractions;

public enum LicenseServerMode { Embedded, Remote }

public sealed class LicenseOptions
{
    public const string SectionName = "License";

    public LicenseServerMode ServerMode { get; set; } = LicenseServerMode.Remote;
    public string? ServerUrl { get; set; }
    public bool ServerEnabled { get; set; }
    public string? KeyPepper { get; set; }
    public string? FilePath { get; set; }
    public string SigningKeyId { get; set; } = "k1";
    public string? SigningPrivateKeyPem { get; set; }
    public string? SigningPrivateKeyPath { get; set; }
    public bool AllowDevSigningKey { get; set; }
    public Dictionary<string, string> TrustedKeys { get; set; } = new();
}

public interface ILicenseSigner
{
    bool IsAvailable { get; }
    string KeyId { get; }
    string PublicKey { get; }
    byte[] Sign(byte[] data);
}

public interface ILicenseKeyRing
{
    bool TryGetPublicKey(string keyId, out string publicKey);
}

public sealed record MachineFingerprint(string Hash, IReadOnlyList<string> Parts);

public interface IMachineFingerprint { MachineFingerprint Get(); }

public interface ILicenseFileStore
{
    string? Read();
    void Write(string content);
    void Delete();                 // ADD
}

public interface ILicenseServerClient
{
    Task<LicenseIssueDto> ActivateAsync(ServerActivateRequest request, CancellationToken ct);
    Task<LicenseIssueDto> StartTrialAsync(ServerTrialRequest request, CancellationToken ct);
    Task DeactivateAsync(ServerDeactivateRequest request, CancellationToken ct);   // ADD
}

public interface ILicenseGate
{
    Task<LicenseStatusDto> GetStatusAsync(CancellationToken ct = default);
    Task<LicenseStatusDto> RefreshAsync(LicenseCheckTrigger trigger, CancellationToken ct = default);
    Task EnsureCanSellAsync(CancellationToken ct = default);
}
