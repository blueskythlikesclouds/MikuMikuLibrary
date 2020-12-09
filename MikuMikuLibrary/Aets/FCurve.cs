using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class Key
    {
        public float Frame { get; set; }
        public float Value { get; set; }
        public float Tangent { get; set; }

        public override string ToString() => 
            $"{Frame}, {Value}, {Tangent}";
    }

    public class FCurve
    {
        public List<Key> Keys { get; }

        internal void Read( EndianBinaryReader reader )
        {
            int count = reader.ReadInt32();

            reader.ReadOffset( () =>
            {
                Keys.Capacity = count;

                if ( count == 1 )
                {
                    Keys.Add( new Key { Value = reader.ReadSingle() } );
                    return;
                }

                for ( int i = 0; i < count; i++ )
                    Keys.Add( new Key { Frame = reader.ReadSingle() } );

                foreach ( var key in Keys )
                {
                    key.Value = reader.ReadSingle();
                    key.Tangent = reader.ReadSingle();
                }
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Keys.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                if ( Keys.Count == 1 )
                {
                    writer.Write( Keys[ 0 ].Value );
                    return;
                }

                foreach ( var key in Keys )
                    writer.Write( key.Frame );

                foreach ( var key in Keys )
                {
                    writer.Write( key.Value );
                    writer.Write( key.Tangent );
                }
            } );
        }

        public FCurve()
        {
            Keys = new List<Key>();
        }
    }
}