using System.Xml.Serialization;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public sealed class XmlBackedSettingsService : ISettingsService
{
    readonly PreferencesSettingsService _prefs = new();
    readonly string _settingsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Settings", "cabcon-settings.xml");

    public XmlBackedSettingsService()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return;
            }

            using var stream = File.OpenRead(_settingsFilePath);
            var serializer = new XmlSerializer(typeof(AppSettings));
            if (serializer.Deserialize(stream) is AppSettings settings)
            {
                _prefs.FromModel(settings);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Settings] XML load failed: {ex.Message}");
        }
    }

    public string SerialPort { get => _prefs.SerialPort; set => _prefs.SerialPort = value; }
    public string SignOnBaudRate { get => _prefs.SignOnBaudRate; set => _prefs.SignOnBaudRate = value; }
    public string CommandBaudRate { get => _prefs.CommandBaudRate; set => _prefs.CommandBaudRate = value; }
    public string StopBits { get => _prefs.StopBits; set => _prefs.StopBits = value; }
    public string DataBits { get => _prefs.DataBits; set => _prefs.DataBits = value; }
    public string Parity { get => _prefs.Parity; set => _prefs.Parity = value; }
    public int CommandTimeOut { get => _prefs.CommandTimeOut; set => _prefs.CommandTimeOut = value; }
    public int IntercharacterDelay { get => _prefs.IntercharacterDelay; set => _prefs.IntercharacterDelay = value; }
    public int InterframeTimeout { get => _prefs.InterframeTimeout; set => _prefs.InterframeTimeout = value; }
    public int ServerSAP { get => _prefs.ServerSAP; set => _prefs.ServerSAP = value; }
    public int ServerLowerMacAddress { get => _prefs.ServerLowerMacAddress; set => _prefs.ServerLowerMacAddress = value; }
    public int ClientSAP { get => _prefs.ClientSAP; set => _prefs.ClientSAP = value; }
    public int AddressingSchem { get => _prefs.AddressingSchem; set => _prefs.AddressingSchem = value; }
    public int ServerPhysicalID { get => _prefs.ServerPhysicalID; set => _prefs.ServerPhysicalID = value; }
    public int HDLCAddressing { get => _prefs.HDLCAddressing; set => _prefs.HDLCAddressing = value; }
    public byte ApplicationContext { get => _prefs.ApplicationContext; set => _prefs.ApplicationContext = value; }
    public byte SecurityMechanism { get => _prefs.SecurityMechanism; set => _prefs.SecurityMechanism = value; }
    public int PDUSize { get => _prefs.PDUSize; set => _prefs.PDUSize = value; }
    public string ConformanceBlock { get => "0100121A"; set => _prefs.ConformanceBlock = "0100121A"; }
    public int DLMSVersion { get => _prefs.DLMSVersion; set => _prefs.DLMSVersion = value; }
    public int InformationSize { get => _prefs.InformationSize; set => _prefs.InformationSize = value; }
    public int WindowSize { get => _prefs.WindowSize; set => _prefs.WindowSize = value; }
    public int CosemBufferSize { get => _prefs.CosemBufferSize; set => _prefs.CosemBufferSize = value; }
    public int DLLBufferSize { get => _prefs.DLLBufferSize; set => _prefs.DLLBufferSize = value; }
    public string Password { get => _prefs.Password; set => _prefs.Password = value; }
    public string HLSKey { get => _prefs.HLSKey; set => _prefs.HLSKey = value; }
    public string HLSPWD { get => _prefs.HLSPWD; set => _prefs.HLSPWD = value; }
    public string ClientSystemTitle { get => _prefs.ClientSystemTitle; set => _prefs.ClientSystemTitle = value; }
    public int Securitysuit { get => _prefs.Securitysuit; set => _prefs.Securitysuit = value; }
    public string GlobalEncryptionKey { get => _prefs.GlobalEncryptionKey; set => _prefs.GlobalEncryptionKey = value; }
    public int DedicatedKey { get => _prefs.DedicatedKey; set => _prefs.DedicatedKey = value; }
    public string AuthenticationKey { get => _prefs.AuthenticationKey; set => _prefs.AuthenticationKey = value; }
    public string AESEncryption { get => _prefs.AESEncryption; set => _prefs.AESEncryption = value; }
    public byte AssociationMode { get => _prefs.AssociationMode; set => _prefs.AssociationMode = value; }
    public byte AssociationType { get => _prefs.AssociationType; set => _prefs.AssociationType = value; }
    public string AssociationAccess { get => _prefs.AssociationAccess; set => _prefs.AssociationAccess = value; }
    public int MeterMode { get => _prefs.MeterMode; set => _prefs.MeterMode = value; }
    public string ScaleXMLPath { get => _prefs.ScaleXMLPath; set => _prefs.ScaleXMLPath = value; }
    public string ReadOut { get => _prefs.ReadOut; set => _prefs.ReadOut = value; }
    public string Billing { get => _prefs.Billing; set => _prefs.Billing = value; }
    public string BillingVZ { get => _prefs.BillingVZ; set => _prefs.BillingVZ = value; }
    public string TamperXML { get => _prefs.TamperXML; set => _prefs.TamperXML = value; }
    public string CommunicationPort { get => _prefs.CommunicationPort; set => _prefs.CommunicationPort = value; }
    public string CommunicationMode { get => _prefs.CommunicationMode; set => _prefs.CommunicationMode = value; }
    public string ModemNumber { get => _prefs.ModemNumber; set => _prefs.ModemNumber = value; }
    public string AppUser { get => _prefs.AppUser; set => _prefs.AppUser = value; }
    public string AppPwd { get => _prefs.AppPwd; set => _prefs.AppPwd = value; }
    public bool AppUserRememberMe { get => _prefs.AppUserRememberMe; set => _prefs.AppUserRememberMe = value; }
    public int BaudRateSelectedIndex { get => _prefs.BaudRateSelectedIndex; set => _prefs.BaudRateSelectedIndex = value; }
    public string DefaultReadClassID { get => _prefs.DefaultReadClassID; set => _prefs.DefaultReadClassID = value; }
    public string DefaultReadOBIS { get => _prefs.DefaultReadOBIS; set => _prefs.DefaultReadOBIS = value; }
    public string DefaultReadAtt { get => _prefs.DefaultReadAtt; set => _prefs.DefaultReadAtt = value; }
    public string DefaultReadDaraType { get => _prefs.DefaultReadDaraType; set => _prefs.DefaultReadDaraType = value; }
    public string DefaultReadLen { get => _prefs.DefaultReadLen; set => _prefs.DefaultReadLen = value; }
    public int DefaultReadAccSelector { get => _prefs.DefaultReadAccSelector; set => _prefs.DefaultReadAccSelector = value; }
    public int DefaultReadCmdType { get => _prefs.DefaultReadCmdType; set => _prefs.DefaultReadCmdType = value; }
    public string DefaultReadSelectiveAccessCommand { get => _prefs.DefaultReadSelectiveAccessCommand; set => _prefs.DefaultReadSelectiveAccessCommand = value; }
    public string DefaultReadDataValueCommand { get => _prefs.DefaultReadDataValueCommand; set => _prefs.DefaultReadDataValueCommand = value; }
    public int Cnf1 { get => _prefs.Cnf1; set => _prefs.Cnf1 = value; }
    public int Cnf2 { get => _prefs.Cnf2; set => _prefs.Cnf2 = value; }
    public int Cnf3 { get => _prefs.Cnf3; set => _prefs.Cnf3 = value; }

    public void Save()
    {
        _prefs.Save();

        try
        {
            var directory = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var stream = File.Create(_settingsFilePath);
            var serializer = new XmlSerializer(typeof(AppSettings));
            serializer.Serialize(stream, _prefs.ToModel());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Settings] XML save failed: {ex.Message}");
        }
    }

    public void SetSecurityMachanism(byte v) => _prefs.SetSecurityMachanism(v);
    public byte GetSecurityMachanism() => _prefs.GetSecurityMachanism();
    public void SetApplicationContext(byte v) => _prefs.SetApplicationContext(v);
    public byte GetApplicationContext() => _prefs.GetApplicationContext();
    public void SetClientSAP(int v) => _prefs.SetClientSAP(v);
    public string GetClientSAP() => _prefs.GetClientSAP();
    public int GetMeterMode() => _prefs.GetMeterMode();
    public void SetMeterMode(int v) => _prefs.SetMeterMode(v);
    public string GetAppUser() => _prefs.GetAppUser();
    public void SetAppUser(string v) => _prefs.SetAppUser(v);
    public string GetAppPwd() => _prefs.GetAppPwd();
    public void SetAppPwd(string v) => _prefs.SetAppPwd(v);
    public void SetApppwd(string v) => _prefs.SetApppwd(v);
    public bool GetAppUserRememberMe() => _prefs.GetAppUserRememberMe();
    public void SetAppUserRememberMe(bool v) => _prefs.SetAppUserRememberMe(v);
    public string GetScaleXMLPath() => _prefs.GetScaleXMLPath();
    public void SetScaleXMLPath(string v) => _prefs.SetScaleXMLPath(v);
    public string GetLLSPassword() => _prefs.GetLLSPassword();
    public void SetLLSPassword(string v) => _prefs.SetLLSPassword(v);
    public string GetHLSPassword() => _prefs.GetHLSPassword();
    public void SetHLSPWD(string v) => _prefs.SetHLSPWD(v);
    public int GetAssociationMode() => _prefs.GetAssociationMode();
    public void SetAssociationMode(int v) => _prefs.SetAssociationMode(v);
    public int GetAssociationType() => _prefs.GetAssociationType();
    public void SetAssociationType(int v) => _prefs.SetAssociationType(v);
    public string GetAssociationAccess() => _prefs.GetAssociationAccess();
    public void SetAssociationAccess(string v) => _prefs.SetAssociationAccess(v);
    public string GetGlobalEncryptionKey() => _prefs.GetGlobalEncryptionKey();
    public string[] GetReadoutCommandStructure() => _prefs.GetReadoutCommandStructure();
    public void SetReadoutCommandStructure(string[] cmd) => _prefs.SetReadoutCommandStructure(cmd);
    public void SetCipheredSecurityResponse(string llsPwd, string hlsPwd, string encKey) => _prefs.SetCipheredSecurityResponse(llsPwd, hlsPwd, encKey);
    public AppSettings ToModel() => _prefs.ToModel();
    public void FromModel(AppSettings model) => _prefs.FromModel(model);
}
