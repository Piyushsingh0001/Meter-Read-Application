using CommunityToolkit.Mvvm.ComponentModel;

namespace CabconMAUI.Models;

public partial class MeterReadDisplayRow : ObservableObject
{
    [ObservableProperty] private int _serialNumber;
    [ObservableProperty] private string _classId = string.Empty;
    [ObservableProperty] private string _obisCode = string.Empty;
    [ObservableProperty] private string _attribute = string.Empty;
    [ObservableProperty] private string _parameterName = string.Empty;
    [ObservableProperty] private string _value = "--";

    public MeterReadDisplayRow()
    {
    }

    public MeterReadDisplayRow(string parameterName, string value)
    {
        _parameterName = parameterName;
        _value = value;
    }
}
