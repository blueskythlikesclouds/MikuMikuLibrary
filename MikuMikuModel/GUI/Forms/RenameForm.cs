using System.Windows.Forms;

namespace MikuMikuModel.GUI.Forms
{
    public partial class RenameForm : Form
    {
        public string TextBoxText
        {
            get => mTextBox.Text;
            set => mTextBox.Text = value;
        }

        public RenameForm( string textBoxText )
        {
            InitializeComponent();
            TextBoxText = textBoxText;
            mTextBox.Focus();
        }
    }
}
