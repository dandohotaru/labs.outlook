using Microsoft.Office.Core;
using System.Diagnostics;

namespace Demo.App
{
    public partial class ThisAddIn
    {
        private RibbonCustom ribbon;

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
    }
}
