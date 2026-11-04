namespace RestaurantSaaS.Application.Abstractions;

/// <summary>Delivers a one-time code or message by SMS or email. The real providers come with module M42.</summary>
public interface IVerificationSender
{
    Task SendAsync(string? phone, string? email, string message, CancellationToken ct);
}
