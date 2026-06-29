#if WINDOWS
using System;
using System.Collections.Generic;
using System.IO.Ports;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services
{
    public class WindowsSerialPortService : ISerialPortService
    {
<<<<<<< HEAD
        private const int MeterProcessingDelayMs = 200;
        private SerialPort? _port;
        private readonly object _bufLock = new();
        private readonly AutoResetEvent _dataSignal = new(false);
        public bool IsOpen { get; private set; }
        public byte[] ReceiveBuffer { get; } = new byte[8192];
        public int BufferIndex { get; private set; }
        public string EndpointDetails => "Windows COM Port";
=======
        private SerialPort? _port;
        private readonly object _bufLock = new();
        public bool IsOpen { get; private set; }
        public byte[] ReceiveBuffer { get; } = new byte[8192];
        public int BufferIndex { get; private set; }
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
        public int CommandTimeout { get; set; } = 3500;
        public int InterchatracterDelay { get; set; } = 2500;
        public int NoOfBytesToBeReceive3PHDLMSCalibCoeff { get; set; } = 200;

        private string _portName = "COM1";
        private int _baudRate = 9600;
        private Parity _parity = Parity.None;
        private int _dataBits = 8;
        private StopBits _stopBits = StopBits.One;

        public void SetSerialPortSettings(string port, string baudRate, string parity, string dataBits, string stopBits, int timeout, int intercharDelay)
        {
            _portName = port;
            if (int.TryParse(baudRate, out var br)) _baudRate = br;
            _parity = Enum.TryParse<Parity>(parity, true, out var p) ? p : Parity.None;
            if (int.TryParse(dataBits, out var db)) _dataBits = db;
            _stopBits = stopBits switch
            {
                "1" => StopBits.One,
                "1.5" => StopBits.OnePointFive,
                "2" => StopBits.Two,
                _ => StopBits.One
            };
            CommandTimeout = timeout;
            InterchatracterDelay = intercharDelay;
        }

        public bool OpenPort()
        {
            try
            {
                if (_port != null && _port.IsOpen) return true;
                _port = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits)
                {
                    ReadTimeout = CommandTimeout,
                    WriteTimeout = CommandTimeout
                };
                _port.DataReceived += OnDataReceived;
                _port.Open();
<<<<<<< HEAD
                ResetReceiveBuffer();
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
                IsOpen = _port.IsOpen;
                return IsOpen;
            }
            catch
            {
                IsOpen = false;
                return false;
            }
        }

        public void ClosePort()
        {
            try
            {
                if (_port == null) return;
                _port.DataReceived -= OnDataReceived;
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
                _port = null;
            }
            catch { }
<<<<<<< HEAD
            finally
            {
                ResetReceiveBuffer();
                IsOpen = false;
            }
        }

        public async Task<bool> ConnectAsync(string portName, int baudRate)
        {
            try
            {
                if (_port != null && _port.IsOpen) _port.Close();

                _port = new SerialPort(portName);
                _port.PortName = portName;
                _port.BaudRate = baudRate;
                
                // Match Desktop App's default settings
                _port.DataBits = 8;
                _port.Parity = Parity.None;
                _port.StopBits = StopBits.One;
                _port.ReadTimeout = 2000;
                _port.WriteTimeout = 2000;

                _port.Open();
                
                IsOpen = true;
                return await Task.FromResult(true);
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

        public async Task<bool> SendAsync(byte[] data)
        {
            return await Task.FromResult(fSendDataToPort(data, data.Length));
        }

        public async Task<byte[]> ReceiveAsync(int maxBytes, TimeSpan timeout)
        {
            var startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeout.TotalMilliseconds)
            {
                if (BufferIndex > 0)
                {
                    var bytesToReturn = Math.Min(BufferIndex, maxBytes);
                    var result = new byte[bytesToReturn];
                    Array.Copy(ReceiveBuffer, result, bytesToReturn);
                    ResetReceiveBuffer();
                    return result;
                }
                await Task.Delay(25);
            }
            return Array.Empty<byte>();
=======
            finally { IsOpen = false; }
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
        }

        public bool fSendDataToPort(byte[] data, int length)
        {
            if (!IsOpen || _port == null) return false;
            try
            {
<<<<<<< HEAD
                ResetReceiveBuffer();
                _port.DiscardInBuffer();
                _port.Write(data, 0, Math.Min(length, data.Length));
                return WaitForReply();
=======
                _port.Write(data, 0, Math.Min(length, data.Length));
                return true;
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
            }
            catch
            {
                return false;
            }
        }

        public bool fSendIrDADataToPort(byte[] data, int length) => fSendDataToPort(data, length);
        public bool fSendIrDADataToPort_1P(byte[] data, int length) => fSendDataToPort(data, length);

<<<<<<< HEAD
        public void SetReceiveBuffer(byte[] data, int length)
        {
            lock (_bufLock)
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

=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
        public int ASCIIHexToDecimalConversion(byte[] buf, int start, int len)
        {
            try
            {
                string h = "";
                for (int i = start; i < start + len && i < buf.Length; i++) h += (char)buf[i];
                return Convert.ToInt32(h, 16);
            }
            catch { return 0; }
        }

        public IEnumerable<string> GetAvailablePorts()
        {
            try { return SerialPort.GetPortNames(); }
            catch { return new[] { "COM1" }; }
        }

<<<<<<< HEAD
        public bool ChangeBaudRate(int newBaudRate)
        {
            if (!IsOpen || _port == null) return false;
            try
            {
                _baudRate = newBaudRate;
                _port.BaudRate = newBaudRate;
                Thread.Sleep(100); // Allow settling time
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IecBaudRateNegotiationAsync()
        {
            if (!IsOpen) return false;
            try
            {
                // Start at 300 baud for IEC handshake
                if (!ChangeBaudRate(300)) return false;

                // Send identification request
                var signOnRequest = System.Text.Encoding.ASCII.GetBytes("/?!\r\n");
                if (!fSendDataToPort(signOnRequest, signOnRequest.Length)) return false;

                // Wait for identification response
                await Task.Delay(500);
                if (BufferIndex == 0) return false;

                // Parse identification string to extract baud rate
                var response = System.Text.Encoding.ASCII.GetString(ReceiveBuffer, 0, BufferIndex);
                var baudRate = ParseIecBaudRate(response);
                if (baudRate == 0) return false;

                // Send ACK
                var ack = new byte[] { 0x06 };
                if (!fSendDataToPort(ack, ack.Length)) return false;

                // Change to negotiated baud rate
                return ChangeBaudRate(baudRate);
            }
            catch
            {
                return false;
            }
        }

        private int ParseIecBaudRate(string identification)
        {
            try
            {
                // IEC identification format: /ABC5... where 5 indicates 9600 baud
                if (string.IsNullOrWhiteSpace(identification) || !identification.StartsWith("/")) return 0;

                // Extract the baud rate indicator character (usually position 4)
                if (identification.Length >= 4)
                {
                    var baudIndicator = identification[3];
                    return baudIndicator switch
                    {
                        '0' => 300, '1' => 600, '2' => 1200, '3' => 2400, '4' => 4800,
                        '5' => 9600, '6' => 19200, '7' => 38400, '8' => 57600, '9' => 115200,
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

=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
        private void OnDataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_port == null) return;
                var toRead = _port.BytesToRead;
                if (toRead <= 0) return;
                var buf = new byte[toRead];
                var read = _port.Read(buf, 0, toRead);
                lock (_bufLock)
                {
                    var copyLen = Math.Min(read, ReceiveBuffer.Length - BufferIndex);
                    Array.Copy(buf, 0, ReceiveBuffer, BufferIndex, copyLen);
                    BufferIndex += copyLen;
                    if (BufferIndex >= ReceiveBuffer.Length) BufferIndex = 0;
                }
<<<<<<< HEAD
                if (read > 0) _dataSignal.Set();
            }
            catch { }
        }

        private void ResetReceiveBuffer()
        {
            lock (_bufLock)
            {
                Array.Clear(ReceiveBuffer, 0, ReceiveBuffer.Length);
                BufferIndex = 0;
            }
        }

        private bool WaitForReply()
        {
            var timeout = Math.Max(250, CommandTimeout);
            var quietWindow = Math.Clamp(InterchatracterDelay, 150, CommandTimeout);
            var started = DateTime.UtcNow;
            var lastChange = started;
            var lastCount = 0;

            while ((DateTime.UtcNow - started).TotalMilliseconds < timeout)
            {
                _dataSignal.WaitOne(25);

                var currentCount = BufferIndex;
                if (currentCount > 0)
                {
                    if (currentCount != lastCount)
                    {
                        lastCount = currentCount;
                        lastChange = DateTime.UtcNow;
                    }
                    else if ((DateTime.UtcNow - lastChange).TotalMilliseconds >= quietWindow)
                    {
                        return true;
                    }
                }
            }

            return BufferIndex > 0;
        }
=======
            }
            catch { }
        }
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    }
}

#endif
