namespace MikuMikuModel.GUI.Forms
{
    partial class ConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer mComponents = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( mComponents != null ) )
            {
                mComponents.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mGroupBox1 = new System.Windows.Forms.GroupBox();
            this.mCreateButton = new System.Windows.Forms.Button();
            this.mRenameButton = new System.Windows.Forms.Button();
            this.mCloneButton = new System.Windows.Forms.Button();
            this.mReloadButton = new System.Windows.Forms.Button();
            this.mRemoveButton = new System.Windows.Forms.Button();
            this.mListBox = new System.Windows.Forms.ListBox();
            this.mGroupBox2 = new System.Windows.Forms.GroupBox();
            this.mMotionDatabaseLabel = new System.Windows.Forms.Label();
            this.mMotionDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.mMotionDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.mBoneDatabaseLabel = new System.Windows.Forms.Label();
            this.mTextureDatabaseLabel = new System.Windows.Forms.Label();
            this.mObjectDatabaseLabel = new System.Windows.Forms.Label();
            this.mBoneDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.mBoneDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.mTextureDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.mTextureDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.mObjectDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.mObjectDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.mOkButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mSearchButton = new System.Windows.Forms.Button();
            this.checkBoxAllowNullConfig = new System.Windows.Forms.CheckBox();
            this.mGroupBox1.SuspendLayout();
            this.mGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mGroupBox1
            // 
            this.mGroupBox1.Controls.Add(this.mCreateButton);
            this.mGroupBox1.Controls.Add(this.mRenameButton);
            this.mGroupBox1.Controls.Add(this.mCloneButton);
            this.mGroupBox1.Controls.Add(this.mReloadButton);
            this.mGroupBox1.Controls.Add(this.mRemoveButton);
            this.mGroupBox1.Controls.Add(this.mListBox);
            this.mGroupBox1.Location = new System.Drawing.Point(18, 18);
            this.mGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mGroupBox1.Name = "mGroupBox1";
            this.mGroupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mGroupBox1.Size = new System.Drawing.Size(615, 278);
            this.mGroupBox1.TabIndex = 0;
            this.mGroupBox1.TabStop = false;
            this.mGroupBox1.Text = "Configurations";
            // 
            // mCreateButton
            // 
            this.mCreateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCreateButton.Location = new System.Drawing.Point(9, 225);
            this.mCreateButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mCreateButton.Name = "mCreateButton";
            this.mCreateButton.Size = new System.Drawing.Size(112, 35);
            this.mCreateButton.TabIndex = 1;
            this.mCreateButton.Text = "Create";
            this.mCreateButton.UseVisualStyleBackColor = true;
            this.mCreateButton.Click += new System.EventHandler(this.OnCreate);
            // 
            // mRenameButton
            // 
            this.mRenameButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mRenameButton.Location = new System.Drawing.Point(252, 225);
            this.mRenameButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mRenameButton.Name = "mRenameButton";
            this.mRenameButton.Size = new System.Drawing.Size(112, 35);
            this.mRenameButton.TabIndex = 5;
            this.mRenameButton.Text = "Rename";
            this.mRenameButton.UseVisualStyleBackColor = true;
            this.mRenameButton.Click += new System.EventHandler(this.OnRename);
            // 
            // mCloneButton
            // 
            this.mCloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCloneButton.Location = new System.Drawing.Point(374, 225);
            this.mCloneButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mCloneButton.Name = "mCloneButton";
            this.mCloneButton.Size = new System.Drawing.Size(112, 35);
            this.mCloneButton.TabIndex = 2;
            this.mCloneButton.Text = "Clone";
            this.mCloneButton.UseVisualStyleBackColor = true;
            this.mCloneButton.Click += new System.EventHandler(this.OnClone);
            // 
            // mReloadButton
            // 
            this.mReloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mReloadButton.Location = new System.Drawing.Point(495, 225);
            this.mReloadButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mReloadButton.Name = "mReloadButton";
            this.mReloadButton.Size = new System.Drawing.Size(112, 35);
            this.mReloadButton.TabIndex = 4;
            this.mReloadButton.Text = "Reload";
            this.mReloadButton.UseVisualStyleBackColor = true;
            this.mReloadButton.Click += new System.EventHandler(this.OnReload);
            // 
            // mRemoveButton
            // 
            this.mRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mRemoveButton.Location = new System.Drawing.Point(130, 225);
            this.mRemoveButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mRemoveButton.Name = "mRemoveButton";
            this.mRemoveButton.Size = new System.Drawing.Size(112, 35);
            this.mRemoveButton.TabIndex = 3;
            this.mRemoveButton.Text = "Remove";
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler(this.OnRemove);
            // 
            // mListBox
            // 
            this.mListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mListBox.FormattingEnabled = true;
            this.mListBox.ItemHeight = 20;
            this.mListBox.Location = new System.Drawing.Point(9, 29);
            this.mListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mListBox.Name = "mListBox";
            this.mListBox.Size = new System.Drawing.Size(595, 184);
            this.mListBox.TabIndex = 0;
            this.mListBox.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // mGroupBox2
            // 
            this.mGroupBox2.Controls.Add(this.mMotionDatabaseLabel);
            this.mGroupBox2.Controls.Add(this.mMotionDatabasePathTextBox);
            this.mGroupBox2.Controls.Add(this.mMotionDatabaseBrowseButton);
            this.mGroupBox2.Controls.Add(this.mBoneDatabaseLabel);
            this.mGroupBox2.Controls.Add(this.mTextureDatabaseLabel);
            this.mGroupBox2.Controls.Add(this.mObjectDatabaseLabel);
            this.mGroupBox2.Controls.Add(this.mBoneDatabaseBrowseButton);
            this.mGroupBox2.Controls.Add(this.mBoneDatabasePathTextBox);
            this.mGroupBox2.Controls.Add(this.mTextureDatabaseBrowseButton);
            this.mGroupBox2.Controls.Add(this.mTextureDatabasePathTextBox);
            this.mGroupBox2.Controls.Add(this.mObjectDatabaseBrowseButton);
            this.mGroupBox2.Controls.Add(this.mObjectDatabasePathTextBox);
            this.mGroupBox2.Location = new System.Drawing.Point(18, 306);
            this.mGroupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mGroupBox2.Name = "mGroupBox2";
            this.mGroupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mGroupBox2.Size = new System.Drawing.Size(615, 209);
            this.mGroupBox2.TabIndex = 1;
            this.mGroupBox2.TabStop = false;
            this.mGroupBox2.Text = "Configuration";
            // 
            // mMotionDatabaseLabel
            // 
            this.mMotionDatabaseLabel.AutoSize = true;
            this.mMotionDatabaseLabel.Location = new System.Drawing.Point(10, 169);
            this.mMotionDatabaseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mMotionDatabaseLabel.Name = "mMotionDatabaseLabel";
            this.mMotionDatabaseLabel.Size = new System.Drawing.Size(135, 20);
            this.mMotionDatabaseLabel.TabIndex = 12;
            this.mMotionDatabaseLabel.Text = "Motion Database:";
            // 
            // mMotionDatabasePathTextBox
            // 
            this.mMotionDatabasePathTextBox.Enabled = false;
            this.mMotionDatabasePathTextBox.Location = new System.Drawing.Point(154, 165);
            this.mMotionDatabasePathTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mMotionDatabasePathTextBox.Name = "mMotionDatabasePathTextBox";
            this.mMotionDatabasePathTextBox.Size = new System.Drawing.Size(330, 26);
            this.mMotionDatabasePathTextBox.TabIndex = 11;
            this.mMotionDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnMotionDatabasePathTextBoxChanged);
            // 
            // mMotionDatabaseBrowseButton
            // 
            this.mMotionDatabaseBrowseButton.Location = new System.Drawing.Point(495, 163);
            this.mMotionDatabaseBrowseButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mMotionDatabaseBrowseButton.Name = "mMotionDatabaseBrowseButton";
            this.mMotionDatabaseBrowseButton.Size = new System.Drawing.Size(112, 34);
            this.mMotionDatabaseBrowseButton.TabIndex = 10;
            this.mMotionDatabaseBrowseButton.Text = "Browse";
            this.mMotionDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mMotionDatabaseBrowseButton.Click += new System.EventHandler(this.OnMotionDatabaseBrowse);
            // 
            // mBoneDatabaseLabel
            // 
            this.mBoneDatabaseLabel.AutoSize = true;
            this.mBoneDatabaseLabel.Location = new System.Drawing.Point(10, 126);
            this.mBoneDatabaseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mBoneDatabaseLabel.Name = "mBoneDatabaseLabel";
            this.mBoneDatabaseLabel.Size = new System.Drawing.Size(125, 20);
            this.mBoneDatabaseLabel.TabIndex = 9;
            this.mBoneDatabaseLabel.Text = "Bone Database:";
            // 
            // mTextureDatabaseLabel
            // 
            this.mTextureDatabaseLabel.AutoSize = true;
            this.mTextureDatabaseLabel.Location = new System.Drawing.Point(10, 83);
            this.mTextureDatabaseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mTextureDatabaseLabel.Name = "mTextureDatabaseLabel";
            this.mTextureDatabaseLabel.Size = new System.Drawing.Size(140, 20);
            this.mTextureDatabaseLabel.TabIndex = 8;
            this.mTextureDatabaseLabel.Text = "Texture Database:";
            // 
            // mObjectDatabaseLabel
            // 
            this.mObjectDatabaseLabel.AutoSize = true;
            this.mObjectDatabaseLabel.Location = new System.Drawing.Point(10, 42);
            this.mObjectDatabaseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mObjectDatabaseLabel.Name = "mObjectDatabaseLabel";
            this.mObjectDatabaseLabel.Size = new System.Drawing.Size(133, 20);
            this.mObjectDatabaseLabel.TabIndex = 7;
            this.mObjectDatabaseLabel.Text = "Object Database:";
            // 
            // mBoneDatabaseBrowseButton
            // 
            this.mBoneDatabaseBrowseButton.Location = new System.Drawing.Point(495, 120);
            this.mBoneDatabaseBrowseButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mBoneDatabaseBrowseButton.Name = "mBoneDatabaseBrowseButton";
            this.mBoneDatabaseBrowseButton.Size = new System.Drawing.Size(112, 34);
            this.mBoneDatabaseBrowseButton.TabIndex = 6;
            this.mBoneDatabaseBrowseButton.Text = "Browse";
            this.mBoneDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mBoneDatabaseBrowseButton.Click += new System.EventHandler(this.OnBoneDatabaseBrowse);
            // 
            // mBoneDatabasePathTextBox
            // 
            this.mBoneDatabasePathTextBox.Location = new System.Drawing.Point(154, 122);
            this.mBoneDatabasePathTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mBoneDatabasePathTextBox.Name = "mBoneDatabasePathTextBox";
            this.mBoneDatabasePathTextBox.Size = new System.Drawing.Size(330, 26);
            this.mBoneDatabasePathTextBox.TabIndex = 5;
            this.mBoneDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnBoneDatabasePathTextBoxTextChanged);
            // 
            // mTextureDatabaseBrowseButton
            // 
            this.mTextureDatabaseBrowseButton.Location = new System.Drawing.Point(495, 77);
            this.mTextureDatabaseBrowseButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mTextureDatabaseBrowseButton.Name = "mTextureDatabaseBrowseButton";
            this.mTextureDatabaseBrowseButton.Size = new System.Drawing.Size(112, 34);
            this.mTextureDatabaseBrowseButton.TabIndex = 4;
            this.mTextureDatabaseBrowseButton.Text = "Browse";
            this.mTextureDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mTextureDatabaseBrowseButton.Click += new System.EventHandler(this.OnTextureDatabaseBrowse);
            // 
            // mTextureDatabasePathTextBox
            // 
            this.mTextureDatabasePathTextBox.Location = new System.Drawing.Point(154, 78);
            this.mTextureDatabasePathTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mTextureDatabasePathTextBox.Name = "mTextureDatabasePathTextBox";
            this.mTextureDatabasePathTextBox.Size = new System.Drawing.Size(330, 26);
            this.mTextureDatabasePathTextBox.TabIndex = 3;
            this.mTextureDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnTextureDatabasePathTextBoxTextChanged);
            // 
            // mObjectDatabaseBrowseButton
            // 
            this.mObjectDatabaseBrowseButton.Location = new System.Drawing.Point(495, 34);
            this.mObjectDatabaseBrowseButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mObjectDatabaseBrowseButton.Name = "mObjectDatabaseBrowseButton";
            this.mObjectDatabaseBrowseButton.Size = new System.Drawing.Size(112, 34);
            this.mObjectDatabaseBrowseButton.TabIndex = 2;
            this.mObjectDatabaseBrowseButton.Text = "Browse";
            this.mObjectDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mObjectDatabaseBrowseButton.Click += new System.EventHandler(this.OnObjectDatabaseBrowse);
            // 
            // mObjectDatabasePathTextBox
            // 
            this.mObjectDatabasePathTextBox.Location = new System.Drawing.Point(154, 35);
            this.mObjectDatabasePathTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mObjectDatabasePathTextBox.Name = "mObjectDatabasePathTextBox";
            this.mObjectDatabasePathTextBox.Size = new System.Drawing.Size(330, 26);
            this.mObjectDatabasePathTextBox.TabIndex = 1;
            this.mObjectDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnObjectDatabasePathTextBoxTextChanged);
            // 
            // mOkButton
            // 
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point(520, 525);
            this.mOkButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mOkButton.Name = "mOkButton";
            this.mOkButton.Size = new System.Drawing.Size(112, 35);
            this.mOkButton.TabIndex = 2;
            this.mOkButton.Text = "OK";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(399, 525);
            this.mCancelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(112, 35);
            this.mCancelButton.TabIndex = 3;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mSearchButton
            // 
            this.mSearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mSearchButton.Enabled = false;
            this.mSearchButton.Location = new System.Drawing.Point(18, 526);
            this.mSearchButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mSearchButton.Name = "mSearchButton";
            this.mSearchButton.Size = new System.Drawing.Size(112, 34);
            this.mSearchButton.TabIndex = 13;
            this.mSearchButton.Text = "Search";
            this.mSearchButton.UseVisualStyleBackColor = true;
            this.mSearchButton.Click += new System.EventHandler(this.OnSearch);
            // 
            // checkBoxAllowNullConfig
            // 
            this.checkBoxAllowNullConfig.AutoSize = true;
            this.checkBoxAllowNullConfig.Location = new System.Drawing.Point(148, 531);
            this.checkBoxAllowNullConfig.Name = "checkBoxAllowNullConfig";
            this.checkBoxAllowNullConfig.Size = new System.Drawing.Size(196, 24);
            this.checkBoxAllowNullConfig.TabIndex = 14;
            this.checkBoxAllowNullConfig.Text = "Allow null configuration";
            this.checkBoxAllowNullConfig.UseVisualStyleBackColor = true;
            this.checkBoxAllowNullConfig.CheckedChanged += new System.EventHandler(this.CheckBoxAllowNullConfig_CheckedChanged);
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(651, 569);
            this.Controls.Add(this.checkBoxAllowNullConfig);
            this.Controls.Add(this.mSearchButton);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOkButton);
            this.Controls.Add(this.mGroupBox2);
            this.Controls.Add(this.mGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurations";
            this.mGroupBox1.ResumeLayout(false);
            this.mGroupBox2.ResumeLayout(false);
            this.mGroupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox mGroupBox1;
        private System.Windows.Forms.Button mRenameButton;
        private System.Windows.Forms.Button mReloadButton;
        private System.Windows.Forms.Button mRemoveButton;
        private System.Windows.Forms.Button mCloneButton;
        private System.Windows.Forms.ListBox mListBox;
        private System.Windows.Forms.Button mCreateButton;
        private System.Windows.Forms.GroupBox mGroupBox2;
        private System.Windows.Forms.Button mObjectDatabaseBrowseButton;
        private System.Windows.Forms.TextBox mObjectDatabasePathTextBox;
        private System.Windows.Forms.Label mBoneDatabaseLabel;
        private System.Windows.Forms.Label mTextureDatabaseLabel;
        private System.Windows.Forms.Label mObjectDatabaseLabel;
        private System.Windows.Forms.Button mBoneDatabaseBrowseButton;
        private System.Windows.Forms.TextBox mBoneDatabasePathTextBox;
        private System.Windows.Forms.Button mTextureDatabaseBrowseButton;
        private System.Windows.Forms.TextBox mTextureDatabasePathTextBox;
        private System.Windows.Forms.Button mOkButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label mMotionDatabaseLabel;
        private System.Windows.Forms.TextBox mMotionDatabasePathTextBox;
        private System.Windows.Forms.Button mMotionDatabaseBrowseButton;
        private System.Windows.Forms.Button mSearchButton;
        private System.Windows.Forms.CheckBox checkBoxAllowNullConfig;
    }
}