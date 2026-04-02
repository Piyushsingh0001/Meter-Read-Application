namespace CabconMAUI.Services.Interfaces;
public interface IBluetoothService
{
    bool IsConnected{get;}
    Task<bool> ConnectAsync(string addr); Task DisconnectAsync();
    Task<byte[]> SendReceiveAsync(byte[] req,int ms=5000);
    Task<IEnumerable<string>> ScanDevicesAsync();
}
