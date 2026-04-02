namespace CabconMAUI.Models;
/// <summary>Plain model for all 45+ settings matching SerialPortSettings.Default.</summary>
public class AppSettings
{
    // Serial
    public string SerialPort            { get; set; } = "COM1";
    public string SignOnBaudRate        { get; set; } = "300";
    public string CommandBaudRate       { get; set; } = "9600";
    public string StopBits              { get; set; } = "1";
    public string DataBits              { get; set; } = "8";
    public string Parity                { get; set; } = "None";
    public int    CommandTimeOut        { get; set; } = 3500;
    public int    IntercharacterDelay   { get; set; } = 2500;
    public int    InterframeTimeout     { get; set; } = 1500;
    // HDLC
    public int    ServerSAP             { get; set; } = 1;
    public int    ServerLowerMacAddress { get; set; } = 256;
    public int    ClientSAP             { get; set; } = 64;
    public int    AddressingSchem       { get; set; } = 2;
    public int    ServerPhysicalID      { get; set; } = 1;
    public int    HDLCAddressing        { get; set; } = 2;
    // COSEM
    public byte   ApplicationContext    { get; set; } = 1;
    public byte   SecurityMechanism     { get; set; } = 1;
    public int    PDUSize               { get; set; } = 9999;
    public string ConformanceBlock      { get; set; } = "1CFF3F";
    public int    DLMSVersion           { get; set; } = 6;
    public int    InformationSize       { get; set; } = 128;
    public int    WindowSize            { get; set; } = 1;
    public int    CosemBufferSize       { get; set; } = 9999;
    public int    DLLBufferSize         { get; set; } = 9999;
    // Security
    public string Password              { get; set; } = "00000000";
    public string HLSKey                { get; set; } = "93BC0FABF6C85E9E1C53D78885373DC7";
    public string HLSPWD                { get; set; } = "000102030405060708090A0B0C0D0E0F";
    public string ClientSystemTitle     { get; set; } = "12345678";
    public int    Securitysuit          { get; set; } = 0x20;
    public string GlobalEncryptionKey   { get; set; } = "000102030405060708090A0B0C0D0E0F";
    public int    DedicatedKey          { get; set; } = 1;
    public string AuthenticationKey     { get; set; } = "000102030405060708090A0B0C0D0E0F";
    public string AESEncryption         { get; set; } = "Non Cyphering";
    // Association
    public byte   AssociationMode       { get; set; } = 1;
    public byte   AssociationType       { get; set; } = 2;
    public string AssociationAccess     { get; set; } = "12345678";
    // Meter mode
    public int    MeterMode             { get; set; } = 2;
    // Paths
    public string ScaleXMLPath          { get; set; } = string.Empty;
    public string ReadOut               { get; set; } = string.Empty;
    public string Billing               { get; set; } = string.Empty;
    public string BillingVZ             { get; set; } = string.Empty;
    public string TamperXML             { get; set; } = string.Empty;
    // Communication
    public string CommunicationPort     { get; set; } = "RJ Port";
    public string CommunicationMode     { get; set; } = "Mode-E";
    public string ModemNumber           { get; set; } = string.Empty;
    // App login
    public string AppUser               { get; set; } = "dlmspt";
    public string AppPwd                { get; set; } = "Dlmspt@12";
    public bool   AppUserRememberMe     { get; set; } = false;
    public int    BaudRateSelectedIndex { get; set; } = 5;
    // Default read cmd
    public string DefaultReadClassID    { get; set; } = "08";
    public string DefaultReadOBIS       { get; set; } = "0000010000FF";
    public string DefaultReadAtt        { get; set; } = "2";
    public string DefaultReadDaraType   { get; set; } = "09";
    public string DefaultReadLen        { get; set; } = "0C";
    public int    DefaultReadAccSelector{ get; set; } = 0;
    public int    DefaultReadCmdType    { get; set; } = 1;
    public string DefaultReadSelectiveAccessCommand { get; set; } = "010204020412000809060000010000FF0F02120000090C000E0507FF0C310AFF800000090C000E0508FF0C310AFF8000000100";
    public string DefaultReadDataValueCommand       { get; set; } = "07DD0404040B2527FF800000";
    // Cnf flags
    public int    Cnf1                  { get; set; } = 0;
    public int    Cnf2                  { get; set; } = 0;
    public int    Cnf3                  { get; set; } = 0;
    // Static lists
    public static string[] BaudRateList  => new[]{"300","600","1200","2400","4800","9600","14400","19200"};
    public static string[] ParityList    => new[]{"None","Even","Odd","Mark","Space"};
    public static string[] StopBitsList  => new[]{"1","1.5","2"};
    public static string[] DataBitsList  => new[]{"7","8"};
    public static string[] CommPortList  => new[]{"RJ Port","USB","Bluetooth"};
    public static string[] CommModeList  => new[]{"Mode-E","Mode-C","Direct"};
}
