# Quick Start Guide - Configuration System

## What's New?

Your CabconMAUI Android application now has a professional configuration system with:

✅ **JSON-based configuration files** - Easy to edit settings without recompiling
✅ **Environment-specific configs** - Different settings for Development and Production
✅ **Type-safe configuration loading** - Strongly-typed configuration classes
✅ **Embedded resource loading** - Configuration files are compiled into the app
✅ **Automatic environment detection** - Switches between configs based on build type

## Files Added/Updated

### New Files Created:
- `appsettings.json` - Development configuration
- `appsettings.Production.json` - Production configuration  
- `Services/JsonConfigurationService.cs` - Configuration loading service
- `Services/Interfaces/IConfigurationService.cs` - Configuration interface
- `Models/Configuration/ConfigurationModels.cs` - Configuration data classes

### Files Updated:
- `MauiProgram.cs` - Registered configuration service

## How to Use

### In Your Services/ViewModels:

```csharp
// 1. Inject the configuration service
public class MyService
{
    private readonly IConfigurationService _config;

    public MyService(IConfigurationService config)
    {
        _config = config;
    }

    // 2. Load configuration
    public async Task InitAsync()
    {
        var serialSettings = await _config.LoadConfiguration<SerialPortSettings>(
            "SerialPortSettings");
        
        // Now use the settings
        var baudRate = serialSettings.CommandBaudRate;  // "9600"
        var timeout = serialSettings.CommandTimeOut;    // 3500
    }
}
```

## Configuration Sections Available

| Section | Purpose |
|---------|---------|
| `SerialPortSettings` | Serial port communication parameters |
| `HDLCSettings` | HDLC protocol configuration |
| `COSEMSettings` | Energy metering protocol settings |
| `SecuritySettings` | Encryption and security keys |
| `AssociationSettings` | DLMS association parameters |
| `MeterSettings` | Meter-specific configuration |
| `ApplicationSettings` | General app settings (Environment, LogLevel, Debug) |

## Build Configuration

| Build Type | Config File Used | Debug Mode |
|-----------|------------------|-----------|
| Debug | `appsettings.json` | Enabled |
| Release | `appsettings.Production.json` | Disabled |

## Important Notes

1. **Mark as Embedded Resource**: The JSON files must be embedded in the project
   - They're already configured correctly as embedded resources
   
2. **Modify Settings**: Edit the `.json` files directly with a text editor
   - No code changes needed for most configuration updates

3. **Type Safety**: Configuration is loaded into strongly-typed C# classes
   - Get intellisense support and compile-time type checking

4. **Environment-Specific**: Production build automatically uses different config
   - Perfect for deploying to different environments

## Example: Loading Serial Port Settings

```csharp
var configService = MauiApplication.Current!.Services.GetService<IConfigurationService>()!;

// Load entire section
var serialSettings = await configService.LoadConfiguration<SerialPortSettings>(
    "SerialPortSettings");

Console.WriteLine($"Port: {serialSettings.SerialPort}");
Console.WriteLine($"Baud Rate: {serialSettings.CommandBaudRate}");
Console.WriteLine($"Timeout: {serialSettings.CommandTimeOut}ms");

// Or get specific value
var baudRate = configService.GetValue<string>(
    "SerialPortSettings", "CommandBaudRate");
```

## Common Tasks

### Change Serial Port Settings
Edit `appsettings.json`:
```json
"SerialPortSettings": {
  "SerialPort": "COM3",
  "CommandBaudRate": "19200"
}
```

### Enable/Disable Debug Mode
Edit `appsettings.json`:
```json
"ApplicationSettings": {
  "EnableDebugMode": true
}
```

### Add New Configuration
1. Add class to `Models/Configuration/ConfigurationModels.cs`
2. Add section to `appsettings.json` and `appsettings.Production.json`
3. Load with: `await _configService.LoadConfiguration<YourClass>("SectionName")`

## Troubleshooting

**Q: Configuration not loading?**
- Ensure JSON files are valid (use online JSON validator)
- Check section names match exactly (case-insensitive in code, but case-sensitive in JSON)
- Rebuild the solution

**Q: Different settings needed for different devices?**
- Modify `appsettings.json` for each build
- Or add new environment-specific config files

**Q: How to use in Android-specific code?**
- Same way! `IConfigurationService` works across all platforms
- Platform-specific logic can read from configuration

## Next Steps

1. ✅ Review your `appsettings.json` file and adjust values as needed
2. ✅ Inject `IConfigurationService` into your services/viewmodels
3. ✅ Load configuration on app startup or service initialization
4. ✅ Replace any hardcoded values with configuration values
5. ✅ Create `appsettings.Development.json` if needed for different developer setups

For detailed documentation, see: **CONFIGURATION_README.md**
