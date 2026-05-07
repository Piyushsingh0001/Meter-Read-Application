using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CabconMAUI.Models;

namespace CabconMAUI.ViewModels;

public partial class SettingsViewModel
{
    [ObservableProperty] private ObservableCollection<ConformanceOption> _leftConformanceOptions = new();
    [ObservableProperty] private ObservableCollection<ConformanceOption> _rightConformanceOptions = new();
    [ObservableProperty] private bool _allConformanceSelected = true;

    public string ConformanceBlockDisplay => $"Conformance Block: {ConformanceBlock}";

    partial void OnConformanceBlockChanged(string value)
    {
        OnPropertyChanged(nameof(ConformanceBlockDisplay));
        LoadConformanceOptions();
    }

    void LoadConformanceOptions()
    {
        LeftConformanceOptions = new ObservableCollection<ConformanceOption>
        {
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Read", IsChecked = true },
            new() { Label = "Write", IsChecked = true },
            new() { Label = "Unconfirmed Write", IsChecked = true },
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Attribute '0' With Set", IsChecked = true },
            new() { Label = "Priority Mgt. Support", IsChecked = true },
            new() { Label = "Attribute '0' With Get", IsChecked = true },
            new() { Label = "Block Transfer With Get", IsChecked = true }
        };

        RightConformanceOptions = new ObservableCollection<ConformanceOption>
        {
            new() { Label = "Block Transfer With Set", IsChecked = true },
            new() { Label = "Block Transfer With Action", IsChecked = true },
            new() { Label = "Multiple Reference", IsChecked = true },
            new() { Label = "Information report", IsChecked = true },
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Reserved", IsChecked = false },
            new() { Label = "Parameterized Access", IsChecked = true },
            new() { Label = "Get", IsChecked = true },
            new() { Label = "Set", IsChecked = true },
            new() { Label = "Selective Access", IsChecked = true },
            new() { Label = "Event Notification", IsChecked = true },
            new() { Label = "Action", IsChecked = true }
        };

        AllConformanceSelected = true;
    }
}
