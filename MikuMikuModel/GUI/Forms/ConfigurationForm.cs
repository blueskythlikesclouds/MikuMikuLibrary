using MikuMikuLibrary.Databases;
using MikuMikuModel.Configurations;
using MikuMikuModel.FormatModules;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Forms
{
    public partial class ConfigurationForm : Form
    {
        private readonly ConfigurationList mConfigurationList;

        public ConfigurationForm()
        {
            InitializeComponent();

            mConfigurationList = ( ConfigurationList )ConfigurationList.Instance.Clone();
        }

        public Configuration SelectedConfiguration =>
            mListBox.SelectedIndex < 0 ? null : mConfigurationList.Configurations[ mListBox.SelectedIndex ];

        private void SetConfiguration( Configuration configuration )
        {
            mListBox.SelectedIndex = mConfigurationList.Configurations.IndexOf( configuration );
            mObjectDatabasePathTextBox.Text = configuration?.ObjectDatabaseFilePath;
            mTextureDatabasePathTextBox.Text = configuration?.TextureDatabaseFilePath;
            mBoneDatabasePathTextBox.Text = configuration?.BoneDatabaseFilePath;
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
                var result = MessageBox.Show( "You have unsaved changes. Do you want to save them?", "Miku Miku Model", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
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
            var name = "New Configuration";
            if ( mConfigurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
            {
                for ( int i = 0; ; i++ )
                {
                    name = $"New Configuration {i}";
                    if ( !mConfigurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
                    {
                        break;
                    }
                }
            }

            var configuration = new Configuration
            {
                Name = name
            };

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
            while ( true )
            {
                using ( var renameForm = new RenameForm( configuration.Name ) )
                {
                    if ( renameForm.ShowDialog( this ) == DialogResult.OK )
                    {
                        if ( mConfigurationList.Configurations.Any( x => x != configuration && x.Name.Equals( renameForm.TextBoxText, StringComparison.OrdinalIgnoreCase ) ) )
                        {
                            MessageBox.Show( "A configuration with the same name already exists.", "Miku Miku Model", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        else
                        {
                            configuration.Name = renameForm.TextBoxText;
                            break;
                        }
                    }

                    else
                    {
                        break;
                    }
                }
            }

            UpdateListBox();
        }

        private void OnClone( object sender, EventArgs e )
        {
            var clone = ( Configuration )SelectedConfiguration.Clone();

            var name = clone.Name;
            if ( mConfigurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
            {
                for ( int i = 0; ; i++ )
                {
                    name = $"{clone.Name} {i}";
                    if ( !mConfigurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
                    {
                        break;
                    }
                }
            }

            clone.Name = name;
            mConfigurationList.Configurations.Insert( mListBox.SelectedIndex + 1, clone );
            UpdateListBox();
        }

        private void OnReload( object sender, EventArgs e )
        {
            SelectedConfiguration.ObjectDatabase = null;
            SelectedConfiguration.TextureDatabase = null;
            SelectedConfiguration.BoneDatabase = null;
        }

        private void OnObjectDatabaseBrowse( object sender, EventArgs e )
        {
            mObjectDatabasePathTextBox.Text =
                FormatModuleUtilities.SelectModuleImport<ObjectDatabase>(
                    "Select an object database file. (obj_db.bin)",
                    SelectedConfiguration?.ObjectDatabaseFilePath ) ?? mObjectDatabasePathTextBox.Text;
        }

        private void OnTextureDatabaseBrowse( object sender, EventArgs e )
        {
            mTextureDatabasePathTextBox.Text =
                FormatModuleUtilities.SelectModuleImport<TextureDatabase>(
                "Select a texture database file. (tex_db.bin)",
                SelectedConfiguration?.TextureDatabaseFilePath ) ?? mTextureDatabasePathTextBox.Text;
        }

        private void OnBoneDatabaseBrowse( object sender, EventArgs e )
        {
            mBoneDatabasePathTextBox.Text =
                FormatModuleUtilities.SelectModuleImport<BoneDatabase>(
                    "Select a bone database file. (bone_data.bin/bone_data.bon)",
                     SelectedConfiguration?.BoneDatabaseFilePath ) ?? mBoneDatabasePathTextBox.Text;
        }

        private void OnSelectedIndexChanged( object sender, EventArgs e )
        {
            mObjectDatabasePathTextBox.Enabled = mListBox.SelectedIndex >= 0;
            mTextureDatabasePathTextBox.Enabled = mListBox.SelectedIndex >= 0;
            mBoneDatabasePathTextBox.Enabled = mListBox.SelectedIndex >= 0;
            mObjectDatabaseBrowseButton.Enabled = mListBox.SelectedIndex >= 0;
            mTextureDatabaseBrowseButton.Enabled = mListBox.SelectedIndex >= 0;
            mBoneDatabaseBrowseButton.Enabled = mListBox.SelectedIndex >= 0;
            mRemoveButton.Enabled = mListBox.SelectedIndex >= 0;
            mRenameButton.Enabled = mListBox.SelectedIndex >= 0;
            mCloneButton.Enabled = mListBox.SelectedIndex >= 0;
            mReloadButton.Enabled = mListBox.SelectedIndex >= 0;

            SetConfiguration( mListBox.SelectedIndex >= 0 ? mConfigurationList.Configurations[ mListBox.SelectedIndex ] : null );
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
    }
}
