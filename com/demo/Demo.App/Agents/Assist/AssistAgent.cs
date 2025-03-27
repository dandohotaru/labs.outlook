using Demo.App.Chats;
using Demo.App.Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.App.Agents.Assist;

public class AssistAgent
{
    private IChatService Service { get; }

    private IPromptLoader Loader { get; set; }

    public AssistAgent(IChatService service, IPromptLoader loader)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        Loader = loader ?? throw new ArgumentNullException(nameof(loader));
    }

    public async Task<AssistResult> Assist(string prompt, DraftModel? draft, IEnumerable<EmailModel> conversation)
    {
        var messages = new List<ChatMessage>();

        // system
        var instructions = Loader.LoadPrompt(@"Agents\Assist\agent.md");

        messages.Add(new ChatMessage
        {
            Role = "system",
            Content = instructions
        });

        // context
        if (conversation != null)
        {
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = @"**Background Conversation:** 
                    The following emails provide context for this request. 
                    Do not include them in the response."
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
        }

        // draft
        if (draft != null && !string.IsNullOrWhiteSpace(draft.Body))
        {
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = $@"
                        **Current Draft:**
                        - **Subject:** {draft.Subject}
                        - **From:** {draft.Sender}
                        - **To:** {string.Join(", ", draft.Recipients)}
                        - **Body:**
                        {draft.Body}

                        This is the current email draft that needs to be refined."
            });
        }

        // prompt
        messages.Add(new ChatMessage
        {
            Role = "user",
            Content = $@"
                    **User Instruction:** {prompt}

                    Please follow this instruction carefully when refining the draft or creating a new email."
        });

        // process
        var result = await Service.Send(messages);

        return new AssistResult
        {
            Body = result
        };
    }
}