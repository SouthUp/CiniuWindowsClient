namespace CheckWordControl
{
    partial class ImageDetailForm
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
            this.WPFElementHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // WPFElementHost
            // 
            this.WPFElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WPFElementHost.Location = new System.Drawing.Point(0, 0);
            this.WPFElementHost.Name = "WPFElementHost";
            this.WPFElementHost.Size = new System.Drawing.Size(1182, 703);
            this.WPFElementHost.TabIndex = 0;
            this.WPFElementHost.Text = "elementHost1";
            this.WPFElementHost.Child = null;
            // 
            // ImageDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 703);
            this.Controls.Add(this.WPFElementHost);
            this.MinimizeBox = false;
            this.Name = "ImageDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "图片详情";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost WPFElementHost;
    }
}