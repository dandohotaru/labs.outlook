using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Demo.App.Shared.Settings.Registries;

public class RegistryConfigurationProvider : ConfigurationProvider
{
    public RegistryConfigurationProvider(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private string Path { get; set; }

    public override void Load()
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        Debug.WriteLine($"Loading registry configuration from {Path}");
        using (var key = Registry.LocalMachine.OpenSubKey(Path))
        {
            if (key != null)
            {
                foreach (var name in key.GetValueNames())
                {
                    string value = key.GetValue(name)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        data[name] = value;
                    }
                }
            }
        }

        Data = data;
    }
}
