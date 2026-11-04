namespace RestaurantSaaS.Application.Abstractions;

/// <summary>Facts about the current request that handlers may record (for the login history).</summary>
public interface IRequestInfo
{
    string? IpAddress { get; }
}

/// <summary>Used where there is no network request (the standalone desktop app, or a call from a Blazor circuit).</summary>
public sealed class NullRequestInfo : IRequestInfo
{
    public string? IpAddress => null;
}
