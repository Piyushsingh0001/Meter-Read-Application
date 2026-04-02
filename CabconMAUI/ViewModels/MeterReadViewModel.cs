using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Helpers;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.ViewModels;
public partial class MeterReadViewModel : BaseViewModel
{
    readonly IDlmsService _dlms; readonly ISettingsService _set; readonly INavigationService _nav;
    readonly IMeterCommunicationFacade _meterFacade;
    readonly IReadExportService _exportService;
    readonly ISerialPortService _serial;
    MeterReadResult? _lastRead;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(ConnectButtonText))][NotifyPropertyChangedFor(nameof(ConnectionStatusColor))][NotifyCanExecuteChangedFor(nameof(ConnectCommand))][NotifyCanExecuteChangedFor(nameof(ReadMeterCommand))][NotifyCanExecuteChangedFor(nameof(DisconnectCommand))] private bool _isConnected;
    public string ConnectButtonText=>IsConnected?"Connected ✓":"Connect";
    public string ConnectionStatusColor=>IsConnected?"#1E8449":"#C0392B";
    [ObservableProperty] private string _selectedPort="USB Serial";
    [ObservableProperty] private string _selectedProtocol="1-Phase Smart / DLMS";
    [ObservableProperty] private string _selectedTransport="Serial";
    public IReadOnlyList<string> PortOptions=>new[]{"USB Serial","Bluetooth","IrDA"};
    public IReadOnlyList<string> ProtocolOptions=>new[]{"1-Phase Smart / DLMS","3-Phase Smart / DLMS","IEC / Non-DLMS"};
    public IReadOnlyList<string> TransportOptions=>new[]{"Serial","Bluetooth","IrDA"};
    [ObservableProperty] private bool   _hasData;
    [ObservableProperty] private string _meterNumber="--";
    [ObservableProperty] private string _meterType="--";
    [ObservableProperty] private string _meterSignature="--";
    [ObservableProperty] private string _energyImport="--";
    [ObservableProperty] private string _energyExport="--";
    [ObservableProperty] private string _voltageR="--";
    [ObservableProperty] private string _voltageY="--";
    [ObservableProperty] private string _voltageB="--";
    [ObservableProperty] private string _currentR="--";
    [ObservableProperty] private string _currentY="--";
    [ObservableProperty] private string _currentB="--";
    [ObservableProperty] private string _powerFactor="--";
    [ObservableProperty] private string _frequency="--";
    [ObservableProperty] private string _activePower="--";
    [ObservableProperty] private string _reactivePower="--";
    [ObservableProperty] private string _timestamp="--";
    [ObservableProperty] private bool   _showRaw;
    [ObservableProperty] private int _selectedReadTab;
    [ObservableProperty] private bool _instantaneousSelected = true;
    [ObservableProperty] private bool _billingSelected = true;
    [ObservableProperty] private bool _tamperSelected = true;
    [ObservableProperty] private bool _loadSurveySelected = true;
    [ObservableProperty] private bool _dailyProfileSelected = true;
    [ObservableProperty] private bool _nameplateSelected = true;
    [ObservableProperty] private bool _selectAllFeatures = true;
    [ObservableProperty] private bool _readCompleteMode = true;
    [ObservableProperty] private bool _readByDateRangeMode;
    [ObservableProperty] private bool _readByEntryRangeMode;
    [ObservableProperty] private DateTime _rangeFrom=DateTime.Now.AddDays(-1);
    [ObservableProperty] private DateTime _rangeTo=DateTime.Now;
    [ObservableProperty] private int _entryFrom=1;
    [ObservableProperty] private int _entryTo=10;
    public MeterReadViewModel(IDlmsService d,ISettingsService s,INavigationService n,ISerialPortService ser,IMeterCommunicationFacade meterFacade,IReadExportService exportService){_dlms=d;_set=s;_nav=n;_serial=ser;_meterFacade=meterFacade;_exportService=exportService;_dlms.StatusUpdated+=(o,e)=>{StatusMessage=e.Message;IsError=e.IsError;};}

    [RelayCommand(CanExecute=nameof(CanConnect))]
    async Task ConnectAsync(){if(IsBusy)return;IsBusy=true;ClearStatus();try{var req=new MeterConnectRequest{ProtocolFamily=GetProtocol(),TransportMode=GetTransport(),PortName=SelectedPort};bool ok=await _meterFacade.ConnectToMeterAsync(req);IsConnected=ok;if(!ok)SetStatus("Connection failed. Check settings and cable.",true);}catch(Exception ex){SetStatus($"Connect error: {ex.Message}",true);}finally{IsBusy=false;}}
    bool CanConnect()=>!IsConnected&&!IsBusy;

    [RelayCommand(CanExecute=nameof(CanReadMeter))]
    async Task ReadMeterAsync(){
        if(IsBusy)return;IsBusy=true;HasData=false;ClearStatus();_lastRead=null;
        try{
            SetStatus("Reading meter data...");
            // Meter Number
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.MeterNumberBytes,0x01,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,true);MeterNumber=v?[0]??"--";}
            // Energy Import
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.EnergyImport,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);EnergyImport=(v?[0]??"0")+" kWh";}
            // Energy Export
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.EnergyExport,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);EnergyExport=(v?[0]??"0")+" kWh";}
            // Voltages
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.VoltageR,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);VoltageR=(v?[0]??"0")+" V";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.VoltageY,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);VoltageY=(v?[0]??"0")+" V";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.VoltageB,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);VoltageB=(v?[0]??"0")+" V";}
            // Currents
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.CurrentR,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);CurrentR=(v?[0]??"0")+" A";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.CurrentY,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);CurrentY=(v?[0]??"0")+" A";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.CurrentB,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);CurrentB=(v?[0]??"0")+" A";}
            // Power Quality
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.SignedPowerFactor,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);PowerFactor=v?[0]??"0";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.Frequency,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);Frequency=(v?[0]??"0")+" Hz";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.ActivePowerTotal,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);ActivePower=(v?[0]??"0")+" W";}
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.ReactivePowerTotal,0x03,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);ReactivePower=(v?[0]??"0")+" VAR";}
            // Clock
            if(await _dlms.ReadByteFromMeterAsync(DlmsHelper.ObisCode.Clock,0x08,0x02)){var v=_dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer,18,false);Timestamp=v?[0]??"--";}
            MeterType=_dlms.GetSelectedMeterType();MeterSignature=_dlms.MeterInfoValue;
            HasData=true;SetStatus("Read successful.");
            _lastRead = new MeterReadResult
            {
                IsSuccess = true,
                Source = "legacy-instantaneous",
                Message = "Legacy read completed.",
                Values = new Dictionary<string, string>
                {
                    ["MeterNumber"] = MeterNumber,
                    ["MeterType"] = MeterType,
                    ["MeterSignature"] = MeterSignature,
                    ["EnergyImport"] = EnergyImport,
                    ["EnergyExport"] = EnergyExport,
                    ["VoltageR"] = VoltageR,
                    ["VoltageY"] = VoltageY,
                    ["VoltageB"] = VoltageB,
                    ["CurrentR"] = CurrentR,
                    ["CurrentY"] = CurrentY,
                    ["CurrentB"] = CurrentB,
                    ["PowerFactor"] = PowerFactor,
                    ["Frequency"] = Frequency,
                    ["ActivePower"] = ActivePower,
                    ["ReactivePower"] = ReactivePower,
                    ["Timestamp"] = Timestamp
                }
            };
        }catch(Exception ex){SetStatus($"Read error: {ex.Message}",true);}finally{IsBusy=false;}
    }
    bool CanReadMeter()=>IsConnected&&!IsBusy;

    [RelayCommand(CanExecute=nameof(CanDisconnect))]
    async Task DisconnectAsync(){IsBusy=true;try{await _meterFacade.DisconnectAsync();IsConnected=false;HasData=false;Reset();SetStatus("Disconnected.");}catch(Exception ex){SetStatus($"Disconnect error: {ex.Message}",true);}finally{IsBusy=false;}}
    bool CanDisconnect()=>IsConnected;
    [RelayCommand] void ToggleRaw()=>ShowRaw=!ShowRaw;

    [RelayCommand]
    async Task ReadInstantaneousAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.Instantaneous);

    [RelayCommand]
    async Task ReadBillingAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.Billing);

    [RelayCommand]
    async Task ReadLoadSurveyAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.LoadSurvey);

    [RelayCommand]
    async Task ReadDailyProfileAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.DailyProfile);

    [RelayCommand]
    async Task ReadTamperAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.Tamper);

    [RelayCommand]
    async Task ReadNameplateAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.Nameplate);

    [RelayCommand]
    async Task ReadAllAsync() => await ExecuteFeatureReadAsync(MeterReadFeature.ReadAll);

    [RelayCommand]
    async Task ReadSelectedAsync()
    {
        if (InstantaneousSelected) await ExecuteFeatureReadAsync(MeterReadFeature.Instantaneous);
        if (BillingSelected) await ExecuteFeatureReadAsync(MeterReadFeature.Billing);
        if (TamperSelected) await ExecuteFeatureReadAsync(MeterReadFeature.Tamper);
        if (LoadSurveySelected) await ExecuteFeatureReadAsync(MeterReadFeature.LoadSurvey);
        if (DailyProfileSelected) await ExecuteFeatureReadAsync(MeterReadFeature.DailyProfile);
        if (NameplateSelected) await ExecuteFeatureReadAsync(MeterReadFeature.Nameplate);
    }

    [RelayCommand]
    void SelectReadTab(object? tabIndex)
    {
        if (tabIndex is int i) { SelectedReadTab = i; return; }
        if (tabIndex is string s && int.TryParse(s, out var parsed)) SelectedReadTab = parsed;
    }

    [RelayCommand]
    async Task ExportCsvAsync()
    {
        if (_lastRead == null) { SetStatus("No read data available for export.", true); return; }
        var path = await _exportService.ExportAsync(_lastRead, ExportFormat.Csv);
        SetStatus($"CSV exported: {path}");
    }

    [RelayCommand]
    async Task ExportXmlAsync()
    {
        if (_lastRead == null) { SetStatus("No read data available for export.", true); return; }
        var path = await _exportService.ExportAsync(_lastRead, ExportFormat.Xml);
        SetStatus($"XML exported: {path}");
    }

    async Task ExecuteFeatureReadAsync(MeterReadFeature feature)
    {
        if (!IsConnected) { SetStatus("Connect meter first.", true); return; }
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var result = await _meterFacade.ReadAsync(new MeterReadRequest
            {
                Feature = feature,
                FromDate = RangeFrom,
                ToDate = RangeTo,
                FromEntry = EntryFrom,
                ToEntry = EntryTo
            });

            _lastRead = result;
            if (!result.IsSuccess)
            {
                SetStatus(result.Message, true);
                return;
            }

            if (result.Values.TryGetValue("meter-number", out var meterNo) && !string.IsNullOrWhiteSpace(meterNo))
                MeterNumber = meterNo;
            if (result.Values.TryGetValue("energy-import", out var imp) && !string.IsNullOrWhiteSpace(imp))
                EnergyImport = $"{imp} kWh";
            if (result.Values.TryGetValue("energy-export", out var exp) && !string.IsNullOrWhiteSpace(exp))
                EnergyExport = $"{exp} kWh";
            if (result.Values.TryGetValue("voltage-r", out var vr) && !string.IsNullOrWhiteSpace(vr))
                VoltageR = $"{vr} V";
            if (result.Values.TryGetValue("voltage-y", out var vy) && !string.IsNullOrWhiteSpace(vy))
                VoltageY = $"{vy} V";
            if (result.Values.TryGetValue("voltage-b", out var vb) && !string.IsNullOrWhiteSpace(vb))
                VoltageB = $"{vb} V";
            if (result.Values.TryGetValue("clock", out var clk) && !string.IsNullOrWhiteSpace(clk))
                Timestamp = clk;

            HasData = true;
            SetStatus($"{feature} read completed.");
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

    MeterProtocolFamily GetProtocol() => SelectedProtocol switch
    {
        "3-Phase Smart / DLMS" => MeterProtocolFamily.Dlms3Phase,
        "IEC / Non-DLMS" => MeterProtocolFamily.IecNonDlms,
        _ => MeterProtocolFamily.Dlms1Phase
    };

    MeterTransportMode GetTransport() => SelectedTransport switch
    {
        "Bluetooth" => MeterTransportMode.Bluetooth,
        "IrDA" => MeterTransportMode.IrDa,
        _ => MeterTransportMode.Serial
    };

    partial void OnSelectAllFeaturesChanged(bool value)
    {
        InstantaneousSelected = value;
        BillingSelected = value;
        TamperSelected = value;
        LoadSurveySelected = value;
        DailyProfileSelected = value;
        NameplateSelected = value;
    }

    partial void OnReadCompleteModeChanged(bool value)
    {
        if (!value) return;
        ReadByDateRangeMode = false;
        ReadByEntryRangeMode = false;
    }

    partial void OnReadByDateRangeModeChanged(bool value)
    {
        if (!value) return;
        ReadCompleteMode = false;
        ReadByEntryRangeMode = false;
    }

    partial void OnReadByEntryRangeModeChanged(bool value)
    {
        if (!value) return;
        ReadCompleteMode = false;
        ReadByDateRangeMode = false;
    }
    void Reset(){MeterNumber=MeterType=MeterSignature=EnergyImport=EnergyExport=VoltageR=VoltageY=VoltageB=CurrentR=CurrentY=CurrentB=PowerFactor=Frequency=ActivePower=ReactivePower=Timestamp="--";}
}
