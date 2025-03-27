using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Demo.App.Shared.Secrets
{
    public class RegistryService : ISecretService
    {
        public RegistryService(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        private string Path { get; set; }

        public string Read(string key)
        {
            try
            {
                using var registry = Registry.LocalMachine.OpenSubKey(Path);
                var value = registry?.GetValue(key);
                return value as string;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Registry read error: {exception.Message}");
                throw;
            }
        }
    }
}
