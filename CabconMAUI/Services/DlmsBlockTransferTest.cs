using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class DlmsBlockTransferTest
{
    private readonly IDlmsService _dlmsService;
    private readonly ICosemDataParser _parser;

    public DlmsBlockTransferTest(IDlmsService dlmsService, ICosemDataParser parser)
    {
        _dlmsService = dlmsService;
        _parser = parser;
    }

    public async Task<bool> TestBlockTransferFunctionality()
    {
        try
        {
            // Test 1: Parse Octet String to ASCII (tag 0x09)
            var octetData = new byte[] { 0x09, 0x06, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46 }; // "ABCDEF"
            var octetResult = _parser.ParseOctetStringToAscii(octetData);
            if (!octetResult.IsSuccess || octetResult.ParsedValue != "ABCDEF")
            {
                return false;
            }

            // Test 2: Parse Double Long Unsigned (tag 0x06)
            var dluData = new byte[] { 0x06, 0x08, 0x00, 0x00, 0x08, 0xFC, 0x00, 0x00, 0x00, 0x00 }; // 2300
            var dluResult = _parser.ParseDoubleLongUnsigned(dluData);
            if (!dluResult.IsSuccess || dluResult.ParsedValue != "2300")
            {
                return false;
            }

            // Test 3: Parse with scaler (-1 should convert 2300 to 230.0)
            var scaledResult = _parser.ParseWithScaler(dluData, 0, -1);
            if (!scaledResult.IsSuccess || scaledResult.ParsedValue != "230")
            {
                return false;
            }

            // Test 4: Parse Long Unsigned (tag 0x12)
            var luData = new byte[] { 0x12, 0x04, 0x00, 0x00, 0x08, 0xFC }; // 2300
            var luResult = _parser.ParseLongUnsigned(luData);
            if (!luResult.IsSuccess || luResult.ParsedValue != "2300")
            {
                return false;
            }

            // Test 5: Parse Integer 8-bit (tag 0x0F)
            var int8Data = new byte[] { 0x0F, 0x01, 0xFF }; // -1
            var int8Result = _parser.ParseInteger8Bit(int8Data);
            if (!int8Result.IsSuccess || int8Result.ParsedValue != "-1")
            {
                return false;
            }

            // Test 6: Parse data with tag detection
            var tagResult = _parser.ParseDataWithTag(octetData, 0);
            if (!tagResult.IsSuccess || tagResult.ParsedValue != "ABCDEF")
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TestBlockTransferWithMeter(byte[] obisCode)
    {
        try
        {
            // Connect to meter
            if (!await _dlmsService.ConnectToMeterAsync())
            {
                return false;
            }

            // Read large object using block transfer
            var largeData = await _dlmsService.ReadLargeObjectAsync(obisCode);
            if (largeData == null || largeData.Length == 0)
            {
                return false;
            }

            // Parse the received data
            var parseResult = _parser.ParseDataWithTag(largeData);
            return parseResult.IsSuccess;
        }
        catch
        {
            return false;
        }
        finally
        {
            // Ensure disconnection
            await _dlmsService.AssociationDisconnectAsync();
        }
    }
}
