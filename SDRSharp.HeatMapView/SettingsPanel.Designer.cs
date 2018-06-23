namespace SDRSharp.HeatMapView
{
    partial class SettingsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPanel));
      this.TopPanel = new System.Windows.Forms.Panel();
      this.HelpButton = new System.Windows.Forms.Button();
      this.EditButton = new System.Windows.Forms.Button();
      this.PlusButton = new System.Windows.Forms.Button();
      this.MinusButton = new System.Windows.Forms.Button();
      this.EnabledCheckBox = new System.Windows.Forms.CheckBox();
      this.ListBox = new System.Windows.Forms.ListBox();
      this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.TopPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // TopPanel
      // 
      this.TopPanel.Controls.Add(this.HelpButton);
      this.TopPanel.Controls.Add(this.EditButton);
      this.TopPanel.Controls.Add(this.PlusButton);
      this.TopPanel.Controls.Add(this.MinusButton);
      this.TopPanel.Controls.Add(this.EnabledCheckBox);
      this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.TopPanel.Location = new System.Drawing.Point(0, 0);
      this.TopPanel.Name = "TopPanel";
      this.TopPanel.Size = new System.Drawing.Size(250, 30);
      this.TopPanel.TabIndex = 2;
      // 
      // HelpButton
      // 
      this.HelpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.HelpButton.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton.Image")));
      this.HelpButton.Location = new System.Drawing.Point(231, 10);
      this.HelpButton.Name = "HelpButton";
      this.HelpButton.Size = new System.Drawing.Size(19, 19);
      this.HelpButton.TabIndex = 6;
      this.toolTip1.SetToolTip(this.HelpButton, "Online Help");
      this.HelpButton.UseVisualStyleBackColor = true;
      this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
      // 
      // EditButton
      // 
      this.EditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.EditButton.Image = ((System.Drawing.Image)(resources.GetObject("EditButton.Image")));
      this.EditButton.Location = new System.Drawing.Point(213, 10);
      this.EditButton.Name = "EditButton";
      this.EditButton.Size = new System.Drawing.Size(19, 19);
      this.EditButton.TabIndex = 5;
      this.toolTip1.SetToolTip(this.EditButton, "Rename heat map");
      this.EditButton.UseVisualStyleBackColor = true;
      this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
      // 
      // PlusButton
      // 
      this.PlusButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.PlusButton.Image = ((System.Drawing.Image)(resources.GetObject("PlusButton.Image")));
      this.PlusButton.Location = new System.Drawing.Point(177, 10);
      this.PlusButton.Name = "PlusButton";
      this.PlusButton.Size = new System.Drawing.Size(19, 19);
      this.PlusButton.TabIndex = 4;
      this.toolTip1.SetToolTip(this.PlusButton, "Create heat map from rtl_sdr data");
      this.PlusButton.UseVisualStyleBackColor = true;
      this.PlusButton.Click += new System.EventHandler(this.PlusButton_Click);
      // 
      // MinusButton
      // 
      this.MinusButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.MinusButton.Image = ((System.Drawing.Image)(resources.GetObject("MinusButton.Image")));
      this.MinusButton.Location = new System.Drawing.Point(195, 10);
      this.MinusButton.Name = "MinusButton";
      this.MinusButton.Size = new System.Drawing.Size(19, 19);
      this.MinusButton.TabIndex = 3;
      this.toolTip1.SetToolTip(this.MinusButton, "Delete heat nap");
      this.MinusButton.UseVisualStyleBackColor = true;
      this.MinusButton.Click += new System.EventHandler(this.MinusButton_Click);
      // 
      // EnabledCheckBox
      // 
      this.EnabledCheckBox.AutoSize = true;
      this.EnabledCheckBox.Location = new System.Drawing.Point(3, 3);
      this.EnabledCheckBox.Name = "EnabledCheckBox";
      this.EnabledCheckBox.Size = new System.Drawing.Size(65, 17);
      this.EnabledCheckBox.TabIndex = 2;
      this.EnabledCheckBox.Text = "Enabled";
      this.EnabledCheckBox.UseVisualStyleBackColor = true;
      this.EnabledCheckBox.CheckedChanged += new System.EventHandler(this.EnabledCheckBox_CheckedChanged);
      // 
      // ListBox
      // 
      this.ListBox.DisplayMember = "Name";
      this.ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ListBox.FormattingEnabled = true;
      this.ListBox.IntegralHeight = false;
      this.ListBox.Location = new System.Drawing.Point(0, 30);
      this.ListBox.Name = "ListBox";
      this.ListBox.Size = new System.Drawing.Size(250, 146);
      this.ListBox.TabIndex = 3;
      this.ListBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
      // 
      // OpenFileDialog
      // 
      this.OpenFileDialog.DefaultExt = "txt";
      this.OpenFileDialog.Filter = "rtl_power files (*.csv)|*.csv|All files (*.*)|*.*";
      this.OpenFileDialog.Title = "Import rtl_power Output File";
      // 
      // SettingsPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.ListBox);
      this.Controls.Add(this.TopPanel);
      this.Name = "SettingsPanel";
      this.Size = new System.Drawing.Size(250, 176);
      this.TopPanel.ResumeLayout(false);
      this.TopPanel.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Button PlusButton;
        private System.Windows.Forms.Button MinusButton;
        private System.Windows.Forms.CheckBox EnabledCheckBox;
        private System.Windows.Forms.ListBox ListBox;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.Button EditButton;
    private System.Windows.Forms.OpenFileDialog OpenFileDialog;
    private System.Windows.Forms.ToolTip toolTip1;
  }
}
