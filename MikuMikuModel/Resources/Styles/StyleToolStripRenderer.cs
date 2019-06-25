using System.Windows.Forms;

namespace MikuMikuModel.Resources.Styles
{
    public class StyleToolStripRenderer : ToolStripProfessionalRenderer
    {
        private readonly Style mStyle;

        protected override void OnRenderItemText( ToolStripItemTextRenderEventArgs e )
        {
            e.TextColor = e.Item.Selected ? mStyle.SelectedText : mStyle.Text;
            base.OnRenderItemText( e );
        }

        public StyleToolStripRenderer( Style style ) : base( style.ColorTable )
        {
            mStyle = style;
        }
    }
}