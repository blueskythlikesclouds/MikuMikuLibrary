using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Motions
{
    public class KeySet
    {
        public List<Key> Keys { get; }
        public bool IsInterpolated { get; set; }

        internal KeySetType Type { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            if ( Type == KeySetType.Static )
                Keys.Add( new Key { Value = reader.ReadSingle() } );

            else if ( Type != KeySetType.None )
            {
                ushort keyCount = reader.ReadUInt16();

                Keys.Capacity = keyCount;
                for ( int i = 0; i < keyCount; i++ )
                    Keys.Add( new Key { FrameIndex = reader.ReadUInt16() } );

                reader.Align( 4 );

                IsInterpolated = Type == KeySetType.Interpolated;
                foreach ( var key in Keys )
                {
                    key.Value = reader.ReadSingle();
                    if ( IsInterpolated )
                        key.Interpolation = reader.ReadSingle();
                }
            }
        }

        internal void Write( EndianBinaryWriter writer )
        {
            if ( Keys.Count == 1 )
                writer.Write( Keys[ 0 ].Value );

            else if ( Keys.Count > 1 )
            {
                writer.Write( ( ushort )Keys.Count );
                foreach ( var key in Keys )
                    writer.Write( ( ushort )key.FrameIndex );

                writer.WriteAlignmentPadding( 4 );
                foreach ( var key in Keys )
                {
                    writer.Write( key.Value );
                    if ( IsInterpolated )
                        writer.Write( key.Interpolation );
                }
            }
        }

        public float Interpolate( float frame )
        {
            return 0;
        }

        public KeySet()
        {
            Keys = new List<Key>();
        }
    }
}