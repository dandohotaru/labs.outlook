using System.Windows;
using System.Windows.Controls;

namespace Demo.App
{
    public partial class SummaryPane : UserControl
    {
        public SummaryPane()
        {
            InitializeComponent();
        }

        public void UpdateSummary(string summary)
        {
            SummaryTextBox.Text = summary;
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
