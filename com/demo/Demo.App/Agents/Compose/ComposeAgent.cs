using Demo.App.Chats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.App.Agents.Compose
{
    public class ComposeAgent
    {
        private IChatService Service { get; set; }

        public ComposeAgent(IChatService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<ComposeResult> Compose(ThreadModel conversation)
        {
            // scope
            var scopeBuilder = new StringBuilder();
            scopeBuilder.AppendLine("You are a professional email assistant. ");
            scopeBuilder.AppendLine("Generate a polite and concise email reply based on the given context.");
            var scope = scopeBuilder.ToString();
            Debug.WriteLine("Scope: " + scope);
            
            // prompt
            var promptBuilder = new StringBuilder();
            foreach (var email in conversation.Emails)
            {
                promptBuilder.AppendLine($"From: {email.Sender}");
                promptBuilder.AppendLine($"Date: {email.Sent}");
                promptBuilder.AppendLine($"Subject: {email.Subject}");
                promptBuilder.AppendLine("Message:");
                promptBuilder.AppendLine(email.Body);
                promptBuilder.AppendLine("\n---\n");
            }

            var prompt = promptBuilder.ToString();
            Debug.WriteLine("Prompt: " + prompt);

            // result
            var result = await Service.Send(
            [
                new ChatMessage
                {
                    Role = "system",
                    Content = scope
                },
                new ChatMessage
                {
                    Role = "user",
                    Content = prompt
                },
            ]);

            return new ComposeResult
            {
                Body = result
            };
        }
    }
}
