using CabconMAUI.Helpers;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class RelayControlService : IRelayControlService
{
    private readonly IDlmsService _dlms;
    private readonly ISerialPortService _serial;

    public event EventHandler<StatusEventArgs> StatusUpdated = delegate { };

    public RelayControlService(IDlmsService dlms, ISerialPortService serial)
    {
        _dlms = dlms;
        _serial = serial;
    }

    public async Task<bool> GetRelayStatusAsync()
    {
        try
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs("Reading relay status...", false));
            
            // Read the current state of the Connect Control Object
            bool success = await _dlms.ReadByteFromMeterAsync(
                DlmsHelper.ObisCode.ConnectControlObject, 
                0x02, // Class ID for Register
                0x02  // Attribute ID for value
            );

            if (!success)
            {
                StatusUpdated?.Invoke(this, new StatusEventArgs("Failed to read relay status.", true));
                return false;
            }

            // Parse the response - 0 = disconnected, 1 = connected
            var formatted = _dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer, 18, false);
            if (formatted?.Length > 0)
            {
                bool isRelayConnected = formatted[0] == "1";
                StatusUpdated?.Invoke(this, new StatusEventArgs(
                    $"Relay status: {(isRelayConnected ? "Connected" : "Disconnected")}", 
                    false));
                return isRelayConnected;
            }

            StatusUpdated?.Invoke(this, new StatusEventArgs("Unable to parse relay status response.", true));
            return false;
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Error reading relay status: {ex.Message}", true));
            return false;
        }
    }

    public async Task<bool> ConnectRelayAsync()
    {
        var result = await SetRelayStateAsync(true);
        return result.IsSuccess;
    }

    public async Task<bool> DisconnectRelayAsync()
    {
        var result = await SetRelayStateAsync(false);
        return result.IsSuccess;
    }

    public async Task<RelayControlResult> SetRelayStateAsync(bool connect)
    {
        var result = new RelayControlResult
        {
            Timestamp = DateTime.Now,
            CurrentState = !connect // Assume opposite for now, will update after command
        };

        try
        {
            string action = connect ? "Connecting" : "Disconnecting";
            StatusUpdated?.Invoke(this, new StatusEventArgs($"{action} relay...", false));

            // Create SET request with desired state
            var dataValue = new byte[] { connect ? (byte)1 : (byte)0 }; // Value: 1 or 0
            var dataType = 0x06; // Unsigned integer type
            var dataLength = 0x01; // Single byte
            var responseType = new byte[] { 0x00, 0x00 }; // Response type
            
            bool success = await _dlms.WriteDataToMeterAsync(
                0x02, // Attribute ID for value
                DlmsHelper.ObisCode.ConnectControlObject,
                0x02, // Class ID for Register
                (byte)dataType,
                (byte)dataLength,
                dataValue.ToList(),
                responseType
            );

            result.IsSuccess = success;
            result.CurrentState = connect;

            if (success)
            {
                result.Message = $"Relay {(connect ? "connected" : "disconnected")} successfully";
                StatusUpdated?.Invoke(this, new StatusEventArgs(result.Message, false));
            }
            else
            {
                result.Message = $"Failed to {(connect ? "connect" : "disconnect")} relay";
                StatusUpdated?.Invoke(this, new StatusEventArgs(result.Message, true));
            }

            return result;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error controlling relay: {ex.Message}";
            StatusUpdated?.Invoke(this, new StatusEventArgs(result.Message, true));
            return result;
        }
    }
}
