using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PrinterRoute: branch table. Part of the Platform module. See Database Schema.</summary>
public sealed class PrinterRoute : BranchEntity
{
    public Guid? StationId { get; set; }
    public Guid PrinterId { get; set; }
    public DocumentType DocumentType { get; set; }
    public int Copies { get; set; }

    public Station? Station { get; set; }
    public Printer Printer { get; set; } = null!;
}
