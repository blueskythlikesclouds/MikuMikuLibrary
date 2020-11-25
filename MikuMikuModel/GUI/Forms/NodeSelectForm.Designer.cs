namespace MikuMikuModel.GUI.Forms
{
    partial class NodeSelectForm<T> where T : class
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer mComponents = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mOkButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mNodeTreeView = new MikuMikuModel.Nodes.Wrappers.NodeTreeView();
            this.SuspendLayout();
            // 
            // mOkButton
            // 
            this.mOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point(277, 336);
            this.mOkButton.Name = "mOkButton";
            this.mOkButton.Size = new System.Drawing.Size(75, 23);
            this.mOkButton.TabIndex = 3;
            this.mOkButton.Text = "OK";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(196, 336);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 4;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mNodeTreeView
            // 
            this.mNodeTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mNodeTreeView.HideSelection = false;
            this.mNodeTreeView.ImageIndex = 0;
            this.mNodeTreeView.Location = new System.Drawing.Point(12, 12);
            this.mNodeTreeView.Name = "mNodeTreeView";
            this.mNodeTreeView.SelectedImageIndex = 0;
            this.mNodeTreeView.SelectedNode = null;
            this.mNodeTreeView.Size = new System.Drawing.Size(340, 318);
            this.mNodeTreeView.TabIndex = 0;
            this.mNodeTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnNodeMouseDoubleClick);
            // 
            // NodeSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 371);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOkButton);
            this.Controls.Add(this.mNodeTreeView);
            this.DoubleBuffered = true;
            this.Name = "NodeSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please select a node.";
            this.ResumeLayout(false);

        }

        #endregion

        private Nodes.Wrappers.NodeTreeView mNodeTreeView;
        private System.Windows.Forms.Button mOkButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.ComponentModel.IContainer components;
    }
}