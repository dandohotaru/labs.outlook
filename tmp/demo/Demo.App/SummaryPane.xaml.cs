using Demo.App.Agents.Compose;
using Demo.App.Agents.Summarize;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Demo.App.Shared.Settings;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Demo.App
{
    public partial class SummaryPane : UserControl
    {
        private ISettingsService Settings { get; set; }

        private IChatService Chatbot { get; set; }

        private SummarizeAgent Summarizer { get; set; }

        public SummaryPane(ISettingsService settings, IChatService chatbot)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Chatbot = chatbot ?? throw new ArgumentNullException(nameof(chatbot));
            Summarizer = new SummarizeAgent(Chatbot);
            InitializeComponent();
        }

        public async void UpdateSummary(string summary)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    SummaryTextBox.Text = "Summarizing conversation...";
                });

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
                                .Map1();
                            var result = await Summarizer.Summarize(conversation);

                            Dispatcher.Invoke(() =>
                            {
                                SummaryTextBox.Text = result.Body;
                            });
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(() => 
                        { 
                            SummaryTextBox.Text = "No conversation selected."; 
                        });
                    }
                }
            }
            catch (System.Exception exception)
            {
                Debug.WriteLine($"Error: {exception.Message}");
                MessageBox.Show($"Error: {exception.Message}");
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SummaryTextBox.Text);
            MessageBox.Show("Summary copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ThisAddIn.CloseSummary();
        }
    }
}
