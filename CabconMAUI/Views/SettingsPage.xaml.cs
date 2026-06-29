using CabconMAUI.ViewModels;
namespace CabconMAUI.Views;
public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm){InitializeComponent();BindingContext=vm;}
}