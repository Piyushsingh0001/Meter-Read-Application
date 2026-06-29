using Android.App;
using Android.Runtime;
namespace CabconMAUI;
[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle,JniHandleOwnership o):base(handle,o){}
    protected override MauiApp CreateMauiApp()=>MauiProgram.CreateMauiApp();
}