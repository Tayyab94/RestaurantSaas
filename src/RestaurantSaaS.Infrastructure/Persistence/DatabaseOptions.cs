namespace RestaurantSaaS.Infrastructure.Persistence;

/// <summary>Bound from the "Database" section of appsettings.json. Only two databases are supported.</summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    public const string SqlServer = "SqlServer";
    public const string PostgreSql = "PostgreSql";

    public string Provider { get; set; } = SqlServer;

    public string ConnectionString { get; set; } = string.Empty;

    public bool IsSqlServer => string.Equals(Provider, SqlServer, StringComparison.OrdinalIgnoreCase);

    public bool IsPostgreSql => string.Equals(Provider, PostgreSql, StringComparison.OrdinalIgnoreCase);

    public void Validate()
    {
        if (!IsSqlServer && !IsPostgreSql)
        {
            throw new InvalidOperationException(
                $"Database:Provider '{Provider}' is not supported. Use '{SqlServer}' or '{PostgreSql}'.");
        }

        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new InvalidOperationException("Database:ConnectionString is not set.");
        }
    }
}
