using System.Security.Cryptography;
using CabconMAUI.Helpers;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class OtaUpdateService : IOtaUpdateService
{
    private readonly IDlmsService _dlms;
    private readonly ISerialPortService _serial;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isUpdateInProgress;

    public event EventHandler<StatusEventArgs> StatusUpdated = delegate { };
    public event EventHandler<OtaProgressEventArgs> ProgressUpdated = delegate { };

    public bool IsUpdateInProgress => _isUpdateInProgress;

    // OBIS codes for firmware update
    private static readonly byte[] ImageTransferObject = { 0x00, 0x00, 0x60, 0x02, 0x00, 0xFF }; // Image transfer
    private static readonly byte[] ImageActivateObject = { 0x00, 0x00, 0x60, 0x02, 0x01, 0xFF }; // Image activation

    public OtaUpdateService(IDlmsService dlms, ISerialPortService serial)
    {
        _dlms = dlms;
        _serial = serial;
    }

    public async Task<OtaUpdateResult> StartFirmwareUpdateAsync(string firmwareFilePath, OtaUpdateOptions options)
    {
        if (_isUpdateInProgress)
        {
            return new OtaUpdateResult
            {
                IsSuccess = false,
                Message = "Firmware update already in progress."
            };
        }

        _isUpdateInProgress = true;
        var startTime = DateTime.Now;
        var result = new OtaUpdateResult();

        try
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs("Starting firmware update...", false));

            // Verify firmware file exists
            if (!File.Exists(firmwareFilePath))
            {
                result.IsSuccess = false;
                result.Message = "Firmware file not found.";
                return result;
            }

            // Read firmware file
            byte[] firmwareData = await File.ReadAllBytesAsync(firmwareFilePath);
            result.TotalBlocks = (int)Math.Ceiling((double)firmwareData.Length / options.BlockSize);

            StatusUpdated?.Invoke(this, new StatusEventArgs(
                $"Firmware size: {firmwareData.Length} bytes, Blocks: {result.TotalBlocks}", false));

            // Step 1: Initialize image transfer
            if (!await InitializeImageTransfer())
            {
                result.IsSuccess = false;
                result.Message = "Failed to initialize image transfer.";
                return result;
            }

            // Step 2: Transfer image blocks
            result.SuccessfulBlocks = await TransferImageBlocks(firmwareData, options, result);
            
            // Step 3: Verify transfer if requested
            if (options.VerifyAfterTransfer && result.SuccessfulBlocks == result.TotalBlocks)
            {
                StatusUpdated?.Invoke(this, new StatusEventArgs("Verifying firmware transfer...", false));
                bool verificationSuccess = await VerifyImageTransfer(firmwareData.Length);
                
                if (!verificationSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Firmware transfer verification failed.";
                    return result;
                }
            }

            // Step 4: Activate firmware
            if (result.SuccessfulBlocks == result.TotalBlocks)
            {
                StatusUpdated?.Invoke(this, new StatusEventArgs("Activating firmware...", false));
                bool activationSuccess = await ActivateFirmware();
                
                if (!activationSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Firmware activation failed.";
                    return result;
                }
            }

            result.IsSuccess = result.SuccessfulBlocks == result.TotalBlocks;
            result.Message = result.IsSuccess 
                ? "Firmware update completed successfully." 
                : $"Firmware update partially completed. {result.SuccessfulBlocks}/{result.TotalBlocks} blocks transferred.";
            result.Duration = DateTime.Now - startTime;

            StatusUpdated?.Invoke(this, new StatusEventArgs(result.Message, !result.IsSuccess));
            return result;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Firmware update error: {ex.Message}";
            result.Duration = DateTime.Now - startTime;
            StatusUpdated?.Invoke(this, new StatusEventArgs(result.Message, true));
            return result;
        }
        finally
        {
            _isUpdateInProgress = false;
        }
    }

    public async Task<OtaUpdateResult> VerifyFirmwareAsync(string firmwareFilePath)
    {
        try
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs("Verifying firmware file...", false));

            if (!File.Exists(firmwareFilePath))
            {
                return new OtaUpdateResult
                {
                    IsSuccess = false,
                    Message = "Firmware file not found."
                };
            }

            byte[] firmwareData = await File.ReadAllBytesAsync(firmwareFilePath);
            
            // Basic firmware validation
            if (firmwareData.Length < 100) // Minimum firmware size check
            {
                return new OtaUpdateResult
                {
                    IsSuccess = false,
                    Message = "Firmware file appears to be invalid (too small)."
                };
            }

            // Calculate checksum
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(firmwareData);
            string checksum = Convert.ToHexString(hash).ToLowerInvariant();

            StatusUpdated?.Invoke(this, new StatusEventArgs(
                $"Firmware verified. Size: {firmwareData.Length} bytes, SHA256: {checksum[..16]}...", false));

            return new OtaUpdateResult
            {
                IsSuccess = true,
                Message = "Firmware file validation successful.",
                TotalBlocks = (int)Math.Ceiling((double)firmwareData.Length / 256)
            };
        }
        catch (Exception ex)
        {
            return new OtaUpdateResult
            {
                IsSuccess = false,
                Message = $"Firmware verification error: {ex.Message}"
            };
        }
    }

    public async Task CancelUpdateAsync()
    {
        if (!_isUpdateInProgress) return;

        StatusUpdated?.Invoke(this, new StatusEventArgs("Cancelling firmware update...", false));
        _cancellationTokenSource.Cancel();
        
        try
        {
            await Task.Delay(100); // Allow cancellation to propagate
            _isUpdateInProgress = false;
            StatusUpdated?.Invoke(this, new StatusEventArgs("Firmware update cancelled.", false));
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Error cancelling update: {ex.Message}", true));
        }
    }

    private async Task<bool> InitializeImageTransfer()
    {
        try
        {
            // Send image transfer initialization command
            var initParams = new List<byte> { 0x01 }; // Initialize transfer
            var responseType = new byte[] { 0x00, 0x00 };

            bool success = await _dlms.WriteMethodToMeterAsync(
                0x01, // Method ID for Initialize
                ImageTransferObject,
                0x08, // Class ID for Image Transfer
                0x02, // Data type
                0x01, // Data length
                initParams,
                responseType,
                0x00 // Access selector
            );

            StatusUpdated?.Invoke(this, new StatusEventArgs(
                success ? "Image transfer initialized." : "Failed to initialize image transfer.", !success));
            
            return success;
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Image transfer initialization error: {ex.Message}", true));
            return false;
        }
    }

    private async Task<int> TransferImageBlocks(byte[] firmwareData, OtaUpdateOptions options, OtaUpdateResult result)
    {
        int successfulBlocks = 0;
        int totalBlocks = (int)Math.Ceiling((double)firmwareData.Length / options.BlockSize);

        for (int blockNumber = 0; blockNumber < totalBlocks; blockNumber++)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                StatusUpdated?.Invoke(this, new StatusEventArgs("Transfer cancelled by user.", false));
                break;
            }

            int offset = blockNumber * options.BlockSize;
            int blockSize = Math.Min(options.BlockSize, firmwareData.Length - offset);
            byte[] blockData = new byte[blockSize];
            Array.Copy(firmwareData, offset, blockData, 0, blockSize);

            ProgressUpdated?.Invoke(this, new OtaProgressEventArgs
            {
                CurrentBlock = blockNumber + 1,
                TotalBlocks = totalBlocks,
                CurrentOperation = $"Transferring block {blockNumber + 1}/{totalBlocks}"
            });

            bool blockSuccess = await TransferImageBlock(blockNumber, blockData, options);
            
            if (blockSuccess)
            {
                successfulBlocks++;
            }
            else
            {
                result.Warnings.Add($"Failed to transfer block {blockNumber + 1}");
                
                // Retry logic
                bool retrySuccess = false;
                for (int retry = 0; retry < options.MaxRetries && !retrySuccess; retry++)
                {
                    StatusUpdated?.Invoke(this, new StatusEventArgs(
                        $"Retrying block {blockNumber + 1}, attempt {retry + 1}/{options.MaxRetries}", false));
                    
                    await Task.Delay(1000 * (retry + 1)); // Exponential backoff
                    retrySuccess = await TransferImageBlock(blockNumber, blockData, options);
                }

                if (retrySuccess)
                {
                    successfulBlocks++;
                    result.Warnings.RemoveAt(result.Warnings.Count - 1); // Remove warning if retry succeeded
                }
            }

            // Small delay between blocks to prevent overwhelming the meter
            await Task.Delay(100);
        }

        return successfulBlocks;
    }

    private async Task<bool> TransferImageBlock(int blockNumber, byte[] blockData, OtaUpdateOptions options)
    {
        try
        {
            var dataLength = (ushort)blockData.Length;
            var blockHeader = new List<byte>
            {
                (byte)(blockNumber & 0xFF),
                (byte)((blockNumber >> 8) & 0xFF),
                (byte)(dataLength & 0xFF),
                (byte)((dataLength >> 8) & 0xFF)
            };

            var fullBlockData = blockHeader.Concat(blockData).ToList();
            var responseType = new byte[] { 0x00, 0x00 };

            bool success = await _dlms.WriteImageBlockDataToMeterAsync(
                0x02, // Attribute ID for image block
                ImageTransferObject,
                0x08, // Class ID for Image Transfer
                0x02, // Data type
                fullBlockData.Count,
                fullBlockData,
                responseType,
                new List<byte>(), // CRC will be calculated by meter
                new List<byte>()  // Footer
            );

            return success;
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs(
                $"Block transfer error: {ex.Message}", true));
            return false;
        }
    }

    private async Task<bool> VerifyImageTransfer(int expectedSize)
    {
        try
        {
            // Read image transfer status to verify
            bool success = await _dlms.ReadByteFromMeterAsync(
                ImageTransferObject,
                0x08, // Class ID
                0x02  // Attribute for transfer status
            );

            if (success && _serial.ReceiveBuffer.Length > 4)
            {
                // Parse response to check if transfer is complete and size matches
                int transferredSize = (_serial.ReceiveBuffer[1] << 24) | 
                                  (_serial.ReceiveBuffer[2] << 16) | 
                                  (_serial.ReceiveBuffer[3] << 8) | 
                                  _serial.ReceiveBuffer[4];

                bool sizeMatches = transferredSize == expectedSize;
                StatusUpdated?.Invoke(this, new StatusEventArgs(
                    sizeMatches ? "Image transfer verified." : $"Size mismatch: expected {expectedSize}, got {transferredSize}", !sizeMatches));
                
                return sizeMatches;
            }

            return false;
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Verification error: {ex.Message}", true));
            return false;
        }
    }

    private async Task<bool> ActivateFirmware()
    {
        try
        {
            var activateParams = new List<byte> { 0x01 }; // Activate command
            var responseType = new byte[] { 0x00, 0x00 };

            bool success = await _dlms.WriteMethodToMeterAsync(
                0x02, // Method ID for Activate
                ImageActivateObject,
                0x08, // Class ID for Image Transfer
                0x02, // Data type
                0x01, // Data length
                activateParams,
                responseType,
                0x00 // Access selector
            );

            StatusUpdated?.Invoke(this, new StatusEventArgs(
                success ? "Firmware activated successfully." : "Firmware activation failed.", !success));
            
            return success;
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke(this, new StatusEventArgs($"Activation error: {ex.Message}", true));
            return false;
        }
    }
}
