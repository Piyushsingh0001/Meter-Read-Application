using System.Text.Json;
using System.Text.Json.Serialization;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

/// <summary>Service for loading application configuration from embedded JSON resources.</summary>
public class JsonConfigurationService : IConfigurationService
{
    private JsonDocument? _configDocument;
    private readonly string _environment;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonConfigurationService()
    {
        _environment = GetCurrentEnvironment();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }

    /// <summary>Load configuration from embedded JSON resource.</summary>
    public async Task<T> LoadConfiguration<T>(string sectionName) where T : class
    {
        if (_configDocument == null)
        {
            await LoadConfigurationFile();
        }

        if (_configDocument == null)
            throw new InvalidOperationException("Configuration document is null.");

        var root = _configDocument.RootElement;
        if (root.TryGetProperty(sectionName, out var section))
        {
            var json = section.GetRawText();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions)
                ?? throw new InvalidOperationException($"Failed to deserialize configuration section '{sectionName}'.");
        }

        throw new KeyNotFoundException($"Configuration section '{sectionName}' not found.");
    }

    /// <summary>Get a specific configuration value by section and key.</summary>
    public T? GetValue<T>(string sectionName, string key)
    {
        if (_configDocument == null)
            return default;

        var root = _configDocument.RootElement;
        if (!root.TryGetProperty(sectionName, out var section))
            return default;

        if (!section.TryGetProperty(key, out var value))
            return default;

        return JsonSerializer.Deserialize<T>(value.GetRawText(), _jsonOptions);
    }

    /// <summary>Reload configuration from file.</summary>
    public async Task ReloadConfiguration()
    {
        _configDocument?.Dispose();
        _configDocument = null;
        await LoadConfigurationFile();
    }

    /// <summary>Get the current environment.</summary>
    public string GetEnvironment() => _environment;

    /// <summary>Load configuration file from embedded resources.</summary>
    private async Task LoadConfigurationFile()
    {
        try
        {
            var assembly = typeof(JsonConfigurationService).Assembly;
            var baseName = "CabconMAUI.appsettings.json";
            var environmentName = "CabconMAUI.appsettings.Production.json";

            // Try environment-specific config first if in Production
            if (_environment == "Production")
            {
                var environmentStream = assembly.GetManifestResourceStream(environmentName);
                if (environmentStream != null)
                {
                    using var reader = new StreamReader(environmentStream);
                    var json = await reader.ReadToEndAsync();
                    _configDocument = JsonDocument.Parse(json);
                    return;
                }
            }

            // Fall back to base configuration
            var stream = assembly.GetManifestResourceStream(baseName)
                ?? throw new FileNotFoundException($"Embedded resource '{baseName}' not found.");

            using var baseReader = new StreamReader(stream);
            var baseJson = await baseReader.ReadToEndAsync();
            _configDocument = JsonDocument.Parse(baseJson);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
            throw;
        }
    }

    /// <summary>Detect the current environment (Development or Production).</summary>
    private static string GetCurrentEnvironment()
    {
#if DEBUG
        return "Development";
#else
        return "Production";
#endif
    }
}
