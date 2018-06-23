namespace SDRSharp.HeatMapView
{
    partial class HeatMapPanel
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
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // toolTip1
      // 
      this.toolTip1.AutomaticDelay = 0;
      this.toolTip1.AutoPopDelay = 200000;
      this.toolTip1.InitialDelay = 200000;
      this.toolTip1.ReshowDelay = 200000;
      this.toolTip1.ShowAlways = true;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.DarkSlateGray;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.White;
      this.label1.Location = new System.Drawing.Point(7, 134);
      this.label1.MaximumSize = new System.Drawing.Size(0, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(13, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "_";
      this.label1.Visible = false;
      // 
      // HeatMapPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label1);
      this.Cursor = System.Windows.Forms.Cursors.Cross;
      this.DoubleBuffered = true;
      this.Name = "HeatMapPanel";
      this.Size = new System.Drawing.Size(743, 150);
      this.ClientSizeChanged += new System.EventHandler(this.HeatMapPanel_ClientSizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.HitMapViewFrontControl_Paint);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeatMapPanel_MouseDown);
      this.MouseLeave += new System.EventHandler(this.HeatMapPanel_MouseLeave);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeatMapPanel_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HeatMapPanel_MouseUp);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Label label1;
  }
}
