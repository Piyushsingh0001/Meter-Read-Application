using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class MeterReadBackgroundService : IMeterReadBackgroundService
{
    private readonly IMeterCommunicationFacade _meterFacade;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isReading;
    private MeterReadResult _lastResult = new();

    public event EventHandler<StatusEventArgs> StatusUpdated = delegate { };
    public event EventHandler<MeterReadResult> ReadCompleted = delegate { };

    public bool IsReading => _isReading;
    public MeterReadResult LastResult => _lastResult;

    public MeterReadBackgroundService(IMeterCommunicationFacade meterFacade)
    {
        _meterFacade = meterFacade;
    }

    public async Task<bool> StartReadAllAsync(MeterReadRequest request)
    {
        if (_isReading)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs("Read operation already in progress.", false));
            return false;
        }

        _isReading = true;
        StatusUpdated?.Invoke(this, new StatusEventArgs("Starting background read-all operation...", false));

        try
        {
            // Execute ReadAll with cancellation support
            _lastResult = await _meterFacade.ReadAsync(new MeterReadRequest
            {
                Feature = MeterReadFeature.ReadAll,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                FromEntry = request.FromEntry,
                ToEntry = request.ToEntry
            });

            if (_lastResult.IsSuccess)
            {
                StatusUpdated?.Invoke(this, new StatusEventArgs("Read-all operation completed successfully.", false));
            }
            else
            {
                StatusUpdated?.Invoke(this, new StatusEventArgs($"Read-all operation failed: {_lastResult.Message}", true));
            }

            ReadCompleted?.Invoke(this, _lastResult);
            return _lastResult.IsSuccess;
        }
        catch (OperationCanceledException)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs("Read-all operation was cancelled.", false));
            return false;
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Read-all operation error: {ex.Message}", true));
            return false;
        }
        finally
        {
            _isReading = false;
        }
    }

    public async Task StopReadAllAsync()
    {
        if (!_isReading) return;

        StatusUpdated?.Invoke(this, new StatusEventArgs("Stopping read-all operation...", false));
        
        try
        {
            _cancellationTokenSource.Cancel();
            await Task.Delay(100); // Give time for cancellation to propagate
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Error stopping operation: {ex.Message}", true));
        }
        finally
        {
            _isReading = false;
        }
    }
}
