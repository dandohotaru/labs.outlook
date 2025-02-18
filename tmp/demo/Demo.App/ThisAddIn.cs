using Microsoft.Office.Core;
using System.Diagnostics;

namespace Demo.App
{
    public partial class ThisAddIn
    {
        private RibbonCustom ribbon;
        private SummaryPaneHost summary;

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

        public void ShowSummary(string content)
        {
            if (summary == null)
                summary = new SummaryPaneHost(CustomTaskPanes);
            summary.Show(content);
        }

        public void CloseSummary()
        {
            summary?.Close();
            summary = null;
        }
    }
}
