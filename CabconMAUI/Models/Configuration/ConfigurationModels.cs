namespace CabconMAUI.Models.Configuration;

/// <summary>Serial port configuration settings.</summary>
public class SerialPortSettings
{
    public string SerialPort { get; set; } = "COM1";
    public string SignOnBaudRate { get; set; } = "300";
    public string CommandBaudRate { get; set; } = "9600";
    public string StopBits { get; set; } = "1";
    public string DataBits { get; set; } = "8";
    public string Parity { get; set; } = "None";
    public int CommandTimeOut { get; set; } = 3500;
    public int IntercharacterDelay { get; set; } = 2500;
    public int InterframeTimeout { get; set; } = 1500;
}

/// <summary>HDLC (High-level Data Link Control) configuration settings.</summary>
public class HDLCSettings
{
    public int ServerSAP { get; set; } = 1;
    public int ServerLowerMacAddress { get; set; } = 17;
    public int ClientSAP { get; set; } = 64;
    public int AddressingScheme { get; set; } = 2;
    public int ServerPhysicalID { get; set; } = 1;
    public int HDLCAddressing { get; set; } = 2;
}

/// <summary>COSEM (Companion Specification for Energy Metering) configuration settings.</summary>
public class COSEMSettings
{
    public int ApplicationContext { get; set; } = 1;
    public int SecurityMechanism { get; set; } = 1;
    public int PDUSize { get; set; } = 9999;
    public string ConformanceBlock { get; set; } = "1CFF3F";
    public int DLMSVersion { get; set; } = 6;
    public int InformationSize { get; set; } = 128;
    public int WindowSize { get; set; } = 1;
    public int CosemBufferSize { get; set; } = 9999;
    public int DLLBufferSize { get; set; } = 9999;
}

/// <summary>Security configuration settings.</summary>
public class SecuritySettings
{
    public string Password { get; set; } = "00000000";
    public string HLSKey { get; set; } = "93BC0FABF6C85E9E1C53D78885373DC7";
    public string HLSPWD { get; set; } = "000102030405060708090A0B0C0D0E0F";
    public string ClientSystemTitle { get; set; } = "12345678";
    public int SecuritySuite { get; set; } = 0x20;
    public string GlobalEncryptionKey { get; set; } = "000102030405060708090A0B0C0D0E0F";
    public int DedicatedKey { get; set; } = 1;
    public string AuthenticationKey { get; set; } = "000102030405060708090A0B0C0D0E0F";
    public string AESEncryption { get; set; } = "Non Cyphering";
}

/// <summary>Association configuration settings.</summary>
public class AssociationSettings
{
    public int AssociationMode { get; set; } = 1;
    public int AssociationType { get; set; } = 2;
    public string AssociationAccess { get; set; } = "12345678";
}

/// <summary>Meter-specific configuration settings.</summary>
public class MeterSettings
{
    public int MeterMode { get; set; } = 2;
    public string ScaleXMLPath { get; set; } = string.Empty;
    public string ReadOut { get; set; } = string.Empty;
}

/// <summary>General application configuration settings.</summary>
public class ApplicationSettings
{
    public string Environment { get; set; } = "Development";
    public string LogLevel { get; set; } = "Information";
    public bool EnableDebugMode { get; set; } = true;
}
