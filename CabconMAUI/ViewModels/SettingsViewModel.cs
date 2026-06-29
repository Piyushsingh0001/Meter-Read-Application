using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CabconMAUI.Services.Interfaces;
<<<<<<< HEAD
using System.Collections.ObjectModel;
namespace CabconMAUI.ViewModels;
public partial class SettingsViewModel : BaseViewModel
{
    readonly ISettingsService _s; readonly INavigationService _nav; readonly ISerialPortService _serialPortService;
    static readonly string[] WindowSizeOptionsInternal = ["1", "2", "3", "4", "5", "6", "7"];
    static readonly string[] HdlcAddressingSchemeOptionsInternal = ["1 Byte", "2 Byte", "3 Byte", "4 Byte"];
    static readonly string[] ClientTypeOptionsInternal =
   [
    "PC", //10
    "MR", //20
    "US", //30
    "FS", //40
    "FU"  //50
   ];
    static readonly string[] ApplicationContextOptionsInternal =
    [
        "Short Name without ciphering",
        "Logical Name without ciphering",
        "Logical Name with ciphering"
    ];
    static readonly string[] AuthenticationLevelOptionsInternal =
    [
        "No - Security",
        "Low - Level",
        "High - Level"
    ];
    static readonly string[] SecuritySuitOptionsInternal =
    [
        "Authentication Only",
        "Encryption Only",
        "Encryption + Authentication"
    ];
    static readonly string[] DedicatedKeyOptionsInternal = ["True", "False"];
=======
namespace CabconMAUI.ViewModels;
public partial class SettingsViewModel : BaseViewModel
{
    readonly ISettingsService _s; readonly INavigationService _nav;
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023

    [ObservableProperty] private int _selectedTab;
    [ObservableProperty] private string _serialPort=string.Empty;
    [ObservableProperty] private string _signOnBaudRate=string.Empty;
    [ObservableProperty] private string _commandBaudRate=string.Empty;
    [ObservableProperty] private string _stopBits=string.Empty;
    [ObservableProperty] private string _dataBits=string.Empty;
    [ObservableProperty] private string _parity=string.Empty;
<<<<<<< HEAD
    [ObservableProperty] private bool _isPasswordEnabled = true;
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
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
<<<<<<< HEAD
    [ObservableProperty] private string _windowSizeOption = string.Empty;
    [ObservableProperty] private string _hdlcAddressingSchemeOption = string.Empty;
    [ObservableProperty] private string _clientTypeOption = string.Empty;
    [ObservableProperty] private string _applicationContextOption = string.Empty;
    [ObservableProperty] private string _authenticationLevelOption = string.Empty;
    [ObservableProperty] private string _securitySuitOption = string.Empty;
    [ObservableProperty] private string _dedicatedKeyOption = string.Empty;
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    public string[] ParityOptions   =>AppSettings.ParityList;
    public string[] BaudRateOptions =>AppSettings.BaudRateList;
    public string[] StopBitsOptions =>AppSettings.StopBitsList;
    public string[] DataBitsOptions =>AppSettings.DataBitsList;
<<<<<<< HEAD
    public ObservableCollection<string> CommPortOptions { get; } = [];
    public string[] CommModeOptions =>AppSettings.CommModeList;
    public IReadOnlyList<string> WindowSizeOptions => WindowSizeOptionsInternal;
    public IReadOnlyList<string> HdlcAddressingSchemeOptions => HdlcAddressingSchemeOptionsInternal;
    public IReadOnlyList<string> ClientTypeOptions => ClientTypeOptionsInternal;
    public IReadOnlyList<string> ApplicationContextOptions => ApplicationContextOptionsInternal;
    public IReadOnlyList<string> AuthenticationLevelOptions => AuthenticationLevelOptionsInternal;
    public IReadOnlyList<string> SecuritySuitOptions => SecuritySuitOptionsInternal;
    public IReadOnlyList<string> DedicatedKeyOptions => DedicatedKeyOptionsInternal;
    public IReadOnlyList<MeterVariant> MeterVariants=>MeterVariant.AllVariants;
    public SettingsViewModel(ISettingsService s,INavigationService n, ISerialPortService serialPortService){_s=s;_nav=n;_serialPortService=serialPortService;Load();}

    partial void OnClientTypeOptionChanged(string value)
    {
        switch (value)
        {
            case "PC":
                AuthenticationLevelOption = AuthenticationLevelOptionsInternal[0]; // No Security
                break;
            case "MR":
                AuthenticationLevelOption = AuthenticationLevelOptionsInternal[1]; // Low Level
                SecuritySuitOption = SecuritySuitOptionsInternal[1]; // Encryption Only
                break;
            case "US":
            case "FU":
                AuthenticationLevelOption = AuthenticationLevelOptionsInternal[2]; // High Level
                SecuritySuitOption = SecuritySuitOptionsInternal[2]; // Enc + Auth
                break;
            case "FS":
                ApplicationContextOption = ApplicationContextOptionsInternal[1]; // Logical Name without ciphering
                break;
        }
    }

    partial void OnAuthenticationLevelOptionChanged(string value)
    {
        IsPasswordEnabled = value != AuthenticationLevelOptionsInternal[0];
    }

    void Load()
    {
        RefreshAvailablePorts();
        SerialPort = _s.SerialPort;
        SignOnBaudRate = _s.SignOnBaudRate;
        CommandBaudRate = _s.CommandBaudRate;
        StopBits = _s.StopBits;
        DataBits = _s.DataBits;
        Parity = _s.Parity;
        CommandTimeOut = _s.CommandTimeOut;
        IntercharacterDelay = _s.IntercharacterDelay;
        InterframeTimeout = _s.InterframeTimeout;
        ServerSAP = _s.ServerSAP;
        ServerLowerMacAddress = _s.ServerLowerMacAddress;
        ClientSAP = _s.ClientSAP;
        AddressingSchem = _s.AddressingSchem;
        ServerPhysicalID = _s.ServerPhysicalID;
        HDLCAddressing = _s.HDLCAddressing;
        InfoSize = _s.InformationSize;
        WindowSize = _s.WindowSize;
        CosemBufferSize = _s.CosemBufferSize;
        DLLBufferSize = _s.DLLBufferSize;
        ApplicationContext = _s.ApplicationContext;
        SecurityMechanism = _s.SecurityMechanism;
        PDUSize = _s.PDUSize;
        ConformanceBlock = _s.ConformanceBlock;
        DLMSVersion = _s.DLMSVersion;
        AssociationMode = _s.AssociationMode;
        AssociationType = _s.AssociationType;
        AssociationAccess = _s.AssociationAccess;
        Password = _s.Password;
        HLSKey = _s.HLSKey;
        HLSPWD = _s.HLSPWD;
        ClientSystemTitle = _s.ClientSystemTitle;
        Securitysuit = _s.Securitysuit;
        GlobalEncryptionKey = _s.GlobalEncryptionKey;
        DedicatedKey = _s.DedicatedKey;
        AuthenticationKey = _s.AuthenticationKey;
        AESEncryption = _s.AESEncryption;
        MeterMode = _s.MeterMode;
        CommunicationPort = _s.CommunicationPort;
        EnsureSelectedPortAvailable();
        CommunicationMode = _s.CommunicationMode;
        ModemNumber = _s.ModemNumber;
        AppUser = _s.AppUser;
        AppPwd = _s.AppPwd;
        AppUserRememberMe = _s.AppUserRememberMe;
        DefaultReadClassID = _s.DefaultReadClassID;
        DefaultReadOBIS = _s.DefaultReadOBIS;
        DefaultReadAtt = _s.DefaultReadAtt;
        DefaultReadDaraType = _s.DefaultReadDaraType;
        DefaultReadLen = _s.DefaultReadLen;
        DefaultReadAccSelector = _s.DefaultReadAccSelector;
        DefaultReadCmdType = _s.DefaultReadCmdType;
        DefaultReadSelectiveAccessCommand = _s.DefaultReadSelectiveAccessCommand;
        DefaultReadDataValueCommand = _s.DefaultReadDataValueCommand;
        Cnf1 = _s.Cnf1;
        Cnf2 = _s.Cnf2;
        Cnf3 = _s.Cnf3;

        WindowSizeOption = GetWindowSizeOption(WindowSize);
        HdlcAddressingSchemeOption = GetHdlcAddressingSchemeOption(AddressingSchem);
        //ClientTypeOption = GetClientTypeOption(AssociationType);
        ClientTypeOption = GetClientTypeOption(AssociationType);

        // Force UI to sync Auth Level, Security Suit, and App Context based strictly on Client Type
        OnClientTypeOptionChanged(ClientTypeOption);
        
        DedicatedKeyOption = GetDedicatedKeyOption(DedicatedKey);
        IsBusy = false;
    }

    [RelayCommand]
    async Task SaveSettingsAsync()
    {
        IsBusy = true;
        try
        {
            WindowSize = ParseWindowSize(WindowSizeOption, WindowSize);
            AddressingSchem = ParseHdlcAddressingScheme(HdlcAddressingSchemeOption, AddressingSchem);
            AssociationType = ParseClientType(ClientTypeOption, AssociationType);
            ClientSAP = GetClientSapFromType(ClientTypeOption, ClientSAP);
            ApplicationContext = ParseApplicationContext(ApplicationContextOption, ApplicationContext);
            SecurityMechanism = ParseAuthenticationLevel(AuthenticationLevelOption, SecurityMechanism);
            Securitysuit = ParseSecuritySuit(SecuritySuitOption, Securitysuit);
            DedicatedKey = ParseDedicatedKey(DedicatedKeyOption, DedicatedKey);

            _s.SerialPort = SerialPort;
            _s.SignOnBaudRate = SignOnBaudRate;
            _s.CommandBaudRate = CommandBaudRate;
            _s.StopBits = StopBits;
            _s.DataBits = DataBits;
            _s.Parity = Parity;
            _s.CommandTimeOut = CommandTimeOut;
            _s.IntercharacterDelay = IntercharacterDelay;
            _s.InterframeTimeout = InterframeTimeout;
            _s.ServerSAP = ServerSAP;
            _s.ServerLowerMacAddress = ServerLowerMacAddress;
            _s.ClientSAP = ClientSAP;
            _s.AddressingSchem = AddressingSchem;
            _s.ServerPhysicalID = ServerPhysicalID;
            _s.HDLCAddressing = HDLCAddressing;
            _s.InformationSize = InfoSize;
            _s.WindowSize = WindowSize;
            _s.CosemBufferSize = CosemBufferSize;
            _s.DLLBufferSize = DLLBufferSize;
            _s.ApplicationContext = (byte)ApplicationContext;
            _s.SecurityMechanism = (byte)SecurityMechanism;
            _s.PDUSize = PDUSize;
            _s.ConformanceBlock = ConformanceBlock;
            _s.DLMSVersion = DLMSVersion;
            _s.AssociationMode = (byte)AssociationMode;
            _s.AssociationType = (byte)AssociationType;
            _s.AssociationAccess = AssociationAccess;
            _s.Password = Password;
            _s.HLSKey = HLSKey;
            _s.HLSPWD = HLSPWD;
            _s.ClientSystemTitle = ClientSystemTitle;
            _s.Securitysuit = Securitysuit;
            _s.GlobalEncryptionKey = GlobalEncryptionKey;
            _s.DedicatedKey = DedicatedKey;
            _s.AuthenticationKey = AuthenticationKey;
            _s.AESEncryption = AESEncryption;
            _s.MeterMode = MeterMode;
            _s.CommunicationPort = CommunicationPort;
            _s.CommunicationMode = CommunicationMode;
            _s.ModemNumber = ModemNumber;
            _s.AppUser = AppUser;
            _s.AppPwd = AppPwd;
            _s.AppUserRememberMe = AppUserRememberMe;
            _s.DefaultReadClassID = DefaultReadClassID;
            _s.DefaultReadOBIS = DefaultReadOBIS;
            _s.DefaultReadAtt = DefaultReadAtt;
            _s.DefaultReadDaraType = DefaultReadDaraType;
            _s.DefaultReadLen = DefaultReadLen;
            _s.DefaultReadAccSelector = DefaultReadAccSelector;
            _s.DefaultReadCmdType = DefaultReadCmdType;
            _s.DefaultReadSelectiveAccessCommand = DefaultReadSelectiveAccessCommand;
            _s.DefaultReadDataValueCommand = DefaultReadDataValueCommand;
            _s.Cnf1 = Cnf1;
            _s.Cnf2 = Cnf2;
            _s.Cnf3 = Cnf3;
            _s.Save();
            SetStatus("Settings saved.");
            await Task.Delay(500);
            await _nav.GoBackAsync();
        }
        catch(Exception ex)
        {
            SetStatus($"Save error: {ex.Message}",true);
        }
        finally
        {
            IsBusy=false;
        }
    }
=======
    public string[] CommPortOptions =>AppSettings.CommPortList;
    public string[] CommModeOptions =>AppSettings.CommModeList;
    public IReadOnlyList<MeterVariant> MeterVariants=>MeterVariant.AllVariants;
    public SettingsViewModel(ISettingsService s,INavigationService n){_s=s;_nav=n;Load();}
    void Load(){SerialPort=_s.SerialPort;SignOnBaudRate=_s.SignOnBaudRate;CommandBaudRate=_s.CommandBaudRate;StopBits=_s.StopBits;DataBits=_s.DataBits;Parity=_s.Parity;CommandTimeOut=_s.CommandTimeOut;IntercharacterDelay=_s.IntercharacterDelay;InterframeTimeout=_s.InterframeTimeout;ServerSAP=_s.ServerSAP;ServerLowerMacAddress=_s.ServerLowerMacAddress;ClientSAP=_s.ClientSAP;AddressingSchem=_s.AddressingSchem;ServerPhysicalID=_s.ServerPhysicalID;HDLCAddressing=_s.HDLCAddressing;InfoSize=_s.InformationSize;WindowSize=_s.WindowSize;CosemBufferSize=_s.CosemBufferSize;DLLBufferSize=_s.DLLBufferSize;ApplicationContext=_s.ApplicationContext;SecurityMechanism=_s.SecurityMechanism;PDUSize=_s.PDUSize;ConformanceBlock=_s.ConformanceBlock;DLMSVersion=_s.DLMSVersion;AssociationMode=_s.AssociationMode;AssociationType=_s.AssociationType;AssociationAccess=_s.AssociationAccess;Password=_s.Password;HLSKey=_s.HLSKey;HLSPWD=_s.HLSPWD;ClientSystemTitle=_s.ClientSystemTitle;Securitysuit=_s.Securitysuit;GlobalEncryptionKey=_s.GlobalEncryptionKey;DedicatedKey=_s.DedicatedKey;AuthenticationKey=_s.AuthenticationKey;AESEncryption=_s.AESEncryption;MeterMode=_s.MeterMode;CommunicationPort=_s.CommunicationPort;CommunicationMode=_s.CommunicationMode;ModemNumber=_s.ModemNumber;AppUser=_s.AppUser;AppPwd=_s.AppPwd;AppUserRememberMe=_s.AppUserRememberMe;DefaultReadClassID=_s.DefaultReadClassID;DefaultReadOBIS=_s.DefaultReadOBIS;DefaultReadAtt=_s.DefaultReadAtt;DefaultReadDaraType=_s.DefaultReadDaraType;DefaultReadLen=_s.DefaultReadLen;DefaultReadAccSelector=_s.DefaultReadAccSelector;DefaultReadCmdType=_s.DefaultReadCmdType;DefaultReadSelectiveAccessCommand=_s.DefaultReadSelectiveAccessCommand;DefaultReadDataValueCommand=_s.DefaultReadDataValueCommand;Cnf1=_s.Cnf1;Cnf2=_s.Cnf2;Cnf3=_s.Cnf3;}
    [RelayCommand] async Task SaveSettingsAsync(){IsBusy=true;try{_s.SerialPort=SerialPort;_s.SignOnBaudRate=SignOnBaudRate;_s.CommandBaudRate=CommandBaudRate;_s.StopBits=StopBits;_s.DataBits=DataBits;_s.Parity=Parity;_s.CommandTimeOut=CommandTimeOut;_s.IntercharacterDelay=IntercharacterDelay;_s.InterframeTimeout=InterframeTimeout;_s.ServerSAP=ServerSAP;_s.ServerLowerMacAddress=ServerLowerMacAddress;_s.ClientSAP=ClientSAP;_s.AddressingSchem=AddressingSchem;_s.ServerPhysicalID=ServerPhysicalID;_s.HDLCAddressing=HDLCAddressing;_s.InformationSize=InfoSize;_s.WindowSize=WindowSize;_s.CosemBufferSize=CosemBufferSize;_s.DLLBufferSize=DLLBufferSize;_s.ApplicationContext=(byte)ApplicationContext;_s.SecurityMechanism=(byte)SecurityMechanism;_s.PDUSize=PDUSize;_s.ConformanceBlock=ConformanceBlock;_s.DLMSVersion=DLMSVersion;_s.AssociationMode=(byte)AssociationMode;_s.AssociationType=(byte)AssociationType;_s.AssociationAccess=AssociationAccess;_s.Password=Password;_s.HLSKey=HLSKey;_s.HLSPWD=HLSPWD;_s.ClientSystemTitle=ClientSystemTitle;_s.Securitysuit=Securitysuit;_s.GlobalEncryptionKey=GlobalEncryptionKey;_s.DedicatedKey=DedicatedKey;_s.AuthenticationKey=AuthenticationKey;_s.AESEncryption=AESEncryption;_s.MeterMode=MeterMode;_s.CommunicationPort=CommunicationPort;_s.CommunicationMode=CommunicationMode;_s.ModemNumber=ModemNumber;_s.AppUser=AppUser;_s.AppPwd=AppPwd;_s.AppUserRememberMe=AppUserRememberMe;_s.DefaultReadClassID=DefaultReadClassID;_s.DefaultReadOBIS=DefaultReadOBIS;_s.DefaultReadAtt=DefaultReadAtt;_s.DefaultReadDaraType=DefaultReadDaraType;_s.DefaultReadLen=DefaultReadLen;_s.DefaultReadAccSelector=DefaultReadAccSelector;_s.DefaultReadCmdType=DefaultReadCmdType;_s.DefaultReadSelectiveAccessCommand=DefaultReadSelectiveAccessCommand;_s.DefaultReadDataValueCommand=DefaultReadDataValueCommand;_s.Cnf1=Cnf1;_s.Cnf2=Cnf2;_s.Cnf3=Cnf3;_s.Save();SetStatus("Settings saved.");await Task.Delay(500);await _nav.GoBackAsync();}catch(Exception ex){SetStatus($"Save error: {ex.Message}",true);}finally{IsBusy=false;}}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
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
<<<<<<< HEAD
    [RelayCommand]
    void RefreshPorts()
    {
        RefreshAvailablePorts();
        EnsureSelectedPortAvailable();
        SetStatus(CommPortOptions.Count > 0 ? "Ports refreshed." : "No communication ports detected.", CommPortOptions.Count == 0);
    }

    void RefreshAvailablePorts()
    {
        var ports = _serialPortService.GetAvailablePorts()
            ?.Where(port => !string.IsNullOrWhiteSpace(port))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(port => port, StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        CommPortOptions.Clear();
        foreach (var port in ports)
        {
            CommPortOptions.Add(port);
        }
    }

    void EnsureSelectedPortAvailable()
    {
        if (!string.IsNullOrWhiteSpace(CommunicationPort) && !CommPortOptions.Contains(CommunicationPort))
        {
            CommPortOptions.Insert(0, CommunicationPort);
        }

        if (string.IsNullOrWhiteSpace(CommunicationPort) && CommPortOptions.Count > 0)
        {
            CommunicationPort = CommPortOptions[0];
        }
    }

    static string GetWindowSizeOption(int value) => WindowSizeOptionsInternal.FirstOrDefault(option => option == value.ToString()) ?? WindowSizeOptionsInternal[0];
    static int ParseWindowSize(string? option, int fallback) => int.TryParse(option, out var value) ? value : fallback;

    static string GetHdlcAddressingSchemeOption(int value) => HdlcAddressingSchemeOptionsInternal.FirstOrDefault(option => option.StartsWith(value.ToString())) ?? HdlcAddressingSchemeOptionsInternal[0];
    static int ParseHdlcAddressingScheme(string? option, int fallback) => !string.IsNullOrWhiteSpace(option) && int.TryParse(option[..1], out var value) ? value : fallback;

    /* static string GetClientTypeOption(int value)
     {
         var index = value - 1;
         return index >= 0 && index < ClientTypeOptionsInternal.Length ? ClientTypeOptionsInternal[index] : ClientTypeOptionsInternal[0];
     }*/
    static string GetClientTypeOption(int value)
    {
        return value switch
        {
            1 => "PC",
            2 => "MR",
            3 => "US",
            4 => "FS",
            5 => "FU",
            _ => "PC"
        };
    }
    static int GetClientSapFromType(string? option, int fallback)
    {
        return option switch
        {
            "PC" => 0x10,
            "MR" => 0x20,
            "US" => 0x30,
            "FS" => 0x40,
            "FU" => 0x50,
            _ => fallback
        };
    }
    /*   static int ParseClientType(string? option, int fallback)
       {
           var index = Array.IndexOf(ClientTypeOptionsInternal, option ?? string.Empty);
           return index >= 0 ? index + 1 : fallback;
       }*/

    static int ParseClientType(string? option, int fallback)
    {
        return option switch
        {
            "PC" => 0x10, //16
            "MR" => 0x20, //32
            "US" => 0x30, //48
            "FS" => 0x40, //64
            "FU" => 0x50, //80
            _ => fallback
        };
    }
    static string GetApplicationContextOption(int value) => value switch
    {
        2 => ApplicationContextOptionsInternal[0],
        1 => ApplicationContextOptionsInternal[1],
        3 => ApplicationContextOptionsInternal[2],
        _ => ApplicationContextOptionsInternal[1]
    };

    static int ParseApplicationContext(string? option, int fallback) => option switch
    {
        "Short Name without ciphering" => 2,
        "Logical Name without ciphering" => 1,
        "Logical Name with ciphering" => 3,
        _ => fallback
    };

    static string GetAuthenticationLevelOption(int value) => value switch
    {
        0 => AuthenticationLevelOptionsInternal[0],
        1 => AuthenticationLevelOptionsInternal[1],
        2 => AuthenticationLevelOptionsInternal[2],
        _ => AuthenticationLevelOptionsInternal[1]
    };

    static int ParseAuthenticationLevel(string? option, int fallback) => option switch
    {
        "No - Security" => 0,
        "Low - Level" => 1,
        "High - Level" => 2,
        _ => fallback
    };

    static string GetSecuritySuitOption(int value) => value switch
    {
        0x10 => SecuritySuitOptionsInternal[0],
        0x20 => SecuritySuitOptionsInternal[1],
        0x30 => SecuritySuitOptionsInternal[2],
        _ => SecuritySuitOptionsInternal[1]
    };

    static int ParseSecuritySuit(string? option, int fallback) => option switch
    {
        "Authentication Only" => 0x10,
        "Encryption Only" => 0x20,
        "Encryption + Authentication" => 0x30,
        _ => fallback
    };

    static string GetDedicatedKeyOption(int value) => value == 1 ? DedicatedKeyOptionsInternal[0] : DedicatedKeyOptionsInternal[1];
    static int ParseDedicatedKey(string? option, int fallback) => option switch
    {
        "True" => 1,
        "False" => 0,
        _ => fallback
    };
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
}
