namespace CabconMAUI.Services.Interfaces;

public interface IStatusLogger
{
    event Action<string> OnMessageLogged;
    void Log(string message);
}