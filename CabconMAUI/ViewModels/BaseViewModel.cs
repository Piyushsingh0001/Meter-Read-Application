using CommunityToolkit.Mvvm.ComponentModel;
namespace CabconMAUI.ViewModels;
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsNotBusy))] private bool _isBusy;
    [ObservableProperty] private string _statusMessage=string.Empty;
    [ObservableProperty] private bool _isError;
    public bool IsNotBusy=>!IsBusy;
    protected void SetStatus(string m,bool e=false){StatusMessage=m;IsError=e;}
    protected void ClearStatus(){StatusMessage=string.Empty;IsError=false;}
}
