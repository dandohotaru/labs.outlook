using Demo.App.Chats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.App.Agents.Assist;

public class AssistAgent
{
    private IChatService Service { get; }

    public AssistAgent(IChatService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<AssistResult> Assist(string prompt, DraftModel? draft, IEnumerable<EmailModel> conversation)
    {
        var messages = new List<ChatMessage>();

        // system
        messages.Add(new ChatMessage
        {
            Role = "system",
            Content = @"
                    You are an intelligent email assistant that assists with drafting or refining emails based on user instructions.
                    You should:
                    - Maintain professionalism, clarity, and conciseness.
                    - Use the background conversation to understand context, but do not include it in the final email.
                    - If an existing draft is provided, refine it while keeping its intent.
                    - If no draft is given, generate a new email from scratch following the prompt.
                    - Follow the user instruction carefully and ensure the response aligns with the conversation."
        });

        // Add background conversation separately
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