using Demo.App.Shared.Settings.Registries;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace Demo.App.Shared.Settings;

public class SettingsService : ISettingsService
{
    public SettingsService()
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        Debug.WriteLine($"Application directory: {directory}");

        var json = Path.Combine(directory, "app.settings.json");
        Debug.WriteLine($"Settings file: {json}");

        Configuration = new ConfigurationBuilder()
            .AddJsonFile(json, optional: true, reloadOnChange: true)
            .AddRegistry(@"SOFTWARE\DEMO\Outlook\demo.com.addin")
            .Build();
    }

    private static readonly Lazy<SettingsService> instance = new(() => new SettingsService());

    public static SettingsService Instance => instance.Value;

    private IConfiguration Configuration { get; }

    public T GetValue<T>(string key)
    {
        return Configuration.GetValue<T>(key);
    }

    public T GetSection<T>(string sectionName) where T : class, new()
    {
        return Configuration.GetSection(sectionName).Get<T>();
    }
}
