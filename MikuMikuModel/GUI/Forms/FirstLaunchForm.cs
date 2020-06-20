using System.Windows.Forms;
using MikuMikuModel.Resources;

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
