using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services.Interfaces;

public interface IHdlcFrameServiceWithRetry : IHdlcFrameService
{
    Task<bool> SendFrameWithRetryAsync(byte[] frame, int maxRetries = 3, TimeSpan? timeout = null);
    Task<bool> SendSNRMWithRetryAsync(int srv, int mac, int cli, int cb, int cbR, int w, int wR, int maxRetries = 3);
    Task<bool> SendDISCWithRetryAsync(int srv, int mac, int cli, int maxRetries = 3);
    event EventHandler<HdlcRetryEventArgs>? RetryAttempted;
}

public class HdlcRetryEventArgs : EventArgs
{
    public int Attempt { get; set; }
    public int MaxRetries { get; set; }
    public string Operation { get; set; } = string.Empty;
    public Exception? LastException { get; set; }
}
