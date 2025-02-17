using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Demo.App.Agents;
using Demo.App.Agents.Compose;
using Demo.App.Agents.Revise;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Demo.App.Shared.Settings;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Demo.App
{
    public partial class CustomRibbon
    {
        private ComposeAgent Composer { get; set; }

        private ReviseAgent Reviser { get; set; }

        private void CustomRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            var settings = SettingsService.Instance;
            var chatbot = new OpenaiChatService(settings);
            Composer = new ComposeAgent(chatbot);
            Reviser = new ReviseAgent(chatbot);

            this.buttonRefine.Visible = false;

            Globals.ThisAddIn.Application.Inspectors.NewInspector += inspector =>
            {
                try
                {
                    var mailItem = inspector.CurrentItem as MailItem;
                    if (mailItem != null && mailItem.Sent == false)
                    {
                        Debug.WriteLine("✅ Refine Reply button enabled.");
                        buttonRefine.Visible = true;
                    }
                    else
                    {
                        Debug.WriteLine("❌ Refine Reply button hidden.");
                        buttonRefine.Visible = false;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine($"❌ Error checking reply state: {ex.Message}");
                }
            };
        }

        private async void btnQuickReply_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var application = Globals.ThisAddIn.Application;
                var explorer = application.ActiveExplorer();
                if (explorer != null)
                {
                    var selection = explorer.Selection;
                    if (selection.Count > 0)
                    {
                        var email = selection[1] as MailItem;
                        if (email != null)
                        {
                            var conversation = email
                                .Conversation()
                                .Map();
                            var result = await Composer.Compose(conversation);

                            var reply = email.Reply();
                            reply.Body = result.Body;
                            reply.Display();

                            Debug.WriteLine("✅ Draft successfully composed.");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private async void buttonRefine_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var inspector = Globals.ThisAddIn.Application.ActiveInspector();
                if (inspector != null)
                {
                    var email = inspector.CurrentItem as MailItem;
                    if (email != null)
                    {
                        var draft = new DraftModel
                        {
                            Subject = email.Subject,
                            Sender = email.SenderEmailAddress,
                            Recipients = email.Recipients
                                .Cast<Recipient>()
                                .Select(p => p.Address)
                                .ToArray(),
                            Body = email.Body
                        };

                        var conversation = email
                            .Conversation()
                            .Map1();

                        var result = await Reviser.Revise(draft, conversation);

                        email.Body = result.Body;
                        email.Save();

                        Debug.WriteLine("✅ Draft successfully refined.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"❌ Error refining reply: {ex.Message}");
            }
        }

        private void buttonSummarize_Click(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("Summarize with AI clicked");
        }
    }
}
