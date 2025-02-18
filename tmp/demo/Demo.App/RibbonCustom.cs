using Demo.App.Agents;
using Demo.App.Agents.Compose;
using Demo.App.Agents.Revise;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Demo.App.Shared.Settings;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Office.Interop.Outlook.Application;

namespace Demo.App
{
    [ComVisible(true)]
    public class RibbonCustom : IRibbonExtensibility
    {
        private IRibbonUI ribbon;

        public RibbonCustom()
        {
        }

        private ComposeAgent Composer { get; set; }

        private ReviseAgent Reviser { get; set; }

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Demo.App.RibbonCustom.xml");
        }

        public void Ribbon_Load(IRibbonUI ribbonUI)
        {
            ribbon = ribbonUI;

            var settings = SettingsService.Instance;
            var chatbot = new OpenaiChatService(settings);
            Composer = new ComposeAgent(chatbot);
            Reviser = new ReviseAgent(chatbot);
        }

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        public async void OnReply_Click(IRibbonControl control)
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

                            Debug.WriteLine("Draft successfully composed");
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                Debug.WriteLine($"Error: {exception.Message}");
                MessageBox.Show($"Error: {exception.Message}");
            }
        }

        public bool OnReply_Visible(IRibbonControl control)
        {
            return true;
        }

        public void OnSummarize_Click(IRibbonControl control)
        {
            Application application = Globals.ThisAddIn.Application;
            Explorer explorer = application.ActiveExplorer();
            MailItem mail = explorer.Selection[1] as MailItem;

            if (mail != null)
            {
                string emailBody = mail.Body;

                // Process with AI model
                //string summary = CallAIService("summarize", emailBody);

                // Show result in a WPF Task Pane
                //ShowTaskPane(summary);

                MessageBox.Show("summarize");
            }
        }

        public bool OnSummarize_Visible(IRibbonControl control)
        {
            return true;
        }

        public async void OnRefine_Click(IRibbonControl control)
        {
            try
            {
                var application = Globals.ThisAddIn.Application;
                var email = application.ActiveInspector() != null
                    ? application.ActiveInspector().CurrentItem as MailItem
                    : application.ActiveExplorer().ActiveInlineResponse as MailItem;
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
                        Body = email.Draft()
                    };

                    var conversation = email
                        .Conversation()
                        .Map1();

                    var result = await Reviser.Revise(draft, conversation);

                    email.Body = result.Body;
                    email.Save();

                    Debug.WriteLine("Draft successfully refined.");
                }
            }
            catch (System.Exception exception)
            {
                Debug.WriteLine($"Error: {exception.Message}");
                MessageBox.Show($"Error: {exception.Message}");
            }
        }

        public bool OnRefine_Visible(IRibbonControl control)
        {

            return true;
        }
    }
}
