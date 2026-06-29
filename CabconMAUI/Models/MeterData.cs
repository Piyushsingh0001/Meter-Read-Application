namespace CabconMAUI.Models;
public class MeterData
{
    public string MeterNumber    { get; set; } = string.Empty;
    public string MeterSignature { get; set; } = string.Empty;
    public string MeterType      { get; set; } = string.Empty;
    public string EnergyImportKwh{ get; set; } = string.Empty;
    public string EnergyExportKwh{ get; set; } = string.Empty;
    public string VoltageR       { get; set; } = string.Empty;
    public string VoltageY       { get; set; } = string.Empty;
    public string VoltageB       { get; set; } = string.Empty;
    public string CurrentR       { get; set; } = string.Empty;
    public string CurrentY       { get; set; } = string.Empty;
    public string CurrentB       { get; set; } = string.Empty;
    public string PowerFactor    { get; set; } = string.Empty;
    public string Frequency      { get; set; } = string.Empty;
    public string ActivePower    { get; set; } = string.Empty;
    public string ReactivePower  { get; set; } = string.Empty;
    public string BillingDate    { get; set; } = string.Empty;
    public string BillingKwh     { get; set; } = string.Empty;
    public string Timestamp      { get; set; } = string.Empty;
    public bool   IsValid        { get; set; }
    public string ErrorMessage   { get; set; } = string.Empty;
    public byte[] RawBuffer      { get; set; } = Array.Empty<byte>();
}
