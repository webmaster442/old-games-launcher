namespace OldGamesLauncher
{
    partial class AddGameForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.TbGameName = new System.Windows.Forms.TextBox();
            this.TbGamePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnBrowse = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.OpenExeDialog = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.CbGameType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TbArgs = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game Name:";
            // 
            // TbGameName
            // 
            this.TbGameName.Location = new System.Drawing.Point(15, 25);
            this.TbGameName.Name = "TbGameName";
            this.TbGameName.Size = new System.Drawing.Size(387, 20);
            this.TbGameName.TabIndex = 1;
            // 
            // TbGamePath
            // 
            this.TbGamePath.Location = new System.Drawing.Point(15, 64);
            this.TbGamePath.Name = "TbGamePath";
            this.TbGamePath.Size = new System.Drawing.Size(306, 20);
            this.TbGamePath.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Game Exe Path:";
            // 
            // BtnBrowse
            // 
            this.BtnBrowse.Location = new System.Drawing.Point(327, 64);
            this.BtnBrowse.Name = "BtnBrowse";
            this.BtnBrowse.Size = new System.Drawing.Size(75, 23);
            this.BtnBrowse.TabIndex = 4;
            this.BtnBrowse.Text = "Browse ...";
            this.BtnBrowse.UseVisualStyleBackColor = true;
            this.BtnBrowse.Click += new System.EventHandler(this.button1_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(327, 140);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 5;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnAdd
            // 
            this.BtnAdd.Location = new System.Drawing.Point(246, 140);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(75, 23);
            this.BtnAdd.TabIndex = 6;
            this.BtnAdd.Text = "Ok";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // OpenExeDialog
            // 
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "GameType:";
            // 
            // CbGameType
            // 
            this.CbGameType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbGameType.FormattingEnabled = true;
            this.CbGameType.Items.AddRange(new object[] {
            "Windows",
            "Dos",
            "SNES"});
            this.CbGameType.Location = new System.Drawing.Point(71, 140);
            this.CbGameType.Name = "CbGameType";
            this.CbGameType.Size = new System.Drawing.Size(121, 21);
            this.CbGameType.TabIndex = 8;
            this.CbGameType.SelectedIndexChanged += new System.EventHandler(this.CbGameType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(241, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Additional arguments to pass to game or emulator:";
            // 
            // TbArgs
            // 
            this.TbArgs.Location = new System.Drawing.Point(15, 103);
            this.TbArgs.Name = "TbArgs";
            this.TbArgs.Size = new System.Drawing.Size(387, 20);
            this.TbArgs.TabIndex = 10;
            // 
            // AddGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(414, 175);
            this.Controls.Add(this.TbArgs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CbGameType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnAdd);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnBrowse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TbGamePath);
            this.Controls.Add(this.TbGameName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddGameForm";
            this.Text = "Add Game";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TbGameName;
        private System.Windows.Forms.TextBox TbGamePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnBrowse;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.OpenFileDialog OpenExeDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox CbGameType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TbArgs;
    }
}