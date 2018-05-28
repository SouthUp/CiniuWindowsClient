namespace MyWordAddIn
{
    partial class MyRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MyRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.groupCheckWord = this.Factory.CreateRibbonGroup();
            this.CheckWordBtn = this.Factory.CreateRibbonCheckBox();
            this.btnCheckWord = this.Factory.CreateRibbonButton();
            this.ViolateDBBtn = this.Factory.CreateRibbonButton();
            this.SynonymDBBtn = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.groupCheckWord.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.Groups.Add(this.groupCheckWord);
            this.tab1.Label = "词牛";
            this.tab1.Name = "tab1";
            // 
            // groupCheckWord
            // 
            this.groupCheckWord.Items.Add(this.CheckWordBtn);
            this.groupCheckWord.Items.Add(this.btnCheckWord);
            this.groupCheckWord.Items.Add(this.ViolateDBBtn);
            this.groupCheckWord.Items.Add(this.SynonymDBBtn);
            this.groupCheckWord.Label = "违禁词检查";
            this.groupCheckWord.Name = "groupCheckWord";
            // 
            // CheckWordBtn
            // 
            this.CheckWordBtn.Checked = true;
            this.CheckWordBtn.Label = "实时检查开关";
            this.CheckWordBtn.Name = "CheckWordBtn";
            this.CheckWordBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CheckWordBtn_Click);
            // 
            // btnCheckWord
            // 
            this.btnCheckWord.Image = global::MyWordAddIn.Properties.Resources.CheckBtn;
            this.btnCheckWord.Label = "违禁检查";
            this.btnCheckWord.Name = "btnCheckWord";
            this.btnCheckWord.ShowImage = true;
            this.btnCheckWord.Visible = false;
            this.btnCheckWord.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCheckWord_Click);
            // 
            // ViolateDBBtn
            // 
            this.ViolateDBBtn.Image = global::MyWordAddIn.Properties.Resources.WordsDB;
            this.ViolateDBBtn.Label = "违禁词库";
            this.ViolateDBBtn.Name = "ViolateDBBtn";
            this.ViolateDBBtn.ShowImage = true;
            this.ViolateDBBtn.Visible = false;
            this.ViolateDBBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ViolateDBBtn_Click);
            // 
            // SynonymDBBtn
            // 
            this.SynonymDBBtn.Image = global::MyWordAddIn.Properties.Resources.SynonymDB;
            this.SynonymDBBtn.Label = "推荐词库";
            this.SynonymDBBtn.Name = "SynonymDBBtn";
            this.SynonymDBBtn.ShowImage = true;
            this.SynonymDBBtn.Visible = false;
            this.SynonymDBBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.SynonymDBBtn_Click);
            // 
            // MyRibbon
            // 
            this.Name = "MyRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.MyRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.groupCheckWord.ResumeLayout(false);
            this.groupCheckWord.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupCheckWord;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCheckWord;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ViolateDBBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton SynonymDBBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox CheckWordBtn;
    }

    partial class ThisRibbonCollection
    {
        internal MyRibbon MyRibbon
        {
            get { return this.GetRibbon<MyRibbon>(); }
        }
    }
}
