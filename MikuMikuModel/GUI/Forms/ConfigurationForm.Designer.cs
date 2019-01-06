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
            this.mGroupBox1.SuspendLayout();
            this.mGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.mGroupBox1.Controls.Add( this.mCreateButton );
            this.mGroupBox1.Controls.Add( this.mRenameButton );
            this.mGroupBox1.Controls.Add( this.mCloneButton );
            this.mGroupBox1.Controls.Add( this.mReloadButton );
            this.mGroupBox1.Controls.Add( this.mRemoveButton );
            this.mGroupBox1.Controls.Add( this.mListBox );
            this.mGroupBox1.Location = new System.Drawing.Point( 12, 12 );
            this.mGroupBox1.Name = "groupBox1";
            this.mGroupBox1.Size = new System.Drawing.Size( 410, 181 );
            this.mGroupBox1.TabIndex = 0;
            this.mGroupBox1.TabStop = false;
            this.mGroupBox1.Text = "Configurations";
            // 
            // createButton
            // 
            this.mCreateButton.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mCreateButton.Location = new System.Drawing.Point( 6, 146 );
            this.mCreateButton.Name = "createButton";
            this.mCreateButton.Size = new System.Drawing.Size( 75, 23 );
            this.mCreateButton.TabIndex = 1;
            this.mCreateButton.Text = "Create";
            this.mCreateButton.UseVisualStyleBackColor = true;
            this.mCreateButton.Click += new System.EventHandler( this.OnCreate );
            // 
            // renameButton
            // 
            this.mRenameButton.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mRenameButton.Location = new System.Drawing.Point( 168, 146 );
            this.mRenameButton.Name = "renameButton";
            this.mRenameButton.Size = new System.Drawing.Size( 75, 23 );
            this.mRenameButton.TabIndex = 5;
            this.mRenameButton.Text = "Rename";
            this.mRenameButton.UseVisualStyleBackColor = true;
            this.mRenameButton.Click += new System.EventHandler( this.OnRename );
            // 
            // cloneButton
            // 
            this.mCloneButton.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mCloneButton.Location = new System.Drawing.Point( 249, 146 );
            this.mCloneButton.Name = "cloneButton";
            this.mCloneButton.Size = new System.Drawing.Size( 75, 23 );
            this.mCloneButton.TabIndex = 2;
            this.mCloneButton.Text = "Clone";
            this.mCloneButton.UseVisualStyleBackColor = true;
            this.mCloneButton.Click += new System.EventHandler( this.OnClone );
            // 
            // reloadButton
            // 
            this.mReloadButton.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mReloadButton.Location = new System.Drawing.Point( 330, 146 );
            this.mReloadButton.Name = "reloadButton";
            this.mReloadButton.Size = new System.Drawing.Size( 75, 23 );
            this.mReloadButton.TabIndex = 4;
            this.mReloadButton.Text = "Reload";
            this.mReloadButton.UseVisualStyleBackColor = true;
            this.mReloadButton.Click += new System.EventHandler( this.OnReload );
            // 
            // removeButton
            // 
            this.mRemoveButton.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mRemoveButton.Location = new System.Drawing.Point( 87, 146 );
            this.mRemoveButton.Name = "removeButton";
            this.mRemoveButton.Size = new System.Drawing.Size( 75, 23 );
            this.mRemoveButton.TabIndex = 3;
            this.mRemoveButton.Text = "Remove";
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler( this.OnRemove );
            // 
            // listBox
            // 
            this.mListBox.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mListBox.FormattingEnabled = true;
            this.mListBox.Location = new System.Drawing.Point( 6, 19 );
            this.mListBox.Name = "listBox";
            this.mListBox.Size = new System.Drawing.Size( 398, 121 );
            this.mListBox.TabIndex = 0;
            this.mListBox.SelectedIndexChanged += new System.EventHandler( this.OnSelectedIndexChanged );
            // 
            // groupBox2
            // 
            this.mGroupBox2.Controls.Add( this.mBoneDatabaseLabel );
            this.mGroupBox2.Controls.Add( this.mTextureDatabaseLabel );
            this.mGroupBox2.Controls.Add( this.mObjectDatabaseLabel );
            this.mGroupBox2.Controls.Add( this.mBoneDatabaseBrowseButton );
            this.mGroupBox2.Controls.Add( this.mBoneDatabasePathTextBox );
            this.mGroupBox2.Controls.Add( this.mTextureDatabaseBrowseButton );
            this.mGroupBox2.Controls.Add( this.mTextureDatabasePathTextBox );
            this.mGroupBox2.Controls.Add( this.mObjectDatabaseBrowseButton );
            this.mGroupBox2.Controls.Add( this.mObjectDatabasePathTextBox );
            this.mGroupBox2.Location = new System.Drawing.Point( 12, 199 );
            this.mGroupBox2.Name = "groupBox2";
            this.mGroupBox2.Size = new System.Drawing.Size( 410, 117 );
            this.mGroupBox2.TabIndex = 1;
            this.mGroupBox2.TabStop = false;
            this.mGroupBox2.Text = "Configuration";
            // 
            // boneDatabaseLabel
            // 
            this.mBoneDatabaseLabel.AutoSize = true;
            this.mBoneDatabaseLabel.Location = new System.Drawing.Point( 7, 82 );
            this.mBoneDatabaseLabel.Name = "boneDatabaseLabel";
            this.mBoneDatabaseLabel.Size = new System.Drawing.Size( 84, 13 );
            this.mBoneDatabaseLabel.TabIndex = 9;
            this.mBoneDatabaseLabel.Text = "Bone Database:";
            // 
            // textureDatabaseLabel
            // 
            this.mTextureDatabaseLabel.AutoSize = true;
            this.mTextureDatabaseLabel.Location = new System.Drawing.Point( 7, 54 );
            this.mTextureDatabaseLabel.Name = "textureDatabaseLabel";
            this.mTextureDatabaseLabel.Size = new System.Drawing.Size( 95, 13 );
            this.mTextureDatabaseLabel.TabIndex = 8;
            this.mTextureDatabaseLabel.Text = "Texture Database:";
            // 
            // objectDatabaseLabel
            // 
            this.mObjectDatabaseLabel.AutoSize = true;
            this.mObjectDatabaseLabel.Location = new System.Drawing.Point( 7, 27 );
            this.mObjectDatabaseLabel.Name = "objectDatabaseLabel";
            this.mObjectDatabaseLabel.Size = new System.Drawing.Size( 90, 13 );
            this.mObjectDatabaseLabel.TabIndex = 7;
            this.mObjectDatabaseLabel.Text = "Object Database:";
            // 
            // boneDatabaseBrowseButton
            // 
            this.mBoneDatabaseBrowseButton.Location = new System.Drawing.Point( 330, 78 );
            this.mBoneDatabaseBrowseButton.Name = "boneDatabaseBrowseButton";
            this.mBoneDatabaseBrowseButton.Size = new System.Drawing.Size( 75, 22 );
            this.mBoneDatabaseBrowseButton.TabIndex = 6;
            this.mBoneDatabaseBrowseButton.Text = "Browse";
            this.mBoneDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mBoneDatabaseBrowseButton.Click += new System.EventHandler( this.OnBoneDatabaseBrowse );
            // 
            // boneDatabasePathTextBox
            // 
            this.mBoneDatabasePathTextBox.Location = new System.Drawing.Point( 103, 79 );
            this.mBoneDatabasePathTextBox.Name = "boneDatabasePathTextBox";
            this.mBoneDatabasePathTextBox.Size = new System.Drawing.Size( 221, 20 );
            this.mBoneDatabasePathTextBox.TabIndex = 5;
            this.mBoneDatabasePathTextBox.TextChanged += new System.EventHandler( this.OnBoneDatabasePathTextBoxTextChanged );
            // 
            // textureDatabaseBrowseButton
            // 
            this.mTextureDatabaseBrowseButton.Location = new System.Drawing.Point( 330, 50 );
            this.mTextureDatabaseBrowseButton.Name = "textureDatabaseBrowseButton";
            this.mTextureDatabaseBrowseButton.Size = new System.Drawing.Size( 75, 22 );
            this.mTextureDatabaseBrowseButton.TabIndex = 4;
            this.mTextureDatabaseBrowseButton.Text = "Browse";
            this.mTextureDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mTextureDatabaseBrowseButton.Click += new System.EventHandler( this.OnTextureDatabaseBrowse );
            // 
            // textureDatabasePathTextBox
            // 
            this.mTextureDatabasePathTextBox.Location = new System.Drawing.Point( 103, 51 );
            this.mTextureDatabasePathTextBox.Name = "textureDatabasePathTextBox";
            this.mTextureDatabasePathTextBox.Size = new System.Drawing.Size( 221, 20 );
            this.mTextureDatabasePathTextBox.TabIndex = 3;
            this.mTextureDatabasePathTextBox.TextChanged += new System.EventHandler( this.OnTextureDatabasePathTextBoxTextChanged );
            // 
            // objectDatabaseBrowseButton
            // 
            this.mObjectDatabaseBrowseButton.Location = new System.Drawing.Point( 330, 22 );
            this.mObjectDatabaseBrowseButton.Name = "objectDatabaseBrowseButton";
            this.mObjectDatabaseBrowseButton.Size = new System.Drawing.Size( 75, 22 );
            this.mObjectDatabaseBrowseButton.TabIndex = 2;
            this.mObjectDatabaseBrowseButton.Text = "Browse";
            this.mObjectDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.mObjectDatabaseBrowseButton.Click += new System.EventHandler( this.OnObjectDatabaseBrowse );
            // 
            // objectDatabasePathTextBox
            // 
            this.mObjectDatabasePathTextBox.Location = new System.Drawing.Point( 103, 23 );
            this.mObjectDatabasePathTextBox.Name = "objectDatabasePathTextBox";
            this.mObjectDatabasePathTextBox.Size = new System.Drawing.Size( 221, 20 );
            this.mObjectDatabasePathTextBox.TabIndex = 1;
            this.mObjectDatabasePathTextBox.TextChanged += new System.EventHandler( this.OnObjectDatabasePathTextBoxTextChanged );
            // 
            // okButton
            // 
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point( 266, 322 );
            this.mOkButton.Name = "okButton";
            this.mOkButton.Size = new System.Drawing.Size( 75, 23 );
            this.mOkButton.TabIndex = 2;
            this.mOkButton.Text = "OK";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point( 347, 322 );
            this.mCancelButton.Name = "cancelButton";
            this.mCancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.mCancelButton.TabIndex = 3;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size( 434, 354 );
            this.Controls.Add( this.mCancelButton );
            this.Controls.Add( this.mOkButton );
            this.Controls.Add( this.mGroupBox2 );
            this.Controls.Add( this.mGroupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurations";
            this.mGroupBox1.ResumeLayout( false );
            this.mGroupBox2.ResumeLayout( false );
            this.mGroupBox2.PerformLayout();
            this.ResumeLayout( false );

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
    }
}