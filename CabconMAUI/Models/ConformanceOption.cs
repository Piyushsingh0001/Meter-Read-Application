using CommunityToolkit.Mvvm.ComponentModel;

namespace CabconMAUI.Models;

public partial class ConformanceOption : ObservableObject
{
    [ObservableProperty] private string _label = string.Empty;
    [ObservableProperty] private bool _isChecked;
}
