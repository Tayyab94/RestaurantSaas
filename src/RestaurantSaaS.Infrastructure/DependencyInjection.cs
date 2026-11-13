using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Infrastructure.Persistence;
using RestaurantSaaS.Infrastructure.Persistence.Interceptors;
using RestaurantSaaS.Infrastructure.Services;

namespace RestaurantSaaS.Infrastructure;

public static class DependencyInjection
{
    public const string SqlServerMigrationsAssembly = "RestaurantSaaS.Migrations.SqlServer";
    public const string PostgreSqlMigrationsAssembly = "RestaurantSaaS.Migrations.PostgreSql";

    /// <summary>
    /// Registers the DbContext for the database named in configuration ("Database:Provider": SqlServer or PostgreSql).
    /// The host must also register ICurrentUser and ITenantProvider.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        options.Validate();

        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.Configure<VerificationOptions>(configuration.GetSection(VerificationOptions.SectionName));
        services.TryAddSingleton<IVerificationSender, LoggingVerificationSender>();
        services.AddScoped<AuditingInterceptor>();

        // ADD after services.AddScoped<AuditingInterceptor>();
        var license = configuration.GetSection(LicenseOptions.SectionName).Get<LicenseOptions>() ?? new LicenseOptions();
        services.AddSingleton(license);
        services.AddSingleton<LicenseKeyStore>();
        services.AddSingleton<ILicenseSigner>(provider => provider.GetRequiredService<LicenseKeyStore>());
        services.AddSingleton<ILicenseKeyRing>(provider => provider.GetRequiredService<LicenseKeyStore>());
        services.AddSingleton<IMachineFingerprint, MachineFingerprintProvider>();
        services.AddSingleton<ILicenseFileStore, FileLicenseStore>();



        services.AddDbContext<AppDbContext>((provider, builder) =>
        {
            if (options.IsPostgreSql)
            {
                builder.UseNpgsql(options.ConnectionString, b => b.MigrationsAssembly(PostgreSqlMigrationsAssembly));
            }
            else
            {
                builder.UseSqlServer(options.ConnectionString, b => b.MigrationsAssembly(SqlServerMigrationsAssembly));
            }

            builder.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
