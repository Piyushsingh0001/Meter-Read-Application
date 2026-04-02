using CabconMAUI.Models;
namespace CabconMAUI.Services.Interfaces;
public interface IIecMeterService
{
    string MeterSignonResponse{get;} string MeterReadoutResponse{get;}
    event EventHandler<StatusEventArgs> StatusUpdated;
    Task<bool>   ConnectToIECMeterAsync(int mode);
    Task<bool>   IECPhysicalLayerConnectAsync(bool isIECSettings);
    Task<string> ReadDataBufferAsync(string label);
    Task<string> ReadProfileBufferAsync(string label,int events);
    Task<string> WriteProfileBufferAsync(string[] labels,List<string> data);
    Task<string> WriteIECBufferAsync(List<string> data);
    Task<string> WriteBootLoaderProfileBufferInitiateAsync(string[] labels,List<string> data);
    Task<string> WriteBootLoaderProfileBufferAsync(string[] labels,List<string> data);
    Task<string> WriteNonProtocolPacketAsync(List<byte> data,byte stop,byte? stop2);
    Task<bool>   IECAssociationStablishAsync();
    Task         IECAssociationDisconnectAsync(); Task IECPortDisconnectAsync();
    void SetCommandProperties(string tag); string ExtractDataFromResponse(string resp);
}
