using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OnboardingProgress: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class OnboardingProgress : TenantEntity
{
    public string StepKey { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime? DoneAt { get; set; }
    public Guid? DoneBy { get; set; }
}
