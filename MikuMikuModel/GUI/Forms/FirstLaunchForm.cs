using System.Windows.Forms;
using MikuMikuModel.Resources;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Forms
{
    public partial class FirstLaunchForm : Form
    {
        public FirstLaunchForm()
        {
            InitializeComponent();

            Icon = ResourceStore.LoadIcon( "Icons/Application.ico" );
        }
    }
}
