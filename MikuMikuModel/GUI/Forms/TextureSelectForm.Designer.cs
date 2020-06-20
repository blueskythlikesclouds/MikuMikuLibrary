namespace MikuMikuModel.GUI.Forms
{
    partial class TextureSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mOkButton = new System.Windows.Forms.Button();
            this.mPanel = new System.Windows.Forms.Panel();
            this.mMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mNodeTreeView = new MikuMikuModel.Nodes.Wrappers.NodeTreeView();
            this.mMaterialTextureTypeLabel = new System.Windows.Forms.Label();
            this.mMaterialTextureTypeComboBox = new System.Windows.Forms.ComboBox();
            this.mPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mMainSplitContainer)).BeginInit();
            this.mMainSplitContainer.Panel2.SuspendLayout();
            this.mMainSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(697, 526);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 0;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mOkButton
            // 
            this.mOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point(616, 526);
            this.mOkButton.Name = "mOkButton";
            this.mOkButton.Size = new System.Drawing.Size(75, 23);
            this.mOkButton.TabIndex = 1;
            this.mOkButton.Text = "OK";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // mPanel
            // 
            this.mPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mPanel.Controls.Add(this.mMainSplitContainer);
            this.mPanel.Location = new System.Drawing.Point(12, 12);
            this.mPanel.Name = "mPanel";
            this.mPanel.Size = new System.Drawing.Size(760, 508);
            this.mPanel.TabIndex = 2;
            // 
            // mMainSplitContainer
            // 
            this.mMainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mMainSplitContainer.Name = "mMainSplitContainer";
            // 
            // mMainSplitContainer.Panel2
            // 
            this.mMainSplitContainer.Panel2.Controls.Add(this.mNodeTreeView);
            this.mMainSplitContainer.Size = new System.Drawing.Size(760, 508);
            this.mMainSplitContainer.SplitterDistance = 499;
            this.mMainSplitContainer.TabIndex = 0;
            // 
            // mNodeTreeView
            // 
            this.mNodeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mNodeTreeView.HideSelection = false;
            this.mNodeTreeView.ImageIndex = 0;
            this.mNodeTreeView.Location = new System.Drawing.Point(0, 0);
            this.mNodeTreeView.Name = "mNodeTreeView";
            this.mNodeTreeView.SelectedImageIndex = 0;
            this.mNodeTreeView.SelectedNode = null;
            this.mNodeTreeView.Size = new System.Drawing.Size(257, 508);
            this.mNodeTreeView.TabIndex = 0;
            this.mNodeTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnAfterSelect);
            this.mNodeTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnNodeMouseDoubleClick);
            // 
            // mMaterialTextureTypeLabel
            // 
            this.mMaterialTextureTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mMaterialTextureTypeLabel.AutoSize = true;
            this.mMaterialTextureTypeLabel.Location = new System.Drawing.Point(12, 531);
            this.mMaterialTextureTypeLabel.Name = "mMaterialTextureTypeLabel";
            this.mMaterialTextureTypeLabel.Size = new System.Drawing.Size(105, 13);
            this.mMaterialTextureTypeLabel.TabIndex = 3;
            this.mMaterialTextureTypeLabel.Text = "Material texture type:";
            this.mMaterialTextureTypeLabel.Visible = false;
            // 
            // mMaterialTextureTypeComboBox
            // 
            this.mMaterialTextureTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mMaterialTextureTypeComboBox.FormattingEnabled = true;
            this.mMaterialTextureTypeComboBox.Location = new System.Drawing.Point(123, 527);
            this.mMaterialTextureTypeComboBox.Name = "mMaterialTextureTypeComboBox";
            this.mMaterialTextureTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.mMaterialTextureTypeComboBox.TabIndex = 4;
            this.mMaterialTextureTypeComboBox.Visible = false;
            // 
            // TextureSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.mMaterialTextureTypeComboBox);
            this.Controls.Add(this.mMaterialTextureTypeLabel);
            this.Controls.Add(this.mPanel);
            this.Controls.Add(this.mOkButton);
            this.Controls.Add(this.mCancelButton);
            this.DoubleBuffered = true;
            this.Name = "TextureSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please select a texture.";
            this.mPanel.ResumeLayout(false);
            this.mMainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mMainSplitContainer)).EndInit();
            this.mMainSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mOkButton;
        private System.Windows.Forms.Panel mPanel;
        private System.Windows.Forms.SplitContainer mMainSplitContainer;
        private Nodes.Wrappers.NodeTreeView mNodeTreeView;
        private System.Windows.Forms.Label mMaterialTextureTypeLabel;
        private System.Windows.Forms.ComboBox mMaterialTextureTypeComboBox;
    }
}