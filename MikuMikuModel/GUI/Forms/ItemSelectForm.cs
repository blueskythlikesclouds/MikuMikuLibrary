using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MikuMikuModel.Resources;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Forms
{
    public partial class ItemSelectForm<T> : Form
    {
        public IEnumerable<T> CheckedItems =>
            from object checkedItem in mListView.CheckedItems select ( T ) ( ( ListViewItem ) checkedItem ).Tag;

        public string GroupBoxText
        {
            get => mGroupBox.Text;
            set => mGroupBox.Text = value;
        }

        public bool CheckBoxChecked
        {
            get => mCheckBox.Checked;
            set => mCheckBox.Checked = value;
        }

        public string CheckBoxText
        {
            get => mCheckBox.Text;
            set
            {
                mCheckBox.Text = value;
                mCheckBox.Visible = !string.IsNullOrEmpty( value );
            }
        }

        private void OnCheckAll( object sender, EventArgs e )
        {
            for ( int i = 0; i < mListView.Items.Count; i++ )
                mListView.Items[ i ].Checked = true;
        }

        private void OnUncheckAll( object sender, EventArgs e )
        {
            for ( int i = 0; i < mListView.Items.Count; i++ )
                mListView.Items[ i ].Checked = false;
        }

        private void OnCheckSelected( object sender, EventArgs e )
        {
            foreach ( int index in mListView.SelectedIndices )
                mListView.Items[ index ].Checked = true;
        }

        private void OnUncheckSelected( object sender, EventArgs e )
        {
            foreach ( int index in mListView.SelectedIndices )
                mListView.Items[ index ].Checked = false;
        }

        public ItemSelectForm( IEnumerable<(T Item, string Name)> items )
        {
            InitializeComponent();

            Icon = ResourceStore.LoadIcon( "Icons/Application.ico" );

            if ( StyleSet.CurrentStyle != null )
                StyleHelpers.ApplyStyle( this, StyleSet.CurrentStyle );

            // Why won't you be set in the designerrrrrr
            mListView.Columns[ 0 ].Width = -1;

            foreach ( ( T item, string name ) in items )
                mListView.Items.Add( new ListViewItem { Name = name, Text = name, Tag = item, Checked = true } );

            StyleHelpers.ApplySystemFont( this );
        }
    }
}