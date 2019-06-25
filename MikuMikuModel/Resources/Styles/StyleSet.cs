using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace MikuMikuModel.Resources.Styles
{
    public static class StyleSet
    {
        private static readonly XmlSerializer sStyleSerializer = new XmlSerializer( typeof( Style ) );
        private static readonly List<Style> sStyles = new List<Style>();
        private static Style sCurrentStyle;

        private static readonly string sStylesDirectory = ResourceStore.GetPath( "Styles" );
        private static readonly string sCurrentStyleFilePath = ResourceStore.GetPath( "Style.txt" );

        public static IReadOnlyList<Style> Styles => sStyles;

        public static event EventHandler<StyleChangedEventArgs> StyleChanged;

        public static Style CurrentStyle
        {
            get => sCurrentStyle;
            set
            {
                if ( value == sCurrentStyle )
                    return;

                if ( !sStyles.Contains( value ) && value != null )
                {
                    using ( var stream = File.CreateText( Path.Combine( sStylesDirectory, value.Name + ".xml" ) ) )
                        sStyleSerializer.Serialize( stream, value );

                    sStyles.Add( value );
                }

                sCurrentStyle = value;
                StyleChanged?.Invoke( null, new StyleChangedEventArgs( value ) );

                File.WriteAllText( sCurrentStyleFilePath, sCurrentStyle?.Name );
            }
        }

        static StyleSet()
        {
            foreach ( string filePath in Directory.GetFiles( sStylesDirectory, "*.xml" ) )
            {
                using ( var stream = File.OpenText( filePath ) )
                    sStyles.Add( ( Style ) sStyleSerializer.Deserialize( stream ) );
            }

            if ( !File.Exists( sCurrentStyleFilePath ) )
                return;

            string currentStyleName = File.ReadAllText( sCurrentStyleFilePath );
            sCurrentStyle = !string.IsNullOrEmpty( currentStyleName )
                ? sStyles.FirstOrDefault( x => x.Name.Equals( currentStyleName ) )
                : null;
        }
    }

    public class StyleChangedEventArgs : EventArgs
    {
        public Style Style { get; }

        public StyleChangedEventArgs( Style style )
        {
            Style = style;
        }
    }
}