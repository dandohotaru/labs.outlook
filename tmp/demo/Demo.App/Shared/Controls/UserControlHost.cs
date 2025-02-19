using Microsoft.Office.Tools;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Demo.App.Shared.Controls;

public partial class UserControlHost : UserControl
{
    private CustomTaskPane Pane { get; }

    private ElementHost Host { get; }

    public UserControlHost(CustomTaskPaneCollection panes, string title = "AI Assistant")
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
        Host = new ElementHost
        {
            Dock = DockStyle.Fill
        };
        this.Controls.Add(Host);

        Pane = panes.Add(this, title);
        Pane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight;
        Pane.Width = 400;
        Pane.Visible = false;
    }

    public void Show<T>(T control, int? width = 500) where T : UIElement
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        if (Host.Child is IDisposable disposableChild)
        {
            disposableChild.Dispose();
            Host.Child = null;
        }
        Host.Child = control;
        Pane.Width = width ?? Pane.Width;
        Pane.Visible = true;
    }

    public void Close()
    {
        if (Host.Child is IDisposable disposableChild)
        {
            disposableChild.Dispose();
            Host.Child = null;
        }
        else
        {
            Host.Child = null;
        }

        Pane.Visible = false;
    }
}

