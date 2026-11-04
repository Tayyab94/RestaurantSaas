using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Feedback: branch table. Part of the Customers module. See Database Schema.</summary>
public sealed class Feedback : BranchEntity
{
    public Guid? OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Source { get; set; }

    public Order? Order { get; set; }
    public Customer? Customer { get; set; }
}
