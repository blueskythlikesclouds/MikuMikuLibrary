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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mRightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mNodeTreeView = new MikuMikuModel.Nodes.Wrappers.NodeTreeView();
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
            this.mToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCombineMotsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mStylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.mMainSplitContainer)).BeginInit();
            this.mMainSplitContainer.Panel2.SuspendLayout();
            this.mMainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mRightSplitContainer)).BeginInit();
            this.mRightSplitContainer.Panel1.SuspendLayout();
            this.mRightSplitContainer.Panel2.SuspendLayout();
            this.mRightSplitContainer.SuspendLayout();
            this.mMenuStrip.SuspendLayout();
            this.mPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMainSplitContainer
            // 
            this.mMainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mMainSplitContainer.Location = new System.Drawing.Point(12, 31);
            this.mMainSplitContainer.Name = "mMainSplitContainer";
            // 
            // mMainSplitContainer.Panel2
            // 
            this.mMainSplitContainer.Panel2.Controls.Add(this.mRightSplitContainer);
            this.mMainSplitContainer.Size = new System.Drawing.Size(712, 397);
            this.mMainSplitContainer.SplitterDistance = 448;
            this.mMainSplitContainer.TabIndex = 0;
            // 
            // mRightSplitContainer
            // 
            this.mRightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mRightSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mRightSplitContainer.Name = "mRightSplitContainer";
            this.mRightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mRightSplitContainer.Panel1
            // 
            this.mRightSplitContainer.Panel1.Controls.Add(this.mNodeTreeView);
            // 
            // mRightSplitContainer.Panel2
            // 
            this.mRightSplitContainer.Panel2.Controls.Add(this.mPropertyGrid);
            this.mRightSplitContainer.Size = new System.Drawing.Size(260, 397);
            this.mRightSplitContainer.SplitterDistance = 181;
            this.mRightSplitContainer.TabIndex = 0;
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
            this.mNodeTreeView.Size = new System.Drawing.Size(260, 181);
            this.mNodeTreeView.TabIndex = 0;
            this.mNodeTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnAfterSelect);
            // 
            // mPropertyGrid
            // 
            this.mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPropertyGrid.HelpVisible = false;
            this.mPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.mPropertyGrid.Name = "mPropertyGrid";
            this.mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.mPropertyGrid.Size = new System.Drawing.Size(260, 212);
            this.mPropertyGrid.TabIndex = 0;
            this.mPropertyGrid.ToolbarVisible = false;
            // 
            // mMenuStrip
            // 
            this.mMenuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.mConfigurationsToolStripMenuItem,
            this.mToolsToolStripMenuItem,
            this.mHelpToolStripMenuItem,
            this.mStylesToolStripMenuItem,
            this.mAboutToolStripMenuItem});
            this.mMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMenuStrip.Name = "mMenuStrip";
            this.mMenuStrip.Size = new System.Drawing.Size(736, 25);
            this.mMenuStrip.TabIndex = 0;
            this.mMenuStrip.Text = "menuStrip1";
            // 
            // mFileToolStripMenuItem
            // 
            this.mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOpenToolStripMenuItem,
            this.mSaveToolStripMenuItem,
            this.mSaveAsToolStripMenuItem,
            this.mCloseToolStripMenuItem,
            this.mToolStripSeparator2,
            this.mExitToolStripMenuItem});
            this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size(37, 21);
            this.mFileToolStripMenuItem.Text = "File";
            // 
            // mOpenToolStripMenuItem
            // 
            this.mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            this.mOpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mOpenToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mOpenToolStripMenuItem.Text = "Open";
            this.mOpenToolStripMenuItem.Click += new System.EventHandler(this.OnOpen);
            // 
            // mSaveToolStripMenuItem
            // 
            this.mSaveToolStripMenuItem.Enabled = false;
            this.mSaveToolStripMenuItem.Name = "mSaveToolStripMenuItem";
            this.mSaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mSaveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mSaveToolStripMenuItem.Text = "Save";
            this.mSaveToolStripMenuItem.Click += new System.EventHandler(this.OnSave);
            // 
            // mSaveAsToolStripMenuItem
            // 
            this.mSaveAsToolStripMenuItem.Enabled = false;
            this.mSaveAsToolStripMenuItem.Name = "mSaveAsToolStripMenuItem";
            this.mSaveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.mSaveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mSaveAsToolStripMenuItem.Text = "Save As";
            this.mSaveAsToolStripMenuItem.Click += new System.EventHandler(this.OnSaveAs);
            // 
            // mCloseToolStripMenuItem
            // 
            this.mCloseToolStripMenuItem.Enabled = false;
            this.mCloseToolStripMenuItem.Name = "mCloseToolStripMenuItem";
            this.mCloseToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mCloseToolStripMenuItem.Text = "Close";
            this.mCloseToolStripMenuItem.Click += new System.EventHandler(this.OnNodeClose);
            // 
            // mToolStripSeparator2
            // 
            this.mToolStripSeparator2.Name = "mToolStripSeparator2";
            this.mToolStripSeparator2.Size = new System.Drawing.Size(183, 6);
            // 
            // mExitToolStripMenuItem
            // 
            this.mExitToolStripMenuItem.Name = "mExitToolStripMenuItem";
            this.mExitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mExitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mExitToolStripMenuItem.Text = "Exit";
            this.mExitToolStripMenuItem.Click += new System.EventHandler(this.OnExit);
            // 
            // mConfigurationsToolStripMenuItem
            // 
            this.mConfigurationsToolStripMenuItem.Name = "mConfigurationsToolStripMenuItem";
            this.mConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(156, 21);
            this.mConfigurationsToolStripMenuItem.Text = "Game data configurations";
            this.mConfigurationsToolStripMenuItem.Click += new System.EventHandler(this.OnConfigurations);
            // 
            // mToolsToolStripMenuItem
            // 
            this.mToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCombineMotsFileToolStripMenuItem});
            this.mToolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
            this.mToolsToolStripMenuItem.Size = new System.Drawing.Size(46, 21);
            this.mToolsToolStripMenuItem.Text = "Tools";
            // 
            // mCombineMotsFileToolStripMenuItem
            // 
            this.mCombineMotsFileToolStripMenuItem.Name = "mCombineMotsFileToolStripMenuItem";
            this.mCombineMotsFileToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.mCombineMotsFileToolStripMenuItem.Text = "Join mot files";
            this.mCombineMotsFileToolStripMenuItem.Click += new System.EventHandler(this.OnCombineMotions);
            // 
            // mHelpToolStripMenuItem
            // 
            this.mHelpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
            this.mHelpToolStripMenuItem.Size = new System.Drawing.Size(75, 21);
            this.mHelpToolStripMenuItem.Text = "User guide";
            this.mHelpToolStripMenuItem.Click += new System.EventHandler(this.OnHelp);
            // 
            // mStylesToolStripMenuItem
            // 
            this.mStylesToolStripMenuItem.Name = "mStylesToolStripMenuItem";
            this.mStylesToolStripMenuItem.Size = new System.Drawing.Size(49, 21);
            this.mStylesToolStripMenuItem.Text = "Styles";
            // 
            // mAboutToolStripMenuItem
            // 
            this.mAboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
            this.mAboutToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.mAboutToolStripMenuItem.Text = "About";
            this.mAboutToolStripMenuItem.Click += new System.EventHandler(this.OnAbout);
            // 
            // mPanel
            // 
            this.mPanel.Controls.Add(this.mMenuStrip);
            this.mPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mPanel.Location = new System.Drawing.Point(0, 0);
            this.mPanel.Name = "mPanel";
            this.mPanel.Size = new System.Drawing.Size(736, 25);
            this.mPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 440);
            this.Controls.Add(this.mPanel);
            this.Controls.Add(this.mMainSplitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Miku Miku Model";
            this.mMainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mMainSplitContainer)).EndInit();
            this.mMainSplitContainer.ResumeLayout(false);
            this.mRightSplitContainer.Panel1.ResumeLayout(false);
            this.mRightSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mRightSplitContainer)).EndInit();
            this.mRightSplitContainer.ResumeLayout(false);
            this.mMenuStrip.ResumeLayout(false);
            this.mMenuStrip.PerformLayout();
            this.mPanel.ResumeLayout(false);
            this.mPanel.PerformLayout();
            this.ResumeLayout(false);

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
        private MikuMikuModel.Nodes.Wrappers.NodeTreeView mNodeTreeView;
        private System.Windows.Forms.ToolStripMenuItem mCloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mConfigurationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mHelpToolStripMenuItem;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem mToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCombineMotsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mStylesToolStripMenuItem;
    }
}
