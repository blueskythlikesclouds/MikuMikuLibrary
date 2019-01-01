using System.Windows.Forms;

namespace MikuMikuModel.GUI.Forms
{
    public partial class RenameForm : Form
    {
        public string TextBoxText
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public RenameForm( string textBoxText )
        {
            InitializeComponent();
            TextBoxText = textBoxText;
            textBox.Focus();
        }
    }
}
