using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface IMeterCommunicationFacade
{
    Task<bool> ConnectToMeterAsync(MeterConnectRequest request);
    Task DisconnectAsync();
    Task<MeterReadResult> ReadAsync(MeterReadRequest request);
    Task<MeterReadResult> ReadSingleObjectAsync(byte[] obis, byte classId, byte attributeId);
    Task<MeterReadResult> ReadBlockAsync(byte[] obis, byte classId, byte attributeId, byte accessSelector, List<byte> descriptor);
}

