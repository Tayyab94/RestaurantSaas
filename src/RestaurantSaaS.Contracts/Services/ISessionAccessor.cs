using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

/// <summary>The signed-in user of the current UI session. Implemented by the UI; read by in-process (Local) services.</summary>
public interface ISessionAccessor
{
    CurrentUserDto? User { get; }
}
