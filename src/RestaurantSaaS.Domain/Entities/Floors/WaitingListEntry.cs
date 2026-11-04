using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>WaitingListEntry: branch table. Part of the Floors module. See Database Schema.</summary>
public sealed class WaitingListEntry : BranchEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int PartySize { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? SeatedAt { get; set; }
    public WaitingStatus Status { get; set; }
}
