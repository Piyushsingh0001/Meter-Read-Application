using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Models;

namespace CabconMAUI.ViewModels;

public partial class MeterReadViewModel
{
    [ObservableProperty] private int _selectedReadDataOption;

    static readonly IReadOnlyList<ReadObjectDefinition> InstantaneousDefinitions =
    [
        new(0, "15", 0x0F, "0.0.1.0.0.255", [0x00,0x00,0x01,0x00,0x00,0xFF], "2", 0x02, "Association View", false),
        new(1, "8", 0x08, "0.0.1.0.0.255", [0x00,0x00,0x01,0x00,0x00,0xFF], "2", 0x02, "Real Time Clock - Date and Time", false),
        new(2, "3", 0x03, "1.0.12.7.0.255", [0x01,0x00,0x0C,0x07,0x00,0xFF], "2", 0x02, "Phase Voltage", false),
        new(3, "3", 0x03, "1.0.11.7.0.255", [0x01,0x00,0x0B,0x07,0x00,0xFF], "2", 0x02, "Phase Current", false),
        new(4, "3", 0x03, "1.0.91.7.0.255", [0x01,0x00,0x5B,0x07,0x00,0xFF], "2", 0x02, "Neutral Current", false),
        new(5, "3", 0x03, "1.0.13.7.0.255", [0x01,0x00,0x0D,0x07,0x00,0xFF], "2", 0x02, "Three Phase Power Factor - PF", false),
        new(6, "3", 0x03, "1.0.14.7.0.255", [0x01,0x00,0x0E,0x07,0x00,0xFF], "2", 0x02, "Frequency", false),
        new(7, "3", 0x03, "1.0.9.7.0.255", [0x01,0x00,0x09,0x07,0x00,0xFF], "2", 0x02, "Neutral Power", false),
        new(8, "3", 0x03, "1.0.1.7.0.255", [0x01,0x00,0x01,0x07,0x00,0xFF], "2", 0x02, "Phase Power", false),
        new(9, "3", 0x03, "1.0.1.8.0.255", [0x01,0x00,0x01,0x08,0x00,0xFF], "2", 0x02, "Cumulative Energy - kWh", false),
        new(10, "3", 0x03, "1.0.9.8.0.255", [0x01,0x00,0x09,0x08,0x00,0xFF], "2", 0x02, "Cumulative Energy - kVAh", false),
        new(11, "4", 0x04, "1.0.1.6.0.255", [0x01,0x00,0x01,0x06,0x00,0xFF], "2", 0x02, "MD - kW", false),
        new(12, "4", 0x04, "1.0.1.6.0.255", [0x01,0x00,0x01,0x06,0x00,0xFF], "5", 0x05, "MD - kW", false),
        new(13, "4", 0x04, "1.0.9.6.0.255", [0x01,0x00,0x09,0x06,0x00,0xFF], "2", 0x02, "MD - kVA", false),
        new(14, "4", 0x04, "1.0.9.6.0.255", [0x01,0x00,0x09,0x06,0x00,0xFF], "5", 0x05, "MD - kVA", false),
        new(15, "3", 0x03, "0.0.94.91.8.255", [0x00,0x00,0x5E,0x5B,0x08,0xFF], "2", 0x02, "Cumulative power-failure duration", false),
        new(16, "1", 0x01, "0.0.94.91.0.255", [0x00,0x00,0x5E,0x5B,0x00,0xFF], "2", 0x02, "Cumulative Tamper count", false),
        new(17, "1", 0x01, "0.0.0.1.0.255", [0x00,0x00,0x00,0x01,0x00,0xFF], "2", 0x02, "MD Reset Count", false),
        new(18, "1", 0x01, "0.0.96.2.0.255", [0x00,0x00,0x60,0x02,0x00,0xFF], "2", 0x02, "Cumulative programming count", false),
        new(19, "1", 0x01, "0.0.96.7.0.255", [0x00,0x00,0x60,0x07,0x00,0xFF], "2", 0x02, "Number of power failures", false),
        new(20, "3", 0x03, "0.0.94.91.20.255", [0x00,0x00,0x5E,0x5B,0x14,0xFF], "2", 0x02, "Average PF present month in instantaneous", false),
        new(21, "3", 0x03, "1.0.3.7.0.255", [0x01,0x00,0x03,0x07,0x00,0xFF], "2", 0x02, "Signed Reactive Power - kvar (+ Lag:- Lead)", false)
    ];

    static readonly IReadOnlyList<ReadObjectDefinition> NameplateDefinitions =
    [
        new(1, "1", 0x01, "0.0.96.1.0.255", [0x00,0x00,0x60,0x01,0x00,0xFF], "2", 0x02, "Meter Serial number", true),
        new(2, "1", 0x01, "0.0.96.1.1.255", [0x00,0x00,0x60,0x01,0x01,0xFF], "2", 0x02, "Manufacturer Name", true),
        new(3, "1", 0x01, "1.0.0.2.0.255", [0x01,0x00,0x00,0x02,0x00,0xFF], "2", 0x02, "Firmware Version for meter", true),
        new(4, "1", 0x01, "0.0.94.91.9.255", [0x00,0x00,0x5E,0x5B,0x09,0xFF], "2", 0x02, "Meter Type (1Phase/3P-3W/3P-4W)", false),
        new(5, "1", 0x01, "0.0.94.91.11.255", [0x00,0x00,0x5E,0x5B,0x0B,0xFF], "2", 0x02, "Category", true),
        new(6, "1", 0x01, "0.0.94.91.12.255", [0x00,0x00,0x5E,0x5B,0x0C,0xFF], "2", 0x02, "Current rating", true),
        new(7, "1", 0x01, "0.0.96.1.4.255", [0x00,0x00,0x60,0x01,0x04,0xFF], "2", 0x02, "Meter Year of Manufacture", true)
    ];

    [ObservableProperty] private ObservableCollection<MeterReadDisplayRow> _instantaneousRows = new();
    [ObservableProperty] private ObservableCollection<MeterReadDisplayRow> _nameplateRows = new();
    [ObservableProperty] private bool _showInstantaneousPanel;
    [ObservableProperty] private bool _showNameplatePanel;

    [RelayCommand]
    void SelectReadDataOption(object? option)
    {
        if (option is int selected)
        {
            SelectedReadDataOption = selected;
            return;
        }

        if (option is string text && int.TryParse(text, out var parsed))
        {
            SelectedReadDataOption = parsed;
        }
    }

    [RelayCommand]
    void CaptureInstantaneousObject()
    {
        InstantaneousRows.Clear();
        ShowInstantaneousPanel = true;
        SetStatus("Ready to read Instantaneous Profile data from meter.");
    }

    [RelayCommand]
    async Task ReadInstantaneousProfileDataAsync()
    {
        ShowInstantaneousPanel = true;
        await ReadObjectGroupAsync(InstantaneousDefinitions, InstantaneousRows, "Instantaneous Profile Read Out");
    }

    [RelayCommand]
    void CloseInstantaneousObject()
    {
        ShowInstantaneousPanel = false;
        InstantaneousRows.Clear();
        SetStatus("Instantaneous Profile Read Out closed.");
    }

    [RelayCommand]
    void CaptureNameplateObject()
    {
        NameplateRows.Clear();
        ShowNameplatePanel = true;
        SetStatus("Ready to read Nameplate data from meter.");
    }

    [RelayCommand]
    async Task ReadNameplateDataAsync()
    {
        ShowNameplatePanel = true;
        await ReadObjectGroupAsync(NameplateDefinitions, NameplateRows, "Nameplate");
    }

    [RelayCommand]
    void CloseNameplateObject()
    {
        ShowNameplatePanel = false;
        NameplateRows.Clear();
        SetStatus("Nameplate closed.");
    }

    async Task ReadObjectGroupAsync(IReadOnlyList<ReadObjectDefinition> definitions, ObservableCollection<MeterReadDisplayRow> rows, string sectionTitle)
    {
        if (!await EnsureConnectedAsync())
        {
            return;
        }

        if (IsBusy)
        {
            return;
        }

        if (rows.Count == 0)
        {
            PopulateRows(rows, definitions);
        }

        IsBusy = true;
        ClearStatus();

        try
        {
            var values = new Dictionary<string, string>();
            var successCount = 0;

            for (var index = 0; index < definitions.Count; index++)
            {
                var definition = definitions[index];
                var row = rows[index];
                string value = "--";
                
                // Special handling for Association View (Class ID 15) - use Block Transfer
                if (definition.ClassIdByte == 0x0F) // Class ID 15 = Association
                {
                    try
                    {
                        // Use Block Transfer for large Association View response
                        var associationResult = await _meterFacade.ReadBlockAsync(
                            definition.ObisBytes, 
                            definition.ClassIdByte, 
                            definition.AttributeByte, 
                            0, // access selector
                            new List<byte>() // descriptor
                        );
                        
                        if (associationResult.IsSuccess && associationResult.Values.Any())
                        {
                            // Parse the Association View data - it contains multiple OBIS codes
                            var associationData = associationResult.Values.FirstOrDefault().Value;
                            if (!string.IsNullOrEmpty(associationData))
                            {
                                // For now, show the raw Association View data
                                // TODO: Parse individual OBIS codes from Association View
                                value = $"Association View ({associationData.Length} chars)";
                                successCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SetStatus($"Association View error: {ex.Message}", true);
                    }
                }
                else
                {
                    // Regular single object read for other objects
                    var ok = await _dlms.ReadByteFromMeterAsync(definition.ObisBytes, definition.ClassIdByte, definition.AttributeByte);
                    value = ok
                        ? _dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer, 18, definition.IsAscii)?.FirstOrDefault() ?? "--"
                        : "--";

                    if (value != "--")
                    {
                        successCount++;
                    }
                }

                row.Value = string.IsNullOrWhiteSpace(value) ? "--" : value;
                values[definition.ParameterName] = row.Value;
            }

            _lastRead = new MeterReadResult
            {
                IsSuccess = successCount > 0,
                Message = successCount > 0 ? $"{sectionTitle} read completed." : $"{sectionTitle} read returned no data.",
                Source = sectionTitle,
                Values = values,
                RawBuffer = _serial.ReceiveBuffer.ToArray()
            };

            HasData = successCount > 0;
            ApplySummaryValues(values, sectionTitle);
            SetStatus(_lastRead.Message, successCount == 0);
        }
        catch (Exception ex)
        {
            SetStatus($"Read failed: {ex.Message}", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    void ApplySummaryValues(Dictionary<string, string> values, string sectionTitle)
    {
        if (sectionTitle == "Nameplate")
        {
            if (values.TryGetValue("Meter Serial number", out var meterNo)) MeterNumber = meterNo;
            if (values.TryGetValue("Meter Type (1Phase/3P-3W/3P-4W)", out var meterType)) MeterType = meterType;
            if (values.TryGetValue("Manufacturer Name", out var manufacturer)) MeterSignature = manufacturer;
            return;
        }

        if (values.TryGetValue("Real Time Clock - Date and Time", out var timestamp)) Timestamp = timestamp;
        if (values.TryGetValue("Phase Voltage", out var voltage)) VoltageR = voltage;
        if (values.TryGetValue("Phase Current", out var current)) CurrentR = current;
        if (values.TryGetValue("Three Phase Power Factor - PF", out var pf)) PowerFactor = pf;
        if (values.TryGetValue("Frequency", out var frequency)) Frequency = frequency;
        if (values.TryGetValue("Phase Power", out var power)) ActivePower = power;
        if (values.TryGetValue("Signed Reactive Power - kvar (+ Lag:- Lead)", out var reactive)) ReactivePower = reactive;
        if (values.TryGetValue("Cumulative Energy - kWh", out var kwh)) EnergyImport = kwh;
    }

    static void PopulateRows(ObservableCollection<MeterReadDisplayRow> rows, IReadOnlyList<ReadObjectDefinition> definitions)
    {
        rows.Clear();
        foreach (var definition in definitions)
        {
            rows.Add(new MeterReadDisplayRow
            {
                SerialNumber = definition.SerialNumber,
                ClassId = definition.ClassId,
                ObisCode = definition.ObisCode,
                Attribute = definition.Attribute,
                ParameterName = definition.ParameterName,
                Value = "--"
            });
        }
    }

    sealed record ReadObjectDefinition(
        int SerialNumber,
        string ClassId,
        byte ClassIdByte,
        string ObisCode,
        byte[] ObisBytes,
        string Attribute,
        byte AttributeByte,
        string ParameterName,
        bool IsAscii);
}
