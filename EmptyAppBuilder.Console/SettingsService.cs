using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmptyAppBuilder.Console;

class SettingsService
{
    private readonly ILogger<SettingsService> _logger;
    
    private readonly IConfiguration _config;
    private readonly Settings _settings;
    private readonly Settings _settingsMonitor;
    private readonly Settings _settingsSnapshot;

    public SettingsService(
        ILogger<SettingsService> logger,
        IConfiguration config, 
        IOptions<Settings> settings, 
        IOptionsMonitor<Settings> settingsMonitor,
        IOptionsSnapshot<Settings> settingsSnapshot
        )
    {
        _logger = logger;
        
        _config = config; // Load the whole configuration directly
        _settings = settings.Value; // It's a singleton, value isn't updated at the runtime
        _settingsMonitor = settingsMonitor.CurrentValue; // Monitor is a singleton, but the CurrentValue is transient
        _settingsSnapshot = settingsSnapshot.Value; // Scoped, value won't be changed during ongoing execution
    }

    public void LogSettingsAsJson() => _logger.LogInformation("Settings: {JsonSettings}", GetSettingsAsJsonString());
    
    private string GetSettingsAsJsonString() => JsonSerializer.Serialize(new
    {
        IConfiguration = _config.GetValue<string>("Settings:Name") ?? string.Empty,
        Options = _settings.Name,
        IOptionsMonitor = _settingsMonitor.Name,
        IOptionsSnapshot = _settingsSnapshot.Name
    }, new JsonSerializerOptions() { WriteIndented = true });
}