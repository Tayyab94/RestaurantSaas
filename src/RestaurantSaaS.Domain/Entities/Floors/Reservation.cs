using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Reservation: branch table. Part of the Floors module. See Database Schema.</summary>
public sealed class Reservation : BranchEntity
{
    public Guid? TableId { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int PartySize { get; set; }
    public DateTime ReservedFrom { get; set; }
    public DateTime ReservedTo { get; set; }
    public ReservationStatus Status { get; set; }
    public decimal? AdvanceAmount { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedByUserId { get; set; }

    public RestaurantTable? Table { get; set; }
    public Customer? Customer { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
