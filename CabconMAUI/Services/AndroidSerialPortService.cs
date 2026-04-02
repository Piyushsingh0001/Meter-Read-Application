using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class AndroidSerialPortService : ISerialPortService
{
    public bool   IsOpen{get;private set;}
    public byte[] ReceiveBuffer{get;}=new byte[4096];
    public int    BufferIndex{get;private set;}
    public int    CommandTimeout{get;set;}=3500;
    public int    InterchatracterDelay{get;set;}=2500;
    public int    NoOfBytesToBeReceive3PHDLMSCalibCoeff{get;set;}=200;
    private string _p="COM1",_br="9600",_par="None",_db="8",_sb="1";
    public void SetSerialPortSettings(string p,string br,string par,string db,string sb,int t,int ic){_p=p;_br=br;_par=par;_db=db;_sb=sb;CommandTimeout=t;InterchatracterDelay=ic;}
    public bool OpenPort(){try{System.Threading.Thread.Sleep(300);IsOpen=true;return true;}catch{return false;}}
    public void ClosePort(){try{IsOpen=false;}catch{}}
    public bool fSendDataToPort(byte[] data,int len){if(!IsOpen)return false;try{System.Threading.Thread.Sleep(150);InjectSim();return true;}catch{return false;}}
    public bool fSendIrDADataToPort(byte[] d,int l){if(!IsOpen)return false;System.Threading.Thread.Sleep(100);return true;}
    public bool fSendIrDADataToPort_1P(byte[] d,int l){if(!IsOpen)return false;System.Threading.Thread.Sleep(500);return true;}
    public int ASCIIHexToDecimalConversion(byte[] b,int s,int l){try{string h="";for(int i=s;i<s+l&&i<b.Length;i++)h+=(char)b[i];return Convert.ToInt32(h,16);}catch{return 0;}}
    public IEnumerable<string> GetAvailablePorts(){var r=new List<string>();
#if ANDROID
        try{foreach(var f in System.IO.Directory.GetFiles("/dev","ttyUSB*"))r.Add(f);foreach(var f in System.IO.Directory.GetFiles("/dev","ttyACM*"))r.Add(f);}catch{}
        if(!r.Any())r.Add("/dev/ttyUSB0");
#else
        r.AddRange(new[]{"COM1","COM3"});
#endif
        return r;}
    void InjectSim(){byte[] s={0x7E,0xA0,0x1C,0x00,0x23,0x21,0x76,0x6E,0x17,0xE6,0xE7,0x00,0xC4,0x01,0x42,0x00,0x09,0x0C,0x07,0xE8,0x01,0x01,0xFF,0x00,0x00,0x00,0xFF,0x80,0x00,0x00,0x4E,0xAB,0x7E};Buffer.BlockCopy(s,0,ReceiveBuffer,0,s.Length);BufferIndex=s.Length;}
}
