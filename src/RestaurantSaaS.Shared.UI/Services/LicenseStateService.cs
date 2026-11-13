using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services;

public sealed class LicenseStateService(ILicenseService licenses) : IDisposable
{
    private Timer? _timer;

    public LicenseStatusDto? Current { get; private set; }
    public event Action? Changed;

    public async Task RefreshAsync()
    {
        try
        {
            Current = await licenses.GetStatusAsync();
        }
        catch (ServiceException)
        {
        }

        Changed?.Invoke();
    }

    public void Set(LicenseStatusDto status)
    {
        Current = status;
        Changed?.Invoke();
    }

    public void StartAutoRefresh() =>
        _timer ??= new Timer(async _ => await RefreshAsync(), null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

    public void Dispose() => _timer?.Dispose();
}
