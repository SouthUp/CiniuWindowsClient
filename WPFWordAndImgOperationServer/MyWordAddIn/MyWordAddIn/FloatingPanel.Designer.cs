namespace MyWordAddIn
{
    partial class FloatingPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MyWordTipsElementHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // MyWordTipsElementHost
            // 
            this.MyWordTipsElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyWordTipsElementHost.Location = new System.Drawing.Point(0, 0);
            this.MyWordTipsElementHost.Name = "MyWordTipsElementHost";
            this.MyWordTipsElementHost.Size = new System.Drawing.Size(400, 100);
            this.MyWordTipsElementHost.TabIndex = 0;
            this.MyWordTipsElementHost.Text = "MyWordTipsElementHost";
            this.MyWordTipsElementHost.Child = null;
            // 
            // FloatingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 100);
            this.Controls.Add(this.MyWordTipsElementHost);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FloatingPanel";
            this.Text = "FloatingPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost MyWordTipsElementHost;
    }
}