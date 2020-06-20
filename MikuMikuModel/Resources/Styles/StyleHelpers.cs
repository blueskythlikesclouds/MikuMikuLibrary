using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MikuMikuModel.GUI.Controls;

namespace MikuMikuModel.Resources.Styles
{
    public static class StyleHelpers
    {
        public static void ApplyStyle( Control control, Style style )
        {
            control.BackColor = style.Background;
            control.ForeColor = style.Foreground;

            switch ( control )
            {
                case Button button:
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = style.ButtonBorderColor;
                    button.BackColor = style.ButtonColor;
                    break;

                case GroupBox groupBox:
                    groupBox.FlatStyle = FlatStyle.Flat;
                    break;

                case ModelViewControl modelView:
                    modelView.GridColor = style.ViewportForeground;
                    modelView.BackgroundColor = style.ViewportBackground;
                    break;

                case PropertyGrid propertyGrid:
                    propertyGrid.CategoryForeColor = style.Foreground;
                    propertyGrid.CategorySplitterColor = style.Border;
                    propertyGrid.LineColor = style.Border;
                    propertyGrid.SelectedItemWithFocusBackColor = style.Border;
                    propertyGrid.ViewBackColor = style.Background;
                    propertyGrid.ViewForeColor = style.Foreground;
                    break;

                case ToolStrip toolStrip:
                    toolStrip.Renderer = style.ToolStripRenderer;
                    break;

                case TreeView treeView:
                    treeView.LineColor = style.SeparatorLight;
                    break;
            }

            foreach ( var childControl in control.Controls )
            {
                if ( !( childControl is Control ) )
                    continue;

                ApplyStyle( ( Control ) childControl, style );
            }
        }

        public static void StoreDefaultStyle( Control control )
        {
            var colorMap = new Dictionary<string, Color>
            {
                { nameof( control.ForeColor ), control.ForeColor },
                { nameof( control.BackColor ), control.BackColor }
            };

            switch ( control )
            {
                case PropertyGrid propertyGrid:
                    colorMap.Add( nameof( propertyGrid.CategoryForeColor ), propertyGrid.CategoryForeColor );
                    colorMap.Add( nameof( propertyGrid.CategorySplitterColor ), propertyGrid.CategorySplitterColor );
                    colorMap.Add( nameof( propertyGrid.LineColor ), propertyGrid.LineColor );
                    colorMap.Add( nameof( propertyGrid.SelectedItemWithFocusBackColor ), propertyGrid.SelectedItemWithFocusBackColor );
                    colorMap.Add( nameof( propertyGrid.ViewBackColor ), propertyGrid.ViewBackColor );
                    colorMap.Add( nameof( propertyGrid.ViewForeColor ), propertyGrid.ViewForeColor );
                    break;

                case TreeView treeView:
                    colorMap.Add( nameof( treeView.LineColor ), treeView.LineColor );
                    break;

                case ModelViewControl modelView:
                    colorMap[ nameof( modelView.GridColor ) ] = modelView.GridColor;
                    colorMap[ nameof( modelView.BackgroundColor ) ] = modelView.BackgroundColor;
                    break;
            }

            control.Tag = colorMap;

            foreach ( var childControl in control.Controls )
            {
                if ( !( childControl is Control ) )
                    continue;

                StoreDefaultStyle( ( Control ) childControl );
            }
        }

        public static void RestoreDefaultStyle( Control control )
        {
            if ( control.Tag is Dictionary<string, Color> colorMap )
            {
                control.ForeColor = colorMap[ nameof( control.ForeColor ) ];
                control.BackColor = colorMap[ nameof( control.BackColor ) ];

                switch ( control )
                {
                    case PropertyGrid propertyGrid:
                        propertyGrid.CategoryForeColor = colorMap[ nameof( propertyGrid.CategoryForeColor ) ];
                        propertyGrid.CategorySplitterColor = colorMap[ nameof( propertyGrid.CategorySplitterColor ) ];
                        propertyGrid.LineColor = colorMap[ nameof( propertyGrid.LineColor ) ];
                        propertyGrid.SelectedItemWithFocusBackColor = colorMap[ nameof( propertyGrid.SelectedItemWithFocusBackColor ) ];
                        propertyGrid.ViewBackColor = colorMap[ nameof( propertyGrid.ViewBackColor ) ];
                        propertyGrid.ViewForeColor = colorMap[ nameof( propertyGrid.ViewForeColor ) ];
                        break;

                    case TreeView treeView:
                        treeView.LineColor = colorMap[ nameof( treeView.LineColor ) ];
                        break;

                    case ToolStrip toolStrip:
                        toolStrip.Renderer = null;
                        break;

                    case ModelViewControl modelView:
                        modelView.GridColor = colorMap[ nameof( modelView.GridColor ) ];
                        modelView.BackgroundColor = colorMap[ nameof( modelView.BackgroundColor ) ];
                        break;
                }
            }

            foreach ( var childControl in control.Controls )
            {
                if ( !( childControl is Control ) )
                    continue;

                RestoreDefaultStyle( ( Control ) childControl );
            }
        }
    }
}