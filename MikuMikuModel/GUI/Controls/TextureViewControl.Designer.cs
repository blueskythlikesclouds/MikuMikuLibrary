namespace MikuMikuModel.GUI.Controls
{
    partial class TextureViewControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.formatLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sizeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mipMapLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.levelLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formatLabel,
            this.sizeLabel,
            this.mipMapLabel,
            this.levelLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 340);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(538, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // formatLabel
            // 
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(130, 17);
            this.formatLabel.Spring = true;
            this.formatLabel.Text = "Format";
            // 
            // sizeLabel
            // 
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(130, 17);
            this.sizeLabel.Spring = true;
            this.sizeLabel.Text = "Size";
            // 
            // mipMapLabel
            // 
            this.mipMapLabel.Name = "mipMapLabel";
            this.mipMapLabel.Size = new System.Drawing.Size(130, 17);
            this.mipMapLabel.Spring = true;
            this.mipMapLabel.Text = "MipMap";
            // 
            // levelLabel
            // 
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(130, 17);
            this.levelLabel.Spring = true;
            this.levelLabel.Text = "Level";
            // 
            // TextureViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip);
            this.Name = "TextureViewControl";
            this.Size = new System.Drawing.Size(538, 362);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel formatLabel;
        private System.Windows.Forms.ToolStripStatusLabel sizeLabel;
        private System.Windows.Forms.ToolStripStatusLabel mipMapLabel;
        private System.Windows.Forms.ToolStripStatusLabel levelLabel;
    }
}
