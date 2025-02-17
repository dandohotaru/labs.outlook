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
            this.buttonRefine = this.Factory.CreateRibbonButton();
            this.buttonSummarize = this.Factory.CreateRibbonButton();
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
            this.tabCustom.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabCustom.ControlId.OfficeId = "TabMail";
            this.tabCustom.Groups.Add(this.groupCustom);
            this.tabCustom.Label = "TabMail";
            this.tabCustom.Name = "tabCustom";
            // 
            // groupCustom
            // 
            this.groupCustom.Items.Add(this.btnQuickReply);
            this.groupCustom.Items.Add(this.buttonRefine);
            this.groupCustom.Items.Add(this.buttonSummarize);
            this.groupCustom.Label = "AI";
            this.groupCustom.Name = "groupCustom";
            // 
            // btnQuickReply
            // 
            this.btnQuickReply.Label = "Reply";
            this.btnQuickReply.Name = "btnQuickReply";
            this.btnQuickReply.OfficeImageId = "Reply";
            this.btnQuickReply.ScreenTip = "Reply with AI";
            this.btnQuickReply.ShowImage = true;
            this.btnQuickReply.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnQuickReply_Click);
            // 
            // buttonRefine
            // 
            this.buttonRefine.Label = "Refine";
            this.buttonRefine.Name = "buttonRefine";
            this.buttonRefine.OfficeImageId = "Redo";
            this.buttonRefine.ScreenTip = "Refine with AI";
            this.buttonRefine.ShowImage = true;
            this.buttonRefine.Visible = false;
            this.buttonRefine.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonRefine_Click);
            // 
            // buttonSummarize
            // 
            this.buttonSummarize.Label = "Summarize";
            this.buttonSummarize.Name = "buttonSummarize";
            this.buttonSummarize.OfficeImageId = "ReadingMode";
            this.buttonSummarize.ScreenTip = "Summarize with AI";
            this.buttonSummarize.ShowImage = true;
            this.buttonSummarize.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonSummarize_Click);
            // 
            // CustomRibbon
            // 
            this.Name = "CustomRibbon";
            this.RibbonType = "Microsoft.Outlook.Explorer";
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
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonRefine;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSummarize;
    }

    partial class ThisRibbonCollection
    {
        internal CustomRibbon CustomRibbon
        {
            get { return this.GetRibbon<CustomRibbon>(); }
        }
    }
}
