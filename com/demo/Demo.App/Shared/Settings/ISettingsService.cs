namespace Demo.App.Shared.Settings;

public interface ISettingsService
{
    T GetValue<T>(string key);

    T GetSection<T>(string section) where T : class, new();
}
