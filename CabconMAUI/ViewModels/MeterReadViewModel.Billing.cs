using CabconMAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CabconMAUI.ViewModels;

public partial class MeterReadViewModel
{
    [ObservableProperty] private ObservableCollection<BillingDisplayRow> _billingRows = new();
    [ObservableProperty] private bool _showBillingPanel;

    [ObservableProperty] private bool _isBillingReadComplete = true;
    [ObservableProperty] private bool _isBillingReadLast;
    [ObservableProperty] private bool _isBillingReadBetween;

    [ObservableProperty] private int _billingLastNIndex = 0;
    [ObservableProperty] private int _billingFromIndex = 0;
    [ObservableProperty] private int _billingToIndex = 0;

    [ObservableProperty] private ObservableCollection<int> _billingIndices = new(Enumerable.Range(1, 12));

    private System.Xml.Linq.XDocument? _billingXmlDoc;
    private System.Xml.Linq.XDocument? _interfaceXmlDoc;

    private async Task LoadBillingXmlAsync()
    {
        if (_billingXmlDoc == null)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("Configuration/Billing.xml");
                using var reader = new System.IO.StreamReader(stream);
                string xmlContent = await reader.ReadToEndAsync();
                
                // Sanitize invalid XML characters (e.g., non-printable chars or corrupted bytes)
                xmlContent = System.Text.RegularExpressions.Regex.Replace(xmlContent, @"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]", "");
                
                _billingXmlDoc = System.Xml.Linq.XDocument.Parse(xmlContent);
            }
            catch { }
        }

        if (_interfaceXmlDoc == null)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("Configuration/DLMSInterfaceClass.xml");
                _interfaceXmlDoc = System.Xml.Linq.XDocument.Load(stream);
            }
            catch { }
        }
    }

    private (string parameterName, int scale, string unit) GetBillingParameterInfo(string classId, string obisCode)
    {
        string paramName = obisCode;
        int scale = 0;
        string unit = "";

        if (_interfaceXmlDoc != null)
        {
            try
            {
                var parts = obisCode.Split('.');
                if (parts.Length == 6)
                {
                    string hexObis = string.Join(".", parts.Select(p => int.Parse(p).ToString("X2")));
                    var node = _interfaceXmlDoc.Descendants("OBIS")
                        .FirstOrDefault(o => o.Attribute("ParamCode")?.Value == hexObis &&
                                             o.Parent?.Name == "Class" &&
                                             o.Parent?.Attribute("ID")?.Value == classId);
                    
                    if (node != null)
                    {
                        paramName = node.Attribute("ParamName")?.Value ?? obisCode;
                    }
                }
            }
            catch { }
        }

        if (_billingXmlDoc != null)
        {
            var node = _billingXmlDoc.Descendants("DLMS").FirstOrDefault(x => 
                x.Element("Class")?.Value == classId && 
                x.Element("ObisCode")?.Value == obisCode);

            if (node != null)
            {
                var overrideName = node.Element("ParameterName")?.Value;
                if (!string.IsNullOrWhiteSpace(overrideName)) paramName = overrideName;
                
                int.TryParse(node.Element("Scale")?.Value ?? "0", out scale);
                unit = node.Element("Unit")?.Value ?? "";
            }
        }

        return (paramName, scale, unit);
    }

    [RelayCommand]
    void OpenBillingProfile()
    {
        SelectedReadDataOption = 3;
        ShowBillingPanel = true;
        SetStatus("Billing Profile panel opened. Please capture object first.");
    }

    [RelayCommand]
    async Task CaptureBillingObjectAsync()
    {
        if (!await EnsureConnectedAsync() || IsBusy) return;
        IsBusy = true;
        ClearStatus();
        try
        {
            SetStatus("Reading Billing Capture Objects from meter...");
            var obis = new byte[] { 0x01, 0x00, 0x62, 0x01, 0x00, 0xFF }; // BillingReadoutOBIS
            bool ok = await _dlms.ReadBlockFromMeterAsync(obis, 0x07, 0x03, 0x00, new List<byte>());
            
            if (ok)
            {
                var blockData = _dlms.GetBlockBuffer();
                if (blockData != null && blockData.Length > 0)
                {
                    BillingRows.Clear();
                    int nByteIndex = 0;
                    nByteIndex++; // Array 01
                    int numEntries = blockData[nByteIndex++];
                    
                    for (int i = 0; i < numEntries && nByteIndex < blockData.Length; i++)
                    {
                        nByteIndex++; // Structure 02
                        nByteIndex++; // Structure length 04
                        
                        nByteIndex++; // UInt16 (12)
                        nByteIndex++; // upper byte
                        int classId = blockData[nByteIndex++];
                        
                        nByteIndex++; // OctetString (09)
                        int obisLen = blockData[nByteIndex++];
                        string obisStr = $"{blockData[nByteIndex]}.{blockData[nByteIndex+1]}.{blockData[nByteIndex+2]}.{blockData[nByteIndex+3]}.{blockData[nByteIndex+4]}.{blockData[nByteIndex+5]}";
                        nByteIndex += obisLen;
                        
                        nByteIndex++; // Int8 (0F)
                        int attr = blockData[nByteIndex++];
                        
                        nByteIndex += 3; // data index (12 00 00)
                        
                        await LoadBillingXmlAsync();
                        var info = GetBillingParameterInfo(classId.ToString(), obisStr);
                        string paramName = info.parameterName;
                        
                        BillingRows.Add(new BillingDisplayRow
                        {
                            SerialNumber = i + 1,
                            ClassId = classId.ToString(),
                            ObisCode = obisStr,
                            Attribute = attr.ToString(),
                            ParameterName = paramName
                        });
                    }
                    SetStatus($"Billing capture objects read successfully. ({numEntries} items)");
                }
            }
            else SetStatus("Failed to read billing capture objects.", true);
        }
        catch (Exception ex)
        {
            SetStatus($"Billing capture error: {ex.Message}", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ReadBillingDataAsync()
    {
        if (!await EnsureConnectedAsync() || IsBusy) return;

        if (BillingRows.Count == 0)
        {
            SetStatus("Please capture object first!", true);
            return;
        }

        if (IsBillingReadBetween && BillingFromIndex > BillingToIndex)
        {
            SetStatus("From Profile should be <= To Profile!", true);
            return;
        }

        IsBusy = true;
        ClearStatus();

        try
        {
            SetStatus("Reading Billing Profile (Block)...");
            var obis = new byte[] { 0x01, 0x00, 0x62, 0x01, 0x00, 0xFF }; // BillingReadoutOBIS
            byte accessSelector = 0x00; // Null_descriptor for Complete
            List<byte> descriptor = new();

            if (!IsBillingReadComplete)
            {
                accessSelector = 0x02; // Entry_descriptor
                long from = 1;
                long to = 1;

                if (IsBillingReadLast)
                {
                    from = 1;
                    to = BillingLastNIndex + 1; // 0-based index from Picker
                }
                else if (IsBillingReadBetween)
                {
                    from = BillingFromIndex + 1;
                    to = BillingToIndex + 1;
                }

                descriptor = _dlms.GetByteByEntryValueType(from, to);
            }

            bool ok = await _dlms.ReadBlockFromMeterAsync(obis, 0x07, 0x02, accessSelector, descriptor);

            if (ok)
            {
                var blockData = _dlms.GetBlockBuffer();
                if (blockData != null && blockData.Length > 0)
                {
                    int nByteIndex = 0;
                    nByteIndex++; // Array 01
                    int numEntries = blockData[nByteIndex++];
                    
                    var newValuesLists = new List<ObservableCollection<string>>();
                    for (int i = 0; i < BillingRows.Count; i++) newValuesLists.Add(new ObservableCollection<string>());

                    for (int hist = 0; hist < numEntries && nByteIndex < blockData.Length; hist++)
                    {
                        nByteIndex++; // Structure 02
                        int numCols = blockData[nByteIndex++];

                        for (int col = 0; col < numCols && nByteIndex < blockData.Length; col++)
                        {
                            bool isAscii = col < BillingRows.Count && BillingRows[col].ObisCode == "0.0.96.2.196.255";
                            var datavalue = Helpers.DlmsHelper.DLMSDataFormator(blockData, nByteIndex, isAscii);
                            if (datavalue != null && datavalue.Length >= 2)
                            {
                                string val = string.IsNullOrWhiteSpace(datavalue[0]) ? "--" : datavalue[0];
                                
                                if (col < newValuesLists.Count)
                                {
                                    var info = GetBillingParameterInfo(BillingRows[col].ClassId, BillingRows[col].ObisCode);
                                    if (info.scale != 0 && double.TryParse(val, out double num))
                                    {
                                        double scaledValue = num * Math.Pow(10, info.scale);
                                        int decimals = Math.Max(0, Math.Abs(info.scale));
                                        val = scaledValue.ToString($"F{decimals}");
                                    }
                                    if (!string.IsNullOrWhiteSpace(info.unit))
                                    {
                                        val += $" {info.unit}";
                                    }

                                    newValuesLists[col].Add(val);
                                }
                                
                                if (int.TryParse(datavalue[1], out int nextIdx)) nByteIndex = nextIdx;
                            }
                            else break;
                        }
                    }

                    for (int i = 0; i < BillingRows.Count; i++)
                    {
                        BillingRows[i].Values.Clear();
                        foreach (var val in newValuesLists[i])
                        {
                            BillingRows[i].Values.Add(val);
                        }
                    }

                    // Dynamically generate column headers
                    UpdateBillingColumnHeaders(numEntries);
                    SetStatus($"Billing data read successfully. ({numEntries} entries)");
                }
                else
                {
                    SetStatus("Failed to parse billing block data.", true);
                }
            }
            else
            {
                SetStatus("Failed to read billing block.", true);
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Billing data error: {ex.Message}", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty] private ObservableCollection<string> _billingColumnHeaders = new();

    private void UpdateBillingColumnHeaders(int numEntries)
    {
        var newHeaders = new ObservableCollection<string>();
        
        if (IsBillingReadComplete)
        {
            for (int k = 1; k <= numEntries; k++)
            {
                if (k == 1) newHeaders.Add("Oldest History");
                else newHeaders.Add($"History {k - 1}");
            }
        }
        else
        {
            int startMonth = IsBillingReadLast ? 1 : BillingFromIndex + 1;
            for (int k = 1; k <= numEntries; k++)
            {
                if (startMonth + k - 1 == 1) newHeaders.Add("Oldest History");
                else newHeaders.Add($"History {startMonth + k - 2}");
            }
        }
        BillingColumnHeaders = newHeaders;
    }

    [RelayCommand]
    void CloseBillingObject()
    {
        ShowBillingPanel = false;
        BillingRows.Clear();
        BillingColumnHeaders.Clear();
        SetStatus("Billing closed.");
    }
}
