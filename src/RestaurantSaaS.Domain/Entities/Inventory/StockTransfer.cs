using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockTransfer: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockTransfer : TenantEntity
{
    public string TransferNumber { get; set; } = string.Empty;
    public Guid FromBranchId { get; set; }
    public Guid ToBranchId { get; set; }
    public StockTransferStatus Status { get; set; }
    public Guid RequestedBy { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public Guid? ReceivedBy { get; set; }

    public Branch FromBranch { get; set; } = null!;
    public Branch ToBranch { get; set; } = null!;
}
