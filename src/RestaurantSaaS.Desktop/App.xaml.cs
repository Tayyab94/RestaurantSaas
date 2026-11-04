namespace RestaurantSaaS.Desktop;

// The base class is fully qualified because the solution has a namespace named RestaurantSaaS.Application.
public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new MainPage())
        {
            Title = "RestoPOS",
            Width = 1366,
            Height = 820,
        };
}
