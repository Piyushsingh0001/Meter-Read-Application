using System.Text.RegularExpressions;
using System.Xml.Linq;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;

public class AuthService : IAuthService
{
    readonly ISettingsService _s;
    public bool IsAuthenticated{get;private set;}
    public string CurrentUser{get;private set;}=string.Empty;
    public AuthService(ISettingsService s){_s=s;}
    public async Task<bool> LoginAsync(string uid,string pwd){
        await Task.Delay(300);
        if(string.IsNullOrWhiteSpace(uid)||string.IsNullOrWhiteSpace(pwd))return false;
        bool ok=uid.Trim().Equals(_s.GetAppUser(),StringComparison.OrdinalIgnoreCase)&&pwd.Equals(_s.GetAppPwd());
        if(ok){IsAuthenticated=true;CurrentUser=uid;if(_s.GetAppUserRememberMe()){_s.SetAppUser(uid);_s.SetApppwd(pwd);}}
        return ok;}
    public void Logout(){IsAuthenticated=false;CurrentUser=string.Empty;}
}

public class BluetoothService : IBluetoothService
{
    public bool IsConnected{get;private set;}
    public async Task<bool> ConnectAsync(string a){await Task.Delay(600);IsConnected=true;return true;}
    public async Task DisconnectAsync(){await Task.Delay(100);IsConnected=false;}
    public async Task<byte[]> SendReceiveAsync(byte[] r,int ms=5000){await Task.Delay(100);return Array.Empty<byte>();}
    public async Task<IEnumerable<string>> ScanDevicesAsync(){await Task.Delay(2000);return new[]{"Cabcon_Meter_BT (AA:BB:CC:DD:EE:FF)"};}
}

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string r)=>await Shell.Current.GoToAsync(r);
    public async Task GoBackAsync()=>await Shell.Current.GoToAsync("..");
}

public class IecMeterService : IIecMeterService
{
    readonly ISettingsService _set; readonly ISerialPortService _ser;
    public string MeterSignonResponse{get;private set;}=string.Empty;
    public string MeterReadoutResponse{get;private set;}=string.Empty;
    public event EventHandler<StatusEventArgs>? StatusUpdated;
    byte[] _cmdBytes=Array.Empty<byte>(); byte _stop=0x03,_stop2=0x03;
    string _outBuff=string.Empty,_repoXml=string.Empty;
    public IecMeterService(ISettingsService s,ISerialPortService ser){_set=s;_ser=ser;}
    void Raise(string m,bool e){System.Diagnostics.Debug.WriteLine($"[IEC] {m}");MainThread.BeginInvokeOnMainThread(()=>StatusUpdated?.Invoke(this,new StatusEventArgs(m,e)));}
    public async Task<bool> ConnectToIECMeterAsync(int mode){Raise("Connecting...",false);if(!await IECPhysicalLayerConnectAsync(false)){Raise("Failed!",true);return false;}SetCommandProperties("MeterSignon");if(!await ReadInternalAsync()){MeterSignonResponse=_outBuff;Raise("Signon Failed!",true);return false;}MeterSignonResponse=_outBuff;SetCommandProperties(mode switch{(int)IECSignOnMode.IEC_PRGRAMING=>"ProgrammingAssociation",(int)IECSignOnMode.IEC_MANUFACURER=>"ManufacurerReadAssociation",_=>"IECReadoutAssociation"});if(!await ReadInternalAsync()){Raise("Association Failed!",true);return false;}MeterReadoutResponse=_outBuff;if(mode==(int)IECSignOnMode.IEC_READ)return true;SetCommandProperties("AccessAssociation");if(!await ReadInternalAsync()){Raise("Access Failed!",true);return false;}return true;}
    public async Task<bool> IECPhysicalLayerConnectAsync(bool iec){return await Task.Run(()=>{_ser.SetSerialPortSettings(_set.SerialPort,_set.CommandBaudRate,_set.Parity,_set.DataBits,_set.StopBits,_set.CommandTimeOut,_set.IntercharacterDelay);return _ser.OpenPort();});}
    public async Task<string> ReadDataBufferAsync(string lbl){SetCommandProperties(lbl);await ReadInternalAsync();return _outBuff;}
    public async Task<string> ReadProfileBufferAsync(string lbl,int ev){string buf="";SetCommandProperties(lbl);do{if(!await ReadInternalAsync())break;buf+=ExtractDataFromResponse(_outBuff);SetCommandProperties("ACKCommand");}while(_ser.ReceiveBuffer.Take(_ser.BufferIndex).Contains((byte)0x04));return buf;}
    public async Task<string> WriteProfileBufferAsync(string[] ls,List<string> data){int c=0;while(ls.Length>c){byte[] d=data[c].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();SetCommandProperties(ls[c]);var dl=_cmdBytes.ToList();int si=dl.IndexOf(0x28);if(si>=0)dl.InsertRange(si+1,d);_cmdBytes=dl.ToArray();if(!await ReadInternalAsync())return"Error: "+_outBuff;c++;}return string.Empty;}
    public async Task<string> WriteIECBufferAsync(List<string> data){byte[] d=data[0].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();_cmdBytes=d;_stop=0x06;_stop2=0x03;if(!await ReadInternalAsync())return"Error: "+_outBuff;return _outBuff;}
    public async Task<string> WriteBootLoaderProfileBufferInitiateAsync(string[] ls,List<string> data){string r="";int c=0;while(ls.Length>c){byte[] d=data[c].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();SetCommandProperties(ls[0]);_cmdBytes=d;if(!await WriteBootloaderInternalAsync())return"Error";if(c==0){for(int i=0;i<=4&&i<_ser.BufferIndex;i++)r+=(char)_ser.ReceiveBuffer[i];return r;}r=_ser.ReceiveBuffer[0].ToString();c++;}return r;}
    public async Task<string> WriteBootLoaderProfileBufferAsync(string[] ls,List<string> data){string r="";int c=1;while(ls.Length>c){byte[] d=data[c].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();SetCommandProperties(ls[0]);_cmdBytes=d;if(!await WriteBootloaderInternalAsync())return"Error";r=_ser.ReceiveBuffer[0].ToString();c++;}return r;}
    public async Task<string> WriteNonProtocolPacketAsync(List<byte> data,byte stop,byte? stop2){_stop=stop;if(stop2!=null)_stop2=(byte)stop2;_cmdBytes=data.ToArray();await WriteBootloaderInternalAsync();await Task.Delay(10);return _outBuff;}
    public async Task<bool> IECAssociationStablishAsync()=>await Task.FromResult(true);
    public async Task IECAssociationDisconnectAsync(){SetCommandProperties("DisconnectAssociation");await ReadInternalAsync();_ser.ClosePort();}
    public async Task IECPortDisconnectAsync(){await Task.Run(()=>_ser.ClosePort());}
    public string ExtractDataFromResponse(string r){var m=Regex.Matches(r,@"(\([\w\W]*?\))",RegexOptions.Multiline|RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);return m.Count>0?m[0].Value:string.Empty;}
    public void SetCommandProperties(string tag){try{if(string.IsNullOrEmpty(_repoXml))LoadRepoXml();var x=XElement.Parse(_repoXml);var rs=x.Elements("COMMAND").Where(e=>e.Element("TAGNO")?.Value==tag).ToList();if(!rs.Any())return;var cmd=rs[0];_cmdBytes=cmd.Element("CommandDataBytes")!.Value.Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();_stop=(byte)Convert.ToInt32(cmd.Element("ResponseStopByte")!.Value,16);string sb2=cmd.Element("ResponseStopByte_2")?.Value??"";_stop2=string.IsNullOrEmpty(sb2)?_stop:(byte)Convert.ToInt32(sb2,16);}catch(Exception ex){System.Diagnostics.Debug.WriteLine($"[IEC] SetCmdProps: {ex.Message}");}}
    void LoadRepoXml(){try{string p=System.IO.Path.Combine(FileSystem.AppDataDirectory,"Configuration","1PCommandRepository.xml");if(System.IO.File.Exists(p))_repoXml=System.IO.File.ReadAllText(p);}catch{}}
    async Task<bool> ReadInternalAsync(){return await Task.Run(()=>{try{if(_cmdBytes.Contains((byte)3)||_cmdBytes.Contains((byte)4)){byte bcc=0;for(int i=1;i<_cmdBytes.Length-1;i++)bcc^=_cmdBytes[i];_cmdBytes[_cmdBytes.Length-1]=bcc;}int retry=3;while(retry-->0){bool ok=_ser.fSendDataToPort(_cmdBytes,_cmdBytes.Length);_outBuff=BuildBuf();if(!ok){if(_outBuff.ToUpperInvariant().Contains("READY"))return true;continue;}if(_outBuff.ToUpperInvariant().Contains("(ER")){System.Threading.Thread.Sleep(200);continue;}return true;}return false;}catch{return false;}});}
    async Task<bool> WriteBootloaderInternalAsync(){return await Task.Run(()=>{try{bool ok=_ser.fSendDataToPort(_cmdBytes,_cmdBytes.Length);_outBuff=BuildBuf();return ok;}catch{return false;}});}
    string BuildBuf(){string r="";for(int i=0;i<_ser.BufferIndex;i++)r+=(char)_ser.ReceiveBuffer[i];return r;}
}
