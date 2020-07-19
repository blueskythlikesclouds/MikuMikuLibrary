using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.Resources.Styles
{
    public class StyleColorTable : ProfessionalColorTable
    {
        public Style Style { get; }

        public override Color ToolStripDropDownBackground => Style.ToolStripDropDownBackground;
        public override Color MenuItemSelected => Style.MenuItemSelected;
        public override Color MenuBorder => Style.MenuBorder;
        public override Color MenuItemBorder => Style.MenuItemBorder;
        public override Color MenuStripGradientBegin => Style.MenuStripGradientBegin;
        public override Color MenuStripGradientEnd => Style.MenuStripGradientEnd;
        public override Color MenuItemSelectedGradientBegin => Style.MenuItemSelectedGradientBegin;
        public override Color MenuItemSelectedGradientEnd => Style.MenuItemSelectedGradientEnd;
        public override Color MenuItemPressedGradientBegin => Style.MenuItemPressedGradientBegin;
        public override Color MenuItemPressedGradientEnd => Style.MenuItemPressedGradientEnd;
        public override Color ImageMarginGradientBegin => Style.ImageMarginGradientBegin;
        public override Color ImageMarginGradientMiddle => Style.ImageMarginGradientMiddle;
        public override Color ImageMarginGradientEnd => Style.ImageMarginGradientEnd;
        public override Color SeparatorDark => Style.SeparatorDark;
        public override Color SeparatorLight => Style.SeparatorLight;

        public StyleColorTable( Style style )
        {
            Style = style;
        }
    }
}