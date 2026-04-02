using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class PreferencesSettingsService : ISettingsService
{
    static string  S(string k,string  d) => Preferences.Get(k,d);
    static int     I(string k,int     d) => Preferences.Get(k,d);
    static bool    B(string k,bool    d) => Preferences.Get(k,d);
    static byte    By(string k,byte   d) => (byte)Preferences.Get(k,(int)d);
    static void Set(string k,string v)   => Preferences.Set(k,v);
    static void Set(string k,int    v)   => Preferences.Set(k,v);
    static void Set(string k,bool   v)   => Preferences.Set(k,v);

    public string SerialPort          {get=>S(nameof(SerialPort),"COM1");           set=>Set(nameof(SerialPort),value);}
    public string SignOnBaudRate       {get=>S(nameof(SignOnBaudRate),"300");        set=>Set(nameof(SignOnBaudRate),value);}
    public string CommandBaudRate      {get=>S(nameof(CommandBaudRate),"9600");      set=>Set(nameof(CommandBaudRate),value);}
    public string StopBits            {get=>S(nameof(StopBits),"1");               set=>Set(nameof(StopBits),value);}
    public string DataBits            {get=>S(nameof(DataBits),"8");               set=>Set(nameof(DataBits),value);}
    public string Parity              {get=>S(nameof(Parity),"None");              set=>Set(nameof(Parity),value);}
    public int    CommandTimeOut      {get=>I(nameof(CommandTimeOut),3500);         set=>Set(nameof(CommandTimeOut),value);}
    public int    IntercharacterDelay {get=>I(nameof(IntercharacterDelay),2500);   set=>Set(nameof(IntercharacterDelay),value);}
    public int    InterframeTimeout   {get=>I(nameof(InterframeTimeout),1500);     set=>Set(nameof(InterframeTimeout),value);}
    public int    ServerSAP           {get=>I(nameof(ServerSAP),1);                set=>Set(nameof(ServerSAP),value);}
    public int    ServerLowerMacAddress{get=>I(nameof(ServerLowerMacAddress),256); set=>Set(nameof(ServerLowerMacAddress),value);}
    public int    ClientSAP           {get=>I(nameof(ClientSAP),64);               set=>Set(nameof(ClientSAP),value);}
    public int    AddressingSchem     {get=>I(nameof(AddressingSchem),2);           set=>Set(nameof(AddressingSchem),value);}
    public int    ServerPhysicalID    {get=>I(nameof(ServerPhysicalID),1);          set=>Set(nameof(ServerPhysicalID),value);}
    public int    HDLCAddressing      {get=>I(nameof(HDLCAddressing),2);            set=>Set(nameof(HDLCAddressing),value);}
    public byte   ApplicationContext  {get=>By(nameof(ApplicationContext),1);       set=>Set(nameof(ApplicationContext),(int)value);}
    public byte   SecurityMechanism   {get=>By(nameof(SecurityMechanism),1);        set=>Set(nameof(SecurityMechanism),(int)value);}
    public int    PDUSize             {get=>I(nameof(PDUSize),9999);                set=>Set(nameof(PDUSize),value);}
    public string ConformanceBlock    {get=>S(nameof(ConformanceBlock),"1CFF3F");   set=>Set(nameof(ConformanceBlock),value);}
    public int    DLMSVersion         {get=>I(nameof(DLMSVersion),6);               set=>Set(nameof(DLMSVersion),value);}
    public int    InformationSize     {get=>I(nameof(InformationSize),128);          set=>Set(nameof(InformationSize),value);}
    public int    WindowSize          {get=>I(nameof(WindowSize),1);                set=>Set(nameof(WindowSize),value);}
    public int    CosemBufferSize     {get=>I(nameof(CosemBufferSize),9999);        set=>Set(nameof(CosemBufferSize),value);}
    public int    DLLBufferSize       {get=>I(nameof(DLLBufferSize),9999);          set=>Set(nameof(DLLBufferSize),value);}
    public string Password            {get=>S(nameof(Password),"00000000");         set=>Set(nameof(Password),value);}
    public string HLSKey              {get=>S(nameof(HLSKey),"93BC0FABF6C85E9E1C53D78885373DC7"); set=>Set(nameof(HLSKey),value);}
    public string HLSPWD              {get=>S(nameof(HLSPWD),"000102030405060708090A0B0C0D0E0F");  set=>Set(nameof(HLSPWD),value);}
    public string ClientSystemTitle   {get=>S(nameof(ClientSystemTitle),"12345678");set=>Set(nameof(ClientSystemTitle),value);}
    public int    Securitysuit        {get=>I(nameof(Securitysuit),0x20);            set=>Set(nameof(Securitysuit),value);}
    public string GlobalEncryptionKey {get=>S(nameof(GlobalEncryptionKey),"000102030405060708090A0B0C0D0E0F"); set=>Set(nameof(GlobalEncryptionKey),value);}
    public int    DedicatedKey        {get=>I(nameof(DedicatedKey),1);               set=>Set(nameof(DedicatedKey),value);}
    public string AuthenticationKey   {get=>S(nameof(AuthenticationKey),"000102030405060708090A0B0C0D0E0F"); set=>Set(nameof(AuthenticationKey),value);}
    public string AESEncryption       {get=>S(nameof(AESEncryption),"Non Cyphering");set=>Set(nameof(AESEncryption),value);}
    public byte   AssociationMode     {get=>By(nameof(AssociationMode),1);           set=>Set(nameof(AssociationMode),(int)value);}
    public byte   AssociationType     {get=>By(nameof(AssociationType),2);           set=>Set(nameof(AssociationType),(int)value);}
    public string AssociationAccess   {get=>S(nameof(AssociationAccess),"12345678"); set=>Set(nameof(AssociationAccess),value);}
    public int    MeterMode           {get=>I(nameof(MeterMode),2);                  set=>Set(nameof(MeterMode),value);}
    public string ScaleXMLPath        {get=>S(nameof(ScaleXMLPath),"");              set=>Set(nameof(ScaleXMLPath),value);}
    public string ReadOut             {get=>S(nameof(ReadOut),"");                   set=>Set(nameof(ReadOut),value);}
    public string Billing             {get=>S(nameof(Billing),"");                   set=>Set(nameof(Billing),value);}
    public string BillingVZ           {get=>S(nameof(BillingVZ),"");                 set=>Set(nameof(BillingVZ),value);}
    public string TamperXML           {get=>S(nameof(TamperXML),"");                 set=>Set(nameof(TamperXML),value);}
    public string CommunicationPort   {get=>S(nameof(CommunicationPort),"RJ Port");  set=>Set(nameof(CommunicationPort),value);}
    public string CommunicationMode   {get=>S(nameof(CommunicationMode),"Mode-E");   set=>Set(nameof(CommunicationMode),value);}
    public string ModemNumber         {get=>S(nameof(ModemNumber),"");               set=>Set(nameof(ModemNumber),value);}
    public string AppUser             {get=>S(nameof(AppUser),"dlmspt");             set=>Set(nameof(AppUser),value);}
    public string AppPwd              {get=>S(nameof(AppPwd),"Dlmspt@12");           set=>Set(nameof(AppPwd),value);}
    public bool   AppUserRememberMe   {get=>B(nameof(AppUserRememberMe),false);      set=>Set(nameof(AppUserRememberMe),value);}
    public int    BaudRateSelectedIndex{get=>I(nameof(BaudRateSelectedIndex),5);     set=>Set(nameof(BaudRateSelectedIndex),value);}
    public string DefaultReadClassID  {get=>S(nameof(DefaultReadClassID),"08");      set=>Set(nameof(DefaultReadClassID),value);}
    public string DefaultReadOBIS     {get=>S(nameof(DefaultReadOBIS),"0000010000FF");set=>Set(nameof(DefaultReadOBIS),value);}
    public string DefaultReadAtt      {get=>S(nameof(DefaultReadAtt),"2");           set=>Set(nameof(DefaultReadAtt),value);}
    public string DefaultReadDaraType {get=>S(nameof(DefaultReadDaraType),"09");     set=>Set(nameof(DefaultReadDaraType),value);}
    public string DefaultReadLen      {get=>S(nameof(DefaultReadLen),"0C");          set=>Set(nameof(DefaultReadLen),value);}
    public int    DefaultReadAccSelector{get=>I(nameof(DefaultReadAccSelector),0);   set=>Set(nameof(DefaultReadAccSelector),value);}
    public int    DefaultReadCmdType  {get=>I(nameof(DefaultReadCmdType),1);         set=>Set(nameof(DefaultReadCmdType),value);}
    public string DefaultReadSelectiveAccessCommand{get=>S(nameof(DefaultReadSelectiveAccessCommand),"010204020412000809060000010000FF0F02120000090C000E0507FF0C310AFF800000090C000E0508FF0C310AFF8000000100");set=>Set(nameof(DefaultReadSelectiveAccessCommand),value);}
    public string DefaultReadDataValueCommand{get=>S(nameof(DefaultReadDataValueCommand),"07DD0404040B2527FF800000");set=>Set(nameof(DefaultReadDataValueCommand),value);}
    public int    Cnf1{get=>I(nameof(Cnf1),0);set=>Set(nameof(Cnf1),value);}
    public int    Cnf2{get=>I(nameof(Cnf2),0);set=>Set(nameof(Cnf2),value);}
    public int    Cnf3{get=>I(nameof(Cnf3),0);set=>Set(nameof(Cnf3),value);}

    public void Save(){}
    public void   SetSecurityMachanism(byte v){SecurityMechanism=v;}
    public byte   GetSecurityMachanism() => SecurityMechanism;
    public void   SetApplicationContext(byte v){ApplicationContext=v;}
    public byte   GetApplicationContext() => ApplicationContext;
    public void   SetClientSAP(int v){ClientSAP=v;}
    public string GetClientSAP() => ClientSAP.ToString();
    public int    GetMeterMode() => MeterMode;
    public void   SetMeterMode(int v){MeterMode=v;}
    public string GetAppUser() => AppUser;
    public void   SetAppUser(string v){AppUser=v;}
    public string GetAppPwd() => AppPwd;
    public void   SetAppPwd(string v){AppPwd=v;}
    public void   SetApppwd(string v){AppPwd=v;}
    public bool   GetAppUserRememberMe() => AppUserRememberMe;
    public void   SetAppUserRememberMe(bool v){AppUserRememberMe=v;}
    public string GetScaleXMLPath() => ScaleXMLPath;
    public void   SetScaleXMLPath(string v){ScaleXMLPath=v;}
    public string GetLLSPassword() => Password;
    public void   SetLLSPassword(string v){Password=v;}
    public string GetHLSPassword() => HLSPWD;
    public void   SetHLSPWD(string v){HLSPWD=v;}
    public int    GetAssociationMode() => AssociationMode;
    public void   SetAssociationMode(int v){AssociationMode=(byte)v;}
    public int    GetAssociationType() => AssociationType;
    public void   SetAssociationType(int v){AssociationType=(byte)v;}
    public string GetAssociationAccess() => AssociationAccess;
    public void   SetAssociationAccess(string v){AssociationAccess=v;}
    public string GetGlobalEncryptionKey() => GlobalEncryptionKey;
    public string[] GetReadoutCommandStructure() => new[]{DefaultReadClassID,DefaultReadOBIS,DefaultReadAtt,DefaultReadDaraType,DefaultReadLen,DefaultReadAccSelector.ToString(),DefaultReadCmdType.ToString(),DefaultReadSelectiveAccessCommand,DefaultReadDataValueCommand};
    public void SetReadoutCommandStructure(string[] cmd){if(cmd.Length<9)return;DefaultReadClassID=cmd[0];DefaultReadOBIS=cmd[1];DefaultReadAtt=cmd[2];DefaultReadDaraType=cmd[3];DefaultReadLen=cmd[4];DefaultReadAccSelector=Convert.ToInt16(cmd[5]);DefaultReadCmdType=Convert.ToInt16(cmd[6]);DefaultReadSelectiveAccessCommand=cmd[7];DefaultReadDataValueCommand=cmd[8];}
    public void SetCipheredSecurityResponse(string llsPwd,string hlsPwd,string encKey)
    {
        if(hlsPwd.Length>=16){ClientSAP=48;SecurityMechanism=2;HLSPWD=hlsPwd;}
        else{ClientSAP=32;SecurityMechanism=1;Password=llsPwd;}
        ApplicationContext=3;Securitysuit=0x30;GlobalEncryptionKey=encKey;DedicatedKey=1;AuthenticationKey=encKey;
    }
    public AppSettings ToModel() => new(){SerialPort=SerialPort,SignOnBaudRate=SignOnBaudRate,CommandBaudRate=CommandBaudRate,StopBits=StopBits,DataBits=DataBits,Parity=Parity,CommandTimeOut=CommandTimeOut,IntercharacterDelay=IntercharacterDelay,InterframeTimeout=InterframeTimeout,ServerSAP=ServerSAP,ServerLowerMacAddress=ServerLowerMacAddress,ClientSAP=ClientSAP,AddressingSchem=AddressingSchem,ServerPhysicalID=ServerPhysicalID,HDLCAddressing=HDLCAddressing,ApplicationContext=ApplicationContext,SecurityMechanism=SecurityMechanism,PDUSize=PDUSize,ConformanceBlock=ConformanceBlock,DLMSVersion=DLMSVersion,InformationSize=InformationSize,WindowSize=WindowSize,CosemBufferSize=CosemBufferSize,DLLBufferSize=DLLBufferSize,Password=Password,HLSKey=HLSKey,HLSPWD=HLSPWD,ClientSystemTitle=ClientSystemTitle,Securitysuit=Securitysuit,GlobalEncryptionKey=GlobalEncryptionKey,DedicatedKey=DedicatedKey,AuthenticationKey=AuthenticationKey,AESEncryption=AESEncryption,AssociationMode=AssociationMode,AssociationType=AssociationType,AssociationAccess=AssociationAccess,MeterMode=MeterMode,ScaleXMLPath=ScaleXMLPath,ReadOut=ReadOut,Billing=Billing,BillingVZ=BillingVZ,TamperXML=TamperXML,CommunicationPort=CommunicationPort,CommunicationMode=CommunicationMode,ModemNumber=ModemNumber,AppUser=AppUser,AppPwd=AppPwd,AppUserRememberMe=AppUserRememberMe,BaudRateSelectedIndex=BaudRateSelectedIndex,DefaultReadClassID=DefaultReadClassID,DefaultReadOBIS=DefaultReadOBIS,DefaultReadAtt=DefaultReadAtt,DefaultReadDaraType=DefaultReadDaraType,DefaultReadLen=DefaultReadLen,DefaultReadAccSelector=DefaultReadAccSelector,DefaultReadCmdType=DefaultReadCmdType,DefaultReadSelectiveAccessCommand=DefaultReadSelectiveAccessCommand,DefaultReadDataValueCommand=DefaultReadDataValueCommand,Cnf1=Cnf1,Cnf2=Cnf2,Cnf3=Cnf3};
    public void FromModel(AppSettings m){SerialPort=m.SerialPort;SignOnBaudRate=m.SignOnBaudRate;CommandBaudRate=m.CommandBaudRate;StopBits=m.StopBits;DataBits=m.DataBits;Parity=m.Parity;CommandTimeOut=m.CommandTimeOut;IntercharacterDelay=m.IntercharacterDelay;InterframeTimeout=m.InterframeTimeout;ServerSAP=m.ServerSAP;ServerLowerMacAddress=m.ServerLowerMacAddress;ClientSAP=m.ClientSAP;AddressingSchem=m.AddressingSchem;ServerPhysicalID=m.ServerPhysicalID;HDLCAddressing=m.HDLCAddressing;ApplicationContext=m.ApplicationContext;SecurityMechanism=m.SecurityMechanism;PDUSize=m.PDUSize;ConformanceBlock=m.ConformanceBlock;DLMSVersion=m.DLMSVersion;InformationSize=m.InformationSize;WindowSize=m.WindowSize;CosemBufferSize=m.CosemBufferSize;DLLBufferSize=m.DLLBufferSize;Password=m.Password;HLSKey=m.HLSKey;HLSPWD=m.HLSPWD;ClientSystemTitle=m.ClientSystemTitle;Securitysuit=m.Securitysuit;GlobalEncryptionKey=m.GlobalEncryptionKey;DedicatedKey=m.DedicatedKey;AuthenticationKey=m.AuthenticationKey;AESEncryption=m.AESEncryption;AssociationMode=m.AssociationMode;AssociationType=m.AssociationType;AssociationAccess=m.AssociationAccess;MeterMode=m.MeterMode;ScaleXMLPath=m.ScaleXMLPath;ReadOut=m.ReadOut;Billing=m.Billing;BillingVZ=m.BillingVZ;TamperXML=m.TamperXML;CommunicationPort=m.CommunicationPort;CommunicationMode=m.CommunicationMode;ModemNumber=m.ModemNumber;AppUser=m.AppUser;AppPwd=m.AppPwd;AppUserRememberMe=m.AppUserRememberMe;BaudRateSelectedIndex=m.BaudRateSelectedIndex;DefaultReadClassID=m.DefaultReadClassID;DefaultReadOBIS=m.DefaultReadOBIS;DefaultReadAtt=m.DefaultReadAtt;DefaultReadDaraType=m.DefaultReadDaraType;DefaultReadLen=m.DefaultReadLen;DefaultReadAccSelector=m.DefaultReadAccSelector;DefaultReadCmdType=m.DefaultReadCmdType;DefaultReadSelectiveAccessCommand=m.DefaultReadSelectiveAccessCommand;DefaultReadDataValueCommand=m.DefaultReadDataValueCommand;Cnf1=m.Cnf1;Cnf2=m.Cnf2;Cnf3=m.Cnf3;}
}
