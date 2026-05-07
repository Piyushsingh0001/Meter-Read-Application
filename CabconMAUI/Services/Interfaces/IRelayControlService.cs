using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface IRelayControlService
{
    event EventHandler<StatusEventArgs> StatusUpdated;
    
    Task<bool> GetRelayStatusAsync();
    Task<bool> ConnectRelayAsync();
    Task<bool> DisconnectRelayAsync();
    Task<RelayControlResult> SetRelayStateAsync(bool connect);
}

public class RelayControlResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool CurrentState { get; set; } // true = connected, false = disconnected
    public DateTime Timestamp { get; set; }
}
