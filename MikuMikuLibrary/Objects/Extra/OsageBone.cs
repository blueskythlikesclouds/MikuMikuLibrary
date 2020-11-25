using System;
using System.ComponentModel;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra
{
    public class OsageBone
    {
        public string Name { get; set; }
        public float Length { get; set; }

        public string SiblingName { get; set; }
        public float SiblingDistance { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Length}";
        }

        internal void Read( EndianBinaryReader reader, StringSet stringSet )
        {
            Name = stringSet.ReadString( reader );
            Length = reader.ReadSingle();
            reader.SeekCurrent( 4 );
        }

        internal void Write( EndianBinaryWriter writer, StringSet stringSet )
        {
            uint id = stringSet.GetStringId( Name );
            writer.Write( id );
            writer.Write( Length );
            writer.Write( id & 0x7FFF ); // Unsure if it's always this way
        }

        internal void WriteSiblingInfo( EndianBinaryWriter writer, StringSet stringSet )
        {
            stringSet.WriteString( writer, Name );
            stringSet.WriteString( writer, SiblingName );
            writer.Write( SiblingDistance );
        }

        [Obsolete( "The real purpose of this property has been figured. Please use Length instead." ), Browsable( false )]
        public float Stiffness
        {
            get => Length;
            set => Length = value;
        }
    }
}