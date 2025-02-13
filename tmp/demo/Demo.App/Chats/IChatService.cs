using System.Threading.Tasks;

namespace Demo.App.Chats
{
    public interface IChatService
    {
        Task<string> Send(string scope, string prompt);
    }
}
