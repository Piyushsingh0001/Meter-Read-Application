using System.Text;
using System.Threading.Tasks;
using CabconMAUI.Helpers;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public sealed class MeterCommunicationFacade : IMeterCommunicationFacade
{
    readonly IDlmsService _dlms;
    readonly IIecMeterService _iec;
    readonly ISettingsService _settings;
    readonly ISerialPortService _serial;
    readonly IStatusLogger _logger;
    MeterProtocolFamily _activeProtocol;

    public MeterCommunicationFacade(IDlmsService dlms, IIecMeterService iec, ISettingsService settings, ISerialPortService serial, IStatusLogger logger)
    {
        _dlms = dlms;
        _iec = iec;
        _settings = settings;
        _serial = serial;
        _logger = logger;
    }

    public async Task<bool> ConnectAndAuthenticateAsync(MeterVariant selectedVariant)
    {
        _logger.Log($"Selected Variant: {selectedVariant.Name}");
        _serial.SetSerialPortSettings(
        _settings.CommunicationPort, // Use the port selected in UI
        _settings.CommandBaudRate,
        _settings.Parity,
        _settings.DataBits,
        _settings.StopBits,
        _settings.CommandTimeOut,
        _settings.IntercharacterDelay
    );

        // 1. Open the Physical Port First
        var portOpened = _serial.OpenPort();
        if (!portOpened)
        {
            _logger.Log("ERROR: Could not open Serial/USB Port.");
            return false;
        }

        // 2. Protocol Switching Logic (The "Desktop" Way)
        if (selectedVariant.IsSmart) // Smart Meter (DLMS)
        {
            _logger.Log("Mode: Smart Meter. Starting DLMS Handshake...");
            _activeProtocol = MeterProtocolFamily.Dlms1Phase;
            return await _dlms.ConnectToMeterAsync();
        }
        else // Non-Smart Meter (IEC 62056-21)
        {
            _logger.Log("Mode: Non-Smart. Starting IEC Sign-on...");
            _activeProtocol = MeterProtocolFamily.IecNonDlms;
            // This wakes up the meter using the "/?! <CR><LF>" command
            return await _iec.PerformSignOnAsync();
        }
    }

    public async Task<bool> ConnectToMeterAsync(MeterConnectRequest request)
    {
        _activeProtocol = request.ProtocolFamily;
        _settings.CommunicationMode = request.TransportMode.ToString();
        if (!string.IsNullOrWhiteSpace(request.PortName))
        {
            _settings.SerialPort = request.PortName;
        }

        // Use the new protocol switching logic instead of hardcoded protocol
        return await ConnectAndAuthenticateAsync(new MeterVariant
        {
            Name = "Manual Connection",
            Type = request.ProtocolFamily == MeterProtocolFamily.IecNonDlms ? MeterTypeInfo.Non_DLMS_1PH : MeterTypeInfo.Smart_Meter_1PH,
            Id = request.ProtocolFamily == MeterProtocolFamily.IecNonDlms ? 1 : 2
        });
    }

    public async Task DisconnectAsync()
    {
        if (_activeProtocol == MeterProtocolFamily.IecNonDlms)
        {
            await _iec.IECAssociationDisconnectAsync();
            await _iec.IECPortDisconnectAsync();
            return;
        }

        await _dlms.AssociationDisconnectAsync();
    }

    public async Task<MeterReadResult> ReadAsync(MeterReadRequest request)
    {
        return _activeProtocol == MeterProtocolFamily.IecNonDlms
            ? await ReadIecAsync(request)
            : await ReadDlmsAsync(request);
    }

    public async Task<MeterReadResult> ReadSingleObjectAsync(byte[] obis, byte classId, byte attributeId)
    {
        var ok = await _dlms.ReadByteFromMeterAsync(obis, classId, attributeId);
        return BuildDlmsResult(ok, "single-object");
    }

    public async Task<MeterReadResult> ReadBlockAsync(byte[] obis, byte classId, byte attributeId, byte accessSelector, List<byte> descriptor)
    {
        var ok = await _dlms.ReadBlockFromMeterAsync(obis, classId, attributeId, accessSelector, descriptor);
        return BuildDlmsResult(ok, "block-read");
    }

    async Task<MeterReadResult> ReadDlmsAsync(MeterReadRequest request)
    {
        var result = request.Feature switch
        {
            MeterReadFeature.Instantaneous => await ReadInstantaneousAsync(),
            MeterReadFeature.Billing => await ReadProfileAsync("billing", DlmsHelper.ObisCode.BillingProfile, request),
            MeterReadFeature.LoadSurvey => await ReadProfileAsync("load-survey", DlmsHelper.ObisCode.LoadProfileInstant, request),
            MeterReadFeature.DailyProfile => await ReadProfileAsync("daily-profile", DlmsHelper.ObisCode.DailyProfile, request),
            MeterReadFeature.Tamper => await ReadProfileAsync("tamper", DlmsHelper.ObisCode.TamperProfile, request),
            MeterReadFeature.Nameplate => await ReadSingleObjectAsync(DlmsHelper.ObisCode.BuildVersionBytes, 0x01, 0x02),
            MeterReadFeature.ReadAll => await ReadAllAsync(request),
            _ => new MeterReadResult { IsSuccess = false, Message = "Unsupported feature." }
        };

        return result;
    }

    async Task<MeterReadResult> ReadIecAsync(MeterReadRequest request)
    {
        string tag = request.Feature switch
        {
            MeterReadFeature.Instantaneous => "IECReadoutAssociation",
            MeterReadFeature.Billing => "Billing",
            MeterReadFeature.LoadSurvey => "LoadSurvey",
            MeterReadFeature.DailyProfile => "Daily",
            MeterReadFeature.Tamper => "Tamper",
            MeterReadFeature.Nameplate => "MeterSignon",
            MeterReadFeature.ReadAll => "IECReadoutAssociation",
            _ => "IECReadoutAssociation"
        };

        var data = await _iec.ReadDataBufferAsync(tag);
        return new MeterReadResult
        {
            IsSuccess = !string.IsNullOrWhiteSpace(data),
            Message = string.IsNullOrWhiteSpace(data) ? "No IEC response." : "IEC read completed.",
            Source = "iec",
            Values = new Dictionary<string, string>
            {
                ["feature"] = request.Feature.ToString(),
                ["payload"] = data
            }
        };
    }

    async Task<MeterReadResult> ReadInstantaneousAsync()
    {
        var result = new MeterReadResult
        {
            Source = "dlms",
            Values = new Dictionary<string, string>()
        };

        var steps = new (string Name, byte[] Obis, byte Cls, byte Att)[]
        {
            ("meter-number", DlmsHelper.ObisCode.MeterNumberBytes, 0x01, 0x02),
            ("energy-import", DlmsHelper.ObisCode.EnergyImport, 0x03, 0x02),
            ("energy-export", DlmsHelper.ObisCode.EnergyExport, 0x03, 0x02),
            ("voltage-r", DlmsHelper.ObisCode.VoltageR, 0x03, 0x02),
            ("voltage-y", DlmsHelper.ObisCode.VoltageY, 0x03, 0x02),
            ("voltage-b", DlmsHelper.ObisCode.VoltageB, 0x03, 0x02),
            ("clock", DlmsHelper.ObisCode.Clock, 0x08, 0x02)
        };

        foreach (var step in steps)
        {
            var ok = await _dlms.ReadByteFromMeterAsync(step.Obis, step.Cls, step.Att);
            if (!ok)
            {
                result.IsSuccess = false;
                result.Message = $"Instantaneous read failed at {step.Name}.";
                return result;
            }

            var formatted = _dlms.DLMSDataFormatorLabView(_serial.ReceiveBuffer, 18, step.Name == "meter-number");
            result.Values[step.Name] = formatted?.FirstOrDefault() ?? string.Empty;
        }

        result.IsSuccess = true;
        result.Message = "Instantaneous read completed.";
        result.RawBuffer = _serial.ReceiveBuffer.ToArray();
        return result;
    }

    async Task<MeterReadResult> ReadProfileAsync(string name, byte[] obis, MeterReadRequest request)
    {
        List<byte> selectorDescriptor;
        byte selector;

        if (request.FromDate.HasValue && request.ToDate.HasValue)
        {
            selector = 1;
            selectorDescriptor = _dlms.GetByteByEntryDateType(request.FromDate.Value, request.ToDate.Value);
        }
        else if (request.FromEntry.HasValue && request.ToEntry.HasValue)
        {
            selector = 2;
            selectorDescriptor = _dlms.GetByteByEntryValueType(request.FromEntry.Value, request.ToEntry.Value);
        }
        else
        {
            selector = 0;
            selectorDescriptor = new List<byte>();
        }

        bool ok = selector == 0
            ? await _dlms.ReadByteFromMeterAsync(obis, 0x07, 0x02)
            : await _dlms.ReadBlockFromMeterAsync(obis, 0x07, 0x02, selector, selectorDescriptor);

        return BuildDlmsResult(ok, name);
    }

    async Task<MeterReadResult> ReadAllAsync(MeterReadRequest request)
    {
        var merged = new MeterReadResult
        {
            Source = "combined",
            IsSuccess = true,
            Message = "Read-all completed.",
            Values = new Dictionary<string, string>()
        };

        var features = new[]
        {
            MeterReadFeature.Instantaneous,
            MeterReadFeature.Billing,
            MeterReadFeature.LoadSurvey,
            MeterReadFeature.DailyProfile,
            MeterReadFeature.Tamper,
            MeterReadFeature.Nameplate
        };

        foreach (var feature in features)
        {
            var part = await ReadAsync(new MeterReadRequest
            {
                Feature = feature,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                FromEntry = request.FromEntry,
                ToEntry = request.ToEntry
            });

            if (!part.IsSuccess)
            {
                merged.IsSuccess = false;
                merged.Message = $"Read-all partial failure at {feature}.";
            }

            foreach (var kv in part.Values)
            {
                merged.Values[$"{feature}:{kv.Key}"] = kv.Value;
            }
        }

        return merged;
    }

    MeterReadResult BuildDlmsResult(bool ok, string source)
    {
        return new MeterReadResult
        {
            IsSuccess = ok,
            Source = source,
            Message = ok ? "Read completed." : "Read failed.",
            RawBuffer = _serial.ReceiveBuffer.ToArray(),
            Values = new Dictionary<string, string>
            {
                ["raw"] = DlmsHelper.ByteArrayToHexString(_serial.ReceiveBuffer, Math.Max(0, _serial.BufferIndex))
            }
        };
    }
}

