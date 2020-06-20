namespace MikuMikuModel.GUI.Forms
{
    partial class ItemSelectForm<T>
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
            this.mOkButton = new System.Windows.Forms.Button();
            this.mCheckAllButton = new System.Windows.Forms.Button();
            this.mGroupBox = new System.Windows.Forms.GroupBox();
            this.mListView = new System.Windows.Forms.ListView();
            this.mColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mUncheckAllButton = new System.Windows.Forms.Button();
            this.mCheckSelectedButton = new System.Windows.Forms.Button();
            this.mUncheckSelectedButton = new System.Windows.Forms.Button();
            this.mCheckBox = new System.Windows.Forms.CheckBox();
            this.mGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mOkButton
            // 
            this.mOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point(397, 426);
            this.mOkButton.Name = "mOkButton";
            this.mOkButton.Size = new System.Drawing.Size(75, 23);
            this.mOkButton.TabIndex = 0;
            this.mOkButton.Text = "OK";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // mCheckAllButton
            // 
            this.mCheckAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mCheckAllButton.Location = new System.Drawing.Point(352, 28);
            this.mCheckAllButton.Name = "mCheckAllButton";
            this.mCheckAllButton.Size = new System.Drawing.Size(120, 23);
            this.mCheckAllButton.TabIndex = 2;
            this.mCheckAllButton.Text = "Check All";
            this.mCheckAllButton.UseVisualStyleBackColor = true;
            this.mCheckAllButton.Click += new System.EventHandler(this.OnCheckAll);
            // 
            // mMaterialsGroupBox
            // 
            this.mGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mGroupBox.Controls.Add(this.mListView);
            this.mGroupBox.Location = new System.Drawing.Point(12, 12);
            this.mGroupBox.Name = "mGroupBox";
            this.mGroupBox.Size = new System.Drawing.Size(334, 408);
            this.mGroupBox.TabIndex = 8;
            this.mGroupBox.TabStop = false;
            this.mGroupBox.Text = "Items";
            // 
            // mListView
            // 
            this.mListView.CheckBoxes = true;
            this.mListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mColumnHeader});
            this.mListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.mListView.HideSelection = false;
            this.mListView.Location = new System.Drawing.Point(3, 16);
            this.mListView.Name = "mListView";
            this.mListView.Size = new System.Drawing.Size(328, 389);
            this.mListView.TabIndex = 0;
            this.mListView.UseCompatibleStateImageBehavior = false;
            this.mListView.View = System.Windows.Forms.View.Details;
            // 
            // mColumnHeader
            // 
            this.mColumnHeader.Width = 25;
            // 
            // mUncheckAllButton
            // 
            this.mUncheckAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mUncheckAllButton.Location = new System.Drawing.Point(352, 57);
            this.mUncheckAllButton.Name = "mUncheckAllButton";
            this.mUncheckAllButton.Size = new System.Drawing.Size(120, 23);
            this.mUncheckAllButton.TabIndex = 9;
            this.mUncheckAllButton.Text = "Uncheck All";
            this.mUncheckAllButton.UseVisualStyleBackColor = true;
            this.mUncheckAllButton.Click += new System.EventHandler(this.OnUncheckAll);
            // 
            // mCheckSelectedButton
            // 
            this.mCheckSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mCheckSelectedButton.Location = new System.Drawing.Point(352, 86);
            this.mCheckSelectedButton.Name = "mCheckSelectedButton";
            this.mCheckSelectedButton.Size = new System.Drawing.Size(120, 23);
            this.mCheckSelectedButton.TabIndex = 10;
            this.mCheckSelectedButton.Text = "Check Selected";
            this.mCheckSelectedButton.UseVisualStyleBackColor = true;
            this.mCheckSelectedButton.Click += new System.EventHandler(this.OnCheckSelected);
            // 
            // mUncheckSelectedButton
            // 
            this.mUncheckSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mUncheckSelectedButton.Location = new System.Drawing.Point(352, 115);
            this.mUncheckSelectedButton.Name = "mUncheckSelectedButton";
            this.mUncheckSelectedButton.Size = new System.Drawing.Size(120, 23);
            this.mUncheckSelectedButton.TabIndex = 11;
            this.mUncheckSelectedButton.Text = "Uncheck Selected";
            this.mUncheckSelectedButton.UseVisualStyleBackColor = true;
            this.mUncheckSelectedButton.Click += new System.EventHandler(this.OnUncheckSelected);
            // 
            // mCheckBox
            // 
            this.mCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mCheckBox.AutoSize = true;
            this.mCheckBox.Location = new System.Drawing.Point(15, 430);
            this.mCheckBox.Name = "mCheckBox";
            this.mCheckBox.Size = new System.Drawing.Size(77, 17);
            this.mCheckBox.TabIndex = 12;
            this.mCheckBox.Text = "Check box";
            this.mCheckBox.UseVisualStyleBackColor = true;
            this.mCheckBox.Visible = false;
            // 
            // ItemSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.mCheckBox);
            this.Controls.Add(this.mUncheckSelectedButton);
            this.Controls.Add(this.mCheckSelectedButton);
            this.Controls.Add(this.mUncheckAllButton);
            this.Controls.Add(this.mGroupBox);
            this.Controls.Add(this.mCheckAllButton);
            this.Controls.Add(this.mOkButton);
            this.DoubleBuffered = true;
            this.Name = "ItemSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.mGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mOkButton;
        private System.Windows.Forms.Button mCheckAllButton;
        private System.Windows.Forms.GroupBox mGroupBox;
        private System.Windows.Forms.Button mUncheckAllButton;
        private System.Windows.Forms.Button mCheckSelectedButton;
        private System.Windows.Forms.Button mUncheckSelectedButton;
        private System.Windows.Forms.CheckBox mCheckBox;
        private System.Windows.Forms.ListView mListView;
        private System.Windows.Forms.ColumnHeader mColumnHeader;
    }
}