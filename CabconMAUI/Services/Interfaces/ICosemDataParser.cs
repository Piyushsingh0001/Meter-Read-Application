using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface ICosemDataParser
{
    CosemParseResult ParseData(byte[] data, int offset = 0);
    CosemParseResult ParseObisData(byte[] data, byte[] obisCode, int offset = 0, int scaler = 0);
    CosemParseResult ParseWithScaler(byte[] data, int offset = 0, int scaler = 0);
    CosemParseResult ParseArray(byte[] data, int offset = 0);
    CosemParseResult ParseStructure(byte[] data, int offset = 0);
    CosemParseResult ParseDateTime(byte[] data, int offset = 0);
    CosemParseResult ParseOctetString(byte[] data, int offset = 0);
    CosemParseResult ParseVisibleString(byte[] data, int offset = 0);
    CosemParseResult ParseInteger(byte[] data, int offset = 0, int bytes = 4);
    CosemParseResult ParseUnsigned(byte[] data, int offset = 0, int bytes = 4);
    CosemParseResult ParseLongUnsigned(byte[] data, int offset = 0, int bytes = 8);
    CosemParseResult ParseFloat(byte[] data, int offset = 0);
    CosemParseResult ParseDouble(byte[] data, int offset = 0);
    CosemParseResult ParseBitString(byte[] data, int offset = 0);
    CosemParseResult ParseBoolean(byte[] data, int offset = 0);
    CosemDataType DetectDataType(byte[] data, int offset = 0);
    
    // Enhanced parsing methods for critical COSEM data types
    CosemParseResult ParseOctetStringToAscii(byte[] data, int offset = 0);
    CosemParseResult ParseDoubleLongUnsigned(byte[] data, int offset = 0);
    CosemParseResult ParseInteger8Bit(byte[] data, int offset = 0);
    CosemParseResult ParseDataWithTag(byte[] data, int offset = 0, int scaler = 0);
}

public class CosemParseResult
{
    public bool IsSuccess { get; set; }
    public string ParsedValue { get; set; } = string.Empty;
    public object? RawValue { get; set; }
    public CosemDataType DataType { get; set; }
    public int BytesConsumed { get; set; }
    public string? Unit { get; set; }
    public string? Description { get; set; }
    public List<string> Warnings { get; set; } = new();
}

public enum CosemDataType
{
    Unknown = 0x00,
    Array = 0x01,
    Structure = 0x02,
    Boolean = 0x03,
    BitString = 0x04,
    DoubleLong = 0x05,
    DoubleLongUnsigned = 0x06,
    OctetString = 0x09,
    VisibleString = 0x0A,
    UTF8String = 0x0C,
    BCD = 0x0D,
    Integer = 0x10,
    Long = 0x11,
    Unsigned = 0x12,
    LongUnsigned = 0x13,
    Enum = 0x15,
    Float32 = 0x16,
    Float64 = 0x17,
    Date = 0x18,
    Time = 0x19,
    DateTime = 0x1C
}
