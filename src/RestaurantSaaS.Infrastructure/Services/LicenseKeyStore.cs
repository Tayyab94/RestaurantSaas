using System.Security.Cryptography;
using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Infrastructure.Services;

public sealed class LicenseKeyStore : ILicenseSigner, ILicenseKeyRing, IDisposable
{
    private readonly ECDsa? _privateKey;
    private readonly Dictionary<string, string> _trusted;

    public LicenseKeyStore(LicenseOptions options)
    {
        KeyId = options.SigningKeyId;
        _trusted = options.TrustedKeys
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);

        var pem = Normalize(options.SigningPrivateKeyPem);

        if (string.IsNullOrWhiteSpace(pem) && !string.IsNullOrWhiteSpace(options.SigningPrivateKeyPath) && File.Exists(options.SigningPrivateKeyPath))
        {
            pem = File.ReadAllText(options.SigningPrivateKeyPath);
        }

        if (string.IsNullOrWhiteSpace(pem) && options.AllowDevSigningKey)
        {
            pem = LoadOrCreateDevKey();
        }

        if (!string.IsNullOrWhiteSpace(pem))
        {
            _privateKey = ECDsa.Create();
            _privateKey.ImportFromPem(pem);
            PublicKey = Convert.ToBase64String(_privateKey.ExportSubjectPublicKeyInfo());
            _trusted[KeyId] = PublicKey;
        }
    }

    public bool IsAvailable => _privateKey is not null;
    public string KeyId { get; }
    public string PublicKey { get; } = string.Empty;

    public byte[] Sign(byte[] data) =>
        _privateKey is null
            ? throw new InvalidOperationException("This host has no license signing key.")
            : _privateKey.SignData(data, HashAlgorithmName.SHA256);

    public bool TryGetPublicKey(string keyId, out string publicKey) => _trusted.TryGetValue(keyId, out publicKey!);

    public void Dispose() => _privateKey?.Dispose();

    private static string? Normalize(string? pem) => pem?.Replace("\\n", "\n");

    private static string LoadOrCreateDevKey()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RestoPOS");
        var path = Path.Combine(folder, "dev-license-signing.pem");

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        Directory.CreateDirectory(folder);
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var pem = key.ExportPkcs8PrivateKeyPem();
        File.WriteAllText(path, pem);
        return pem;
    }
}
