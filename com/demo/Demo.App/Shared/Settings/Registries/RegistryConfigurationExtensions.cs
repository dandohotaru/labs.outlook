using Microsoft.Extensions.Configuration;

namespace Demo.App.Shared.Settings.Registries
{
    public static class RegistryConfigurationExtensions
    {
        public static IConfigurationBuilder AddRegistry(this IConfigurationBuilder builder, string path)
        {
            return builder.Add(new RegistryConfigurationSource(path));
        }
    }
}
