namespace CabconMAUI.Services.Interfaces;
public interface ISerialPortService
{
    bool IsOpen{get;} byte[] ReceiveBuffer{get;} int BufferIndex{get;}
    string EndpointDetails { get; }
    int CommandTimeout{get;set;} int InterchatracterDelay{get;set;}
    int NoOfBytesToBeReceive3PHDLMSCalibCoeff{get;set;}
    void SetSerialPortSettings(string port,string baudRate,string parity,string dataBits,string stopBits,int timeout,int intercharDelay);
    bool OpenPort(); void ClosePort();
    bool fSendDataToPort(byte[] data,int length);
    bool fSendIrDADataToPort(byte[] data,int length);
    bool fSendIrDADataToPort_1P(byte[] data,int length);
    void SetReceiveBuffer(byte[] data,int length);
    int ASCIIHexToDecimalConversion(byte[] buf,int start,int len);
    IEnumerable<string> GetAvailablePorts();
    bool ChangeBaudRate(int newBaudRate);
    Task<bool> IecBaudRateNegotiationAsync();
    Task<bool> ConnectAsync(string portName, int baudRate);
    Task<bool> SendAsync(byte[] data);
    Task<byte[]> ReceiveAsync(int maxBytes, TimeSpan timeout);
}
