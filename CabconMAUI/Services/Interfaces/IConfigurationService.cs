namespace CabconMAUI.Services.Interfaces;

/// <summary>Service for loading and managing application configuration from JSON files.</summary>
public interface IConfigurationService
{
    /// <summary>Load configuration from embedded JSON resource.</summary>
    Task<T> LoadConfiguration<T>(string sectionName) where T : class;

    /// <summary>Get a specific configuration value by section and key.</summary>
    T? GetValue<T>(string sectionName, string key);

    /// <summary>Reload configuration from file.</summary>
    Task ReloadConfiguration();

    /// <summary>Get the current environment.</summary>
    string GetEnvironment();
}
