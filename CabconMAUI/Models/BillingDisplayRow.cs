using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CabconMAUI.Models;

public partial class BillingDisplayRow : ObservableObject
{
    [ObservableProperty] private int _serialNumber;
    [ObservableProperty] private string _classId = string.Empty;
    [ObservableProperty] private string _obisCode = string.Empty;
    [ObservableProperty] private string _attribute = string.Empty;
    [ObservableProperty] private string _parameterName = string.Empty;

    [ObservableProperty] private ObservableCollection<string> _values = new();

    public BillingDisplayRow()
    {
    }
}
