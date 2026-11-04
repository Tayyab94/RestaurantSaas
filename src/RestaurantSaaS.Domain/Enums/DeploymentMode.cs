namespace RestaurantSaaS.Domain.Enums;

/// <summary>How a business uses the product.</summary>
public enum DeploymentMode
{
    /// <summary>Hosted by us. Many businesses, one system. Web UI, API and the desktop app in connected mode.</summary>
    Cloud = 0,

    /// <summary>The server (web UI, API, SignalR) runs on a PC or mini PC in the restaurant, for several devices on the LAN.</summary>
    OnPremise = 1,

    /// <summary>The desktop app on one PC, with its own local database (SQL Server or PostgreSQL). No server.</summary>
    Desktop = 2,
}
