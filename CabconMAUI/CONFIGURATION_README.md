# Configuration System Documentation

## Overview
This Android application now includes a comprehensive configuration system using JSON configuration files. This allows you to manage application settings without recompiling the code.

## Configuration Files

### 1. **appsettings.json** (Development)
- Default configuration file used during development
- Contains all settings with development-appropriate values
- Used when `DEBUG` directive is active

### 2. **appsettings.Production.json** (Production)
- Environment-specific configuration for production deployments
- Lower logging levels and debug features disabled
- Automatically used when the app is built in Release mode

## Configuration Sections

### SerialPortSettings
Configures serial port communication parameters:
- `SerialPort`: COM port name (e.g., "COM1")
- `SignOnBaudRate`: Initial baud rate for sign-on
- `CommandBaudRate`: Baud rate for commands
- `StopBits`, `DataBits`, `Parity`: Serial frame format
- `CommandTimeOut`: Timeout in milliseconds for commands
- `IntercharacterDelay`: Delay between characters
- `InterframeTimeout`: Timeout between frames

### HDLCSettings
High-level Data Link Control (HDLC) protocol settings:
- `ServerSAP`: Server Service Access Point
- `ServerLowerMacAddress`: Server MAC address
- `ClientSAP`: Client Service Access Point
- `AddressingScheme`: HDLC addressing mode
- `ServerPhysicalID`: Physical ID of server
- `HDLCAddressing`: HDLC addressing type

### COSEMSettings
Companion Specification for Energy Metering settings:
- `ApplicationContext`: Application context type
- `SecurityMechanism`: Type of security mechanism
- `PDUSize`: Protocol Data Unit size
- `ConformanceBlock`: Conformance block identifier
- `DLMSVersion`: DLMS version number
- `InformationSize`: Size of information block
- `CosemBufferSize`: Buffer size for COSEM
- `DLLBufferSize`: Data Link Layer buffer size

### SecuritySettings
Security and encryption parameters:
- `Password`: Default password
- `HLSKey`: High-Level Security key
- `HLSPWD`: HLS password
- `ClientSystemTitle`: System title for client
- `SecuritySuite`: Security suite identifier
- `GlobalEncryptionKey`: Global encryption key
- `AuthenticationKey`: Authentication key
- `AESEncryption`: AES encryption mode

### AssociationSettings
DLMS Association parameters:
- `AssociationMode`: Mode of association
- `AssociationType`: Type of association
- `AssociationAccess`: Access credentials

### MeterSettings
Meter-specific configuration:
- `MeterMode`: Operating mode of the meter
- `ScaleXMLPath`: Path to scale XML file
- `ReadOut`: Read-out configuration

### ApplicationSettings
General application settings:
- `Environment`: "Development" or "Production"
- `LogLevel`: Logging verbosity (Debug, Information, Warning, Error)
- `EnableDebugMode`: Enable/disable debug features

## Usage in Code

### Option 1: Inject IConfigurationService
```csharp
public class MyService
{
    private readonly IConfigurationService _configService;

    public MyService(IConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task InitializeAsync()
    {
        // Load entire configuration section
        var serialSettings = await _configService.LoadConfiguration<SerialPortSettings>(
            "SerialPortSettings");

        // Get specific value
        var baudRate = _configService.GetValue<string>(
            "SerialPortSettings", "CommandBaudRate");
    }
}
```

### Option 2: Use in Views/ViewModels
```csharp
public class SettingsViewModel : BaseViewModel
{
    private readonly IConfigurationService _configService;

    public SettingsViewModel(IConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task LoadSettingsAsync()
    {
        var appSettings = await _configService.LoadConfiguration<ApplicationSettings>(
            "ApplicationSettings");
        
        IsDebugModeEnabled = appSettings.EnableDebugMode;
        CurrentEnvironment = appSettings.Environment;
    }
}
```

## Environment-Based Configuration

The configuration system automatically detects the build configuration:

```csharp
public string GetEnvironment() => _environment;

private static string GetCurrentEnvironment()
{
#if DEBUG
    return "Development";  // Uses appsettings.json
#else
    return "Production";   // Uses appsettings.Production.json
#endif
}
```

## Adding New Configuration Sections

1. **Create a new class in `Models/Configuration/ConfigurationModels.cs`:**
```csharp
public class MyNewSettings
{
    public string SomeSetting { get; set; } = "default";
    public int AnotherSetting { get; set; } = 100;
}
```

2. **Add the section to `appsettings.json` and `appsettings.Production.json`:**
```json
{
  "MyNewSettings": {
    "SomeSetting": "value",
    "AnotherSetting": 200
  }
}
```

3. **Load in your code:**
```csharp
var mySettings = await _configService.LoadConfiguration<MyNewSettings>("MyNewSettings");
```

## Best Practices

1. **Never hardcode values** - Use configuration files instead
2. **Environment-specific settings** - Override sensitive settings in Production config
3. **Type-safe loading** - Always use strongly-typed configuration classes
4. **Error handling** - Configuration loading can throw exceptions, handle appropriately
5. **Lazy loading** - Configuration is loaded on-demand, cache the results if needed
6. **Security** - Don't commit production secrets to version control; use secure storage for sensitive keys

## Migrating Existing Code

To migrate existing hardcoded settings:

1. Move values to `appsettings.json`
2. Inject `IConfigurationService` into your service/viewmodel
3. Load configuration during initialization
4. Remove hardcoded values

Example:
```csharp
// Before
var baudRate = "9600";

// After
var baudRate = await _configService.LoadConfiguration<SerialPortSettings>(
    "SerialPortSettings").Result.CommandBaudRate;
```

## Troubleshooting

### Configuration file not found
- Ensure `appsettings.json` is marked as "Embedded Resource" in project properties
- Check namespace matches: `CabconMAUI.appsettings.json`

### Configuration values not loading
- Verify property names match exactly (case-insensitive but should match JSON)
- Ensure JSON is valid (use JSON validator)
- Check the section name matches exactly

### Environment not switching
- Verify build configuration (Debug/Release)
- Check `#if DEBUG` preprocessor directives
- Ensure both config files exist
