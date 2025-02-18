using Demo.App.Shared.Controls;
using Microsoft.Office.Core;
using System.Diagnostics;

namespace Demo.App
{
    public partial class ThisAddIn
    {
        private RibbonCustom ribbon;
        private PaneHost host;

        private void InternalStartup()
        {
            Startup += (sender, e) =>
            {
                Debug.WriteLine("🚀 ThisAddIn has started");
            };
            Shutdown += (sender, e) =>
            {
                Debug.WriteLine("🔻 ThisAddIn is shutting down");
            };
        }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            ribbon = new RibbonCustom();
            return ribbon;
        }

        public void ShowSummary(string summary)
        {
            var control = new SummaryPane();
            control.UpdateSummary(summary);

            if (host == null)
                host = new PaneHost(CustomTaskPanes);
            host.Show(control, 600);
        }

        public void CloseSummary()
        {
            host?.Close();
            host = null;
        }
    }
}
