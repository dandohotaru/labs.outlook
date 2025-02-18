using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Office.Tools;
using System.Windows;
using Microsoft.Office.Interop.Outlook;

namespace Demo.App.Shared.Controls
{
    public class PaneHost : UserControl
    {
        public PaneHost(CustomTaskPaneCollection panes)
        {
            Panes = panes ?? throw new ArgumentNullException(nameof(panes));
        }

        public CustomTaskPaneCollection Panes { get; set; }

        private CustomTaskPane Pane { get; set; }

        public void Show<TControl>(TControl control, int width = 750, string title = "Custom Pane") where TControl : UIElement
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            Dock = DockStyle.Fill;

            Controls.Add(new ElementHost
            {
                Child = control,
                Dock = DockStyle.Fill
            });

            Pane = Panes.Add(this, title);
            Pane.Width = width;
            Pane.Visible = true;
        }

        public void Close()
        {
            if (Pane == null)
                return;
            Pane.Visible = false;
            Pane.Dispose();
            Pane = null;
        }
    }
}
