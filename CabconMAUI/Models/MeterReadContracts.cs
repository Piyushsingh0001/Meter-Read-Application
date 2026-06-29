namespace CabconMAUI.Models;

public enum MeterProtocolFamily
{
    Dlms1Phase,
    Dlms3Phase,
    IecNonDlms
}

public enum MeterTransportMode
{
    Serial,
    Bluetooth,
    IrDa
}

public enum MeterReadFeature
{
    Instantaneous,
    Billing,
    LoadSurvey,
    DailyProfile,
    Tamper,
    Nameplate,
    ReadAll
}

public enum ExportFormat
{
    Csv,
    Xml
}

public sealed class MeterConnectRequest
{
    public MeterProtocolFamily ProtocolFamily { get; set; }
    public MeterTransportMode TransportMode { get; set; }
    public string? PortName { get; set; }
}

public sealed class MeterReadRequest
{
    public MeterReadFeature Feature { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public long? FromEntry { get; set; }
    public long? ToEntry { get; set; }
}

public sealed class MeterReadResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public Dictionary<string, string> Values { get; set; } = new();
    public byte[] RawBuffer { get; set; } = Array.Empty<byte>();
}

