using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Views;
public partial class SplashPage : ContentPage
{
    readonly ISettingsService _settings;
    public SplashPage(ISettingsService s){InitializeComponent();_settings=s;}
    protected override async void OnAppearing(){base.OnAppearing();await InitAsync();await Task.Delay(1800);await Shell.Current.GoToAsync(nameof(LoginPage));}
    async Task InitAsync(){try{string d=Path.Combine(FileSystem.AppDataDirectory,"Configuration");Directory.CreateDirectory(d);foreach(var f in new[]{"Instantaneous_c.xml","Billing.xml","BillingVZ.xml","Tamper.xml","1PCommandRepository.xml"}){string dest=Path.Combine(d,f);if(!File.Exists(dest)){try{using var s=await FileSystem.OpenAppPackageFileAsync($"Configuration/{f}");using var ds=File.Create(dest);await s.CopyToAsync(ds);}catch{}}}if(string.IsNullOrEmpty(_settings.ScaleXMLPath))_settings.SetScaleXMLPath(d);}catch(Exception ex){System.Diagnostics.Debug.WriteLine($"[Splash] {ex.Message}");}}
}