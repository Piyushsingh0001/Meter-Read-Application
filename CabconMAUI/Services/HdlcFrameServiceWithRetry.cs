using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class HdlcFrameServiceWithRetry : IHdlcFrameServiceWithRetry
{
    private readonly IHdlcFrameService _baseService;
    private readonly ISerialPortService _serial;

    public event EventHandler<HdlcRetryEventArgs>? RetryAttempted;

    public long InitializationCounter 
    { 
        get => _baseService.InitializationCounter; 
        set => _baseService.InitializationCounter = value; 
    }

    public int SecuritysuitByte 
    { 
        get => _baseService.SecuritysuitByte; 
        set => _baseService.SecuritysuitByte = value; 
    }

    public byte nCMDByte => _baseService.nCMDByte;

    public HdlcFrameServiceWithRetry(IHdlcFrameService baseService, ISerialPortService serial)
    {
        _baseService = baseService;
        _serial = serial;
    }

    public async Task<bool> SendFrameWithRetryAsync(byte[] frame, int maxRetries = 3, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(5);
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                bool success = _serial.fSendDataToPort(frame, frame.Length);
                
                if (success)
                {
                    // Wait for response with timeout
                    var responseTask = Task.Run(() => 
                    {
                        var startTime = DateTime.Now;
                        while (DateTime.Now - startTime < timeout)
                        {
                            if (_serial.BufferIndex > 0)
                            {
                                return true;
                            }
                            Thread.Sleep(50);
                        }
                        return false;
                    });

                    bool hasResponse = await responseTask;
                    if (hasResponse && _baseService.fCheckFCS(_serial.ReceiveBuffer))
                    {
                        return true;
                    }
                }

                if (attempt < maxRetries)
                {
                    RetryAttempted?.Invoke(this, new HdlcRetryEventArgs
                    {
                        Attempt = attempt,
                        MaxRetries = maxRetries,
                        Operation = "SendFrame",
                        LastException = new Exception("Frame send failed or invalid FCS")
                    });

                    // Exponential backoff
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                }
            }
            catch (Exception ex)
            {
                if (attempt < maxRetries)
                {
                    RetryAttempted?.Invoke(this, new HdlcRetryEventArgs
                    {
                        Attempt = attempt,
                        MaxRetries = maxRetries,
                        Operation = "SendFrame",
                        LastException = ex
                    });

                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                }
                else
                {
                    throw;
                }
            }
        }

        return false;
    }

    public async Task<bool> SendSNRMWithRetryAsync(int srv, int mac, int cli, int cb, int cbR, int w, int wR, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                bool success = _baseService.fSendSNRM(srv, mac, cli, cb, cbR, w, wR);
                if (success)
                {
                    return true;
                }

                if (attempt < maxRetries)
                {
                    RetryAttempted?.Invoke(this, new HdlcRetryEventArgs
                    {
                        Attempt = attempt,
                        MaxRetries = maxRetries,
                        Operation = "SendSNRM",
                        LastException = new Exception("SNRM negotiation failed")
                    });

                    await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt));
                }
            }
            catch (Exception ex)
            {
                if (attempt < maxRetries)
                {
                    RetryAttempted?.Invoke(this, new HdlcRetryEventArgs
                    {
                        Attempt = attempt,
                        MaxRetries = maxRetries,
                        Operation = "SendSNRM",
                        LastException = ex
                    });

                    await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt));
                }
                else
                {
                    throw;
                }
            }
        }

        return false;
    }

    public async Task<bool> SendDISCWithRetryAsync(int srv, int mac, int cli, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                bool success = _baseService.fSendDISC(srv, mac, cli);
                if (success)
                {
                    return true;
                }

                if (attempt < maxRetries)
                {
                    RetryAttempted?.Invoke(this, new HdlcRetryEventArgs
                    {
                        Attempt = attempt,
                        MaxRetries = maxRetries,
                        Operation = "SendDISC",
                        LastException = new Exception("DISC command failed")
                    });

                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                }
            }
            catch (Exception ex)
            {
                if (attempt < maxRetries)
                {
                    RetryAttempted?.Invoke(this, new HdlcRetryEventArgs
                    {
                        Attempt = attempt,
                        MaxRetries = maxRetries,
                        Operation = "SendDISC",
                        LastException = ex
                    });

                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                }
                else
                {
                    throw;
                }
            }
        }

        return false;
    }

    // Delegate all other methods to base service
    public int fAdd7E(byte[] b, int i) => _baseService.fAdd7E(b, i);
    public int fAddHDLCFrameTag(byte[] b, int i) => _baseService.fAddHDLCFrameTag(b, i);
    public int fAddServerSAP(byte[] b, int i, int srv, int mac) => _baseService.fAddServerSAP(b, i, srv, mac);
    public int fAddClientSAP(byte[] b, int i, int cli) => _baseService.fAddClientSAP(b, i, cli);
    public void fIncSend() => _baseService.fIncSend();
    public int fAddCmdByte(byte[] b, int i) => _baseService.fAddCmdByte(b, i);
    public int fAddBlankFCS(byte[] b, int i) => _baseService.fAddBlankFCS(b, i);
    public void ffillLength(byte[] b, int idx) => _baseService.ffillLength(b, idx);
    public void fGenerateFCS(byte[] b, int s, int e) => _baseService.fGenerateFCS(b, s, e);
    public void fFillFCS(byte[] b, int p1, int p2) => _baseService.fFillFCS(b, p1, p2);
    public int FillWriteParameters(byte[] b, int i, List<byte> data) => _baseService.FillWriteParameters(b, i, data);
    public bool fCheckStartEndTag(byte[] b) => _baseService.fCheckStartEndTag(b);
    public bool fCheckFCS(byte[] b) => _baseService.fCheckFCS(b);
    public bool fCheckServerSAP(byte[] b, int cli) => _baseService.fCheckServerSAP(b, cli);
    public bool fCheckCommand(byte[] b, byte e) => _baseService.fCheckCommand(b, e);
    public void fIncRecieve() => _baseService.fIncRecieve();
    public bool fSendSNRM(int srv, int mac, int cli, int cb, int cbR, int w, int wR) => _baseService.fSendSNRM(srv, mac, cli, cb, cbR, w, wR);
    public bool fSendDISC(int srv, int mac, int cli) => _baseService.fSendDISC(srv, mac, cli);
    public bool IrDACheckBCC(byte[] b) => _baseService.IrDACheckBCC(b);
    public bool IrDACheckSyncWord(byte[] b) => _baseService.IrDACheckSyncWord(b);
    public bool IrDACheckCommandID(byte[] b, byte c) => _baseService.IrDACheckCommandID(b, c);
    public bool IrDACheckBCC_1P(byte[] b, int l) => _baseService.IrDACheckBCC_1P(b, l);
    public bool IrDACheckSyncWord_1P(byte[] b, int l) => _baseService.IrDACheckSyncWord_1P(b, l);
    
    // Block transfer methods - delegate to base service
    public byte[] CreateGetRequest(byte[] obisCode) => _baseService.CreateGetRequest(obisCode);
    public byte[] CreateGetNextRequest(uint blockIndex) => _baseService.CreateGetNextRequest(blockIndex);
    
    public static ushort FCS16(byte[] d, int s, int n) => HdlcFrameService.FCS16(d, s, n);
}
