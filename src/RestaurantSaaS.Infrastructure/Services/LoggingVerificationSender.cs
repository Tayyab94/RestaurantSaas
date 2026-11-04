using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Infrastructure.Services;

/// <summary>Bound from the "Verification" section. LogCodes is true in development only.</summary>
public sealed class VerificationOptions
{
    public const string SectionName = "Verification";

    /// <summary>Writes the code into the log. Use it only while no SMS or email provider is connected, and only in development.</summary>
    public bool LogCodes { get; set; }
}

/// <summary>
/// Placeholder sender until the messaging module (M42) connects an SMS and email provider.
/// It sends nothing; in development it writes the message into the log so the flow can be tried.
/// </summary>
public sealed class LoggingVerificationSender(ILogger<LoggingVerificationSender> logger, IOptions<VerificationOptions> options)
    : IVerificationSender
{
    public Task SendAsync(string? phone, string? email, string message, CancellationToken ct)
    {
        if (options.Value.LogCodes)
        {
            logger.LogWarning("VERIFICATION MESSAGE (not sent, no provider connected) to {Target}: {Message}", phone ?? email, message);
        }
        else
        {
            logger.LogWarning("A verification message was requested but no SMS or email provider is connected.");
        }

        return Task.CompletedTask;
    }
}
