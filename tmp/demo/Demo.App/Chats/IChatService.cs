using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.App.Chats;

public interface IChatService
{
    Task<string> Send(IEnumerable<ChatMessage> messages);
}

public class ChatMessage
{
    public string Role { get; set; }

    public string Content { get; set; }
}
