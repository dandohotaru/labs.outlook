using Demo.App.Agents;
using Demo.App.Agents.Assist;
using Demo.App.Agents.Summarize;
using Demo.App.Chats;
using Demo.App.Shared.Extensions;
using Demo.App.Shared.Prompts;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Demo.App.Shared.Controls;

public partial class AssistantPane : UserControl
{
    private IChatService Chatbot { get; }

    private IPromptLoader Loader { get; }

    private AssistAgent Assistant { get; }

    private string Intro { get; }

    public AssistantPane(IChatService chatbot, IPromptLoader loader)
    {
        Intro = new StringBuilder()
            .AppendLine("Hi!")
            .AppendLine("I am your AI email assistant.")
            .AppendLine("How can I assist you with today?")
            .ToString();

        Chatbot = chatbot ?? throw new ArgumentNullException(nameof(chatbot));
        Loader = loader ?? throw new ArgumentNullException(nameof(loader));
        Assistant = new AssistAgent(Chatbot, Loader);

        InitializeComponent();

        ResponseTextBlock.Text = Intro;
        PromptTextBox.Focus();

        Suggestions = new ObservableCollection<string>
        {
            "Compose a new email based on draft",
            "Summarize conversation in one sentence",
            "Generate a timeline of key events",
            "List action points from the conversation",
            "Draft a polite professional reply",
            "Refine current draft for clarity and tone",
            "Analyze draft and provide feedback",
            "Draft a witty humorous Yoda style reply",
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

    private void AcceptButton_Click(object sender, RoutedEventArgs e)
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

                ResponseTextBlock.Text = Intro;
                PromptTextBox.Clear();
            }
            else
            {
                MessageBox.Show("No editing email in scope");
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
        Clipboard.SetText(ResponseTextBlock.Text);
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
