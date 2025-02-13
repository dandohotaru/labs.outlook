using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Demo.App.Agents.Compose;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Demo.App.Shared.Settings;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;

namespace Demo.App
{
    public partial class CustomRibbon
    {
        private ComposeAgent Composer { get; set; }

        private void CustomRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            var settings = SettingsService.Instance;
            var chatbot = new OpenaiChatService(settings);
            Composer = new ComposeAgent(chatbot);
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
                            var conversation = email.ToThread();
                            var result = await Composer.Compose(conversation);

                            var reply = email.Reply();
                            reply.Body = result.Body;
                            reply.Display();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
