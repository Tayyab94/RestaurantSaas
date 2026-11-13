using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Infrastructure.Services;

public sealed class MachineFingerprintProvider : IMachineFingerprint
{
    private readonly Lazy<MachineFingerprint> _value = new(Compute);

    public MachineFingerprint Get() => _value.Value;

    private static MachineFingerprint Compute()
    {
        var parts = new[]
        {
            Hash("machine-id", ReadMachineId()),
            Hash("machine-name", Environment.MachineName),
            Hash("network-card", ReadFirstMac()),
            Hash("processor", $"{RuntimeInformation.OSArchitecture}|{Environment.ProcessorCount}"),
        };

        var overall = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('|', parts))));
        return new MachineFingerprint(overall, parts);
    }

    private static string Hash(string label, string value) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes($"restopos-fp|{label}|{value}")));

    private static string ReadMachineId()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                using var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                if (key?.GetValue("MachineGuid") is string guid && guid.Length > 0)
                {
                    return guid;
                }
            }
            else if (File.Exists("/etc/machine-id"))
            {
                return File.ReadAllText("/etc/machine-id").Trim();
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or System.Security.SecurityException)
        {
        }

        return "unknown";
    }

    private static string ReadFirstMac()
    {
        try
        {
            var mac = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType is NetworkInterfaceType.Ethernet or NetworkInterfaceType.Wireless80211)
                .Select(n => n.GetPhysicalAddress().ToString())
                .Where(a => a.Length == 12)
                .OrderBy(a => a, StringComparer.Ordinal)
                .FirstOrDefault();
            return mac ?? "none";
        }
        catch (NetworkInformationException)
        {
            return "none";
        }
    }
}
