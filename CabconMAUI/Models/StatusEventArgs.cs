namespace CabconMAUI.Models;
public class StatusEventArgs : EventArgs
{
    public string Message { get; init; } = string.Empty;
    public bool   IsError { get; init; }
    public StatusEventArgs(string message, bool isError) { Message=message; IsError=isError; }
}
