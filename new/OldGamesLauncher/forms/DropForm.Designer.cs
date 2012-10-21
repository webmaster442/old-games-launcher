namespace OldGamesLauncher
{
    partial class DropForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.CbArgumentPass = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::OldGamesLauncher.Properties.Resources.konsole;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(138, 168);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 138);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Drop a Dos game exe here, to start it with dosbox";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CbArgumentPass
            // 
            this.CbArgumentPass.Dock = System.Windows.Forms.DockStyle.Top;
            this.CbArgumentPass.Location = new System.Drawing.Point(0, 0);
            this.CbArgumentPass.Name = "CbArgumentPass";
            this.CbArgumentPass.Size = new System.Drawing.Size(138, 20);
            this.CbArgumentPass.TabIndex = 0;
            this.CbArgumentPass.Text = "Suply Arguments";
            this.CbArgumentPass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CbArgumentPass.UseVisualStyleBackColor = true;
            // 
            // DropForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(138, 168);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CbArgumentPass);
            this.Controls.Add(this.panel1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::OldGamesLauncher.Properties.Settings.Default, "DropLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = global::OldGamesLauncher.Properties.Settings.Default.DropLocation;
            this.Name = "DropForm";
            this.Opacity = 0.8D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DropForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DropForm_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DropForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DropForm_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox CbArgumentPass;
    }
}