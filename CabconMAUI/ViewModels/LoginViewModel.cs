using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Services.Interfaces;
using CabconMAUI.Views;
using CabconMAUI.Models;
namespace CabconMAUI.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    readonly IAuthService _auth; readonly ISettingsService _set; readonly INavigationService _nav;
    [ObservableProperty] private string _userId=string.Empty;
    [ObservableProperty] private string _password=string.Empty;
    [ObservableProperty] private bool _rememberMe;
    [ObservableProperty] private string _appVersion="v1.0 — DLMS/COSEM";
    [ObservableProperty] private IReadOnlyList<MeterVariant> _meterVariants = MeterVariant.VisibleVariants;
    [ObservableProperty] private MeterVariant? _selectedMeterVariant;
<<<<<<< HEAD
    public LoginViewModel(IAuthService a,ISettingsService s,INavigationService n){_auth=a;_set=s;_nav=n;if(_set.GetAppUserRememberMe()){UserId=_set.GetAppUser();Password=_set.GetAppPwd();RememberMe=true;}SelectedMeterVariant=MeterVariants.FirstOrDefault(variant=>(int)variant.Type==_set.GetMeterMode());}
=======
    public LoginViewModel(IAuthService a,ISettingsService s,INavigationService n){_auth=a;_set=s;_nav=n;if(_set.GetAppUserRememberMe()){UserId=_set.GetAppUser();Password=_set.GetAppPwd();RememberMe=true;}}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    [RelayCommand]
    async Task LoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearStatus();
        try
        {
            if (SelectedMeterVariant is null)
            {
                SetStatus("Please select Meter Variant.", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(UserId))
            {
                SetStatus("Please enter User ID.", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                SetStatus("Please enter Password.", true);
                return;
            }
            _set.SetAppUserRememberMe(RememberMe);
            bool ok = await _auth.LoginAsync(UserId, Password);
            if (ok)
<<<<<<< HEAD
            {
                _set.SetMeterMode((int)SelectedMeterVariant.Type);
                _set.Save();
                await _nav.NavigateToAsync(nameof(DashboardPage));
            }
=======
                await _nav.NavigateToAsync(nameof(DashboardPage));
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
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
}
