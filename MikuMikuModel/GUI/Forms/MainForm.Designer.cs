namespace MikuMikuModel.GUI.Forms
{
    partial class MainForm
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
            this.mComponents = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
            this.mMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mRightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mTreeView = new MikuMikuModel.DataNodes.Wrappers.DataTreeView();
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.mMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mConfigurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPanel = new System.Windows.Forms.Panel();
            ( ( System.ComponentModel.ISupportInitialize )( this.mMainSplitContainer ) ).BeginInit();
            this.mMainSplitContainer.Panel2.SuspendLayout();
            this.mMainSplitContainer.SuspendLayout();
            ( ( System.ComponentModel.ISupportInitialize )( this.mRightSplitContainer ) ).BeginInit();
            this.mRightSplitContainer.Panel1.SuspendLayout();
            this.mRightSplitContainer.Panel2.SuspendLayout();
            this.mRightSplitContainer.SuspendLayout();
            this.mMenuStrip.SuspendLayout();
            this.mPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mMainSplitContainer.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mMainSplitContainer.Location = new System.Drawing.Point( 12, 31 );
            this.mMainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel2
            // 
            this.mMainSplitContainer.Panel2.Controls.Add( this.mRightSplitContainer );
            this.mMainSplitContainer.Size = new System.Drawing.Size( 712, 397 );
            this.mMainSplitContainer.SplitterDistance = 448;
            this.mMainSplitContainer.TabIndex = 0;
            // 
            // rightSplitContainer
            // 
            this.mRightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mRightSplitContainer.Location = new System.Drawing.Point( 0, 0 );
            this.mRightSplitContainer.Name = "rightSplitContainer";
            this.mRightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightSplitContainer.Panel1
            // 
            this.mRightSplitContainer.Panel1.Controls.Add( this.mTreeView );
            // 
            // rightSplitContainer.Panel2
            // 
            this.mRightSplitContainer.Panel2.Controls.Add( this.mPropertyGrid );
            this.mRightSplitContainer.Size = new System.Drawing.Size( 260, 397 );
            this.mRightSplitContainer.SplitterDistance = 181;
            this.mRightSplitContainer.TabIndex = 0;
            // 
            // treeView
            // 
            this.mTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTreeView.ImageIndex = 0;
            this.mTreeView.Location = new System.Drawing.Point( 0, 0 );
            this.mTreeView.Name = "treeView";
            this.mTreeView.SelectedImageIndex = 0;
            this.mTreeView.SelectedNode = null;
            this.mTreeView.Size = new System.Drawing.Size( 260, 181 );
            this.mTreeView.TabIndex = 0;
            this.mTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.OnAfterSelect );
            // 
            // propertyGrid
            // 
            this.mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPropertyGrid.HelpVisible = false;
            this.mPropertyGrid.Location = new System.Drawing.Point( 0, 0 );
            this.mPropertyGrid.Name = "propertyGrid";
            this.mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.mPropertyGrid.Size = new System.Drawing.Size( 260, 212 );
            this.mPropertyGrid.TabIndex = 0;
            this.mPropertyGrid.ToolbarVisible = false;
            this.mPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler( this.OnPropertyValueChanged );
            // 
            // menuStrip
            // 
            this.mMenuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMenuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.mConfigurationsToolStripMenuItem,
            this.mHelpToolStripMenuItem,
            this.mAboutToolStripMenuItem} );
            this.mMenuStrip.Location = new System.Drawing.Point( 0, 0 );
            this.mMenuStrip.Name = "menuStrip";
            this.mMenuStrip.Size = new System.Drawing.Size( 736, 25 );
            this.mMenuStrip.TabIndex = 0;
            this.mMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.mFileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.mOpenToolStripMenuItem,
            this.mSaveToolStripMenuItem,
            this.mSaveAsToolStripMenuItem,
            this.mCloseToolStripMenuItem,
            this.mToolStripSeparator2,
            this.mExitToolStripMenuItem} );
            this.mFileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size( 37, 21 );
            this.mFileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.mOpenToolStripMenuItem.Name = "openToolStripMenuItem";
            this.mOpenToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys )( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.mOpenToolStripMenuItem.Size = new System.Drawing.Size( 186, 22 );
            this.mOpenToolStripMenuItem.Text = "Open";
            this.mOpenToolStripMenuItem.Click += new System.EventHandler( this.OnOpen );
            // 
            // saveToolStripMenuItem
            // 
            this.mSaveToolStripMenuItem.Enabled = false;
            this.mSaveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.mSaveToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys )( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S ) ) );
            this.mSaveToolStripMenuItem.Size = new System.Drawing.Size( 186, 22 );
            this.mSaveToolStripMenuItem.Text = "Save";
            this.mSaveToolStripMenuItem.Click += new System.EventHandler( this.OnSave );
            // 
            // saveAsToolStripMenuItem
            // 
            this.mSaveAsToolStripMenuItem.Enabled = false;
            this.mSaveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.mSaveAsToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys )( ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift )
            | System.Windows.Forms.Keys.S ) ) );
            this.mSaveAsToolStripMenuItem.Size = new System.Drawing.Size( 186, 22 );
            this.mSaveAsToolStripMenuItem.Text = "Save As";
            this.mSaveAsToolStripMenuItem.Click += new System.EventHandler( this.OnSaveAs );
            // 
            // closeToolStripMenuItem
            // 
            this.mCloseToolStripMenuItem.Enabled = false;
            this.mCloseToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.mCloseToolStripMenuItem.Size = new System.Drawing.Size( 186, 22 );
            this.mCloseToolStripMenuItem.Text = "Close";
            this.mCloseToolStripMenuItem.Click += new System.EventHandler( this.OnNodeClose );
            // 
            // toolStripSeparator2
            // 
            this.mToolStripSeparator2.Name = "toolStripSeparator2";
            this.mToolStripSeparator2.Size = new System.Drawing.Size( 183, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.mExitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.mExitToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys )( ( System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4 ) ) );
            this.mExitToolStripMenuItem.Size = new System.Drawing.Size( 186, 22 );
            this.mExitToolStripMenuItem.Text = "Exit";
            this.mExitToolStripMenuItem.Click += new System.EventHandler( this.OnExit );
            // 
            // configurationsToolStripMenuItem
            // 
            this.mConfigurationsToolStripMenuItem.Name = "configurationsToolStripMenuItem";
            this.mConfigurationsToolStripMenuItem.Size = new System.Drawing.Size( 98, 21 );
            this.mConfigurationsToolStripMenuItem.Text = "Configurations";
            this.mConfigurationsToolStripMenuItem.Click += new System.EventHandler( this.OnConfigurations );
            // 
            // helpToolStripMenuItem
            // 
            this.mHelpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.mHelpToolStripMenuItem.Size = new System.Drawing.Size( 44, 21 );
            this.mHelpToolStripMenuItem.Text = "Help";
            this.mHelpToolStripMenuItem.Click += new System.EventHandler( this.OnHelp );
            // 
            // aboutToolStripMenuItem
            // 
            this.mAboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.mAboutToolStripMenuItem.Size = new System.Drawing.Size( 52, 21 );
            this.mAboutToolStripMenuItem.Text = "About";
            this.mAboutToolStripMenuItem.Click += new System.EventHandler( this.OnAbout );
            // 
            // panel
            // 
            this.mPanel.Controls.Add( this.mMenuStrip );
            this.mPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mPanel.Location = new System.Drawing.Point( 0, 0 );
            this.mPanel.Name = "panel";
            this.mPanel.Size = new System.Drawing.Size( 736, 25 );
            this.mPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 736, 440 );
            this.Controls.Add( this.mPanel );
            this.Controls.Add( this.mMainSplitContainer );
            this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
            this.Name = "MainForm";
            this.Text = "Miku Miku Model";
            this.mMainSplitContainer.Panel2.ResumeLayout( false );
            ( ( System.ComponentModel.ISupportInitialize )( this.mMainSplitContainer ) ).EndInit();
            this.mMainSplitContainer.ResumeLayout( false );
            this.mRightSplitContainer.Panel1.ResumeLayout( false );
            this.mRightSplitContainer.Panel2.ResumeLayout( false );
            ( ( System.ComponentModel.ISupportInitialize )( this.mRightSplitContainer ) ).EndInit();
            this.mRightSplitContainer.ResumeLayout( false );
            this.mMenuStrip.ResumeLayout( false );
            this.mMenuStrip.PerformLayout();
            this.mPanel.ResumeLayout( false );
            this.mPanel.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.SplitContainer mMainSplitContainer;
        private System.Windows.Forms.MenuStrip mMenuStrip;
        private System.Windows.Forms.SplitContainer mRightSplitContainer;
        private System.Windows.Forms.ToolStripMenuItem mFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator mToolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mExitToolStripMenuItem;
        private System.Windows.Forms.Panel mPanel;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private MikuMikuModel.DataNodes.Wrappers.DataTreeView mTreeView;
        private System.Windows.Forms.ToolStripMenuItem mCloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mConfigurationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mHelpToolStripMenuItem;
    }
}
