using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services;

/// <summary>
/// Which optional modules (inventory, kitchen display, delivery) are switched on for the signed-in business.
/// The menu uses it to hide a switched-off module. Until the switches are loaded, every module is shown.
/// </summary>
public sealed class ModuleState(ISettingsService settings)
{
    private Dictionary<string, bool> _enabled = [];

    public event Action? Changed;

    public bool IsEnabled(string key) => !_enabled.TryGetValue(key, out var on) || on;

    public async Task RefreshAsync()
    {
        try
        {
            var switches = await settings.GetModuleSwitchesAsync();
            _enabled = switches.ToDictionary(m => m.Key, m => m.IsEnabled);
        }
        catch (ServiceException)
        {
            // Not signed in yet, or the application is locked: keep what was loaded before.
        }

        Changed?.Invoke();
    }
}
