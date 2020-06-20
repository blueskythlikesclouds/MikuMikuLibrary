namespace MikuMikuModel.GUI.Forms
{
    partial class FirstLaunchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstLaunchForm));
            this.mMikuMikuModelPictureBox = new System.Windows.Forms.PictureBox();
            this.mMikuMikuModelLabel = new System.Windows.Forms.Label();
            this.mDescriptionLabel = new System.Windows.Forms.Label();
            this.mOkButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mMikuMikuModelPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mMikuMikuModelPictureBox
            // 
            this.mMikuMikuModelPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mMikuMikuModelPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("mMikuMikuModelPictureBox.Image")));
            this.mMikuMikuModelPictureBox.Location = new System.Drawing.Point(69, 10);
            this.mMikuMikuModelPictureBox.Name = "mMikuMikuModelPictureBox";
            this.mMikuMikuModelPictureBox.Size = new System.Drawing.Size(32, 32);
            this.mMikuMikuModelPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.mMikuMikuModelPictureBox.TabIndex = 0;
            this.mMikuMikuModelPictureBox.TabStop = false;
            // 
            // mMikuMikuModelLabel
            // 
            this.mMikuMikuModelLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mMikuMikuModelLabel.AutoSize = true;
            this.mMikuMikuModelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.mMikuMikuModelLabel.Location = new System.Drawing.Point(113, 10);
            this.mMikuMikuModelLabel.Name = "mMikuMikuModelLabel";
            this.mMikuMikuModelLabel.Size = new System.Drawing.Size(242, 32);
            this.mMikuMikuModelLabel.TabIndex = 1;
            this.mMikuMikuModelLabel.Text = "Miku Miku Model";
            // 
            // mDescriptionLabel
            // 
            this.mDescriptionLabel.AutoSize = true;
            this.mDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.mDescriptionLabel.Location = new System.Drawing.Point(12, 45);
            this.mDescriptionLabel.Name = "mDescriptionLabel";
            this.mDescriptionLabel.Size = new System.Drawing.Size(399, 102);
            this.mDescriptionLabel.TabIndex = 2;
            this.mDescriptionLabel.Text = resources.GetString("mDescriptionLabel.Text");
            // 
            // mOkButton
            // 
            this.mOkButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point(69, 164);
            this.mOkButton.Name = "mOkButton";
            this.mOkButton.Size = new System.Drawing.Size(286, 23);
            this.mOkButton.TabIndex = 3;
            this.mOkButton.Text = "OK";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // FirstLaunchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 199);
            this.Controls.Add(this.mOkButton);
            this.Controls.Add(this.mDescriptionLabel);
            this.Controls.Add(this.mMikuMikuModelLabel);
            this.Controls.Add(this.mMikuMikuModelPictureBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstLaunchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome to Miku Miku Model";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.mMikuMikuModelPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mMikuMikuModelPictureBox;
        private System.Windows.Forms.Label mMikuMikuModelLabel;
        private System.Windows.Forms.Label mDescriptionLabel;
        private System.Windows.Forms.Button mOkButton;
    }
}