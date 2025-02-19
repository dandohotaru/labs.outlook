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

        Suggestions = new ObservableCollection<string>
        {
            "Reply to this email politely acknowledging the sender’s request",
            "Write a thank you message for the information provided in this email",
            "Provide a detailed explanation of the issue and suggest potential solutions",
            "Confirm receipt of the email and provide a timeline for your response",
            "Can you reply to this email based on the draft I have already provided",
            "Send a humorous response referencing star wars style conversation",
            "Reply in old English style and vocabulary using formal language"
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
                    : application.ActiveExplorer().ActiveInlineResponse as MailItem;
            if (email != null)
            {
                var draft = email.Draft();

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

    private void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Reply Accepted!", "AI Assistant", MessageBoxButton.OK, MessageBoxImage.Information);
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


}
