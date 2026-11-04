using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Printer: branch table. Part of the Platform module. See Database Schema.</summary>
public sealed class Printer : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public PrinterPrinterType PrinterType { get; set; }
    public PrinterConnectionType ConnectionType { get; set; }
    public string? Address { get; set; }
    public int? Port { get; set; }
    public int PaperWidthMm { get; set; }
    public int CharsPerLine { get; set; }
    public Guid? DeviceId { get; set; }
    public bool IsDefault { get; set; }
    public bool OpenDrawer { get; set; }

    public Device? Device { get; set; }
}
