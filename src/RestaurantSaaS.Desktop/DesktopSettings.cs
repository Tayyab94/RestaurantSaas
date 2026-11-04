using Microsoft.Maui.Storage;

namespace RestaurantSaaS.Desktop;

public enum AppMode
{
    /// <summary>The app runs the application code itself with its own local database on this PC.</summary>
    Standalone,

    /// <summary>The app shows the screens and calls the Web server API (LAN or cloud).</summary>
    Connected,
}

/// <summary>
/// Stored with MAUI Preferences. The installer (or a first-run screen, module M38) writes them.
/// Defaults are for development: Standalone with SQL Server LocalDB.
/// </summary>
public sealed record DesktopSettings(AppMode Mode, string ServerUrl, string DatabaseProvider, string ConnectionString, bool RememberSignIn)
{
    //public const string DefaultConnection =
    //    "Server=(localdb)\\mssqllocaldb;Database=RestoPOS_Desktop;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";


    public const string DefaultConnection =
        "Server=TAYYAB;Database=RestaurantSaaS;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";



    public static DesktopSettings Load()
    {
        var mode = Enum.TryParse<AppMode>(Preferences.Default.Get("mode", nameof(AppMode.Standalone)), out var parsed)
            ? parsed
            : AppMode.Standalone;

        return new DesktopSettings(
            mode,
            Preferences.Default.Get("server_url", "http://localhost:5080"),
            Preferences.Default.Get("db_provider", "SqlServer"),
            Preferences.Default.Get("db_connection", DefaultConnection),
               Preferences.Default.Get("remember_signin", true));
    }
}
