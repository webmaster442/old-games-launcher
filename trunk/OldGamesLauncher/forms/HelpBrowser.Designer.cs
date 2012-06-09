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
            this.TabSelector = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.RtbHelp = new System.Windows.Forms.RichTextBox();
            this.TabSelector.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabSelector
            // 
            this.TabSelector.Controls.Add(this.tabPage1);
            this.TabSelector.Controls.Add(this.tabPage2);
            this.TabSelector.Controls.Add(this.tabPage3);
            this.TabSelector.Controls.Add(this.tabPage5);
            this.TabSelector.Controls.Add(this.tabPage4);
            this.TabSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.TabSelector.Location = new System.Drawing.Point(0, 0);
            this.TabSelector.Name = "TabSelector";
            this.TabSelector.SelectedIndex = 0;
            this.TabSelector.Size = new System.Drawing.Size(632, 22);
            this.TabSelector.TabIndex = 0;
            this.TabSelector.SelectedIndexChanged += new System.EventHandler(this.TabSelector_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(624, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Release Notes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(624, 0);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Licence";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(624, 0);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "DosBox Manual";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(624, 0);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "DosBox keys";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(624, 0);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Compatiblility Options";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // RtbHelp
            // 
            this.RtbHelp.BackColor = System.Drawing.SystemColors.Control;
            this.RtbHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RtbHelp.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.RtbHelp.Location = new System.Drawing.Point(0, 22);
            this.RtbHelp.Name = "RtbHelp";
            this.RtbHelp.ReadOnly = true;
            this.RtbHelp.Size = new System.Drawing.Size(632, 406);
            this.RtbHelp.TabIndex = 1;
            this.RtbHelp.Text = "";
            // 
            // HelpBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 428);
            this.Controls.Add(this.RtbHelp);
            this.Controls.Add(this.TabSelector);
            this.Name = "HelpBrowser";
            this.Text = "HelpBrowser";
            this.TabSelector.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabSelector;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox RtbHelp;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
    }
}