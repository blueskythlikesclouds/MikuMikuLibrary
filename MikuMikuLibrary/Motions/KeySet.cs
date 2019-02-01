using System;
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
                    Keys.Add( new Key { Frame = reader.ReadUInt16() } );

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
                writer.Write( ( ushort ) Keys.Count );
                foreach ( var key in Keys )
                    writer.Write( ( ushort ) key.Frame );

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
            if ( Keys.Count <= 1 )
                return Keys.Count != 1 ? 0 : Keys[ 0 ].Value;

            Key previous = null;
            Key next = null;

            foreach ( var key in Keys )
            {
                previous = next;
                next = key;

                if ( Math.Abs( key.Frame - frame ) < 0.000001 )
                    return key.Value;

                if ( frame < next.Frame )
                    break;
            }

            float factor = ( frame - Keys[ Keys.Count - 1 ].Frame ) /
                           ( next.Frame - Keys[ Keys.Count - 1 ].Frame );

            if ( IsInterpolated )
                return ( ( factor - 1.0f ) * 2.0f - 1.0f ) * ( factor * factor ) * ( previous.Value - next.Value ) +
                       ( ( factor - 1.0f ) * previous.Interpolation + factor * next.Interpolation ) *
                       ( factor - 1.0f ) * ( frame - Keys[ Keys.Count - 1 ].Frame ) + previous.Value;

            return ( factor * 2.0f - 3.0f ) * ( factor * factor ) * ( previous.Value - next.Value ) + previous.Value;
        }   

        public KeySet()
        {
            Keys = new List<Key>();
        }
    }
}