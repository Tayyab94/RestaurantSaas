namespace RestaurantSaaS.Contracts.Services;

/// <summary>
/// The one error type the UI handles. Local services convert application errors into it,
/// Remote services convert HTTP problem responses into it.
/// </summary>
public sealed class ServiceException(string code, string message, IReadOnlyDictionary<string, string[]>? errors = null)
    : Exception(message)
{
    public string Code { get; } = code;

    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors ?? new Dictionary<string, string[]>();
}
