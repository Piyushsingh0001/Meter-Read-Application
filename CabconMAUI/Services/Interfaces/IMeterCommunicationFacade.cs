using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface IMeterCommunicationFacade
{
    Task<bool> ConnectToMeterAsync(MeterConnectRequest request);
<<<<<<< HEAD
    Task<bool> ConnectAndAuthenticateAsync(MeterVariant selectedVariant);
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    Task DisconnectAsync();
    Task<MeterReadResult> ReadAsync(MeterReadRequest request);
    Task<MeterReadResult> ReadSingleObjectAsync(byte[] obis, byte classId, byte attributeId);
    Task<MeterReadResult> ReadBlockAsync(byte[] obis, byte classId, byte attributeId, byte accessSelector, List<byte> descriptor);
}

