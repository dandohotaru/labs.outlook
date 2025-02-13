namespace Demo.App
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
            this.tabCustom = this.Factory.CreateRibbonTab();
            this.groupCustom = this.Factory.CreateRibbonGroup();
            this.btnQuickReply = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.tabCustom.SuspendLayout();
            this.groupCustom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // tabCustom
            // 
            this.tabCustom.Groups.Add(this.groupCustom);
            this.tabCustom.Label = "Custom";
            this.tabCustom.Name = "tabCustom";
            // 
            // groupCustom
            // 
            this.groupCustom.Items.Add(this.btnQuickReply);
            this.groupCustom.Label = "Custom";
            this.groupCustom.Name = "groupCustom";
            // 
            // btnQuickReply
            // 
            this.btnQuickReply.Label = "Quick Reply";
            this.btnQuickReply.Name = "btnQuickReply";
            this.btnQuickReply.ShowImage = true;
            this.btnQuickReply.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnQuickReply_Click);
            // 
            // CustomRibbon
            // 
            this.Name = "CustomRibbon";
            this.RibbonType = "Microsoft.Outlook.Explorer, Microsoft.Outlook.Mail.Read";
            this.Tabs.Add(this.tab1);
            this.Tabs.Add(this.tabCustom);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.CustomRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.tabCustom.ResumeLayout(false);
            this.tabCustom.PerformLayout();
            this.groupCustom.ResumeLayout(false);
            this.groupCustom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabCustom;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupCustom;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnQuickReply;
    }

    partial class ThisRibbonCollection
    {
        internal CustomRibbon CustomRibbon
        {
            get { return this.GetRibbon<CustomRibbon>(); }
        }
    }
}
