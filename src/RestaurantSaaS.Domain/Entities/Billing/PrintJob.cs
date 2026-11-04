using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PrintJob: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class PrintJob : BranchEntity
{
    public Guid PrinterId { get; set; }
    public DocumentType DocumentType { get; set; }
    public Guid DocumentId { get; set; }
    public int Copies { get; set; }
    public PrintJobStatus Status { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
    public DateTime QueuedAt { get; set; }
    public DateTime? PrintedAt { get; set; }
    public Guid? DeviceId { get; set; }
    public bool IsReprint { get; set; }

    public Printer Printer { get; set; } = null!;
    public Device? Device { get; set; }
}
