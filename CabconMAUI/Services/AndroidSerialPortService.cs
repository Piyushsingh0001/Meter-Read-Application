using CabconMAUI.Services.Interfaces;
<<<<<<< HEAD
#if ANDROID
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
#endif

namespace CabconMAUI.Services;
public class AndroidSerialPortService : ISerialPortService
{
    private const string UsbPermissionAction = "com.cabcon.maui.USB_PERMISSION";
    private const int MeterProcessingDelayMs = 200;
    private readonly object _bufferLock = new();
    private CancellationTokenSource? _readerCts;
    private Task? _readerTask;
    public bool   IsOpen{get;private set;}
    public byte[] ReceiveBuffer{get;}=new byte[4096];
    public int    BufferIndex{get;private set;}
#if ANDROID
    public string EndpointDetails => _writeEndpoint != null ? $"WriteEP: Address={_writeEndpoint.Address}, MaxPacket={_writeEndpoint.MaxPacketSize}, Dir={_writeEndpoint.Direction}, Att={_writeEndpoint.Attributes}, EpNum={_writeEndpoint.EndpointNumber}" : "WriteEP: NULL";
#else
    public string EndpointDetails => "Not Android";
#endif
    public static string EndpointError { get; set; } = string.Empty;
=======
namespace CabconMAUI.Services;
public class AndroidSerialPortService : ISerialPortService
{
    public bool   IsOpen{get;private set;}
    public byte[] ReceiveBuffer{get;}=new byte[4096];
    public int    BufferIndex{get;private set;}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    public int    CommandTimeout{get;set;}=3500;
    public int    InterchatracterDelay{get;set;}=2500;
    public int    NoOfBytesToBeReceive3PHDLMSCalibCoeff{get;set;}=200;
    private string _p="COM1",_br="9600",_par="None",_db="8",_sb="1";
<<<<<<< HEAD

#if ANDROID
    private UsbManager? _usbManager;
    private UsbDevice? _device;
    private UsbDeviceConnection? _connection;
    private UsbInterface? _controlInterface;
    private UsbInterface? _dataInterface;
    private UsbEndpoint? _readEndpoint;
    private UsbEndpoint? _writeEndpoint;
    private UsbPermissionReceiver? _permissionReceiver;
    private TaskCompletionSource<bool>? _permissionTcs;
    private bool _receiverRegistered;
#endif

    public void SetSerialPortSettings(string p,string br,string par,string db,string sb,int t,int ic){_p=p;_br=br;_par=par;_db=db;_sb=sb;CommandTimeout=t;InterchatracterDelay=ic;}
    public bool OpenPort()
    {
        try
        {
            ResetBuffer();
#if ANDROID
            return OpenAndroidUsbPort();
#else
            System.Threading.Thread.Sleep(300);
            StartReaderLoop();
            IsOpen=true;
            return true;
#endif
        }
        catch
        {
            return false;
        }
    }
    public void ClosePort()
    {
        try
        {
            _readerCts?.Cancel();
            _readerTask?.Wait(100);
            
            // Disable DTR/RTS to power down optical probe
            SetDtrRts(false);
            
#if ANDROID
            CloseAndroidResources();
#endif
            IsOpen=false;
            ResetBuffer();
        }
        catch{}
    }
    public bool fSendDataToPort(byte[] data, int len)
    {
        if (!IsOpen)
            return false;

        try
        {
            ResetBuffer();

#if ANDROID

            if (_connection == null || _writeEndpoint == null || _readEndpoint == null)
                return false;

            FlushInputBuffer();

            var tx = new byte[Math.Min(len, data.Length)];
            Buffer.BlockCopy(data, 0, tx, 0, tx.Length);

            int result = _connection.BulkTransfer(
                _writeEndpoint,
                tx,
                tx.Length,
                1000);

            if (result < 0) 
            {
                System.Diagnostics.Debug.WriteLine($"[Android] ERROR: BulkTransfer failed with result {result}");
                EndpointError = $"BulkTransfer Failed! Error Code: {result}";
                return false;
            }
            else if (result == 0)
            {
                System.Diagnostics.Debug.WriteLine("[Android] ERROR: BulkTransfer wrote 0 bytes");
                EndpointError = "BulkTransfer Failed! Wrote 0 bytes";
                return false;
            }

            return WaitForReply();

#else
            return false;
#endif
        }
        catch
        {
            return false;
        }
    }
    public bool fSendIrDADataToPort(byte[] d,int l)
    {
        if(!IsOpen)return false;
        try
        {
            ResetBuffer();
#if ANDROID
            // TODO: Implement Android IrDA support when available
            return false;
#else
            // Use serial port for IrDA simulation on non-Android platforms
            return fSendDataToPort(d,l);
#endif
        }
        catch{return false;}
    }
    public bool fSendIrDADataToPort_1P(byte[] d,int l)
    {
        if(!IsOpen)return false;
        try
        {
            ResetBuffer();
#if ANDROID
            // TODO: Implement Android IrDA support when available
            return false;
#else
            // Use serial port for IrDA simulation on non-Android platforms
            return fSendDataToPort(d,l);
#endif
        }
        catch{return false;}
    }

    public async Task<bool> ConnectAsync(string portName, int baudRate)
    {
        try
        {
            if (IsOpen) ClosePort();
            
            SetSerialPortSettings(portName, baudRate.ToString(), "None", "8", "1", 3500, 2500);
            var opened = OpenPort();
            
            if (opened)
            {
                // Configure serial hardware with proper baud rate and parity from settings
                ConfigureSerialHardware(baudRate, _par, int.TryParse(_db, out var db) ? db : 8);
            }
            
            return await Task.FromResult(opened);
        }
        catch (UnauthorizedAccessException)
        {
            return await Task.FromResult(false);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(false);
        }
    }

    private void ConfigureSerialHardware(int baudRate, string parity = "Even", int dataBits = 8)
    {
        try
        {
#if ANDROID
            if (_connection == null || _device == null) return;
            
            if (_device.VendorId == 0x0403)
            {
                // FTDI Chip Configuration
                System.Diagnostics.Debug.WriteLine($"[Android] Configuring FTDI hardware: {baudRate} {parity} {dataBits}");
                
                // 1. Set Baud Rate
                int value = 0;
                int index = 0;
                if (baudRate == 300) { value = 0x2710; }
                else if (baudRate == 9600) { value = 0x4138; }
                else 
                {
                    // Generic FT232R calc
                    int[] fracCode = { 0, 3, 2, 4, 1, 5, 6, 7 };
                    int val = 3000000 * 8 / baudRate;
                    int frac = val & 7;
                    value = (val >> 3) | (fracCode[frac] << 14);
                    index = (value >> 16) & 0xFFFF;
                    value = value & 0xFFFF;
                }
                int result = _connection.ControlTransfer((UsbAddressing)0x40, 0x03, value, index, null, 0, 1000);
                System.Diagnostics.Debug.WriteLine($"[Android] FTDI SET_BAUDRATE result: {result}");

                // 2. Set Line Control
                int parityCode = parity.ToUpperInvariant() switch {
                    "ODD" => 1,
                    "EVEN" => 2,
                    "MARK" => 3,
                    "SPACE" => 4,
                    _ => 0
                };
                int ftdiLineControl = (parityCode << 8) | (0 << 11) | dataBits;
                result = _connection.ControlTransfer((UsbAddressing)0x40, 0x04, ftdiLineControl, 0, null, 0, 1000);
                System.Diagnostics.Debug.WriteLine($"[Android] FTDI SET_DATA result: {result}");

                // 3. Disable Flow Control
                result = _connection.ControlTransfer((UsbAddressing)0x40, 0x02, 0, 0, null, 0, 1000);
                System.Diagnostics.Debug.WriteLine($"[Android] FTDI SET_FLOW result: {result}");
                
                return;
            }

            // Log VID/PID for debugging
            System.Diagnostics.Debug.WriteLine($"[Android] Configuring device VID:{_device.VendorId:X4} PID:{_device.ProductId:X4}");

            // Check for CP210x chip (Silicon Labs)
            if (_device.VendorId == 0x10C4)
            {
                ConfigureCP210xBaudRate(baudRate, parity, dataBits);
            }
            else
            {
                // Use standard CDC for other chips
                ConfigureStandardCdcBaudRate(baudRate, parity, dataBits);
            }
            
            // Ensure DTR/RTS are high to power the probe
            _connection.ControlTransfer((UsbAddressing)0x21, 0x22, 0x03, 0, null, 0, 1000);
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Android] ConfigureSerialHardware failed: {ex.Message}");
        }
    }

    private void ConfigureCP210xBaudRate(int baudRate, string parity, int dataBits)
    {
#if ANDROID
        if (_connection == null || _controlInterface == null) return;
        
        // CRITICAL: Correct CP210x baud rate command - 4-byte integer format
        byte[] baudData = new byte[] {
            (byte)(baudRate & 0xFF),
            (byte)((baudRate >> 8) & 0xFF),
            (byte)((baudRate >> 16) & 0xFF),
            (byte)((baudRate >> 24) & 0xFF)
        };
        
        // Request: 0x1E (SET_BAUDRATE), Value: 0, Data: 4-byte baud
        int result = _connection.ControlTransfer((UsbAddressing)0x41, 0x1E, 0, 0, baudData, 4, 1000);
        System.Diagnostics.Debug.WriteLine($"[Android] CP210x SET_BAUDRATE result: {result}");
        
        // Set line control for CP210x (parity and data bits)
        var parityCode = parity.ToUpperInvariant() switch
        {
            "ODD" => 0x01,
            "EVEN" => 0x02,
            "MARK" => 0x03,
            "SPACE" => 0x04,
            _ => 0x00
        };
        
        int lineControl = (dataBits - 5) | (parityCode << 4);
        result = _connection.ControlTransfer((UsbAddressing)0x41, 0x03, lineControl, 0, null, 0, 1000);
        System.Diagnostics.Debug.WriteLine($"[Android] CP210x line control set result: {result}");
        
        // CRITICAL: Disable hardware flow control to prevent deadlock
        // Request: 0x13 (SET_FLOW), Value: 0 (Disable all flow control)
        result = _connection.ControlTransfer((UsbAddressing)0x41, 0x13, 0, 0, null, 0, 1000);
        System.Diagnostics.Debug.WriteLine($"[Android] CP210x SET_FLOW disable result: {result}");
#endif
    }

    private void ConfigureStandardCdcBaudRate(int baudRate, string parity, int dataBits)
    {
#if ANDROID
        if (_connection == null || _controlInterface == null) return;
        
        // Convert parity string to parity code
        var parityCode = parity.ToUpperInvariant() switch
        {
            "ODD" => (byte)1,
            "EVEN" => (byte)2,
            "MARK" => (byte)3,
            "SPACE" => (byte)4,
            _ => (byte)0  // Default to None
        };

        // Standard USB CDC 'Set Line Coding' structure:
        // [Baud Rate (4 bytes)] [Stop Bits (1 byte)] [Parity (1 byte)] [Data Bits (1 byte)]
        byte[] lineCoding = new byte[] {
            (byte)(baudRate & 0xff),
            (byte)((baudRate >> 8) & 0xff),
            (byte)((baudRate >> 16) & 0xff),
            (byte)((baudRate >> 24) & 0xff),
            0x00, // 0 = 1 Stop Bit
            parityCode, // Parity based on parameter
            (byte)dataBits  // Data bits parameter
        };

        // 0x21: Host-to-Interface-Class request
        // 0x20: SET_LINE_CODING
        int result = _connection.ControlTransfer((UsbAddressing)0x21, 0x20, 0, _controlInterface.Id, lineCoding, lineCoding.Length, 1000);
        System.Diagnostics.Debug.WriteLine($"[Android] CDC SET_LINE_CODING result: {result}");
#endif
    }

    private void SetDtrRts(bool enabled)
    {
        try
        {
#if ANDROID
            if (_connection == null) return;
            
            if (_device != null && _device.VendorId == 0x0403)
            {
                // FTDI SET_MODEM_CTRL for DTR/RTS (probe power)
                int ftdiValue = enabled ? 0x0303 : 0x0300;
                int ftdiResult = _connection.ControlTransfer((UsbAddressing)0x40, 0x01, ftdiValue, 0, null, 0, 500);
                System.Diagnostics.Debug.WriteLine($"[Android] FTDI SetDtrRts: {(enabled ? "ON" : "OFF")} (result: {ftdiResult})");
                return;
            }

            // 0x01 = DTR, 0x02 = RTS. 0x03 = Both ON.
            int value = enabled ? 0x03 : 0x00; 
            
            // Standard USB CDC request to set control line state
            int result = _connection.ControlTransfer((UsbAddressing)0x21, 0x22, value, 0, null, 0, 500);
            
            if (enabled)
            {
                System.Diagnostics.Debug.WriteLine($"[Android] SetDtrRts: Power ON probe (result: {result})");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Android] SetDtrRts: Power OFF probe (result: {result})");
            }
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Android] SetDtrRts failed: {ex.Message}");
        }
    }

    private void SendBreak(int durationMs)
    {
#if ANDROID
        if (_connection == null) return;
        // 0x23 is the CDC standard for SEND_BREAK
        _connection.ControlTransfer((UsbAddressing)0x21, 0x23, durationMs, 0, null, 0, 1000);
        System.Threading.Thread.Sleep(durationMs + 50); 
#endif
    }

    public async Task<bool> SendAsync(byte[] data)
    {
        return await Task.FromResult(fSendDataToPort(data, data.Length));
    }

    public async Task<byte[]> ReceiveAsync(int maxBytes, TimeSpan timeout)
    {
        return await Task.FromResult(Array.Empty<byte>());
    }

    public void SetReceiveBuffer(byte[] data,int length)
    {
        lock (_bufferLock)
        {
            Array.Clear(ReceiveBuffer, 0, ReceiveBuffer.Length);
            var copyLen = Math.Min(Math.Max(0, length), Math.Min(ReceiveBuffer.Length, data?.Length ?? 0));
            if (copyLen > 0 && data != null)
            {
                Buffer.BlockCopy(data, 0, ReceiveBuffer, 0, copyLen);
            }

            BufferIndex = copyLen;
        }
    }
    public int ASCIIHexToDecimalConversion(byte[] b,int s,int l){try{string h="";for(int i=s;i<s+l&&i<b.Length;i++)h+=(char)b[i];return Convert.ToInt32(h,16);}catch{return 0;}}
    public IEnumerable<string> GetAvailablePorts()
    {
        var r=new List<string>();
#if ANDROID
        try
        {
            _usbManager ??= Android.App.Application.Context.GetSystemService(Context.UsbService) as UsbManager;
            if(_usbManager != null)
            {
                foreach(var device in _usbManager.DeviceList.Values.OrderBy(d => d.DeviceName))
                {
                    r.Add($"{device.DeviceName} ({device.VendorId:X4}:{device.ProductId:X4})");
                }
            }
        }
        catch{}
        if(!r.Any())r.Add("USB Serial");
#else
        r.AddRange(new[]{"COM1","COM3"});
#endif
        return r;
    }

    #if ANDROID
    public bool ChangeBaudRate(int newBaudRate)
    {
        // Interface-compatible overload - use current parity setting
        return ChangeBaudRate(newBaudRate, _par);
    }
    
    public bool ChangeBaudRate(int newBaudRate, string parity = "None")
    {
        if (!IsOpen || _connection == null || _controlInterface == null)
        {
            return false;
        }

        try
        {
            _br = newBaudRate.ToString();
            _par = parity; // Update parity setting
            
            // Determine data bits based on baud rate and parity
            int dataBits = 8; // Default for most operations
            if (newBaudRate == 300 && parity.Equals("Even", StringComparison.OrdinalIgnoreCase))
            {
                dataBits = 7; // 7-E-1 for IEC handshake
                _db = "7";
            }
            else
            {
                _db = "8";
            }
            
            ConfigureSerialHardware(newBaudRate, parity, dataBits);
            System.Threading.Thread.Sleep(100); // Allow settling time
            return true;
        }
        catch
        {
            return false;
        }
    }
#else
    public bool ChangeBaudRate(int newBaudRate)
    {
        // For non-Android platforms, this is handled by WindowsSerialPortService
        return false;
    }
    
    public bool ChangeBaudRate(int newBaudRate, string parity = "None")
    {
        // For non-Android platforms, this is handled by WindowsSerialPortService
        return false;
    }
#endif

    public async Task<bool> IecBaudRateNegotiationAsync()
    {
        if (!IsOpen)
        {
            return false;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine("[Android] Starting IEC Baud Rate Negotiation");
            
            // Send wake-up break signal before initial communication
            SendBreak(200);
            
            // CRITICAL: Configure hardware for 7-E-1 BEFORE sending any data
            // Start at 300 baud for IEC handshake with Even Parity and 7 Data Bits (7-E-1)
            ConfigureSerialHardware(300, "Even", 7);
            System.Threading.Thread.Sleep(100); // Allow hardware to settle
            
            // Update internal settings to match
            _br = "300";
            _par = "Even";
            _db = "7";
            
            System.Diagnostics.Debug.WriteLine("[Android] Configured for 300 baud, 7-E-1 handshake");
            
            // Flush any ghost buffer noise after configuration
            FlushInputBuffer();

            // Send identification request
            var signOnRequest = System.Text.Encoding.ASCII.GetBytes("/?!\r\n");
            System.Diagnostics.Debug.WriteLine($"[Android] Sending identification request: {BitConverter.ToString(signOnRequest)}");
            
            if (!fSendDataToPort(signOnRequest, signOnRequest.Length))
            {
                System.Diagnostics.Debug.WriteLine("[Android] Failed to send identification request");
                return false;
            }

            // Wait for identification response
            await Task.Delay(500);
            
            System.Diagnostics.Debug.WriteLine($"[Android] BufferIndex after request: {BufferIndex}");
            if (BufferIndex == 0)
            {
                System.Diagnostics.Debug.WriteLine("[Android] No response received from meter");
                return false;
            }

            // Parse identification string to extract baud rate
            var response = System.Text.Encoding.ASCII.GetString(ReceiveBuffer, 0, BufferIndex);
            System.Diagnostics.Debug.WriteLine($"[Android] Received response: {response.Trim()}");
            
            var baudRate = ParseIecBaudRate(response);
            
            if (baudRate == 0)
            {
                System.Diagnostics.Debug.WriteLine("[Android] Failed to parse baud rate from response");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[Android] Negotiated baud rate: {baudRate}");

            // Send ACK
            var ack = new byte[] { 0x06 };
            if (!fSendDataToPort(ack, ack.Length))
            {
                System.Diagnostics.Debug.WriteLine("[Android] Failed to send ACK");
                return false;
            }

            // Change to negotiated baud rate with No Parity and 8 Data Bits (8-N-1 for DLMS mode)
            System.Diagnostics.Debug.WriteLine($"[Android] Switching to {baudRate} baud, 8-N-1 for DLMS");
            return ChangeBaudRate(baudRate, "None");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Android] IecBaudRateNegotiationAsync exception: {ex.Message}");
            return false;
        }
    }

    private int ParseIecBaudRate(string identification)
    {
        try
        {
            // IEC identification format: /ABC5... where 5 indicates 9600 baud
            if (string.IsNullOrWhiteSpace(identification) || !identification.StartsWith("/"))
            {
                return 0;
            }

            // Extract the baud rate indicator character (usually position 4)
            if (identification.Length >= 4)
            {
                var baudIndicator = identification[3];
                return baudIndicator switch
                {
                    '0' => 300,
                    '1' => 600,
                    '2' => 1200,
                    '3' => 2400,
                    '4' => 4800,
                    '5' => 9600,
                    '6' => 19200,
                    '7' => 38400,
                    '8' => 57600,
                    '9' => 115200,
                    _ => 9600 // Default to 9600 if unknown
                };
            }

            return 0;
        }
        catch
        {
            return 0;
        }
    }

#if ANDROID
    private bool OpenAndroidUsbPort()
    {
        _usbManager ??= Android.App.Application.Context.GetSystemService(Context.UsbService) as UsbManager;
        if (_usbManager == null)
        {
            return false;
        }

        EnsureReceiverRegistered();
        _device = FindTargetDevice();
        if (_device == null)
        {
            return false;
        }

        if (!_usbManager.HasPermission(_device) && !RequestPermission(_device))
        {
            return false;
        }

        _connection = _usbManager.OpenDevice(_device);
        if (_connection == null)
        {
            return false;
        }

        if (!TryBindInterfaces(_device, _connection))
        {
            CloseAndroidResources();
            return false;
        }

        if (_device.VendorId == 0x10C4)
        {
            // CRITICAL: CP210x Initialization Sequence - Exact Order Required
            
            // 2. ENABLE THE UART ENGINE (CRITICAL - Data Gate)
            // Request: 0x00 (IFC_ENABLE), Value: 0x01 (Enable)
            int result = _connection.ControlTransfer((UsbAddressing)0x41, 0x00, 0x01, 0, null, 0, 1000);
            System.Diagnostics.Debug.WriteLine($"[Android] CP210x IFC_ENABLE result: {result}");
            
            // 3. SET THE BAUD RATE (Use the 4-byte method for CP210x)
            byte[] baudData = new byte[] { 0x2C, 0x01, 0x00, 0x00 }; // 300 Baud in Hex
            result = _connection.ControlTransfer((UsbAddressing)0x41, 0x1E, 0, 0, baudData, 4, 1000);
            System.Diagnostics.Debug.WriteLine($"[Android] CP210x SET_BAUDRATE (300) result: {result}");
            
            // 6. Disable hardware flow control
            result = _connection.ControlTransfer((UsbAddressing)0x41, 0x13, 0, 0, null, 0, 1000);
            System.Diagnostics.Debug.WriteLine($"[Android] CP210x SET_FLOW disable result: {result}");
        }

        // 4. POWER THE PROBE (Set DTR/RTS once for all chips)
        SetDtrRts(true);
        
        // 5. WAIT FOR STABILIZATION
        System.Threading.Thread.Sleep(200); 
        
        // CRITICAL: Flush input buffer immediately after opening port to clear ghost data
        FlushInputBuffer();
        
        StartReaderLoop();
        IsOpen = true;
        return true;
    }

    private UsbDevice? FindTargetDevice()
    {
        if (_usbManager == null || _usbManager.DeviceList.Count == 0)
        {
            return null;
        }

        foreach (var device in _usbManager.DeviceList.Values)
        {
            if (string.Equals(device.DeviceName, _p, StringComparison.OrdinalIgnoreCase) ||
                _p.Contains(device.DeviceName, StringComparison.OrdinalIgnoreCase))
            {
                return device;
            }
        }

        return _usbManager.DeviceList.Values.OrderBy(d => d.DeviceName).FirstOrDefault();
    }

    private void EnsureReceiverRegistered()
    {
        if (_receiverRegistered)
        {
            return;
        }

        _permissionReceiver = new UsbPermissionReceiver(this);
        var filter = new IntentFilter(UsbPermissionAction);
        filter.AddAction(UsbManager.ActionUsbDeviceDetached);
        Android.App.Application.Context.RegisterReceiver(_permissionReceiver, filter);
        _receiverRegistered = true;
    }

    private bool RequestPermission(UsbDevice device)
    {
        _permissionTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var intent = new Intent(UsbPermissionAction);
        var pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, intent, PendingIntentFlags.Immutable);
        _usbManager?.RequestPermission(device, pendingIntent);
        return _permissionTcs.Task.Wait(Math.Max(1000, CommandTimeout));
    }

    private bool TryBindInterfaces(UsbDevice device, UsbDeviceConnection connection)
    {
        UsbInterface? data = null;
        UsbInterface? control = null;
        UsbEndpoint? read = null;
        UsbEndpoint? write = null;

        System.Diagnostics.Debug.WriteLine($"[Android] Device has {device.InterfaceCount} interfaces");

        // Special handling for single interface devices (common for CP210x, CH340, etc.)
        if (device.InterfaceCount == 1)
        {
            var singleInterface = device.GetInterface(0);
            var endpoints = FindBulkEndpoints(singleInterface);
            
            if (endpoints.read != null && endpoints.write != null)
            {
                data = singleInterface;
                control = singleInterface; // Single interface serves both roles
                read = endpoints.read;
                write = endpoints.write;
                System.Diagnostics.Debug.WriteLine($"[Android] Single interface device detected, class: {singleInterface.InterfaceClass}");
            }
        }
        else
        {
            // Multi-interface device: Find Data Interface with bulk endpoints (priority)
            for (var i = 0; i < device.InterfaceCount; i++)
            {
                var candidate = device.GetInterface(i);
                var endpoints = FindBulkEndpoints(candidate);
                if (endpoints.read != null && endpoints.write != null)
                {
                    data = candidate;
                    read = endpoints.read;
                    write = endpoints.write;
                    break; // Found data interface, stop searching
                }
            }

            // Second pass: Find and claim Control Interface (optional)
            for (var i = 0; i < device.InterfaceCount; i++)
            {
                var candidate = device.GetInterface(i);
                if (candidate.InterfaceClass == UsbClass.Comm && candidate.Id != data?.Id)
                {
                    control = candidate;
                    break;
                }
            }
        }

        if (data == null || read == null || write == null)
        {
            System.Diagnostics.Debug.WriteLine("[Android] Failed to find suitable data interface with bulk endpoints");
            return false;
        }

        // Claim Data Interface first (critical for communication)
        if (!connection.ClaimInterface(data, true))
        {
            System.Diagnostics.Debug.WriteLine($"[Android] Failed to claim data interface {data.Id}");
            return false;
        }

        // Only claim control interface if it's separate from data interface
        if (control != null && control.Id != data.Id)
        {
            if (!connection.ClaimInterface(control, true))
            {
                System.Diagnostics.Debug.WriteLine($"[Android] Failed to claim control interface {control.Id}, continuing with data interface only");
                control = null; // Failed to claim, but continue with data interface
            }
        }
        else if (control == null)
        {
            // For single interface devices, use data interface as control interface
            control = data;
        }

        _controlInterface = control;
        _dataInterface = data;
        _readEndpoint = read;
        _writeEndpoint = write;

        System.Diagnostics.Debug.WriteLine($"[Android] Successfully claimed interfaces - Data: {data.Id}, Control: {control.Id}");

        // ConfigureSerialHardware will be called after the proper CP210x sequence above
        // This is kept for non-CP210x chips only
        
        return true;
    }

    private static (UsbEndpoint? read, UsbEndpoint? write) FindBulkEndpoints(UsbInterface candidate)
    {
        UsbEndpoint? read = null;
        UsbEndpoint? write = null;
        for (var i = 0; i < candidate.EndpointCount; i++)
        {
            var endpoint = candidate.GetEndpoint(i);
            if (endpoint.Type != UsbAddressing.XferBulk)
            {
                continue;
            }

            if (endpoint.Direction == UsbAddressing.In)
            {
                read = endpoint;
            }
            else if (endpoint.Direction == UsbAddressing.Out)
            {
                write = endpoint;
            }
        }

        return (read, write);
    }

    private void ConfigureLineCoding()
    {
        // DEPRECATED: Use ConfigureSerialHardware instead
        // This method is kept for compatibility but should not be used
        System.Diagnostics.Debug.WriteLine("[Android] WARNING: ConfigureLineCoding is deprecated, use ConfigureSerialHardware");
        ConfigureSerialHardware(int.TryParse(_br, out var baud) ? baud : 9600, _par, byte.TryParse(_db, out var db) ? db : (byte)8);
    }

    private byte[] BuildLineCodingPayload()
    {
        var baud = int.TryParse(_br, out var parsedBaud) ? parsedBaud : 9600;
        var stopBitsCode = _sb switch
        {
            "1.5" => (byte)1,
            "2" => (byte)2,
            _ => (byte)0
        };
        var parityCode = _par.ToUpperInvariant() switch
        {
            "ODD" => (byte)1,
            "EVEN" => (byte)2,
            "MARK" => (byte)3,
            "SPACE" => (byte)4,
            _ => (byte)2  // Default to EVEN PARITY for Indian meters
        };
        var dataBits = byte.TryParse(_db, out var parsedDataBits) ? parsedDataBits : (byte)8;

        return new[]
        {
            (byte)(baud & 0xFF),
            (byte)((baud >> 8) & 0xFF),
            (byte)((baud >> 16) & 0xFF),
            (byte)((baud >> 24) & 0xFF),
            stopBitsCode,
            parityCode,
            dataBits
        };
    }

    private void AppendReceivedData(byte[] data, int length)
    {
        lock (_bufferLock)
        {
            var copyLen = Math.Min(length, ReceiveBuffer.Length - BufferIndex);
            Array.Copy(data, 0, ReceiveBuffer, BufferIndex, copyLen);
            BufferIndex += copyLen;
            
            // HDLC Frame Recognition: Check for complete frames
            for (int i = 0; i < copyLen; i++)
            {
                if (ReceiveBuffer[i] == 0x7E) // HDLC start/end flag
                {
                    // Found potential frame boundary, signal to WaitForReply
                    break;
                }
            }
            
            if (BufferIndex >= ReceiveBuffer.Length)
            {
                BufferIndex = ReceiveBuffer.Length;
            }
        }
    }

    private void CloseAndroidResources()
    {
        try
        {
            if (_connection != null && _dataInterface != null)
            {
                _connection.ReleaseInterface(_dataInterface);
            }

            if (_connection != null && _controlInterface != null)
            {
                _connection.ReleaseInterface(_controlInterface);
            }
        }
        catch { }

        try
        {
            _connection?.Close();
        }
        catch { }

        if (_receiverRegistered && _permissionReceiver != null)
        {
            try { Android.App.Application.Context.UnregisterReceiver(_permissionReceiver); } catch { }
            _receiverRegistered = false;
        }

        _permissionReceiver = null;
        _permissionTcs = null;
        _connection = null;
        _controlInterface = null;
        _dataInterface = null;
        _readEndpoint = null;
        _writeEndpoint = null;
        _device = null;
    }

    private sealed class UsbPermissionReceiver : BroadcastReceiver
    {
        private readonly AndroidSerialPortService _owner;

        public UsbPermissionReceiver(AndroidSerialPortService owner)
        {
            _owner = owner;
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Action == UsbPermissionAction)
            {
                var granted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
                _owner._permissionTcs?.TrySetResult(granted);
                return;
            }

            if (intent?.Action == UsbManager.ActionUsbDeviceDetached)
            {
                var detached = intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;
                if (detached != null && _owner._device != null && detached.DeviceId == _owner._device.DeviceId)
                {
                    _owner.ClosePort();
                }
            }
        }
    }
#endif

    void ResetBuffer()
    {
        lock (_bufferLock)
        {
            Array.Clear(ReceiveBuffer,0,ReceiveBuffer.Length);
            BufferIndex=0;
        }
    }

    bool WaitForReply()
    {
        var timeout = DateTime.UtcNow.AddMilliseconds(Math.Max(250, CommandTimeout));
        var quietDelay = Math.Clamp(InterchatracterDelay, 150, CommandTimeout);
        var lastCount = 0;
        var stableSince = DateTime.UtcNow;

        while (DateTime.UtcNow < timeout)
        {
            var currentCount = BufferIndex;
            if (currentCount > 0)
            {
                if (currentCount != lastCount)
                {
                    lastCount = currentCount;
                    stableSince = DateTime.UtcNow;
                }
                else if ((DateTime.UtcNow - stableSince).TotalMilliseconds >= quietDelay)
                {
                    return true;
                }
            }

            System.Threading.Thread.Sleep(25);
        }

        return BufferIndex > 0;
    }

    void StartReaderLoop()
    {
        _readerCts?.Cancel();
        _readerCts = new CancellationTokenSource();
        _readerTask = Task.Run(async () =>
        {
#if ANDROID
            int packetSize = _readEndpoint?.MaxPacketSize ?? 64;
            var rx = new byte[packetSize * 8]; // Use multiple of packet size
#else
            var rx = new byte[512];
#endif
            
            while (!_readerCts.IsCancellationRequested)
            {
                try
                {
#if ANDROID
                    if (_connection != null && _readEndpoint != null)
                    {
                        var read = _connection.BulkTransfer(_readEndpoint, rx, rx.Length, 100);
                        if (read > 0)
                        {
                            if (_device?.VendorId == 0x0403)
                            {
                                // FTDI chips send a 2-byte modem/line status header per packet chunk
                                int dataRead = 0;
                                byte[] parsedRx = new byte[read];
                                int offset = 0;
                                while (offset < read)
                                {
                                    int chunkLen = Math.Min(packetSize, read - offset);
                                    if (chunkLen > 2)
                                    {
                                        int dataLen = chunkLen - 2;
                                        Buffer.BlockCopy(rx, offset + 2, parsedRx, dataRead, dataLen);
                                        dataRead += dataLen;
                                    }
                                    offset += chunkLen;
                                }
                                if (dataRead > 0)
                                {
                                    var hexData = BitConverter.ToString(parsedRx, 0, dataRead);
                                    var asciiData = System.Text.Encoding.ASCII.GetString(parsedRx, 0, dataRead);
                                    System.Diagnostics.Debug.WriteLine($"[Android] ReaderLoop: Received {dataRead} actual bytes (FTDI stripped)");
                                    System.Diagnostics.Debug.WriteLine($"[Android] HEX: {hexData}");
                                    System.Diagnostics.Debug.WriteLine($"[Android] ASCII: {asciiData.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0")}");
                                    AppendReceivedData(parsedRx, dataRead);
                                }
                            }
                            else
                            {
                                var hexData = BitConverter.ToString(rx, 0, read);
                                var asciiData = System.Text.Encoding.ASCII.GetString(rx, 0, read);
                                System.Diagnostics.Debug.WriteLine($"[Android] ReaderLoop: Received {read} bytes");
                                System.Diagnostics.Debug.WriteLine($"[Android] HEX: {hexData}");
                                System.Diagnostics.Debug.WriteLine($"[Android] ASCII: {asciiData.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0")}");
                                AppendReceivedData(rx, read);
                            }
                            continue;
                        }
                        else if (read == 0)
                        {
                            // Zero bytes could be timing issue, check connection health
                            if (!IsConnectionHealthy())
                            {
                                System.Diagnostics.Debug.WriteLine("[Android] ReaderLoop: Connection lost, breaking loop");
                                break;
                            }
                            // Continue waiting for data
                        }
                        else if (read < 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Android] ReaderLoop: BulkTransfer error {read} - pipe may be halted");
                            
                            // -1 indicates USB pipe is halted, need to clear it
                            if (read == -1 && _connection != null && _readEndpoint != null)
                            {
                                // Try to clear the halted pipe
                                try
                                {
                                    // Note: ClearHalt is not available on UsbDeviceConnection in Android
                                    // We need to reset the connection by re-opening the port
                                    System.Diagnostics.Debug.WriteLine("[Android] Pipe halted, connection reset required");
                                    
                                    // Signal that connection needs reset
                                    // The calling code should handle re-opening the port
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[Android] Error handling halted pipe: {ex.Message}");
                                }
                            }
                            
                            // Check if probe lost power (common with RTS/DTR issues)
                            await Task.Delay(200, _readerCts.Token).ConfigureAwait(false);
                        }
                    }
#endif
                    await Task.Delay(5, _readerCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Android] ReaderLoop exception: {ex.Message}");
                    await Task.Delay(100, _readerCts.Token).ConfigureAwait(false);
                }
            }
        }, _readerCts.Token);
    }
    
    private void FlushInputBuffer()
    {
#if ANDROID
        try
        {
            if (_connection == null || _readEndpoint == null) return;
            
            int packetSize = _readEndpoint.MaxPacketSize;
            byte[] trash = new byte[packetSize];
            
            // Keep reading until buffer is empty (returns 0 or negative)
            int attempts = 0;
            int maxAttempts = 10; // Prevent infinite loop
            
            while (attempts < maxAttempts)
            {
                int result = _connection.BulkTransfer(_readEndpoint, trash, trash.Length, 10);
                if (result <= 0)
                {
                    break; // Buffer is empty or error
                }
                System.Diagnostics.Debug.WriteLine($"[Android] Flushed {result} bytes from input buffer");
                attempts++;
            }
            
            if (attempts >= maxAttempts)
            {
                System.Diagnostics.Debug.WriteLine("[Android] WARNING: FlushInputBuffer max attempts reached");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Android] FlushInputBuffer error: {ex.Message}");
        }
#endif
    }
    
    private bool IsConnectionHealthy()
    {
#if ANDROID
        try
        {
            if (_connection == null || _device == null) return false;
            
            // Check if device is still connected
            var descriptor = _connection.GetRawDescriptors();
            return descriptor != null && descriptor.Length > 0;
        }
        catch
        {
            return false;
        }
#else
        return true;
#endif
    }
=======
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
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
}
