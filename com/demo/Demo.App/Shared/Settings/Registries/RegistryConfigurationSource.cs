using Microsoft.Extensions.Configuration;
using System;

namespace Demo.App.Shared.Settings.Registries;

public class RegistryConfigurationSource : IConfigurationSource
{
    public RegistryConfigurationSource(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private string Path { get; set; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new RegistryConfigurationProvider(Path);
    }
}
