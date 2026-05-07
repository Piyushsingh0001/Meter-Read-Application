using CommunityToolkit.Maui;
using CabconMAUI.Services;
using CabconMAUI.Services.Interfaces;
using CabconMAUI.ViewModels;
using CabconMAUI.Views;
using Microsoft.Extensions.Logging;
namespace CabconMAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<ISettingsService,  PreferencesSettingsService>();
        // Register platform-specific serial port service
#if ANDROID
        builder.Services.AddSingleton<ISerialPortService, AndroidSerialPortService>();
#else
        builder.Services.AddSingleton<ISerialPortService, WindowsSerialPortService>();
#endif
        builder.Services.AddSingleton<IBluetoothService,  BluetoothService>();
        builder.Services.AddSingleton<IHdlcFrameService,  HdlcFrameService>();
        builder.Services.AddSingleton<ICosemService,       CosemService>();
        builder.Services.AddSingleton<ICryptoService,      AesGcmCryptoService>();
        builder.Services.AddSingleton<IDlmsService,        DlmsService>();
        builder.Services.AddSingleton<IIecMeterService,    IecMeterService>();
        builder.Services.AddSingleton<IMeterCommunicationFacade, MeterCommunicationFacade>();
        builder.Services.AddSingleton<IReadExportService, ReadExportService>();
        builder.Services.AddSingleton<IAuthService,        AuthService>();
        builder.Services.AddSingleton<INavigationService,  NavigationService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<MeterReadViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<MeterReadPage>();
        builder.Services.AddTransient<SettingsPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}