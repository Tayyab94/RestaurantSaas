using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Infrastructure.Services;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
