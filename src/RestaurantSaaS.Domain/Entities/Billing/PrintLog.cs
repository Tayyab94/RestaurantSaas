using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PrintLog: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class PrintLog : BranchEntity
{
    public Guid? PrintJobId { get; set; }
    public DocumentType DocumentType { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? PrinterId { get; set; }
    public Guid PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsReprint { get; set; }
    public string? ReprintReason { get; set; }

    public PrintJob? PrintJob { get; set; }
    public Printer? Printer { get; set; }
}
