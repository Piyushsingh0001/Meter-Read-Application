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
<<<<<<< HEAD
        builder.Services.AddSingleton<IConfigurationService, JsonConfigurationService>();
        builder.Services.AddSingleton<ISettingsService,  XmlBackedSettingsService>();
        // Register platform-specific serial port service
#if ANDROID
        builder.Services.AddSingleton<ISerialPortService, AndroidSerialPortService>();
#elif WINDOWS
=======
        builder.Services.AddSingleton<ISettingsService,  PreferencesSettingsService>();
        // Register platform-specific serial port service
#if ANDROID
        builder.Services.AddSingleton<ISerialPortService, AndroidSerialPortService>();
#else
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
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
<<<<<<< HEAD
        builder.Services.AddSingleton<IMeterReadBackgroundService, MeterReadBackgroundService>();
        builder.Services.AddSingleton<ICommandRepositoryService, CommandRepositoryService>();
        builder.Services.AddSingleton<IRelayControlService, RelayControlService>();
        builder.Services.AddSingleton<IOtaUpdateService, OtaUpdateService>();
        builder.Services.AddSingleton<IHdlcFrameService, HdlcFrameService>();
        builder.Services.AddSingleton<IHdlcFrameServiceWithRetry, HdlcFrameServiceWithRetry>();
        builder.Services.AddSingleton<ICosemDataParser, CosemDataParser>();
        builder.Services.AddSingleton<IStatusLogger, StatusLogger>();
        builder.Services.AddSingleton<IIecMeterService, IecMeterService>();
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
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
<<<<<<< HEAD
}
=======
}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
