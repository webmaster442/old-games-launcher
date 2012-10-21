namespace OldGamesLauncher
{
    partial class HelpBrowser
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
            this.RtbHelp = new System.Windows.Forms.RichTextBox();
            this.TvDocs = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RtbHelp
            // 
            this.RtbHelp.BackColor = System.Drawing.SystemColors.Control;
            this.RtbHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RtbHelp.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.RtbHelp.Location = new System.Drawing.Point(0, 0);
            this.RtbHelp.Name = "RtbHelp";
            this.RtbHelp.ReadOnly = true;
            this.RtbHelp.Size = new System.Drawing.Size(428, 428);
            this.RtbHelp.TabIndex = 1;
            this.RtbHelp.Text = "";
            // 
            // TvDocs
            // 
            this.TvDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TvDocs.Location = new System.Drawing.Point(0, 0);
            this.TvDocs.Name = "TvDocs";
            this.TvDocs.Size = new System.Drawing.Size(200, 428);
            this.TvDocs.TabIndex = 2;
            this.TvDocs.DoubleClick += new System.EventHandler(this.TvDocs_DoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TvDocs);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.RtbHelp);
            this.splitContainer1.Size = new System.Drawing.Size(632, 428);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 3;
            // 
            // HelpBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 428);
            this.Controls.Add(this.splitContainer1);
            this.Name = "HelpBrowser";
            this.Text = "HelpBrowser";
            this.Load += new System.EventHandler(this.HelpBrowser_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox RtbHelp;
        private System.Windows.Forms.TreeView TvDocs;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}