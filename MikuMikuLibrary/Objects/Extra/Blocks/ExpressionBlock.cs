using System.Collections.Generic;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class ExpressionBlock : Block
    {
        public override string Signature => "EXP";

        public string BoneName { get; set; }
        public List<string> Expressions { get; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            BoneName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            int expressionCount = reader.ReadInt32();

            Expressions.Capacity = expressionCount;
            for ( int i = 0; i < expressionCount; i++ )
                Expressions.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( BoneName );
            writer.Write( Expressions.Count );

            foreach ( string expression in Expressions )
                writer.AddStringToStringTable( expression );

            writer.WriteNulls( ( 9 - Expressions.Count ) * writer.AddressSpace.GetByteSize() );
        }

        public ExpressionBlock()
        {
            Expressions = new List<string>();
        }
    }
}