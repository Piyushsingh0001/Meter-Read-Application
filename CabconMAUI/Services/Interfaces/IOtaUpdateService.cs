using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface IOtaUpdateService
{
    event EventHandler<StatusEventArgs> StatusUpdated;
    event EventHandler<OtaProgressEventArgs> ProgressUpdated;
    
    Task<OtaUpdateResult> StartFirmwareUpdateAsync(string firmwareFilePath, OtaUpdateOptions options);
    Task<OtaUpdateResult> VerifyFirmwareAsync(string firmwareFilePath);
    Task CancelUpdateAsync();
    bool IsUpdateInProgress { get; }
}

public class OtaUpdateOptions
{
    public int BlockSize { get; set; } = 256; // Default block size
    public int MaxRetries { get; set; } = 3;
    public bool VerifyAfterTransfer { get; set; } = true;
    public TimeSpan TimeoutPerBlock { get; set; } = TimeSpan.FromSeconds(30);
}

public class OtaUpdateResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalBlocks { get; set; }
    public int SuccessfulBlocks { get; set; }
    public TimeSpan Duration { get; set; }
    public string? FirmwareVersion { get; set; }
    public List<string> Warnings { get; set; } = new();
}

public class OtaProgressEventArgs : EventArgs
{
    public int CurrentBlock { get; set; }
    public int TotalBlocks { get; set; }
    public int ProgressPercentage => TotalBlocks > 0 ? (int)((double)CurrentBlock / TotalBlocks * 100) : 0;
    public string CurrentOperation { get; set; } = string.Empty;
}
