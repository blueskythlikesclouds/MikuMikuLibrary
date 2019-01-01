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
        private readonly ConfigurationList configurationList;

        public ConfigurationForm()
        {
            InitializeComponent();

            configurationList = ( ConfigurationList )ConfigurationList.Instance.Clone();
        }

        public Configuration SelectedConfiguration => 
            listBox.SelectedIndex < 0 ? null : configurationList.Configurations[ listBox.SelectedIndex ];

        private void SetConfiguration( Configuration configuration )
        {
            listBox.SelectedIndex = configurationList.Configurations.IndexOf( configuration );
            objectDatabasePathTextBox.Text = configuration?.ObjectDatabaseFilePath;
            textureDatabasePathTextBox.Text = configuration?.TextureDatabaseFilePath;
            boneDatabasePathTextBox.Text = configuration?.BoneDatabaseFilePath;
        }

        private void UpdateListBox()
        {
            int previousIndex = listBox.SelectedIndex;

            listBox.Items.Clear();

            foreach ( var configuration in configurationList.Configurations )
                listBox.Items.Add( configuration.Name );

            if ( previousIndex >= listBox.Items.Count )
                listBox.SelectedIndex = listBox.Items.Count - 1;
            else
                listBox.SelectedIndex = previousIndex;
        }

        protected override void OnLoad( EventArgs e )
        {
            UpdateListBox();
            objectDatabasePathTextBox.Enabled = false;
            textureDatabasePathTextBox.Enabled = false;
            boneDatabasePathTextBox.Enabled = false;
            objectDatabaseBrowseButton.Enabled = false;
            textureDatabaseBrowseButton.Enabled = false;
            boneDatabaseBrowseButton.Enabled = false;
            removeButton.Enabled = false;
            renameButton.Enabled = false;
            cloneButton.Enabled = false;
            reloadButton.Enabled = false;

            base.OnLoad( e );
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            bool instanceEquals = configurationList.Equals( ConfigurationList.Instance );
            if ( DialogResult != DialogResult.OK && !instanceEquals )
            {
                var result = MessageBox.Show( "You have unsaved changes. Do you want to save them?", "Miku Miku Model", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
                if ( result == DialogResult.OK )
                {
                    ConfigurationList.Instance.Configurations.Clear();
                    ConfigurationList.Instance.Configurations.AddRange( configurationList.Configurations );
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
                ConfigurationList.Instance.Configurations.AddRange( configurationList.Configurations );
                ConfigurationList.Instance.Save();
            }

            base.OnFormClosing( e );
        }

        private void OnCreate( object sender, EventArgs e )
        {
            var name = "New Configuration";
            if ( configurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
            {
                for ( int i = 0; ; i++ )
                {
                    name = $"New Configuration {i}";
                    if ( !configurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
                    {
                        break;
                    }
                }
            }

            var configuration = new Configuration
            {
                Name = name
            };

            configurationList.Configurations.Add( configuration );
            UpdateListBox();
            SetConfiguration( configuration );
        }

        private void OnRemove( object sender, EventArgs e )
        {
            configurationList.Configurations.RemoveAt( listBox.SelectedIndex );
            UpdateListBox();
        }

        private void OnRename( object sender, EventArgs e )
        {
            var configuration = configurationList.Configurations[ listBox.SelectedIndex ];
            while ( true )
            {
                using ( var renameForm = new RenameForm( configuration.Name ) )
                {
                    if ( renameForm.ShowDialog( this ) == DialogResult.OK )
                    {
                        if ( configurationList.Configurations.Any( x => x != configuration && x.Name.Equals( renameForm.TextBoxText, StringComparison.OrdinalIgnoreCase ) ) )
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
            if ( configurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
            {
                for ( int i = 0; ; i++ )
                {
                    name = $"{clone.Name} {i}";
                    if ( !configurationList.Configurations.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) )
                    {
                        break;
                    }
                }
            }

            clone.Name = name;
            configurationList.Configurations.Insert( listBox.SelectedIndex + 1, clone );
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
            objectDatabasePathTextBox.Text =
                FormatModuleUtilities.SelectModuleImport<ObjectDatabase>(
                    "Select an object database file. (obj_db.bin)",
                    SelectedConfiguration?.ObjectDatabaseFilePath ) ?? objectDatabasePathTextBox.Text;
        }

        private void OnTextureDatabaseBrowse( object sender, EventArgs e )
        {
            textureDatabasePathTextBox.Text =
                FormatModuleUtilities.SelectModuleImport<TextureDatabase>(
                "Select a texture database file. (tex_db.bin)",
                SelectedConfiguration?.TextureDatabaseFilePath ) ?? textureDatabasePathTextBox.Text;
        }

        private void OnBoneDatabaseBrowse( object sender, EventArgs e )
        {
            boneDatabasePathTextBox.Text =
                FormatModuleUtilities.SelectModuleImport<BoneDatabase>(
                    "Select a bone database file. (bone_data.bin/bone_data.bon)",
                     SelectedConfiguration?.BoneDatabaseFilePath ) ?? boneDatabasePathTextBox.Text;
        }

        private void OnSelectedIndexChanged( object sender, EventArgs e )
        {
            objectDatabasePathTextBox.Enabled = listBox.SelectedIndex >= 0;
            textureDatabasePathTextBox.Enabled = listBox.SelectedIndex >= 0;
            boneDatabasePathTextBox.Enabled = listBox.SelectedIndex >= 0;
            objectDatabaseBrowseButton.Enabled = listBox.SelectedIndex >= 0;
            textureDatabaseBrowseButton.Enabled = listBox.SelectedIndex >= 0;
            boneDatabaseBrowseButton.Enabled = listBox.SelectedIndex >= 0;
            removeButton.Enabled = listBox.SelectedIndex >= 0;
            renameButton.Enabled = listBox.SelectedIndex >= 0;
            cloneButton.Enabled = listBox.SelectedIndex >= 0;
            reloadButton.Enabled = listBox.SelectedIndex >= 0;

            SetConfiguration( listBox.SelectedIndex >= 0 ? configurationList.Configurations[ listBox.SelectedIndex ] : null );
        }

        private void OnObjectDatabasePathTextBoxTextChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.ObjectDatabaseFilePath = objectDatabasePathTextBox.Text;
            SelectedConfiguration.ObjectDatabase = null;
        }

        private void OnTextureDatabasePathTextBoxTextChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.TextureDatabaseFilePath = textureDatabasePathTextBox.Text;
            SelectedConfiguration.TextureDatabase = null;
        }

        private void OnBoneDatabasePathTextBoxTextChanged( object sender, EventArgs e )
        {
            SelectedConfiguration.BoneDatabaseFilePath = boneDatabasePathTextBox.Text;
            SelectedConfiguration.BoneDatabase = null;
        }
    }
}
