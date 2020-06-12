using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Databases;
using MikuMikuModel.Configurations;
using MikuMikuModel.Modules;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.GUI.Forms
{
    public partial class ConfigurationForm : Form
    {
        private readonly ConfigurationList mConfigurationList;

        public Configuration SelectedConfiguration =>
            mListBox.SelectedIndex < 0 ? null : mConfigurationList.Configurations[ mListBox.SelectedIndex ];

        private void SetConfiguration( Configuration configuration )
        {
            mListBox.SelectedIndex = mConfigurationList.Configurations.IndexOf( configuration );
            mObjectDatabasePathTextBox.Text = configuration?.ObjectDatabaseFilePath;
            mTextureDatabasePathTextBox.Text = configuration?.TextureDatabaseFilePath;
            mBoneDatabasePathTextBox.Text = configuration?.BoneDatabaseFilePath;
            mMotionDatabasePathTextBox.Text = configuration?.MotionDatabaseFilePath;
        }

        private void UpdateListBox()
        {
            int previousIndex = mListBox.SelectedIndex;

            mListBox.Items.Clear();

            foreach ( var configuration in mConfigurationList.Configurations )
                mListBox.Items.Add( configuration.Name );

            if ( previousIndex >= mListBox.Items.Count )
                mListBox.SelectedIndex = mListBox.Items.Count - 1;
            else
                mListBox.SelectedIndex = previousIndex;
        }

        protected override void OnLoad( EventArgs e )
        {
            UpdateListBox();
            mObjectDatabasePathTextBox.Enabled = false;
            mTextureDatabasePathTextBox.Enabled = false;
            mBoneDatabasePathTextBox.Enabled = false;
            mObjectDatabaseBrowseButton.Enabled = false;
            mTextureDatabaseBrowseButton.Enabled = false;
            mBoneDatabaseBrowseButton.Enabled = false;
            mMotionDatabaseBrowseButton.Enabled = false;
            mRemoveButton.Enabled = false;
            mRenameButton.Enabled = false;
            mCloneButton.Enabled = false;
            mReloadButton.Enabled = false;

            base.OnLoad( e );
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            bool instanceEquals = mConfigurationList.Equals( ConfigurationList.Instance );
            if ( DialogResult != DialogResult.OK && !instanceEquals )
            {
                var result = MessageBox.Show( "You have unsaved changes. Do you want to save them?", Program.Name,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
                if ( result == DialogResult.OK )
                {
                    ConfigurationList.Instance.Configurations.Clear();
                    ConfigurationList.Instance.Configurations.AddRange( mConfigurationList.Configurations );
                    ConfigurationList.Instance.Save();
                }
                else if ( result == DialogResult.Cancel )
                {
                    e.Cancel = true;
                }
            }
            else if ( DialogResult == DialogResult.OK && !instanceEquals )
            {
                ConfigurationList.Instance.Configurations.Clear();
                ConfigurationList.Instance.Configurations.AddRange( mConfigurationList.Configurations );
                ConfigurationList.Instance.Save();
            }

            base.OnFormClosing( e );
        }

        private void OnCreate( object sender, EventArgs e )
        {
            string name = "New Configuration";
            if ( mConfigurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) )
            )
                for ( int i = 0;; i++ )
                {
                    name = $"New Configuration {i + 1}";
                    if ( !mConfigurationList.Configurations.Any( x =>
                        x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
                        break;
                }

            var configuration = new Configuration { Name = name };
            mConfigurationList.Configurations.Add( configuration );
            UpdateListBox();
            SetConfiguration( configuration );
        }

        private void OnRemove( object sender, EventArgs e )
        {
            mConfigurationList.Configurations.RemoveAt( mListBox.SelectedIndex );
            UpdateListBox();
        }

        private void OnRename( object sender, EventArgs e )
        {
            var configuration = mConfigurationList.Configurations[ mListBox.SelectedIndex ];
            using ( var inputDialog = new InputDialog
                { WindowTitle = "Rename configuration", Input = configuration.Name } )
            {
                while ( inputDialog.ShowDialog( this ) == DialogResult.OK )
                    if ( string.IsNullOrEmpty( inputDialog.Input ) )
                    {
                        MessageBox.Show( "Please enter a valid name.", Program.Name, MessageBoxButtons.OK,
                            MessageBoxIcon.Error );

                        inputDialog.Input = configuration.Name;
                    }
                    else if ( mConfigurationList.Configurations.Any( x =>
                        x != configuration && x.Name.Equals( inputDialog.Input, StringComparison.OrdinalIgnoreCase ) ) )
                    {
                        MessageBox.Show( "A configuration with the same name already exists.", Program.Name,
                            MessageBoxButtons.OK, MessageBoxIcon.Error );
                    }
                    else
                    {
                        configuration.Name = inputDialog.Input;
                        break;
                    }
            }

            UpdateListBox();
        }

        private void OnClone( object sender, EventArgs e )
        {
            var clone = ( Configuration ) SelectedConfiguration.Clone();

            string name = clone.Name;
            if ( mConfigurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) )
            )
                for ( int i = 0;; i++ )
                {
                    name = $"{clone.Name} {i}";
                    if ( !mConfigurationList.Configurations.Any( x =>
                        x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
                        break;
                }

            clone.Name = name;
            mConfigurationList.Configurations.Insert( mListBox.SelectedIndex + 1, clone );
            UpdateListBox();
        }

        private void OnReload( object sender, EventArgs e )
        {
            SelectedConfiguration.Clean();
        }

        private void OnObjectDatabaseBrowse( object sender, EventArgs e )
        {
            mObjectDatabasePathTextBox.Text =
                ModuleImportUtilities.SelectModuleImport<ObjectDatabase>(
                    "Select an object database file. (obj_db.bin)",
                    SelectedConfiguration?.ObjectDatabaseFilePath ) ?? mObjectDatabasePathTextBox.Text;
        }

        private void OnTextureDatabaseBrowse( object sender, EventArgs e )
        {
            mTextureDatabasePathTextBox.Text =
                ModuleImportUtilities.SelectModuleImport<TextureDatabase>(
                    "Select a texture database file. (tex_db.bin)",
                    SelectedConfiguration?.TextureDatabaseFilePath ) ?? mTextureDatabasePathTextBox.Text;
        }

        private void OnBoneDatabaseBrowse( object sender, EventArgs e )
        {
            mBoneDatabasePathTextBox.Text =
                ModuleImportUtilities.SelectModuleImport<BoneDatabase>(
                    "Select a bone database file. (bone_data.bin/bone_data.bon)",
                    SelectedConfiguration?.BoneDatabaseFilePath ) ?? mBoneDatabasePathTextBox.Text;
        }


        private void OnMotionDatabaseBrowse( object sender, EventArgs e )
        {
            mMotionDatabasePathTextBox.Text =
                ModuleImportUtilities.SelectModuleImport(
                    new[] { typeof( MotionDatabase ), typeof( FarcArchive ) },
                    "Select a motion database file. (mot_db.bin/mot_db.farc)",
                    SelectedConfiguration?.MotionDatabaseFilePath ) ??
                mMotionDatabasePathTextBox.Text;
        }

        private void OnSearch( object sender, EventArgs e )
        {
            using ( var folderBrowseDialog = new VistaFolderBrowserDialog() )
            {
                folderBrowseDialog.Description = "Select a folder to search for databases. (preferably rom folder)";
                folderBrowseDialog.UseDescriptionForTitle = true;

                if ( folderBrowseDialog.ShowDialog( this ) != DialogResult.OK )
                    return;

                if ( string.IsNullOrEmpty( SelectedConfiguration.ObjectDatabaseFilePath ) )
                    SelectedConfiguration.ObjectDatabaseFilePath =
                        PickPath( "rom/objset/obj_db.bin", "objset/obj_db.bin", "obj_db.bin" );

                if ( string.IsNullOrEmpty( SelectedConfiguration.TextureDatabaseFilePath ) )
                    SelectedConfiguration.TextureDatabaseFilePath =
                        PickPath( "rom/objset/tex_db.bin", "objset/tex_db.bin", "tex_db.bin" );

                if ( string.IsNullOrEmpty( SelectedConfiguration.BoneDatabaseFilePath ) )
                    SelectedConfiguration.BoneDatabaseFilePath =
                        PickPath( "rom/bone_data.bin", "bone_data.bin", "data/bone_data.bon",
                            "bone_data.bon" );

                if ( string.IsNullOrEmpty( SelectedConfiguration.MotionDatabaseFilePath ) )
                    SelectedConfiguration.MotionDatabaseFilePath =
                        PickPath(
                            "rom/rob/mot_db.farc", "rob/mot_db.farc", "mot_db.farc",
                            "rom/rob/mot_db/mot_db.bin", "rob/mot_db/mot_db.bin", "mot_db/mot_db.bin",
                            "rom/rob/mot_db.bin", "rob/mot_db.bin", "mot_db.bin" );

                SetConfiguration( SelectedConfiguration );

                string PickPath( params string[] relativePaths )
                {
                    return relativePaths
                        .Select( relativePath =>
                            Path.GetFullPath( Path.Combine( folderBrowseDialog.SelectedPath, relativePath ) ) )
                        .FirstOrDefault( File.Exists );
                }
            }
        }

        private void OnSelectedIndexChanged( object sender, EventArgs e )
        {
            // This looks very weird
            mObjectDatabasePathTextBox.Enabled =
                mTextureDatabasePathTextBox.Enabled =
                    mBoneDatabasePathTextBox.Enabled =
                        mMotionDatabasePathTextBox.Enabled =
                            mObjectDatabaseBrowseButton.Enabled =
                                mTextureDatabaseBrowseButton.Enabled =
                                    mBoneDatabaseBrowseButton.Enabled =
                                        mMotionDatabaseBrowseButton.Enabled =
                                            mRemoveButton.Enabled =
                                                mRenameButton.Enabled =
                                                    mCloneButton.Enabled =
                                                        mReloadButton.Enabled =
                                                            mSearchButton.Enabled =
                                                                mListBox.SelectedIndex >= 0;

            SetConfiguration( mListBox.SelectedIndex >= 0
                ? mConfigurationList.Configurations[ mListBox.SelectedIndex ]
                : null );
        }

        private void OnObjectDatabasePathTextBoxTextChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.ObjectDatabaseFilePath = mObjectDatabasePathTextBox.Text;
            SelectedConfiguration.ObjectDatabase = null;
        }

        private void OnTextureDatabasePathTextBoxTextChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.TextureDatabaseFilePath = mTextureDatabasePathTextBox.Text;
            SelectedConfiguration.TextureDatabase = null;
        }

        private void OnBoneDatabasePathTextBoxTextChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.BoneDatabaseFilePath = mBoneDatabasePathTextBox.Text;
            SelectedConfiguration.BoneDatabase = null;
        }

        private void OnMotionDatabasePathTextBoxChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.MotionDatabaseFilePath = mMotionDatabasePathTextBox.Text;
            SelectedConfiguration.MotionDatabase = null;
        }

        public ConfigurationForm()
        {
            InitializeComponent();

            mConfigurationList = ( ConfigurationList ) ConfigurationList.Instance.Clone();
        }
    }
}