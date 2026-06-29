using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class StatusLogger : IStatusLogger
{
    public event Action<string>? OnMessageLogged;

    public void Log(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            OnMessageLogged?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        });
    }
}