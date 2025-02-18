using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Office.Tools;

namespace Demo.App
{
    public class SummaryPaneHost : UserControl
    {
        private ElementHost host;

        private SummaryPane control;

        private CustomTaskPane pane;

        public SummaryPaneHost(CustomTaskPaneCollection panes)
        {
            host = new ElementHost();
            control = new SummaryPane();

            host.Child = control;
            host.Dock = DockStyle.Fill;

            Controls.Add(host);
            Dock = DockStyle.Fill;

            pane = panes.Add(this, "Task panel");
            pane.Width = 600;
        }

        public void Show(string summary)
        {
            control.UpdateSummary(summary);
            pane.Visible = true;
        }

        public void Close()
        {
            if (pane != null)
            {
                pane.Visible = false;
                pane.Dispose();
                pane = null;
            }
        }
    }
}
