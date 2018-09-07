namespace MikuMikuModel.GUI.Forms
{
    partial class ConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.createButton = new System.Windows.Forms.Button();
            this.renameButton = new System.Windows.Forms.Button();
            this.cloneButton = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.boneDatabaseLabel = new System.Windows.Forms.Label();
            this.textureDatabaseLabel = new System.Windows.Forms.Label();
            this.objectDatabaseLabel = new System.Windows.Forms.Label();
            this.boneDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.boneDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.textureDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.textureDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.objectDatabaseBrowseButton = new System.Windows.Forms.Button();
            this.objectDatabasePathTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.createButton);
            this.groupBox1.Controls.Add(this.renameButton);
            this.groupBox1.Controls.Add(this.cloneButton);
            this.groupBox1.Controls.Add(this.reloadButton);
            this.groupBox1.Controls.Add(this.removeButton);
            this.groupBox1.Controls.Add(this.listBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 181);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configurations";
            // 
            // createButton
            // 
            this.createButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.createButton.Location = new System.Drawing.Point(6, 146);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 1;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.OnCreate);
            // 
            // renameButton
            // 
            this.renameButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renameButton.Location = new System.Drawing.Point(168, 146);
            this.renameButton.Name = "renameButton";
            this.renameButton.Size = new System.Drawing.Size(75, 23);
            this.renameButton.TabIndex = 5;
            this.renameButton.Text = "Rename";
            this.renameButton.UseVisualStyleBackColor = true;
            this.renameButton.Click += new System.EventHandler(this.OnRename);
            // 
            // cloneButton
            // 
            this.cloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cloneButton.Location = new System.Drawing.Point(249, 146);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(75, 23);
            this.cloneButton.TabIndex = 2;
            this.cloneButton.Text = "Clone";
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.OnClone);
            // 
            // reloadButton
            // 
            this.reloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reloadButton.Location = new System.Drawing.Point(330, 146);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(75, 23);
            this.reloadButton.TabIndex = 4;
            this.reloadButton.Text = "Reload";
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.OnReload);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(87, 146);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.OnRemove);
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(6, 19);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(398, 121);
            this.listBox.TabIndex = 0;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.boneDatabaseLabel);
            this.groupBox2.Controls.Add(this.textureDatabaseLabel);
            this.groupBox2.Controls.Add(this.objectDatabaseLabel);
            this.groupBox2.Controls.Add(this.boneDatabaseBrowseButton);
            this.groupBox2.Controls.Add(this.boneDatabasePathTextBox);
            this.groupBox2.Controls.Add(this.textureDatabaseBrowseButton);
            this.groupBox2.Controls.Add(this.textureDatabasePathTextBox);
            this.groupBox2.Controls.Add(this.objectDatabaseBrowseButton);
            this.groupBox2.Controls.Add(this.objectDatabasePathTextBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 199);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(410, 117);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // boneDatabaseLabel
            // 
            this.boneDatabaseLabel.AutoSize = true;
            this.boneDatabaseLabel.Location = new System.Drawing.Point(7, 82);
            this.boneDatabaseLabel.Name = "boneDatabaseLabel";
            this.boneDatabaseLabel.Size = new System.Drawing.Size(84, 13);
            this.boneDatabaseLabel.TabIndex = 9;
            this.boneDatabaseLabel.Text = "Bone Database:";
            // 
            // textureDatabaseLabel
            // 
            this.textureDatabaseLabel.AutoSize = true;
            this.textureDatabaseLabel.Location = new System.Drawing.Point(7, 54);
            this.textureDatabaseLabel.Name = "textureDatabaseLabel";
            this.textureDatabaseLabel.Size = new System.Drawing.Size(95, 13);
            this.textureDatabaseLabel.TabIndex = 8;
            this.textureDatabaseLabel.Text = "Texture Database:";
            // 
            // objectDatabaseLabel
            // 
            this.objectDatabaseLabel.AutoSize = true;
            this.objectDatabaseLabel.Location = new System.Drawing.Point(7, 27);
            this.objectDatabaseLabel.Name = "objectDatabaseLabel";
            this.objectDatabaseLabel.Size = new System.Drawing.Size(90, 13);
            this.objectDatabaseLabel.TabIndex = 7;
            this.objectDatabaseLabel.Text = "Object Database:";
            // 
            // boneDatabaseBrowseButton
            // 
            this.boneDatabaseBrowseButton.Location = new System.Drawing.Point(330, 78);
            this.boneDatabaseBrowseButton.Name = "boneDatabaseBrowseButton";
            this.boneDatabaseBrowseButton.Size = new System.Drawing.Size(75, 22);
            this.boneDatabaseBrowseButton.TabIndex = 6;
            this.boneDatabaseBrowseButton.Text = "Browse";
            this.boneDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.boneDatabaseBrowseButton.Click += new System.EventHandler(this.OnBoneDatabaseBrowse);
            // 
            // boneDatabasePathTextBox
            // 
            this.boneDatabasePathTextBox.Location = new System.Drawing.Point(103, 79);
            this.boneDatabasePathTextBox.Name = "boneDatabasePathTextBox";
            this.boneDatabasePathTextBox.Size = new System.Drawing.Size(221, 20);
            this.boneDatabasePathTextBox.TabIndex = 5;
            this.boneDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnBoneDatabasePathTextBoxTextChanged);
            // 
            // textureDatabaseBrowseButton
            // 
            this.textureDatabaseBrowseButton.Location = new System.Drawing.Point(330, 50);
            this.textureDatabaseBrowseButton.Name = "textureDatabaseBrowseButton";
            this.textureDatabaseBrowseButton.Size = new System.Drawing.Size(75, 22);
            this.textureDatabaseBrowseButton.TabIndex = 4;
            this.textureDatabaseBrowseButton.Text = "Browse";
            this.textureDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.textureDatabaseBrowseButton.Click += new System.EventHandler(this.OnTextureDatabaseBrowse);
            // 
            // textureDatabasePathTextBox
            // 
            this.textureDatabasePathTextBox.Location = new System.Drawing.Point(103, 51);
            this.textureDatabasePathTextBox.Name = "textureDatabasePathTextBox";
            this.textureDatabasePathTextBox.Size = new System.Drawing.Size(221, 20);
            this.textureDatabasePathTextBox.TabIndex = 3;
            this.textureDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnTextureDatabasePathTextBoxTextChanged);
            // 
            // objectDatabaseBrowseButton
            // 
            this.objectDatabaseBrowseButton.Location = new System.Drawing.Point(330, 22);
            this.objectDatabaseBrowseButton.Name = "objectDatabaseBrowseButton";
            this.objectDatabaseBrowseButton.Size = new System.Drawing.Size(75, 22);
            this.objectDatabaseBrowseButton.TabIndex = 2;
            this.objectDatabaseBrowseButton.Text = "Browse";
            this.objectDatabaseBrowseButton.UseVisualStyleBackColor = true;
            this.objectDatabaseBrowseButton.Click += new System.EventHandler(this.OnObjectDatabaseBrowse);
            // 
            // objectDatabasePathTextBox
            // 
            this.objectDatabasePathTextBox.Location = new System.Drawing.Point(103, 23);
            this.objectDatabasePathTextBox.Name = "objectDatabasePathTextBox";
            this.objectDatabasePathTextBox.Size = new System.Drawing.Size(221, 20);
            this.objectDatabasePathTextBox.TabIndex = 1;
            this.objectDatabasePathTextBox.TextChanged += new System.EventHandler(this.OnObjectDatabasePathTextBoxTextChanged);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(266, 322);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(347, 322);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(434, 354);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurations";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button renameButton;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button cloneButton;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button objectDatabaseBrowseButton;
        private System.Windows.Forms.TextBox objectDatabasePathTextBox;
        private System.Windows.Forms.Label boneDatabaseLabel;
        private System.Windows.Forms.Label textureDatabaseLabel;
        private System.Windows.Forms.Label objectDatabaseLabel;
        private System.Windows.Forms.Button boneDatabaseBrowseButton;
        private System.Windows.Forms.TextBox boneDatabasePathTextBox;
        private System.Windows.Forms.Button textureDatabaseBrowseButton;
        private System.Windows.Forms.TextBox textureDatabasePathTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}