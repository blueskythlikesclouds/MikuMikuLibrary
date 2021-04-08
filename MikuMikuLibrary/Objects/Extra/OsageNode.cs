using System;
using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra
{
    public class OsageNode
    {
        public string Name { get; set; }
        public float Length { get; set; }
        public Vector3 Rotation { get; set; }

        public string SiblingName { get; set; }
        public float SiblingMaxDistance { get; set; }

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

        internal void ReadOsgBlockInfo( EndianBinaryReader reader, StringSet stringSet )
        {
            Name = stringSet.ReadString( reader );
            Length = reader.ReadSingle();
            Rotation = reader.ReadVector3();
        }

        internal void WriteOsgBlockInfo( EndianBinaryWriter writer, StringSet stringSet )
        {
            stringSet.WriteString( writer, Name );
            writer.Write( Length );
            writer.Write( Rotation );
        }

        internal void WriteSiblingInfo( EndianBinaryWriter writer, StringSet stringSet )
        {
            stringSet.WriteString( writer, Name );
            stringSet.WriteString( writer, SiblingName );
            writer.Write( SiblingMaxDistance );
        }
    }
}