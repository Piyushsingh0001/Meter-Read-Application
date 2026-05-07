using CabconMAUI.Models;
namespace CabconMAUI.Services.Interfaces;
public interface IDlmsService
{
    bool IsMeterConnected{get;} string MeterInfoValue{get;} string CurrentMeterID{get;}
    string AESstr{get;} int GetWriteResponseCode{get;} List<string>? ListSecurityKeys{get;} string RSAPrivateKey{get;set;}
    event EventHandler<StatusEventArgs> StatusUpdated;
    Task<bool> ConnectToMeterAsync(); Task<bool> AssociationDisconnectAsync(); void PhysicalLayerDisconnect();
    Task<bool> PhysicalLayerConnectAsync(); Task<bool> HDLCLayerConnectAsync(); Task<bool> AssociationStablishAsync();
    Task<bool> ReadAssociationForInvocationCounterAsync(); Task<bool> ValidMeterTypeInfoAsync();
    bool GetHTMeterTypes(); int GetDisplayProgrammingVariant();
    Task<bool> ReadByteFromMeterAsync(byte[] obis,byte cls,byte att);
    Task<bool> ReadBlockFromMeterAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc);
    Task<int>  ReadAllTamperBlockFromMeterAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc);
    Task<bool> WriteDataToMeterAsync(byte att,byte[] obis,byte cls,byte dt,byte dl,List<byte> data,byte[] rt);
    Task<bool> WriteMethodToMeterAsync(byte att,byte[] obis,byte cls,byte dt,byte dl,List<byte> data,byte[] rt,byte acc);
    Task<bool> WriteBlockDataToMeterAsync(byte att,byte[] obis,byte cls,byte dt,int dl,List<byte> data,byte[] rt);
    Task<bool> WriteImageBlockDataToMeterAsync(byte att,byte[] obis,byte cls,byte dt,int dl,List<byte> data,byte[] rt,List<byte> crc,List<byte> footer);
    Task<bool> WriteImageActionNormalToMeterAsync(byte att,byte[] obis,byte cls,byte dt,int dl,List<byte> data,byte[] rt,List<byte> crc,List<byte> footer);
    Task<int>  ReadDataCommandAsync(byte[] obis,byte cls,byte att);
    Task<int>  ReadDataCommandCypheredAsync(byte[] obis,byte cls,byte att);
    Task<int>  ReadDataBlockCommandAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc);
    Task<int>  ReadDataBlockCommandCypheredAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc);
    Task<byte[]?> Read3PHDLMSCalibCoeffAsync(byte[] cmd);
    List<byte> GetByteByEntryValueType(long from,long to);
    List<byte> GetByteByEntryDateType(DateTime from,DateTime to);
    List<string> GetMeterTypeList(); string GetSelectedMeterType();
    Task<bool> ReadBlockFromMeterBytesAsync(byte[] obis,byte cls,byte att,byte acc,byte[] desc);
    Task<bool> WriteDataToMeterBytesAsync(byte att,byte[] obis,byte cls,byte dt,byte dl,byte[] data,byte[] rt);
    Task<bool> WriteBlockDataToMeterBytesAsync(byte att,byte[] obis,byte cls,byte dt,int dl,byte[] data,byte[] rt);
    Task<int>  ReadDataBlockCommandCypheredBytesAsync(byte[] obis,byte cls,byte att,byte acc,byte[] desc);
    string[]? DLMSDataFormatorLabView(byte[] data,int idx,bool ascii);
    int  DedicatedCommand(byte[] buf,int idx,string type,int len);
    bool fCheckHDLCResponse(byte[] buf);
}
