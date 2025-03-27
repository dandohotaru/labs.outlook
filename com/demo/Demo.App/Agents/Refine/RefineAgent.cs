using Demo.App.Chats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.App.Agents.Revise;

public class RefineAgent
{
    private IChatService Service { get; set; }

    public RefineAgent(IChatService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<RefineResult> Revise(DraftModel draft, IEnumerable<EmailModel> conversation)
    {
        var messages = new List<ChatMessage>();

        // system
        messages.Add(new ChatMessage
        {
            Role = "system",
            Content = @"
                    You are a professional email assistant that refines and enhances email drafts. 
                    Your role is to improve clarity and professionalism and tone while preserving the original intent. 
                    Use previous emails as context but do not include them in the response. 
                    Always provide a complete and refined version of the latest email draft formatted for immediate use. 
                    If the email is a reply consider past messages for coherence and appropriate responses. 
                    Use polite and professional language and ensure grammatical accuracy and keep it concise. "
        });

        // context
        messages.Add(new ChatMessage
        {
            Role = "user",
            Content = @"
                    Here is the background context for the conversation. 
                    Use this to understand the ongoing discussion but do not include it in the final refined email."
        });

        foreach (var email in conversation)
        {
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = $@"
                        Subject: {email.Subject}
                        Sent: {email.Sent}
                        From: {email.Sender}
                        To: {string.Join(", ", email.Recipients)}
                
                        {email.Body}"
            });
        }

        messages.Add(new ChatMessage
        {
            Role = "user",
            Content = $@"
                    Subject: {draft.Subject}
                    From: {draft.Sender}
                    To: {string.Join(", ", draft.Recipients)}

                    {draft.Body}

                    Please refine this email to improve professionalism, clarity and tone while maintaining the context of the conversation."
        });

        // result
        var result = await Service.Send(messages);

        return new RefineResult
        {
            Body = result
        };
    }
}
