using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects.Extra.Blocks;

namespace MikuMikuLibrary.Objects.Extra
{
    public class StringSet
    {
        private readonly List<string> mStrings;

        public IReadOnlyList<string> Strings => mStrings;

        public string ReadString( EndianBinaryReader reader )
        {
            int index = reader.ReadInt32();
            return ( index & 0x8000 ) == 0 ? null : mStrings[ index & 0x7FFF ];
        }

        public uint GetStringId( string value )
        {
            int index = mStrings.IndexOf( value );
            return ( uint ) ( index != -1 ? 0x8000 | index : 0 );
        }

        public void WriteString( EndianBinaryWriter writer, string value )
        {
            writer.Write( GetStringId( value ) );
        }

        public StringSet( EndianBinaryReader reader, long offset, int count ) : this()
        {
            mStrings.Capacity = count;

            reader.ReadAtOffset( offset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    mStrings.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );
        }

        public StringSet( Skin skin ) : this()
        {
            foreach ( var block in skin.Blocks )
            {
                switch ( block )
                {
                    case ConstraintBlock constraintBlock:
                    {
                        AddString( constraintBlock.BoneName );
                        AddString( constraintBlock.SourceBoneName );

                        switch ( constraintBlock.Data )
                        {
                            case DirectionConstraintData directionData:
                                AddString( directionData.Field20 );
                                break;            
                            
                            case PositionConstraintData positionData:
                                AddString( positionData.Field20 );
                                break;
                        }

                        break;
                    }

                    case ExpressionBlock expressionBlock:
                        AddString( expressionBlock.BoneName );
                        break;

                    case MotionBlock motionBlock:
                    {
                        foreach ( var bone in motionBlock.Bones )
                            AddString( bone.Name );

                        break;
                    }

                    case OsageBlock osageBlock:
                    {
                        AddString( osageBlock.ExternalName );

                        foreach ( var osageBone in osageBlock.Bones )
                        {
                            AddString( osageBone.Name );
                            AddString( osageBone.SiblingName );
                        }

                        AddString( osageBlock.InternalName );
                        break;
                    }
                }
            }

            foreach ( var node in skin.Bones.Where( node => node.IsEx ) )
                AddString( node.Name );

            void AddString( string value )
            {
                if ( !string.IsNullOrEmpty( value ) && !mStrings.Contains( value ) )
                    mStrings.Add( value );
            }
        }

        public StringSet()
        {
            mStrings = new List<string>();
        }
    }
}