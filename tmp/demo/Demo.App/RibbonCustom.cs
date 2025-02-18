using Demo.App.Agents;
using Demo.App.Agents.Compose;
using Demo.App.Agents.Revise;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Demo.App.Shared.Settings;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Demo.App
{
    [ComVisible(true)]
    public class RibbonCustom : IRibbonExtensibility
    {
        private IRibbonUI ribbon;

        public RibbonCustom(ISettingsService settings, IChatService chatbot)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Chatbot = chatbot ?? throw new ArgumentNullException(nameof(chatbot));
            Composer = new ComposeAgent(Chatbot);
            Refiner = new RefineAgent(Chatbot);
        }

        private ISettingsService Settings { get; set; }

        private IChatService Chatbot { get; set; }

        private ComposeAgent Composer { get; set; }

        private RefineAgent Refiner { get; set; }

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Demo.App.RibbonCustom.xml");
        }

        public void Ribbon_Load(IRibbonUI ribbonUI)
        {
            ribbon = ribbonUI;
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

        public async void OnSummarize_Click(IRibbonControl control)
        {
            try
            {
                var application = Globals.ThisAddIn.Application;
                MailItem email = null;

                // Get currently selected email
                if (application.ActiveInspector() != null)
                {
                    email = application.ActiveInspector().CurrentItem as MailItem;
                }
                else if (application.ActiveExplorer().Selection.Count > 0)
                {
                    email = application.ActiveExplorer().Selection[1] as MailItem;
                }

                if (email != null)
                {
                    var conversation = email.Conversation().Map1();

                    // Call AI service for summarization
                    //var result = await Summarizer.Summarize(conversation);
                    var result = "this is a test";

                    // Show the Task Pane and display the summary
                    Globals.ThisAddIn.ShowSummary(result);
                }
                else
                {
                    MessageBox.Show("No email selected for summarization.");
                }
            }
            catch (System.Exception exception)
            {
                Debug.WriteLine($"Error: {exception.Message}");
                MessageBox.Show($"Error: {exception.Message}");
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

                    var result = await Refiner.Revise(draft, conversation);

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
