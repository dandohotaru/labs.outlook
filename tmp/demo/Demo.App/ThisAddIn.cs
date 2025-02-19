using Demo.App.Chats;
using Demo.App.Shared.Controls;
using Demo.App.Shared.Settings;
using Microsoft.Office.Core;
using System.Diagnostics;

namespace Demo.App
{
    public partial class ThisAddIn
    {
        public RibbonCustom Ribbon { get; set; }

        public UserControlHost Container { get; set; }

        public PaneHost Host { get; set; }

        private ISettingsService Settings { get; set; }

        private IChatService Chatbot { get; set; }

        private void InternalStartup()
        {
            Startup += (sender, e) =>
            {
                Debug.WriteLine("🚀 ThisAddIn has started");
                
            };
            Shutdown += (sender, e) =>
            {
                Debug.WriteLine("🔻 ThisAddIn is shutting down");
                Container.Dispose();
            };
        }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            Settings = SettingsService.Instance;
            Chatbot = new OpenaiChatService(Settings);
            Ribbon = new RibbonCustom(Settings, Chatbot);
            return Ribbon;
        }

        public void ShowSummary(string summary)
        {
            var control = new SummaryPane(Chatbot);
            control.UpdateSummary(summary);

            if (Host != null)
                Host.Close();

            Host = new PaneHost(CustomTaskPanes);
            Host.Show(control);
        }

        public void CloseSummary()
        {
            Host?.Close();
            Host = null;
        }

        public void ShowAssistant()
        {
            if (Container != null)
                Container.Close();
            Container = new UserControlHost(CustomTaskPanes, "AI Assistant");
            Container.Show(new AssistantPane(Chatbot));
        }

        public void CloseAssistant()
        {
            if (Container == null)
                return;
            Container.Close();
            Container = null;
        }
    }
}
