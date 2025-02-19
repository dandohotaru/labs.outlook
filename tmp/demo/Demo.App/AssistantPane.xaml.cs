using System;
using System.Windows;
using System.Windows.Controls;

namespace Demo.App.Shared.Controls
{
    public partial class AssistantPane : UserControl
    {
        public AssistantPane()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userPrompt = PromptTextBox.Text;
            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                ResponseTextBlock.Text = "Please enter a prompt.";
                return;
            }
            ResponseTextBlock.Text = $"Generated response for: {userPrompt}";
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

        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the ContextMenu when the dropdown button is clicked
            DropdownMenu.IsOpen = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the ContextMenu
            var selectedMenuItem = sender as MenuItem;
            // Populate the PromptTextBox with the selected option
            PromptTextBox.Text = selectedMenuItem.Header.ToString();
            // Close the ContextMenu after selection
            DropdownMenu.IsOpen = false;
        }


    }
}
