using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CustomerMergeLog: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class CustomerMergeLog : TenantEntity
{
    public Guid KeptCustomerId { get; set; }
    public Guid MergedCustomerId { get; set; }
    public Guid MergedBy { get; set; }
    public DateTime MergedAt { get; set; }
    public string? MovedRecordsJson { get; set; }

    public Customer KeptCustomer { get; set; } = null!;
    public Customer MergedCustomer { get; set; } = null!;
}
