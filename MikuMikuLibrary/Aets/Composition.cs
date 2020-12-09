using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class Composition
    {
        internal long ReferenceOffset { get; private set; }

        public List<Layer> Layers { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            ReferenceOffset = reader.Offset;

            int count = reader.ReadInt32();
            reader.ReadOffset( () =>
            {
                Layers.Capacity = count;
                for ( int i = 0; i < count; i++ )
                {
                    var layer = new Layer();
                    layer.Read( reader );
                    Layers.Add( layer );
                }
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            ReferenceOffset = writer.Offset;

            writer.Write( Layers.Count );
            writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
            {
                foreach ( var layer in Layers )
                    layer.Write( writer );
            } );
        }

        public Composition()
        {
            Layers = new List<Layer>();
        }
    }
}