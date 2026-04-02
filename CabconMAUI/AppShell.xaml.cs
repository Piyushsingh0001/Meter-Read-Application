using CabconMAUI.Views;
namespace CabconMAUI;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPage),     typeof(LoginPage));
        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(MeterReadPage), typeof(MeterReadPage));
        Routing.RegisterRoute(nameof(SettingsPage),  typeof(SettingsPage));
    }
}