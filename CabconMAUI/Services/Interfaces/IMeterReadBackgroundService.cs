using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface IMeterReadBackgroundService
{
    event EventHandler<StatusEventArgs> StatusUpdated;
    event EventHandler<MeterReadResult> ReadCompleted;
    
    Task<bool> StartReadAllAsync(MeterReadRequest request);
    Task StopReadAllAsync();
    bool IsReading { get; }
    MeterReadResult LastResult { get; }
}
