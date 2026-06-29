using System.Text;
using CabconMAUI.Helpers;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class CosemDataParser : ICosemDataParser
{
    private readonly Dictionary<CosemDataType, string> _dataTypeDescriptions = new()
    {
        { CosemDataType.Array, "Array" },
        { CosemDataType.Structure, "Structure" },
        { CosemDataType.Boolean, "Boolean" },
        { CosemDataType.BitString, "Bit String" },
        { CosemDataType.DoubleLong, "Double Long (64-bit signed)" },
        { CosemDataType.DoubleLongUnsigned, "Double Long Unsigned (64-bit)" },
        { CosemDataType.OctetString, "Octet String" },
        { CosemDataType.VisibleString, "Visible String" },
        { CosemDataType.UTF8String, "UTF-8 String" },
        { CosemDataType.BCD, "BCD" },
        { CosemDataType.Integer, "Integer (32-bit signed)" },
        { CosemDataType.Long, "Long (64-bit signed)" },
        { CosemDataType.Unsigned, "Unsigned (32-bit)" },
        { CosemDataType.LongUnsigned, "Long Unsigned (64-bit)" },
        { CosemDataType.Enum, "Enum" },
        { CosemDataType.Float32, "Float (32-bit)" },
        { CosemDataType.Float64, "Double (64-bit)" },
        { CosemDataType.Date, "Date" },
        { CosemDataType.Time, "Time" },
        { CosemDataType.DateTime, "DateTime" }
    };

    public CosemParseResult ParseData(byte[] data, int offset = 0)
    {
        if (data == null || offset >= data.Length)
        {
            return new CosemParseResult
            {
                IsSuccess = false,
                ParsedValue = "Invalid data or offset",
                BytesConsumed = 0
            };
        }

        var dataType = DetectDataType(data, offset);
        
        return dataType switch
        {
            CosemDataType.Array => ParseArray(data, offset),
            CosemDataType.Structure => ParseStructure(data, offset),
            CosemDataType.Boolean => ParseBoolean(data, offset),
            CosemDataType.BitString => ParseBitString(data, offset),
            CosemDataType.DoubleLong => ParseInteger(data, offset, 8),
            CosemDataType.DoubleLongUnsigned => ParseLongUnsigned(data, offset, 8),
            CosemDataType.OctetString => ParseOctetString(data, offset),
            CosemDataType.VisibleString => ParseVisibleString(data, offset),
            CosemDataType.UTF8String => ParseVisibleString(data, offset), // Handle UTF-8 as visible string for now
            CosemDataType.BCD => ParseBCD(data, offset),
            CosemDataType.Integer => ParseInteger(data, offset, 4),
            CosemDataType.Long => ParseInteger(data, offset, 8),
            CosemDataType.Unsigned => ParseUnsigned(data, offset, 4),
            CosemDataType.LongUnsigned => ParseLongUnsigned(data, offset, 8),
            CosemDataType.Enum => ParseUnsigned(data, offset, 2),
            CosemDataType.Float32 => ParseFloat(data, offset),
            CosemDataType.Float64 => ParseDouble(data, offset),
            CosemDataType.Date => ParseDateTime(data, offset),
            CosemDataType.Time => ParseTime(data, offset),
            CosemDataType.DateTime => ParseDateTime(data, offset),
            _ => new CosemParseResult
            {
                IsSuccess = false,
                ParsedValue = $"Unknown data type: 0x{dataType:X2}",
                DataType = CosemDataType.Unknown,
                BytesConsumed = 1
            }
        };
    }

    public CosemParseResult ParseObisData(byte[] data, byte[] obisCode, int offset = 0, int scaler = 0)
    {
        var result = ParseData(data, offset);
        if (!result.IsSuccess) return result;

        // Apply scaler if provided
        if (scaler != 0 && result.RawValue is double rawValue)
        {
            var scaledValue = rawValue * Math.Pow(10, scaler);
            result.RawValue = scaledValue;
            result.ParsedValue = scaledValue.ToString();
        }

        // Apply OBIS-specific formatting and units
        var formattedResult = FormatObisValue(result, obisCode);
        return formattedResult;
    }

    public CosemParseResult ParseWithScaler(byte[] data, int offset = 0, int scaler = 0)
    {
        var result = ParseData(data, offset);
        if (!result.IsSuccess) return result;

        // Apply scaler if provided
        if (scaler != 0 && result.RawValue is double rawValue)
        {
            var scaledValue = rawValue * Math.Pow(10, scaler);
            result.RawValue = scaledValue;
            result.ParsedValue = scaledValue.ToString();
        }

        return result;
    }

    private CosemParseResult FormatObisValue(CosemParseResult result, byte[] obisCode)
    {
        // Convert OBIS code to string representation for comparison
        var obisStr = $"{obisCode[0]}-{obisCode[1]}:{obisCode[2]}.{obisCode[3]}.{obisCode[4]}.{obisCode[5]}";
        
        // Apply OBIS-specific formatting based on common meter parameters
        switch (obisStr)
        {
            case "1-0:1.7.0.255": // Active Power Total (W)
                return FormatPowerValue(result, "W");
            case "1-0:2.7.0.255": // Reactive Power Total (VAR)
                return FormatPowerValue(result, "VAR");
            case "1-0:21.7.0.255": // Active Power L1 (W)
            case "1-0:22.7.0.255": // Active Power L2 (W)
            case "1-0:23.7.0.255": // Active Power L3 (W)
                return FormatPowerValue(result, "W");
            case "1-0:32.7.0.255": // Voltage L1-N (V)
            case "1-0:52.7.0.255": // Voltage L2-N (V)
            case "1-0:72.7.0.255": // Voltage L3-N (V)
                return FormatVoltageValue(result);
            case "1-0:31.7.0.255": // Current L1 (A)
            case "1-0:51.7.0.255": // Current L2 (A)
            case "1-0:71.7.0.255": // Current L3 (A)
                return FormatCurrentValue(result);
            case "1-0:1.8.0.255": // Active Energy Import Total (kWh)
            case "1-0:2.8.0.255": // Active Energy Export Total (kWh)
                return FormatEnergyValue(result, "kWh");
            case "1-0:3.8.0.255": // Reactive Energy Import Total (kVARh)
            case "1-0:4.8.0.255": // Reactive Energy Export Total (kVARh)
                return FormatEnergyValue(result, "kVARh");
            case "1-0:14.7.0.255": // Power Factor
                return FormatPowerFactorValue(result);
            case "1-0:12.7.0.255": // Frequency (Hz)
                return FormatFrequencyValue(result);
            case "0-0:43.0.0.255": // Clock/DateTime
                return result; // Already formatted as DateTime
            default:
                return result; // No special formatting
        }
    }

    private CosemParseResult FormatPowerValue(CosemParseResult result, string unit)
    {
        if (result.RawValue is not double value) return result;
        
        // Convert to appropriate scale (W, kW, MW)
        string formattedUnit;
        double scaledValue;
        
        if (value >= 1000000) // MW
        {
            scaledValue = value / 1000000;
            formattedUnit = "MW";
        }
        else if (value >= 1000) // kW
        {
            scaledValue = value / 1000;
            formattedUnit = "kW";
        }
        else // W
        {
            scaledValue = value;
            formattedUnit = unit;
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            ParsedValue = $"{scaledValue:F2} {formattedUnit}",
            RawValue = result.RawValue,
            DataType = result.DataType,
            BytesConsumed = result.BytesConsumed,
            Unit = formattedUnit,
            Description = $"{unit}: {scaledValue:F2} {formattedUnit}"
        };
    }

    private CosemParseResult FormatVoltageValue(CosemParseResult result)
    {
        if (result.RawValue is not double value) return result;
        
        return new CosemParseResult
        {
            IsSuccess = true,
            ParsedValue = $"{value:F1} V",
            RawValue = result.RawValue,
            DataType = result.DataType,
            BytesConsumed = result.BytesConsumed,
            Unit = "V",
            Description = $"Voltage: {value:F1} V"
        };
    }

    private CosemParseResult FormatCurrentValue(CosemParseResult result)
    {
        if (result.RawValue is not double value) return result;
        
        return new CosemParseResult
        {
            IsSuccess = true,
            ParsedValue = $"{value:F3} A",
            RawValue = result.RawValue,
            DataType = result.DataType,
            BytesConsumed = result.BytesConsumed,
            Unit = "A",
            Description = $"Current: {value:F3} A"
        };
    }

    private CosemParseResult FormatEnergyValue(CosemParseResult result, string unit)
    {
        if (result.RawValue is not double value) return result;
        
        // Convert from Wh to kWh if needed
        double kWhValue = value / 1000;
        
        return new CosemParseResult
        {
            IsSuccess = true,
            ParsedValue = $"{kWhValue:F3} {unit}",
            RawValue = result.RawValue,
            DataType = result.DataType,
            BytesConsumed = result.BytesConsumed,
            Unit = unit,
            Description = $"Energy: {kWhValue:F3} {unit}"
        };
    }

    private CosemParseResult FormatPowerFactorValue(CosemParseResult result)
    {
        if (result.RawValue is not double value) return result;
        
        // Power factor is typically between -1 and 1
        return new CosemParseResult
        {
            IsSuccess = true,
            ParsedValue = $"{value:F3}",
            RawValue = result.RawValue,
            DataType = result.DataType,
            BytesConsumed = result.BytesConsumed,
            Unit = "PF",
            Description = $"Power Factor: {value:F3}"
        };
    }

    private CosemParseResult FormatFrequencyValue(CosemParseResult result)
    {
        if (result.RawValue is not double value) return result;
        
        return new CosemParseResult
        {
            IsSuccess = true,
            ParsedValue = $"{value:F2} Hz",
            RawValue = result.RawValue,
            DataType = result.DataType,
            BytesConsumed = result.BytesConsumed,
            Unit = "Hz",
            Description = $"Frequency: {value:F2} Hz"
        };
    }

    public CosemParseResult ParseArray(byte[] data, int offset = 0)
    {
        if (offset + 1 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for array", offset);
        }

        var length = data[offset + 1];
        var result = new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.Array,
            BytesConsumed = 2 + length,
            ParsedValue = $"Array[{length}]",
            RawValue = length,
            Description = $"Array with {length} elements"
        };

        // Parse array elements if needed
        var elements = new List<string>();
        var currentOffset = offset + 2;
        
        for (int i = 0; i < length && currentOffset < data.Length; i++)
        {
            var elementResult = ParseData(data, currentOffset);
            elements.Add($"[{i}]: {elementResult.ParsedValue}");
            currentOffset += elementResult.BytesConsumed;
        }

        if (elements.Any())
        {
            result.ParsedValue = $"Array[{length}]: {string.Join(", ", elements)}";
        }

        return result;
    }

    public CosemParseResult ParseStructure(byte[] data, int offset = 0)
    {
        if (offset + 1 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for structure", offset);
        }

        var elementCount = data[offset + 1];
        var result = new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.Structure,
            BytesConsumed = 2,
            ParsedValue = $"Structure[{elementCount}]",
            Description = $"Structure with {elementCount} elements"
        };

        // Parse structure elements if needed
        var elements = new List<string>();
        var currentOffset = offset + 2;
        
        for (int i = 0; i < elementCount && currentOffset < data.Length; i++)
        {
            var elementResult = ParseData(data, currentOffset);
            elements.Add($"field{i}: {elementResult.ParsedValue}");
            currentOffset += elementResult.BytesConsumed;
        }

        result.BytesConsumed = currentOffset - offset;
        if (elements.Any())
        {
            result.ParsedValue = $"Structure[{elementCount}]: {string.Join(", ", elements)}";
        }

        return result;
    }

    public CosemParseResult ParseDateTime(byte[] data, int offset = 0)
    {
        if (offset + 12 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for DateTime", offset);
        }

        try
        {
            // DLMS DateTime format: 12 bytes
            var year = (data[offset + 2] << 8) | data[offset + 3];
            var month = data[offset + 4];
            var day = data[offset + 5];
            var dayOfWeek = data[offset + 6];
            var hour = data[offset + 7];
            var minute = data[offset + 8];
            var second = data[offset + 9];
            var hundredths = data[offset + 10];
            var deviation = (sbyte)data[offset + 11];

            var dateTime = new DateTime(year, month, day, hour, minute, second, hundredths * 10);
            
            // Apply deviation (in minutes)
            if (deviation != 0)
            {
                dateTime = dateTime.AddMinutes(deviation);
            }

            return new CosemParseResult
            {
                IsSuccess = true,
                DataType = CosemDataType.DateTime,
                BytesConsumed = 12,
                ParsedValue = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                RawValue = dateTime,
                Unit = "DateTime",
                Description = $"Date: {dateTime:yyyy-MM-dd}, Time: {dateTime:HH:mm:ss.fff}, Deviation: {deviation} min"
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResult($"DateTime parse error: {ex.Message}", offset);
        }
    }

    public CosemParseResult ParseTime(byte[] data, int offset = 0)
    {
        if (offset + 4 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for Time", offset);
        }

        try
        {
            var hour = data[offset + 2];
            var minute = data[offset + 3];
            var second = data[offset + 4];
            var hundredths = data[offset + 5];

            var time = new TimeSpan(0, hour, minute, second, hundredths * 10);

            return new CosemParseResult
            {
                IsSuccess = true,
                DataType = CosemDataType.Time,
                BytesConsumed = 5,
                ParsedValue = time.ToString(@"hh\:mm\:ss\.fff"),
                RawValue = time,
                Unit = "Time",
                Description = $"Time: {time:hh\\:mm\\:ss.fff}"
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResult($"Time parse error: {ex.Message}", offset);
        }
    }

    public CosemParseResult ParseOctetString(byte[] data, int offset = 0)
    {
        if (offset + 1 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for OctetString", offset);
        }

        var length = data[offset + 1];
        if (offset + 2 + length > data.Length)
        {
            return CreateErrorResult("OctetString extends beyond data", offset);
        }

        var octetData = new byte[length];
        Array.Copy(data, offset + 2, octetData, 0, length);

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.OctetString,
            BytesConsumed = 2 + length,
            ParsedValue = DlmsHelper.ByteArrayToHexString(octetData, length),
            RawValue = octetData,
            Unit = "Hex",
            Description = $"Octet String ({length} bytes)"
        };
    }

    public CosemParseResult ParseVisibleString(byte[] data, int offset = 0)
    {
        if (offset + 1 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for VisibleString", offset);
        }

        var length = data[offset + 1];
        if (offset + 2 + length > data.Length)
        {
            return CreateErrorResult("VisibleString extends beyond data", offset);
        }

        var stringData = Encoding.ASCII.GetString(data, offset + 2, length).TrimEnd('\0');

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.VisibleString,
            BytesConsumed = 2 + length,
            ParsedValue = stringData,
            RawValue = stringData,
            Unit = "String",
            Description = $"Visible String ({length} chars): {stringData}"
        };
    }

    public CosemParseResult ParseInteger(byte[] data, int offset = 0, int bytes = 4)
    {
        if (offset + bytes + 1 >= data.Length)
        {
            return CreateErrorResult($"Insufficient data for Integer ({bytes} bytes)", offset);
        }

        long value = 0;
        for (int i = 0; i < bytes; i++)
        {
            value = (value << 8) | data[offset + 2 + i];
        }

        // Handle sign for signed integers
        if (bytes <= 4 && (data[offset + 2] & 0x80) != 0)
        {
            value -= (1L << (bytes * 8));
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = bytes <= 4 ? CosemDataType.Integer : CosemDataType.Long,
            BytesConsumed = bytes + 2,
            ParsedValue = value.ToString(),
            RawValue = value,
            Unit = "Count",
            Description = $"{(bytes <= 4 ? "Integer" : "Long")}: {value}"
        };
    }

    public CosemParseResult ParseUnsigned(byte[] data, int offset = 0, int bytes = 4)
    {
        if (offset + bytes + 1 >= data.Length)
        {
            return CreateErrorResult($"Insufficient data for Unsigned ({bytes} bytes)", offset);
        }

        ulong value = 0;
        for (int i = 0; i < bytes; i++)
        {
            value = (value << 8) | data[offset + 2 + i];
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = bytes <= 4 ? CosemDataType.Unsigned : CosemDataType.LongUnsigned,
            BytesConsumed = bytes + 2,
            ParsedValue = value.ToString(),
            RawValue = (double)value,
            Unit = "Count",
            Description = $"{(bytes <= 4 ? "Unsigned" : "Long Unsigned")}: {value}"
        };
    }

    public CosemParseResult ParseLongUnsigned(byte[] data, int offset = 0, int bytes = 8)
    {
        return ParseUnsigned(data, offset, bytes);
    }

    public CosemParseResult ParseFloat(byte[] data, int offset = 0)
    {
        if (offset + 5 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for Float32", offset);
        }

        var bytes = new byte[4];
        Array.Copy(data, offset + 2, bytes, 0, 4);
        Array.Reverse(bytes); // Convert from big-endian to little-endian
        
        var value = BitConverter.ToSingle(bytes, 0);

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.Float32,
            BytesConsumed = 6,
            ParsedValue = value.ToString("F6"),
            RawValue = value,
            Unit = "Float",
            Description = $"Float32: {value:F6}"
        };
    }

    public CosemParseResult ParseDouble(byte[] data, int offset = 0)
    {
        if (offset + 9 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for Float64", offset);
        }

        var bytes = new byte[8];
        Array.Copy(data, offset + 2, bytes, 0, 8);
        Array.Reverse(bytes); // Convert from big-endian to little-endian
        
        var value = BitConverter.ToDouble(bytes, 0);

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.Float64,
            BytesConsumed = 10,
            ParsedValue = value.ToString("F6"),
            RawValue = value,
            Unit = "Double",
            Description = $"Float64: {value:F6}"
        };
    }

    public CosemParseResult ParseBitString(byte[] data, int offset = 0)
    {
        if (offset + 1 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for BitString", offset);
        }

        var length = data[offset + 1];
        if (offset + 2 + length > data.Length)
        {
            return CreateErrorResult("BitString extends beyond data", offset);
        }

        var bitString = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            var b = data[offset + 2 + i];
            for (int bit = 7; bit >= 0; bit--)
            {
                bitString.Append((b >> bit) & 1);
            }
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.BitString,
            BytesConsumed = 2 + length,
            ParsedValue = bitString.ToString(),
            RawValue = bitString.ToString(),
            Unit = "Bits",
            Description = $"Bit String ({length * 8} bits)"
        };
    }

    public CosemParseResult ParseBoolean(byte[] data, int offset = 0)
    {
        if (offset + 2 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for Boolean", offset);
        }

        var value = data[offset + 2] != 0;

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.Boolean,
            BytesConsumed = 3,
            ParsedValue = value ? "true" : "false",
            RawValue = value,
            Unit = "Boolean",
            Description = $"Boolean: {value}"
        };
    }

    public CosemParseResult ParseBCD(byte[] data, int offset = 0)
    {
        if (offset + 1 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for BCD", offset);
        }

        var length = data[offset + 1];
        if (offset + 2 + length > data.Length)
        {
            return CreateErrorResult("BCD extends beyond data", offset);
        }

        var bcdString = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            var b = data[offset + 2 + i];
            bcdString.Append($"{(b >> 4):X1}{(b & 0x0F):X1}");
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.BCD,
            BytesConsumed = 2 + length,
            ParsedValue = bcdString.ToString(),
            RawValue = bcdString.ToString(),
            Unit = "BCD",
            Description = $"BCD ({length} bytes): {bcdString}"
        };
    }

    public CosemDataType DetectDataType(byte[] data, int offset = 0)
    {
        if (data == null || offset >= data.Length)
            return CosemDataType.Unknown;

        var typeByte = data[offset];
        return (CosemDataType)typeByte;
    }

    private CosemParseResult CreateErrorResult(string message, int offset)
    {
        return new CosemParseResult
        {
            IsSuccess = false,
            ParsedValue = message,
            BytesConsumed = 0,
            DataType = CosemDataType.Unknown,
            Warnings = { message }
        };
    }

    // Enhanced parsing methods for critical COSEM data types
    public CosemParseResult ParseOctetStringToAscii(byte[] data, int offset = 0)
    {
        var result = ParseOctetString(data, offset);
        if (!result.IsSuccess) return result;

        if (result.RawValue is byte[] octetData)
        {
            try
            {
                var asciiString = Encoding.ASCII.GetString(octetData).TrimEnd('\0');
                result.ParsedValue = asciiString;
                result.RawValue = asciiString;
                result.Unit = "ASCII";
                result.Description = $"Octet String to ASCII: {asciiString}";
            }
            catch
            {
                result.ParsedValue = DlmsHelper.ByteArrayToHexString(octetData, octetData.Length);
            }
        }

        return result;
    }

    public CosemParseResult ParseDoubleLongUnsigned(byte[] data, int offset = 0)
    {
        if (offset + 9 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for Double Long Unsigned", offset);
        }

        // Parse 8-byte unsigned integer (big-endian)
        ulong value = 0;
        for (int i = 0; i < 8; i++)
        {
            value = (value << 8) | data[offset + 2 + i];
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.DoubleLongUnsigned,
            BytesConsumed = 10,
            ParsedValue = value.ToString(),
            RawValue = (double)value,
            Unit = "Count",
            Description = $"Double Long Unsigned: {value}"
        };
    }

    public CosemParseResult ParseLongUnsigned(byte[] data, int offset = 0)
    {
        if (offset + 5 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for Long Unsigned", offset);
        }

        // Parse 4-byte unsigned integer (big-endian)
        uint value = 0;
        for (int i = 0; i < 4; i++)
        {
            value = (value << 8) | data[offset + 2 + i];
        }

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.LongUnsigned,
            BytesConsumed = 6,
            ParsedValue = value.ToString(),
            RawValue = (double)value,
            Unit = "Count",
            Description = $"Long Unsigned: {value}"
        };
    }

    public CosemParseResult ParseInteger8Bit(byte[] data, int offset = 0)
    {
        if (offset + 2 >= data.Length)
        {
            return CreateErrorResult("Insufficient data for 8-bit Integer", offset);
        }

        sbyte value = (sbyte)data[offset + 2];

        return new CosemParseResult
        {
            IsSuccess = true,
            DataType = CosemDataType.Integer,
            BytesConsumed = 3,
            ParsedValue = value.ToString(),
            RawValue = (double)value,
            Unit = "Count",
            Description = $"8-bit Integer: {value}"
        };
    }

    // Method to parse data based on tag and apply scaler
    public CosemParseResult ParseDataWithTag(byte[] data, int offset = 0, int scaler = 0)
    {
        if (data == null || offset >= data.Length)
        {
            return CreateErrorResult("Invalid data or offset", offset);
        }

        var tag = data[offset];
        CosemParseResult result;

        switch (tag)
        {
            case 0x09: // Octet String
                result = ParseOctetStringToAscii(data, offset);
                break;
            case 0x06: // Double Long Unsigned
                result = ParseDoubleLongUnsigned(data, offset);
                break;
            case 0x12: // Long Unsigned
                result = ParseLongUnsigned(data, offset);
                break;
            case 0x0F: // Integer (8-bit)
                result = ParseInteger8Bit(data, offset);
                break;
            default:
                result = ParseData(data, offset);
                break;
        }

        // Apply scaler if provided and value is numeric
        if (scaler != 0 && result.IsSuccess && result.RawValue is double rawValue)
        {
            var scaledValue = rawValue * Math.Pow(10, scaler);
            result.RawValue = scaledValue;
            result.ParsedValue = scaledValue.ToString();
            result.Description += $" (scaled by 10^{scaler})";
        }

        return result;
    }
}
