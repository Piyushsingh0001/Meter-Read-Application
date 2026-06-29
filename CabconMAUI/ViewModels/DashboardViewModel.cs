using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Services.Interfaces;
using CabconMAUI.Views;
namespace CabconMAUI.ViewModels;
public partial class DashboardViewModel : BaseViewModel
{
    readonly IAuthService _auth; readonly ISettingsService _set; readonly IDlmsService _dlms; readonly INavigationService _nav;
    public string WelcomeMessage=>$"Welcome, {_auth.CurrentUser}";
    public string MeterModeLabel=>_dlms.GetSelectedMeterType();
    public DashboardViewModel(IAuthService a,ISettingsService s,IDlmsService d,INavigationService n){_auth=a;_set=s;_dlms=d;_nav=n;}
    [RelayCommand] async Task GoToMeterReadAsync()=>await _nav.NavigateToAsync(nameof(MeterReadPage));
    [RelayCommand] async Task GoToSettingsAsync()=>await _nav.NavigateToAsync(nameof(SettingsPage));
    [RelayCommand] async Task LogoutAsync(){_auth.Logout();await Shell.Current.GoToAsync(nameof(LoginPage));}
}
