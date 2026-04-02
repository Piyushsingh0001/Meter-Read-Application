using CabconMAUI.Models;
namespace CabconMAUI.Services.Interfaces;
public interface ISettingsService
{
    string SerialPort{get;set;} string SignOnBaudRate{get;set;} string CommandBaudRate{get;set;}
    string StopBits{get;set;} string DataBits{get;set;} string Parity{get;set;}
    int CommandTimeOut{get;set;} int IntercharacterDelay{get;set;} int InterframeTimeout{get;set;}
    int ServerSAP{get;set;} int ServerLowerMacAddress{get;set;} int ClientSAP{get;set;}
    int AddressingSchem{get;set;} int ServerPhysicalID{get;set;} int HDLCAddressing{get;set;}
    byte ApplicationContext{get;set;} byte SecurityMechanism{get;set;}
    int PDUSize{get;set;} string ConformanceBlock{get;set;} int DLMSVersion{get;set;}
    int InformationSize{get;set;} int WindowSize{get;set;} int CosemBufferSize{get;set;} int DLLBufferSize{get;set;}
    string Password{get;set;} string HLSKey{get;set;} string HLSPWD{get;set;}
    string ClientSystemTitle{get;set;} int Securitysuit{get;set;}
    string GlobalEncryptionKey{get;set;} int DedicatedKey{get;set;} string AuthenticationKey{get;set;} string AESEncryption{get;set;}
    byte AssociationMode{get;set;} byte AssociationType{get;set;} string AssociationAccess{get;set;}
    int MeterMode{get;set;}
    string ScaleXMLPath{get;set;} string ReadOut{get;set;} string Billing{get;set;} string BillingVZ{get;set;} string TamperXML{get;set;}
    string CommunicationPort{get;set;} string CommunicationMode{get;set;} string ModemNumber{get;set;}
    string AppUser{get;set;} string AppPwd{get;set;} bool AppUserRememberMe{get;set;}
    int BaudRateSelectedIndex{get;set;}
    string DefaultReadClassID{get;set;} string DefaultReadOBIS{get;set;} string DefaultReadAtt{get;set;}
    string DefaultReadDaraType{get;set;} string DefaultReadLen{get;set;}
    int DefaultReadAccSelector{get;set;} int DefaultReadCmdType{get;set;}
    string DefaultReadSelectiveAccessCommand{get;set;} string DefaultReadDataValueCommand{get;set;}
    int Cnf1{get;set;} int Cnf2{get;set;} int Cnf3{get;set;}
    void Save();
    void   SetSecurityMachanism(byte v); byte   GetSecurityMachanism();
    void   SetApplicationContext(byte v); byte   GetApplicationContext();
    void   SetClientSAP(int v); string GetClientSAP();
    int    GetMeterMode(); void   SetMeterMode(int v);
    string GetAppUser(); void   SetAppUser(string v);
    string GetAppPwd();  void   SetAppPwd(string v);  void SetApppwd(string v);
    bool   GetAppUserRememberMe(); void SetAppUserRememberMe(bool v);
    string GetScaleXMLPath(); void SetScaleXMLPath(string v);
    string GetLLSPassword(); void SetLLSPassword(string v);
    string GetHLSPassword(); void SetHLSPWD(string v);
    int    GetAssociationMode(); void SetAssociationMode(int v);
    int    GetAssociationType(); void SetAssociationType(int v);
    string GetAssociationAccess(); void SetAssociationAccess(string v);
    string GetGlobalEncryptionKey();
    string[] GetReadoutCommandStructure(); void SetReadoutCommandStructure(string[] cmd);
    void   SetCipheredSecurityResponse(string llsPwd, string hlsPwd, string encKey);
    AppSettings ToModel(); void FromModel(AppSettings model);
}
