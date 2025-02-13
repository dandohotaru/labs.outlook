namespace Demo.App.Shared.Secrets
{
    public interface ISecretService
    {
        string Read(string key);
    }
}
