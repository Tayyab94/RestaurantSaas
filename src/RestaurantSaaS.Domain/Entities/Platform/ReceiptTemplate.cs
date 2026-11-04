using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ReceiptTemplate: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class ReceiptTemplate : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string Language { get; set; } = string.Empty;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public bool ShowLogo { get; set; }
    public bool ShowQr { get; set; }
    public bool ShowTaxBreakdown { get; set; }
    public bool ShowTrialMark { get; set; }
    public string? TemplateBody { get; set; }
}
