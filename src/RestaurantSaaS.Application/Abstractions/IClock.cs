namespace RestaurantSaaS.Application.Abstractions;

/// <summary>All code reads time from here. DateTime.Now and DateTime.UtcNow are not used directly.</summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
