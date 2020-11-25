using System;
using System.Collections.Generic;
using System.ComponentModel;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class ExpressionBlock : NodeBlock
    {
        public override string Signature => "EXP";

        public List<string> Expressions { get; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            int expressionCount = reader.ReadInt32();

            Expressions.Capacity = expressionCount;

            for ( int i = 0; i < expressionCount; i++ )
                Expressions.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( Name );
            writer.Write( Expressions.Count );

            foreach ( string expression in Expressions )
                writer.AddStringToStringTable( expression );

            writer.WriteNulls( ( 9 - Expressions.Count ) * writer.AddressSpace.GetByteSize() );
        }

        public ExpressionBlock()
        {
            Expressions = new List<string>();
        }

        // Obsolete properties

        [Obsolete( "This property has been renamed. Please use Name instead." ), Browsable( false )]
        public string BoneName
        {
            get => Name;
            set => Name = value;
        }
    }
}