<<<<<<< HEAD
using System.Text;
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
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
<<<<<<< HEAD
    char _negotiatedBaudIndex='5';
    public IecMeterService(ISettingsService s,ISerialPortService ser){_set=s;_ser=ser;}
    void Raise(string m,bool e){System.Diagnostics.Debug.WriteLine($"[IEC] {m}");if(StatusUpdated!=null) StatusUpdated?.Invoke(this,new StatusEventArgs(m,e));}
    public async Task<bool> ConnectToIECMeterAsync(int mode)
    {
        Raise("Opening IEC sign-on channel at 300 baud...",false);
        if(!await IECPhysicalLayerConnectAsync(true))
        {
            Raise("Unable to open serial port.",true);
            return false;
        }

        // Step 1: Send Sign-On sequence "/?!\r\n" 
        SetCommandProperties("MeterSignon");
        if(!await ReadInternalAsync())
        {
            Raise("IEC sign-on failed.",true);
            return false;
        }

        MeterSignonResponse=_outBuff;
        _negotiatedBaudIndex = ExtractBaudIndex(MeterSignonResponse);
        Raise($"Meter sign-on received. Baud index {_negotiatedBaudIndex}.",false);

        // Step 2: Close current connection and reopen at negotiated baud rate
        await IECPortDisconnectAsync();
        await Task.Delay(100); // Brief delay for port closure

        Raise($"Switching to negotiated baud rate: {ResolveBaudRate(_negotiatedBaudIndex)}",false);
        if(!await IECPhysicalLayerConnectAsync(false))
        {
            Raise("Failed to reopen port at negotiated baud rate.",true);
            return false;
        }

        // Step 3: Send protocol selection handshake
        SetCommandProperties(mode switch
        {
            (int)IECSignOnMode.IEC_PRGRAMING=>"ProgrammingAssociation",
            (int)IECSignOnMode.IEC_MANUFACURER=>"ManufacurerReadAssociation",
            _=>"IECReadoutAssociation"
        });

        if(!await ReadInternalAsync())
        {
            Raise("IEC association handshake failed.",true);
            return false;
        }

        MeterReadoutResponse=_outBuff;
        if(mode==(int)IECSignOnMode.IEC_READ)
        {
            return true;
        }

        SetCommandProperties("AccessAssociation");
        if(!await ReadInternalAsync())
        {
            Raise("IEC access association failed.",true);
            return false;
        }

        return true;
    }
    public async Task<bool> IECPhysicalLayerConnectAsync(bool iec)
    {
        return await Task.Run(() =>
        {
            var baud = iec ? _set.SignOnBaudRate : ResolveBaudRate(_negotiatedBaudIndex);
            var parity = iec && !string.Equals(_set.CommunicationMode, "Direct", StringComparison.OrdinalIgnoreCase) ? "Even" : _set.Parity;
            var dataBits = iec && !string.Equals(_set.CommunicationMode, "Direct", StringComparison.OrdinalIgnoreCase) ? "7" : _set.DataBits;
            _ser.SetSerialPortSettings(_set.SerialPort, baud, parity, dataBits, _set.StopBits, _set.CommandTimeOut, _set.IntercharacterDelay);
            return _ser.OpenPort();
        });
    }
=======
    public IecMeterService(ISettingsService s,ISerialPortService ser){_set=s;_ser=ser;}
    void Raise(string m,bool e){System.Diagnostics.Debug.WriteLine($"[IEC] {m}");MainThread.BeginInvokeOnMainThread(()=>StatusUpdated?.Invoke(this,new StatusEventArgs(m,e)));}
    public async Task<bool> ConnectToIECMeterAsync(int mode){Raise("Connecting...",false);if(!await IECPhysicalLayerConnectAsync(false)){Raise("Failed!",true);return false;}SetCommandProperties("MeterSignon");if(!await ReadInternalAsync()){MeterSignonResponse=_outBuff;Raise("Signon Failed!",true);return false;}MeterSignonResponse=_outBuff;SetCommandProperties(mode switch{(int)IECSignOnMode.IEC_PRGRAMING=>"ProgrammingAssociation",(int)IECSignOnMode.IEC_MANUFACURER=>"ManufacurerReadAssociation",_=>"IECReadoutAssociation"});if(!await ReadInternalAsync()){Raise("Association Failed!",true);return false;}MeterReadoutResponse=_outBuff;if(mode==(int)IECSignOnMode.IEC_READ)return true;SetCommandProperties("AccessAssociation");if(!await ReadInternalAsync()){Raise("Access Failed!",true);return false;}return true;}
    public async Task<bool> IECPhysicalLayerConnectAsync(bool iec){return await Task.Run(()=>{_ser.SetSerialPortSettings(_set.SerialPort,_set.CommandBaudRate,_set.Parity,_set.DataBits,_set.StopBits,_set.CommandTimeOut,_set.IntercharacterDelay);return _ser.OpenPort();});}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    public async Task<string> ReadDataBufferAsync(string lbl){SetCommandProperties(lbl);await ReadInternalAsync();return _outBuff;}
    public async Task<string> ReadProfileBufferAsync(string lbl,int ev){string buf="";SetCommandProperties(lbl);do{if(!await ReadInternalAsync())break;buf+=ExtractDataFromResponse(_outBuff);SetCommandProperties("ACKCommand");}while(_ser.ReceiveBuffer.Take(_ser.BufferIndex).Contains((byte)0x04));return buf;}
    public async Task<string> WriteProfileBufferAsync(string[] ls,List<string> data){int c=0;while(ls.Length>c){byte[] d=data[c].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();SetCommandProperties(ls[c]);var dl=_cmdBytes.ToList();int si=dl.IndexOf(0x28);if(si>=0)dl.InsertRange(si+1,d);_cmdBytes=dl.ToArray();if(!await ReadInternalAsync())return"Error: "+_outBuff;c++;}return string.Empty;}
    public async Task<string> WriteIECBufferAsync(List<string> data){byte[] d=data[0].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();_cmdBytes=d;_stop=0x06;_stop2=0x03;if(!await ReadInternalAsync())return"Error: "+_outBuff;return _outBuff;}
    public async Task<string> WriteBootLoaderProfileBufferInitiateAsync(string[] ls,List<string> data){string r="";int c=0;while(ls.Length>c){byte[] d=data[c].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();SetCommandProperties(ls[0]);_cmdBytes=d;if(!await WriteBootloaderInternalAsync())return"Error";if(c==0){for(int i=0;i<=4&&i<_ser.BufferIndex;i++)r+=(char)_ser.ReceiveBuffer[i];return r;}r=_ser.ReceiveBuffer[0].ToString();c++;}return r;}
    public async Task<string> WriteBootLoaderProfileBufferAsync(string[] ls,List<string> data){string r="";int c=1;while(ls.Length>c){byte[] d=data[c].Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();SetCommandProperties(ls[0]);_cmdBytes=d;if(!await WriteBootloaderInternalAsync())return"Error";r=_ser.ReceiveBuffer[0].ToString();c++;}return r;}
    public async Task<string> WriteNonProtocolPacketAsync(List<byte> data,byte stop,byte? stop2){_stop=stop;if(stop2!=null)_stop2=(byte)stop2;_cmdBytes=data.ToArray();await WriteBootloaderInternalAsync();await Task.Delay(10);return _outBuff;}
<<<<<<< HEAD
    public async Task<bool> IECAssociationStablishAsync()=>await ConnectToIECMeterAsync((int)IECSignOnMode.IEC_READ);
    public async Task IECAssociationDisconnectAsync(){SetCommandProperties("DisconnectAssociation");await ReadInternalAsync();_ser.ClosePort();}
    public async Task IECPortDisconnectAsync(){await Task.Run(()=>_ser.ClosePort());}
    public string ExtractDataFromResponse(string r){var m=Regex.Matches(r,@"(\([\w\W]*?\))",RegexOptions.Multiline|RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);return m.Count>0?m[0].Value:string.Empty;}
    public void SetCommandProperties(string tag)
    {
        try
        {
            if(string.IsNullOrEmpty(_repoXml))LoadRepoXml();
            var resolvedTag = ResolveTag(tag);
            if(string.Equals(resolvedTag,"IECReadoutAssociation",StringComparison.OrdinalIgnoreCase) ||
               string.Equals(resolvedTag,"ManufacurerReadAssociation",StringComparison.OrdinalIgnoreCase) ||
               string.Equals(resolvedTag,"ProgrammingAssociation",StringComparison.OrdinalIgnoreCase))
            {
                _cmdBytes = BuildAssociationCommand(resolvedTag);
                _stop=0x03;
                _stop2=0x03;
                return;
            }

            var x=XElement.Parse(_repoXml);
            var rs=x.Elements("COMMAND").Where(e=>string.Equals(e.Element("TAGNO")?.Value,resolvedTag,StringComparison.OrdinalIgnoreCase)).ToList();
            if(!rs.Any())
            {
                _cmdBytes = BuildFallbackCommand(resolvedTag);
                return;
            }

            var cmd=rs[0];
            _cmdBytes=cmd.Element("CommandDataBytes")!.Value.Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();
            _stop=(byte)Convert.ToInt32(cmd.Element("ResponseStopByte")!.Value,16);
            string sb2=cmd.Element("ResponseStopByte_2")?.Value??"";
            _stop2=string.IsNullOrEmpty(sb2)?_stop:(byte)Convert.ToInt32(sb2,16);
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[IEC] SetCmdProps: {ex.Message}");
        }
    }

    public async Task<bool> PerformSignOnAsync()
    {
        try 
        {
            // Clear buffers first to avoid "Nothing on console"
            _ser.ClosePort();
            await Task.Delay(100);
            
            // IEC meters usually start at 300 baud for sign-on
            _ser.SetSerialPortSettings(_set.SerialPort, "300", "None", "8", "1", 3500, 2500);
            
            var opened = _ser.OpenPort();
            if (!opened)
            {
                Raise("Failed to open port at 300 baud for IEC sign-on", true);
                return false;
            }

            // Send IEC wake-up sequence: / ? ! <CR> <LF>
            byte[] signOn = Encoding.ASCII.GetBytes("/?!\r\n");
            await _ser.SendAsync(signOn);

            // Wait for Meter ID response (e.g., /ABC5\Meter123)
            var response = _ser != null ? await _ser.ReceiveAsync(30, TimeSpan.FromSeconds(2)) : null;
            if (response != null && response.Length > 0)
            {
                string meterId = Encoding.ASCII.GetString(response);
                Raise($"IEC Meter Detected: {meterId}", false);
                
                // Optional: Send ACK to switch to 9600 baud as Desktop app does
                byte[] ack = new byte[] { 0x06 }; // ACK character
                await _ser.SendAsync(ack);
                
                // Brief delay then switch to higher baud rate
                await Task.Delay(100);
                _ser.SetSerialPortSettings(_set.SerialPort, "9600", "None", "8", "1", 3500, 2500);
                
                return true;
            }
            
            Raise("No response from IEC Meter", true);
            return false;
        }
        catch (Exception ex) 
        {
            Raise($"IEC Error: {ex.Message}", true);
            return false;
        }
    }

    void LoadRepoXml()
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("Configuration/1PCommandRepository.xml").GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            _repoXml = reader.ReadToEnd();
        }
        catch
        {
            try
            {
                string p=System.IO.Path.Combine(FileSystem.AppDataDirectory,"Configuration","1PCommandRepository.xml");
                if(System.IO.File.Exists(p))_repoXml=System.IO.File.ReadAllText(p);
            }
            catch{}
        }
    }
    async Task<bool> ReadInternalAsync(){return await Task.Run(()=>{try{if(_cmdBytes.Contains((byte)3)||_cmdBytes.Contains((byte)4)){byte bcc=0;for(int i=1;i<_cmdBytes.Length-1;i++)bcc^=_cmdBytes[i];_cmdBytes[_cmdBytes.Length-1]=bcc;}int retry=3;while(retry-->0){bool ok=_ser.fSendDataToPort(_cmdBytes,_cmdBytes.Length);_outBuff=BuildBuf();if(!ok){if(_outBuff.ToUpperInvariant().Contains("READY"))return true;continue;}if(_outBuff.ToUpperInvariant().Contains("(ER")){System.Threading.Thread.Sleep(200);continue;}return true;}return false;}catch{return false;}});}
    async Task<bool> WriteBootloaderInternalAsync(){return await Task.Run(()=>{try{bool ok=_ser.fSendDataToPort(_cmdBytes,_cmdBytes.Length);_outBuff=BuildBuf();return ok;}catch{return false;}});}
    string BuildBuf(){string r="";for(int i=0;i<_ser.BufferIndex;i++)r+=(char)_ser.ReceiveBuffer[i];return r;}
    string ResolveTag(string tag)=>tag switch
    {
        "Billing" => "IECReadoutAssociation",
        "LoadSurvey" => "LoadProfileCommand",
        "Daily" => "DailyProfileCommand",
        "DailyProfile" => "DailyProfileCommand",
        "Tamper" => "TamperCommand",
        _ => tag
    };
    byte[] BuildAssociationCommand(string tag)
    {
        var mode = tag switch
        {
            "ProgrammingAssociation" => '2',
            "ManufacurerReadAssociation" => '1',
            _ => '0'
        };

        return new[] { (byte)0x06, (byte)'0', (byte)_negotiatedBaudIndex, (byte)mode, (byte)0x0D, (byte)0x0A };
    }
    byte[] BuildFallbackCommand(string tag)
    {
        return tag switch
        {
            "MeterSignon" => new byte[] { 0x2F, 0x3F, 0x21, 0x0D, 0x0A },
            "ACKCommand" => new byte[] { 0x06 },
            "DisconnectAssociation" => new byte[] { 0x01, 0x42, 0x30, 0x0D, 0x0A },
            _ => System.Text.Encoding.ASCII.GetBytes($"{tag}\r\n")
        };
    }
    static char ExtractBaudIndex(string signonResponse)
    {
        var match = Regex.Match(signonResponse ?? string.Empty, @"\/.{3}([0-6])");
        return match.Success ? match.Groups[1].Value[0] : '5';
    }
    static string ResolveBaudRate(char baudIndex)=>baudIndex switch
    {
        '0' => "300",
        '1' => "600",
        '2' => "1200",
        '3' => "2400",
        '4' => "4800",
        '5' => "9600",
        '6' => "19200",
        _ => "9600"
    };
=======
    public async Task<bool> IECAssociationStablishAsync()=>await Task.FromResult(true);
    public async Task IECAssociationDisconnectAsync(){SetCommandProperties("DisconnectAssociation");await ReadInternalAsync();_ser.ClosePort();}
    public async Task IECPortDisconnectAsync(){await Task.Run(()=>_ser.ClosePort());}
    public string ExtractDataFromResponse(string r){var m=Regex.Matches(r,@"(\([\w\W]*?\))",RegexOptions.Multiline|RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);return m.Count>0?m[0].Value:string.Empty;}
    public void SetCommandProperties(string tag){try{if(string.IsNullOrEmpty(_repoXml))LoadRepoXml();var x=XElement.Parse(_repoXml);var rs=x.Elements("COMMAND").Where(e=>e.Element("TAGNO")?.Value==tag).ToList();if(!rs.Any())return;var cmd=rs[0];_cmdBytes=cmd.Element("CommandDataBytes")!.Value.Split('.').Select(s=>Convert.ToByte(s,16)).ToArray();_stop=(byte)Convert.ToInt32(cmd.Element("ResponseStopByte")!.Value,16);string sb2=cmd.Element("ResponseStopByte_2")?.Value??"";_stop2=string.IsNullOrEmpty(sb2)?_stop:(byte)Convert.ToInt32(sb2,16);}catch(Exception ex){System.Diagnostics.Debug.WriteLine($"[IEC] SetCmdProps: {ex.Message}");}}
    void LoadRepoXml(){try{string p=System.IO.Path.Combine(FileSystem.AppDataDirectory,"Configuration","1PCommandRepository.xml");if(System.IO.File.Exists(p))_repoXml=System.IO.File.ReadAllText(p);}catch{}}
    async Task<bool> ReadInternalAsync(){return await Task.Run(()=>{try{if(_cmdBytes.Contains((byte)3)||_cmdBytes.Contains((byte)4)){byte bcc=0;for(int i=1;i<_cmdBytes.Length-1;i++)bcc^=_cmdBytes[i];_cmdBytes[_cmdBytes.Length-1]=bcc;}int retry=3;while(retry-->0){bool ok=_ser.fSendDataToPort(_cmdBytes,_cmdBytes.Length);_outBuff=BuildBuf();if(!ok){if(_outBuff.ToUpperInvariant().Contains("READY"))return true;continue;}if(_outBuff.ToUpperInvariant().Contains("(ER")){System.Threading.Thread.Sleep(200);continue;}return true;}return false;}catch{return false;}});}
    async Task<bool> WriteBootloaderInternalAsync(){return await Task.Run(()=>{try{bool ok=_ser.fSendDataToPort(_cmdBytes,_cmdBytes.Length);_outBuff=BuildBuf();return ok;}catch{return false;}});}
    string BuildBuf(){string r="";for(int i=0;i<_ser.BufferIndex;i++)r+=(char)_ser.ReceiveBuffer[i];return r;}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
}
