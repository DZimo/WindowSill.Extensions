using Microsoft.Extensions.Configuration;


/// <summary>
/// Represents the settings for the application.
/// </summary>
public class SettingsAzure
{
    /// <summary>
    /// The client ID (aka application ID) of your app registration.
    /// </summary>
    public string? ClientId { get; set; }
    /// <summary>
    /// The tenant ID of your Azure tenant. Set to "common" if app is registered as multi-tenant.
    /// </summary>
    public string? TenantId { get; set; }
    /// <summary>
    /// The Microsoft Graph permission scopes required by the app.
    /// </summary>
    public string[]? GraphUserScopes { get; set; }
    /// <summary>
    /// The number of seconds to wait between poll requests.
    /// </summary>
    public int PollInterval { get; set; }
    /// <summary>
    /// The path to a file where the authentication record should be persisted. Leave blank to disable auth caching.
    /// </summary>
    public string? AuthRecordCachePath { get; set; }

    /// <summary>
    /// Deserializes settings from appsettings.json + appsettings.Development.json.
    /// </summary>
    /// <returns>A Settings instance with the values from the JSON files.</returns>
    /// <exception cref="Exception">Thrown if the JSON files cannot be deserialized into a Settings instance.</exception>
    public static SettingsAzure LoadSettings()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        return config.GetRequiredSection("Settings").Get<SettingsAzure>() ??
            throw new Exception("Could not load app settings. See README for configuration instructions.");
    }
}
