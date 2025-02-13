namespace Demo.UI
{
    partial class CustomRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public CustomRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.Custom = this.Factory.CreateRibbonGroup();
            this.replyButton = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.Custom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.Custom);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // Custom
            // 
            this.Custom.Items.Add(this.replyButton);
            this.Custom.Label = "group1";
            this.Custom.Name = "Custom";
            // 
            // replyButton
            // 
            this.replyButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.replyButton.Label = "Reply";
            this.replyButton.Name = "replyButton";
            this.replyButton.ShowImage = true;
            this.replyButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.replyButton_Click);
            // 
            // CustomRibbon
            // 
            this.Name = "CustomRibbon";
            this.RibbonType = "Microsoft.Outlook.Mail.Read";
            this.Tabs.Add(this.tab1);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.Custom.ResumeLayout(false);
            this.Custom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Custom;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton replyButton;
    }

    partial class ThisRibbonCollection
    {
        internal CustomRibbon Ribbon
        {
            get { return this.GetRibbon<CustomRibbon>(); }
        }
    }
}
