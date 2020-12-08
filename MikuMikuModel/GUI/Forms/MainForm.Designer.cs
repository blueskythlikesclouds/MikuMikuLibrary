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
            this.mMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mRightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mNodeTreeView = new MikuMikuModel.Nodes.Wrappers.NodeTreeView();
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.mMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mUndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mConfigurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCombineMotsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mGenerateMurmurHashesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAutoCheckUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mStylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mCamerasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOrbitCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFreeCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mUserGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mCheckForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPanel = new System.Windows.Forms.Panel();
            this.mConvertOsageSkinParametersToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mConvertOspToClassicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mConvertOspToF2ndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mConvertOspToXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.mPropertyGrid.Size = new System.Drawing.Size(260, 212);
            this.mPropertyGrid.TabIndex = 0;
            this.mPropertyGrid.ToolbarVisible = false;
            this.mPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnPropertyValueChanged);
            // 
            // mMenuStrip
            // 
            this.mMenuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.mConfigurationsToolStripMenuItem,
            this.mToolsToolStripMenuItem,
            this.mOptionsToolStripMenuItem,
            this.mHelpToolStripMenuItem});
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
            this.mOpenRecentToolStripMenuItem,
            this.mSaveToolStripMenuItem,
            this.mSaveAsToolStripMenuItem,
            this.mCloseToolStripMenuItem,
            this.mToolStripSeparator2,
            this.mExitToolStripMenuItem});
            this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size(37, 21);
            this.mFileToolStripMenuItem.Text = "File";
            this.mFileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.OnFileDropDownOpening);
            // 
            // mOpenToolStripMenuItem
            // 
            this.mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            this.mOpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mOpenToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mOpenToolStripMenuItem.Text = "Open";
            this.mOpenToolStripMenuItem.Click += new System.EventHandler(this.OnOpen);
            // 
            // mOpenRecentToolStripMenuItem
            // 
            this.mOpenRecentToolStripMenuItem.Name = "mOpenRecentToolStripMenuItem";
            this.mOpenRecentToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mOpenRecentToolStripMenuItem.Text = "Open Recent";
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
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mUndoToolStripMenuItem,
            this.mRedoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownClosed += new System.EventHandler(this.OnEditDropDownClosed);
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.OnEditDropDownOpening);
            // 
            // mUndoToolStripMenuItem
            // 
            this.mUndoToolStripMenuItem.Name = "mUndoToolStripMenuItem";
            this.mUndoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mUndoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.mUndoToolStripMenuItem.Text = "Undo";
            this.mUndoToolStripMenuItem.Click += new System.EventHandler(this.OnUndo);
            // 
            // mRedoToolStripMenuItem
            // 
            this.mRedoToolStripMenuItem.Name = "mRedoToolStripMenuItem";
            this.mRedoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.mRedoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.mRedoToolStripMenuItem.Text = "Redo";
            this.mRedoToolStripMenuItem.Click += new System.EventHandler(this.OnRedo);
            // 
            // mConfigurationsToolStripMenuItem
            // 
            this.mConfigurationsToolStripMenuItem.Name = "mConfigurationsToolStripMenuItem";
            this.mConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(98, 21);
            this.mConfigurationsToolStripMenuItem.Text = "Configurations";
            this.mConfigurationsToolStripMenuItem.Click += new System.EventHandler(this.OnConfigurations);
            // 
            // mToolsToolStripMenuItem
            // 
            this.mToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCombineMotsFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.mGenerateMurmurHashesToolStripMenuItem,
            this.toolStripSeparator6,
            this.mConvertOsageSkinParametersToToolStripMenuItem});
            this.mToolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
            this.mToolsToolStripMenuItem.Size = new System.Drawing.Size(46, 21);
            this.mToolsToolStripMenuItem.Text = "Tools";
            // 
            // mCombineMotsFileToolStripMenuItem
            // 
            this.mCombineMotsFileToolStripMenuItem.Name = "mCombineMotsFileToolStripMenuItem";
            this.mCombineMotsFileToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.mCombineMotsFileToolStripMenuItem.Text = "Combine divided .mot files into one";
            this.mCombineMotsFileToolStripMenuItem.Click += new System.EventHandler(this.OnCombineMotions);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(261, 6);
            // 
            // mGenerateMurmurHashesToolStripMenuItem
            // 
            this.mGenerateMurmurHashesToolStripMenuItem.Name = "mGenerateMurmurHashesToolStripMenuItem";
            this.mGenerateMurmurHashesToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.mGenerateMurmurHashesToolStripMenuItem.Text = "Generate murmur hashes";
            this.mGenerateMurmurHashesToolStripMenuItem.Click += new System.EventHandler(this.OnGenerateMurmurHashes);
            // 
            // mOptionsToolStripMenuItem
            // 
            this.mOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAutoCheckUpdatesToolStripMenuItem,
            this.toolStripSeparator4,
            this.mStylesToolStripMenuItem,
            this.toolStripSeparator5,
            this.mCamerasToolStripMenuItem});
            this.mOptionsToolStripMenuItem.Name = "mOptionsToolStripMenuItem";
            this.mOptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 21);
            this.mOptionsToolStripMenuItem.Text = "Options";
            // 
            // mAutoCheckUpdatesToolStripMenuItem
            // 
            this.mAutoCheckUpdatesToolStripMenuItem.Checked = true;
            this.mAutoCheckUpdatesToolStripMenuItem.CheckOnClick = true;
            this.mAutoCheckUpdatesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mAutoCheckUpdatesToolStripMenuItem.Name = "mAutoCheckUpdatesToolStripMenuItem";
            this.mAutoCheckUpdatesToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.mAutoCheckUpdatesToolStripMenuItem.Text = "Auto-check for updates";
            this.mAutoCheckUpdatesToolStripMenuItem.Click += new System.EventHandler(this.OnAutoCheckUpdates);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(196, 6);
            // 
            // mStylesToolStripMenuItem
            // 
            this.mStylesToolStripMenuItem.Name = "mStylesToolStripMenuItem";
            this.mStylesToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.mStylesToolStripMenuItem.Text = "Styles";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(196, 6);
            // 
            // mCamerasToolStripMenuItem
            // 
            this.mCamerasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOrbitCameraToolStripMenuItem,
            this.mFreeCameraToolStripMenuItem});
            this.mCamerasToolStripMenuItem.Name = "mCamerasToolStripMenuItem";
            this.mCamerasToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.mCamerasToolStripMenuItem.Text = "Cameras";
            // 
            // mOrbitCameraToolStripMenuItem
            // 
            this.mOrbitCameraToolStripMenuItem.Checked = true;
            this.mOrbitCameraToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mOrbitCameraToolStripMenuItem.Name = "mOrbitCameraToolStripMenuItem";
            this.mOrbitCameraToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.mOrbitCameraToolStripMenuItem.Text = "Orbit Camera";
            this.mOrbitCameraToolStripMenuItem.Click += new System.EventHandler(this.OnCameraModeClicked);
            // 
            // mFreeCameraToolStripMenuItem
            // 
            this.mFreeCameraToolStripMenuItem.Name = "mFreeCameraToolStripMenuItem";
            this.mFreeCameraToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.mFreeCameraToolStripMenuItem.Text = "Free Camera";
            this.mFreeCameraToolStripMenuItem.Click += new System.EventHandler(this.OnCameraModeClicked);
            // 
            // mHelpToolStripMenuItem
            // 
            this.mHelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mUserGuideToolStripMenuItem,
            this.toolStripSeparator3,
            this.mCheckForUpdatesToolStripMenuItem,
            this.toolStripSeparator2,
            this.mAboutToolStripMenuItem});
            this.mHelpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
            this.mHelpToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.mHelpToolStripMenuItem.Text = "Help";
            // 
            // mUserGuideToolStripMenuItem
            // 
            this.mUserGuideToolStripMenuItem.Name = "mUserGuideToolStripMenuItem";
            this.mUserGuideToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mUserGuideToolStripMenuItem.Text = "User guide";
            this.mUserGuideToolStripMenuItem.Click += new System.EventHandler(this.OnUserGuide);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(201, 6);
            // 
            // mCheckForUpdatesToolStripMenuItem
            // 
            this.mCheckForUpdatesToolStripMenuItem.Name = "mCheckForUpdatesToolStripMenuItem";
            this.mCheckForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mCheckForUpdatesToolStripMenuItem.Text = "Check for updates";
            this.mCheckForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.OnCheckForUpdates);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
            // 
            // mAboutToolStripMenuItem
            // 
            this.mAboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
            this.mAboutToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mAboutToolStripMenuItem.Text = "About Miku Miku Model";
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
            // mConvertOsageSkinParametersToToolStripMenuItem
            // 
            this.mConvertOsageSkinParametersToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mConvertOspToClassicToolStripMenuItem,
            this.mConvertOspToF2ndToolStripMenuItem,
            this.mConvertOspToXToolStripMenuItem});
            this.mConvertOsageSkinParametersToToolStripMenuItem.Name = "mConvertOsageSkinParametersToToolStripMenuItem";
            this.mConvertOsageSkinParametersToToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.mConvertOsageSkinParametersToToolStripMenuItem.Text = "Convert osage skin parameters to...";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(261, 6);
            // 
            // mConvertOspToClassicToolStripMenuItem
            // 
            this.mConvertOspToClassicToolStripMenuItem.Name = "mConvertOspToClassicToolStripMenuItem";
            this.mConvertOspToClassicToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mConvertOspToClassicToolStripMenuItem.Text = "DT/F/FT";
            this.mConvertOspToClassicToolStripMenuItem.Click += new System.EventHandler(this.OnConvertOspToClassic);
            // 
            // mConvertOspToF2ndToolStripMenuItem
            // 
            this.mConvertOspToF2ndToolStripMenuItem.Name = "mConvertOspToF2ndToolStripMenuItem";
            this.mConvertOspToF2ndToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mConvertOspToF2ndToolStripMenuItem.Text = "F 2nd";
            this.mConvertOspToF2ndToolStripMenuItem.Click += new System.EventHandler(this.OnConvertOspToF2nd);
            // 
            // mConvertOspToXToolStripMenuItem
            // 
            this.mConvertOspToXToolStripMenuItem.Name = "mConvertOspToXToolStripMenuItem";
            this.mConvertOspToXToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mConvertOspToXToolStripMenuItem.Text = "X";
            this.mConvertOspToXToolStripMenuItem.Click += new System.EventHandler(this.OnConvertOspToX);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 440);
            this.Controls.Add(this.mPanel);
            this.Controls.Add(this.mMainSplitContainer);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = typeof(MikuMikuModel.Program).Name;
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
        private System.Windows.Forms.ToolStripMenuItem mHelpToolStripMenuItem;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem mToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCombineMotsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mGenerateMurmurHashesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mUndoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRedoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mUserGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mCheckForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mAboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mStylesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mCamerasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOrbitCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mFreeCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mAutoCheckUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOpenRecentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem mConvertOsageSkinParametersToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mConvertOspToClassicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mConvertOspToF2ndToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mConvertOspToXToolStripMenuItem;
    }
}
