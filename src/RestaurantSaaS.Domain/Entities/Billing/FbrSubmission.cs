using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>FbrSubmission: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class FbrSubmission : BranchEntity
{
    public Guid InvoiceId { get; set; }
    public FbrSubmissionStatus Status { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public string RequestJson { get; set; } = string.Empty;
    public string? ResponseJson { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? NextRetryAt { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
