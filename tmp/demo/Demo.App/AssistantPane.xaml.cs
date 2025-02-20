using Demo.App.Agents;
using Demo.App.Agents.Assist;
using Demo.App.Agents.Summarize;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Demo.App.Shared.Controls;

public partial class AssistantPane : UserControl
{
    private IChatService Chatbot { get; set; }

    private AssistAgent Assistant { get; set; }

    public AssistantPane(IChatService chatbot)
    {
        Chatbot = chatbot ?? throw new ArgumentNullException(nameof(chatbot));
        Assistant = new AssistAgent(Chatbot);

        InitializeComponent();

        ResponseTextBlock.Text = @"Hello! I am your AI email assistant. How can I assist you with today?";
        PromptTextBox.Focus();

        Suggestions = new ObservableCollection<string>
        {
            "Refine the provided draft email, maintaining the original intent while improving clarity and tone as needed.",
            "Reply a polite and professional response to the email, acknowledging the request clearly and concisely.",
            "Compose a thank you message for the information provided in this email, maintaining a professional tone and clarity.",
            "Invent clear witty plausible yet funny rather short excuses as reply for to the recipient of this email.",
            "Confirm receipt of the email, and outline a clear timeline in the future for your response in a concise manner.",
            "Compose a short humorous response to this email with a Star Wars themed conversation, maintaining a relaxed tone.",
            "Generate a short reply to this email using formal Old English, using an appropriate vocabulary and style.",
            "Provide a brief structured summary of this email conversation using simple markup highlighting important topics"
        };

        DataContext = this;
    }

    public ObservableCollection<string> Suggestions { get; set; }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                ResponseTextBlock.Text = "Generating output...";
            });

            var application = Globals.ThisAddIn.Application;
            var email = application.ActiveInspector() != null
                    ? application.ActiveInspector().CurrentItem as MailItem
                    : application.ActiveExplorer() != null
                        ? application.ActiveExplorer().ActiveInlineResponse != null
                            ? application.ActiveExplorer().ActiveInlineResponse as MailItem
                            : application.ActiveExplorer().Selection.Cast<MailItem>().FirstOrDefault()
                        : null;
            if (email != null)
            {
                var draft = email.Draft(application.Session.CurrentUser);

                var conversation = email
                    .Conversation()
                    .Map1();

                var prompt = PromptTextBox.Text;

                var result = await Assistant.Assist(prompt, draft, conversation);

                Dispatcher.Invoke(() =>
                {
                    ResponseTextBlock.Text = result.Body;
                });
            }

            else
            {
                Dispatcher.Invoke(() =>
                {
                    ResponseTextBlock.Text = "No conversation selected";
                });
            }

        }
        catch (System.Exception exception)
        {
            Debug.WriteLine($"Error: {exception.Message}");
            MessageBox.Show($"Error: {exception.Message}");
        }
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var application = Globals.ThisAddIn.Application;
            var email = application.ActiveInspector() != null
                    ? application.ActiveInspector().CurrentItem as MailItem
                    : application.ActiveExplorer() != null
                        ? application.ActiveExplorer().ActiveInlineResponse != null
                            ? application.ActiveExplorer().ActiveInlineResponse as MailItem
                            : null
                        : null;
            if (email != null)
            {
                email.Body = ResponseTextBlock.Text;
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    ResponseTextBlock.Text = "No editing email found";
                });
            }

        }
        catch (System.Exception exception)
        {
            Debug.WriteLine($"Error: {exception.Message}");
            MessageBox.Show($"Error: {exception.Message}");
        }
    }

    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        ResponseTextBlock.Text = "(Response will appear here)";
        PromptTextBox.Clear();
    }

    private void SuggestionsButton_Click(object sender, RoutedEventArgs e)
    {
        OptionsPopup.IsOpen = !OptionsPopup.IsOpen;
    }

    private void SuggestionOption_Click(object sender, RoutedEventArgs e)
    {
        var option = sender as Button;
        if (option != null)
        {
            var text = (option.Content as StackPanel)
                .Children.OfType<TextBlock>()
                .LastOrDefault()?
                .Text;
            if (text != null)
            {
                PromptTextBox.Text = text;
                OptionsPopup.IsOpen = false;
            }
        }
    }

    private void PromptTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter 
            && !Keyboard.IsKeyDown(Key.LeftShift) 
            && !Keyboard.IsKeyDown(Key.RightShift))
        {
            e.Handled = true;
            SendButton_Click(sender, new RoutedEventArgs());
        }
    }
}
