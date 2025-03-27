using Demo.App.Chats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.App.Agents.Summarize;

public class SummarizeAgent
{
    private IChatService Service { get; }

    public SummarizeAgent(IChatService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<SummarizeResult> Summarize(IEnumerable<EmailModel> conversation)
    {
        var messages = new List<ChatMessage>();

        // system
        messages.Add(new ChatMessage
        {
            Role = "system",
            Content = @"
                    You are a professional email assistant specializing in summarizing conversations.
                    Your goal is to provide a **clear, concise summary** of an email thread, capturing key points, decisions, and next steps.
                    **Do NOT include greetings, signatures, or minor details.** 
                    Only focus on **the main discussion points** and **important action items**.
                    The summary should be formatted in **bullet points** or a **short paragraph** for readability."
        });

        // context
        messages.Add(new ChatMessage
        {
            Role = "user",
            Content = "Summarize the following email conversation while preserving key discussion points."
        });

        foreach (var email in conversation)
        {
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = $@"
                        **Email Details:**
                        - **Subject:** {email.Subject}
                        - **Sent:** {email.Sent}
                        - **From:** {email.Sender}
                        - **To:** {string.Join(", ", email.Recipients)}
                        - **Body:**
                        {email.Body}"
            });
        }

        // result
        var result = await Service.Send(messages);

        return new SummarizeResult
        {
            Body = result
        };
    }
}
