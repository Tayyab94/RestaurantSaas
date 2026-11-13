using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Application.Features.Licensing;

internal static class LicenseKeyFormat
{
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
    private const int DataLength = 16;
    private const int ChecksumLength = 4;

    public static string Generate()
    {
        var data = new char[DataLength];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        var text = new string(data);
        return text + Checksum(text);
    }

    public static string Normalize(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        var chars = key.Where(c => c is not ('-' or ' ')).Select(char.ToUpperInvariant).Select(c => c switch
        {
            'O' => '0',
            'I' or 'L' => '1',
            _ => c,
        });
        return new string(chars.ToArray());
    }

    public static bool IsWellFormed(string normalized) =>
        normalized.Length == DataLength + ChecksumLength
        && normalized.All(c => Alphabet.Contains(c))
        && Checksum(normalized[..DataLength]) == normalized[DataLength..];

    public static string Display(string normalized) =>
        string.Join('-', Enumerable.Range(0, normalized.Length / 5).Select(i => normalized.Substring(i * 5, 5)));

    public static string Hint(string normalized) => normalized[^4..];

    public static string Checksum(string data)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes("restopos-key|" + data));
        return new string(Enumerable.Range(0, ChecksumLength).Select(i => Alphabet[hash[i] % Alphabet.Length]).ToArray());
    }
}

internal static class LicenseHashing
{
    public static string Hmac(string secret, string text) =>
        Convert.ToBase64String(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(text)));

    public static string Sha256(string text) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

    public static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    public static byte[] FromBase64Url(string text)
    {
        var s = text.Replace('-', '+').Replace('_', '/');
        s = s.PadRight(s.Length + ((4 - (s.Length % 4)) % 4), '=');
        return Convert.FromBase64String(s);
    }
}

internal sealed record LicensePayload(
    int Version, Guid LicenseId, Guid ActivationId, long Sequence, string KeyHint, string KeyHash,
    string LicenseType, string PlanCode, string LicensedTo, string Fingerprint, string[] FingerprintParts,
    DateTime IssuedAt, DateTime ActivatedAt, DateTime? ExpiresAt, int MaxDevices, int MaxBranches,
    string FeaturesJson, string KeyId);

internal sealed record LicenseEnvelope(string KeyId, string Payload, string Signature);

internal static class LicenseFileCodec
{
    public const int CurrentVersion = 1;

    private static readonly JsonSerializerOptions Json = new() { WriteIndented = false };

    public static (string Content, string SignatureHash) Create(LicensePayload payload, ILicenseSigner signer)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(payload, Json);
        var signature = signer.Sign(bytes);
        var content = JsonSerializer.Serialize(
            new LicenseEnvelope(signer.KeyId, LicenseHashing.Base64Url(bytes), LicenseHashing.Base64Url(signature)),
            new JsonSerializerOptions { WriteIndented = true });
        return (content, Convert.ToBase64String(SHA256.HashData(signature)));
    }

    public static LicenseEnvelope? ReadEnvelope(string? content)
    {
        try
        {
            return JsonSerializer.Deserialize<LicenseEnvelope>(content ?? string.Empty);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static bool TryOpen(string? content, ILicenseKeyRing keys, out LicensePayload payload, out string signatureHash, out string reason)
    {
        payload = null!;
        signatureHash = string.Empty;
        reason = "malformed";

        try
        {
            var envelope = JsonSerializer.Deserialize<LicenseEnvelope>(content ?? string.Empty);
            if (envelope is null || string.IsNullOrEmpty(envelope.Payload) || string.IsNullOrEmpty(envelope.Signature))
            {
                return false;
            }

            if (!keys.TryGetPublicKey(envelope.KeyId, out var publicKey))
            {
                reason = "unknown_key";
                return false;
            }

            var bytes = LicenseHashing.FromBase64Url(envelope.Payload);
            var signature = LicenseHashing.FromBase64Url(envelope.Signature);

            using var ecdsa = ECDsa.Create();
            ecdsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

            if (!ecdsa.VerifyData(bytes, signature, HashAlgorithmName.SHA256))
            {
                reason = "bad_signature";
                return false;
            }

            var parsed = JsonSerializer.Deserialize<LicensePayload>(bytes, Json);
            if (parsed is null || parsed.KeyId != envelope.KeyId)
            {
                return false;
            }

            payload = parsed;
            signatureHash = Convert.ToBase64String(SHA256.HashData(signature));
            reason = string.Empty;
            return true;
        }
        catch (Exception ex) when (ex is JsonException or FormatException or CryptographicException or ArgumentException)
        {
            return false;
        }
    }
}

internal static class FingerprintMatcher
{
    public static bool Matches(string hashA, IReadOnlyCollection<string> partsA, string hashB, IReadOnlyCollection<string> partsB)
    {
        if (!string.IsNullOrEmpty(hashA) && hashA == hashB)
        {
            return true;
        }

        return partsA.Intersect(partsB, StringComparer.Ordinal).Count() >= LicensePolicy.MinMatchingParts;
    }
}
