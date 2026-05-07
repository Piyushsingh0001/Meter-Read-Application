#if WINDOWS
using System;
using System.Collections.Generic;
using System.IO.Ports;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services
{
    public class WindowsSerialPortService : ISerialPortService
    {
        private SerialPort? _port;
        private readonly object _bufLock = new();
        public bool IsOpen { get; private set; }
        public byte[] ReceiveBuffer { get; } = new byte[8192];
        public int BufferIndex { get; private set; }
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
            finally { IsOpen = false; }
        }

        public bool fSendDataToPort(byte[] data, int length)
        {
            if (!IsOpen || _port == null) return false;
            try
            {
                _port.Write(data, 0, Math.Min(length, data.Length));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool fSendIrDADataToPort(byte[] data, int length) => fSendDataToPort(data, length);
        public bool fSendIrDADataToPort_1P(byte[] data, int length) => fSendDataToPort(data, length);

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
            }
            catch { }
        }
    }
}

#endif
