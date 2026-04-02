using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.ViewModels;
public partial class SettingsViewModel : BaseViewModel
{
    readonly ISettingsService _s; readonly INavigationService _nav;

    [ObservableProperty] private int _selectedTab;
    [ObservableProperty] private string _serialPort=string.Empty;
    [ObservableProperty] private string _signOnBaudRate=string.Empty;
    [ObservableProperty] private string _commandBaudRate=string.Empty;
    [ObservableProperty] private string _stopBits=string.Empty;
    [ObservableProperty] private string _dataBits=string.Empty;
    [ObservableProperty] private string _parity=string.Empty;
    [ObservableProperty] private int    _commandTimeOut;
    [ObservableProperty] private int    _intercharacterDelay;
    [ObservableProperty] private int    _interframeTimeout;
    [ObservableProperty] private int    _serverSAP;
    [ObservableProperty] private int    _serverLowerMacAddress;
    [ObservableProperty] private int    _clientSAP;
    [ObservableProperty] private int    _addressingSchem;
    [ObservableProperty] private int    _serverPhysicalID;
    [ObservableProperty] private int    _hDLCAddressing;
    [ObservableProperty] private int    _infoSize;
    [ObservableProperty] private int    _windowSize;
    [ObservableProperty] private int    _cosemBufferSize;
    [ObservableProperty] private int    _dLLBufferSize;
    [ObservableProperty] private int    _applicationContext;
    [ObservableProperty] private int    _securityMechanism;
    [ObservableProperty] private int    _pDUSize;
    [ObservableProperty] private string _conformanceBlock=string.Empty;
    [ObservableProperty] private int    _dLMSVersion;
    [ObservableProperty] private int    _associationMode;
    [ObservableProperty] private int    _associationType;
    [ObservableProperty] private string _associationAccess=string.Empty;
    [ObservableProperty] private string _password=string.Empty;
    [ObservableProperty] private string _hLSKey=string.Empty;
    [ObservableProperty] private string _hLSPWD=string.Empty;
    [ObservableProperty] private string _clientSystemTitle=string.Empty;
    [ObservableProperty] private int    _securitysuit;
    [ObservableProperty] private string _globalEncryptionKey=string.Empty;
    [ObservableProperty] private int    _dedicatedKey;
    [ObservableProperty] private string _authenticationKey=string.Empty;
    [ObservableProperty] private string _aESEncryption=string.Empty;
    [ObservableProperty] private int    _meterMode;
    [ObservableProperty] private string _communicationPort=string.Empty;
    [ObservableProperty] private string _communicationMode=string.Empty;
    [ObservableProperty] private string _modemNumber=string.Empty;
    [ObservableProperty] private string _appUser=string.Empty;
    [ObservableProperty] private string _appPwd=string.Empty;
    [ObservableProperty] private bool   _appUserRememberMe;
    [ObservableProperty] private string _defaultReadClassID=string.Empty;
    [ObservableProperty] private string _defaultReadOBIS=string.Empty;
    [ObservableProperty] private string _defaultReadAtt=string.Empty;
    [ObservableProperty] private string _defaultReadDaraType=string.Empty;
    [ObservableProperty] private string _defaultReadLen=string.Empty;
    [ObservableProperty] private int    _defaultReadAccSelector;
    [ObservableProperty] private int    _defaultReadCmdType;
    [ObservableProperty] private string _defaultReadSelectiveAccessCommand=string.Empty;
    [ObservableProperty] private string _defaultReadDataValueCommand=string.Empty;
    [ObservableProperty] private int    _cnf1,_cnf2,_cnf3;
    public string[] ParityOptions   =>AppSettings.ParityList;
    public string[] BaudRateOptions =>AppSettings.BaudRateList;
    public string[] StopBitsOptions =>AppSettings.StopBitsList;
    public string[] DataBitsOptions =>AppSettings.DataBitsList;
    public string[] CommPortOptions =>AppSettings.CommPortList;
    public string[] CommModeOptions =>AppSettings.CommModeList;
    public IReadOnlyList<MeterVariant> MeterVariants=>MeterVariant.AllVariants;
    public SettingsViewModel(ISettingsService s,INavigationService n){_s=s;_nav=n;Load();}
    void Load(){SerialPort=_s.SerialPort;SignOnBaudRate=_s.SignOnBaudRate;CommandBaudRate=_s.CommandBaudRate;StopBits=_s.StopBits;DataBits=_s.DataBits;Parity=_s.Parity;CommandTimeOut=_s.CommandTimeOut;IntercharacterDelay=_s.IntercharacterDelay;InterframeTimeout=_s.InterframeTimeout;ServerSAP=_s.ServerSAP;ServerLowerMacAddress=_s.ServerLowerMacAddress;ClientSAP=_s.ClientSAP;AddressingSchem=_s.AddressingSchem;ServerPhysicalID=_s.ServerPhysicalID;HDLCAddressing=_s.HDLCAddressing;InfoSize=_s.InformationSize;WindowSize=_s.WindowSize;CosemBufferSize=_s.CosemBufferSize;DLLBufferSize=_s.DLLBufferSize;ApplicationContext=_s.ApplicationContext;SecurityMechanism=_s.SecurityMechanism;PDUSize=_s.PDUSize;ConformanceBlock=_s.ConformanceBlock;DLMSVersion=_s.DLMSVersion;AssociationMode=_s.AssociationMode;AssociationType=_s.AssociationType;AssociationAccess=_s.AssociationAccess;Password=_s.Password;HLSKey=_s.HLSKey;HLSPWD=_s.HLSPWD;ClientSystemTitle=_s.ClientSystemTitle;Securitysuit=_s.Securitysuit;GlobalEncryptionKey=_s.GlobalEncryptionKey;DedicatedKey=_s.DedicatedKey;AuthenticationKey=_s.AuthenticationKey;AESEncryption=_s.AESEncryption;MeterMode=_s.MeterMode;CommunicationPort=_s.CommunicationPort;CommunicationMode=_s.CommunicationMode;ModemNumber=_s.ModemNumber;AppUser=_s.AppUser;AppPwd=_s.AppPwd;AppUserRememberMe=_s.AppUserRememberMe;DefaultReadClassID=_s.DefaultReadClassID;DefaultReadOBIS=_s.DefaultReadOBIS;DefaultReadAtt=_s.DefaultReadAtt;DefaultReadDaraType=_s.DefaultReadDaraType;DefaultReadLen=_s.DefaultReadLen;DefaultReadAccSelector=_s.DefaultReadAccSelector;DefaultReadCmdType=_s.DefaultReadCmdType;DefaultReadSelectiveAccessCommand=_s.DefaultReadSelectiveAccessCommand;DefaultReadDataValueCommand=_s.DefaultReadDataValueCommand;Cnf1=_s.Cnf1;Cnf2=_s.Cnf2;Cnf3=_s.Cnf3;}
    [RelayCommand] async Task SaveSettingsAsync(){IsBusy=true;try{_s.SerialPort=SerialPort;_s.SignOnBaudRate=SignOnBaudRate;_s.CommandBaudRate=CommandBaudRate;_s.StopBits=StopBits;_s.DataBits=DataBits;_s.Parity=Parity;_s.CommandTimeOut=CommandTimeOut;_s.IntercharacterDelay=IntercharacterDelay;_s.InterframeTimeout=InterframeTimeout;_s.ServerSAP=ServerSAP;_s.ServerLowerMacAddress=ServerLowerMacAddress;_s.ClientSAP=ClientSAP;_s.AddressingSchem=AddressingSchem;_s.ServerPhysicalID=ServerPhysicalID;_s.HDLCAddressing=HDLCAddressing;_s.InformationSize=InfoSize;_s.WindowSize=WindowSize;_s.CosemBufferSize=CosemBufferSize;_s.DLLBufferSize=DLLBufferSize;_s.ApplicationContext=(byte)ApplicationContext;_s.SecurityMechanism=(byte)SecurityMechanism;_s.PDUSize=PDUSize;_s.ConformanceBlock=ConformanceBlock;_s.DLMSVersion=DLMSVersion;_s.AssociationMode=(byte)AssociationMode;_s.AssociationType=(byte)AssociationType;_s.AssociationAccess=AssociationAccess;_s.Password=Password;_s.HLSKey=HLSKey;_s.HLSPWD=HLSPWD;_s.ClientSystemTitle=ClientSystemTitle;_s.Securitysuit=Securitysuit;_s.GlobalEncryptionKey=GlobalEncryptionKey;_s.DedicatedKey=DedicatedKey;_s.AuthenticationKey=AuthenticationKey;_s.AESEncryption=AESEncryption;_s.MeterMode=MeterMode;_s.CommunicationPort=CommunicationPort;_s.CommunicationMode=CommunicationMode;_s.ModemNumber=ModemNumber;_s.AppUser=AppUser;_s.AppPwd=AppPwd;_s.AppUserRememberMe=AppUserRememberMe;_s.DefaultReadClassID=DefaultReadClassID;_s.DefaultReadOBIS=DefaultReadOBIS;_s.DefaultReadAtt=DefaultReadAtt;_s.DefaultReadDaraType=DefaultReadDaraType;_s.DefaultReadLen=DefaultReadLen;_s.DefaultReadAccSelector=DefaultReadAccSelector;_s.DefaultReadCmdType=DefaultReadCmdType;_s.DefaultReadSelectiveAccessCommand=DefaultReadSelectiveAccessCommand;_s.DefaultReadDataValueCommand=DefaultReadDataValueCommand;_s.Cnf1=Cnf1;_s.Cnf2=Cnf2;_s.Cnf3=Cnf3;_s.Save();SetStatus("Settings saved.");await Task.Delay(500);await _nav.GoBackAsync();}catch(Exception ex){SetStatus($"Save error: {ex.Message}",true);}finally{IsBusy=false;}}
    [RelayCommand] async Task CancelAsync()
    {
        IsBusy = true;
        try
        {
            Load();
            ClearStatus();
            await _nav.GoBackAsync();
        }
        catch (Exception ex)
        {
            SetStatus($"Cancel error: {ex.Message}", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void SelectTab(object? tabIndex)
    {
        if (tabIndex is int i)
        {
            SelectedTab = i;
            return;
        }

        if (tabIndex is string s && int.TryParse(s, out var parsed))
        {
            SelectedTab = parsed;
        }
    }
    [RelayCommand] void ResetDefaults(){_s.FromModel(new AppSettings());Load();SetStatus("Settings reset to defaults.");}
}
