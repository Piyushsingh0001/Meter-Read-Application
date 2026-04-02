using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Services.Interfaces;
using CabconMAUI.Views;
using CabconMAUI.Models;
namespace CabconMAUI.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    readonly IAuthService _auth; readonly ISettingsService _set; readonly INavigationService _nav;
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(LoginCommand))] private string _userId=string.Empty;
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(LoginCommand))] private string _password=string.Empty;
    [ObservableProperty] private bool _rememberMe;
    [ObservableProperty] private string _appVersion="v1.0 — DLMS/COSEM";
    [ObservableProperty] private IReadOnlyList<MeterVariant> _meterVariants = MeterVariant.VisibleVariants;
    [ObservableProperty] private MeterVariant _selectedMeterVariant = MeterVariant.VisibleVariants.FirstOrDefault();
    public LoginViewModel(IAuthService a,ISettingsService s,INavigationService n){_auth=a;_set=s;_nav=n;if(_set.GetAppUserRememberMe()){UserId=_set.GetAppUser();Password=_set.GetAppPwd();RememberMe=true;}}
    [RelayCommand(CanExecute=nameof(CanLogin))]
    async Task LoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearStatus();
        try
        {
            if (SelectedMeterVariant == null)
            {
                SetStatus("Please select Meter Variant", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(Password))
            {
                SetStatus("User ID and Password are required.", true);
                return;
            }
            _set.SetAppUserRememberMe(RememberMe);
            bool ok = await _auth.LoginAsync(UserId, Password);
            if (ok)
                await _nav.NavigateToAsync(nameof(DashboardPage));
            else
                SetStatus("Invalid User ID or Password.", true);
        }
        catch (Exception ex)
        {
            SetStatus($"Login error: {ex.Message}", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    bool CanLogin()
        => SelectedMeterVariant != null
        && !string.IsNullOrWhiteSpace(UserId)
        && !string.IsNullOrWhiteSpace(Password)
        && !IsBusy;
}
