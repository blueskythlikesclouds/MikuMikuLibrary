﻿using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks;

public class ExpressionBlock : NodeBlock
{
    public override string Signature => "EXP";

    public List<string> Expressions { get; }

    internal override void ReadBody(EndianBinaryReader reader, StringSet stringSet)
    {
        Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

        int expressionCount = reader.ReadInt32();

        Expressions.Capacity = expressionCount;

        for (int i = 0; i < expressionCount; i++)
            Expressions.Add(reader.ReadStringOffset(StringBinaryFormat.NullTerminated));
    }

    internal override void WriteBody(EndianBinaryWriter writer, StringSet stringSet, BinaryFormat format)
    {
        writer.WriteStringOffset(Name);
        writer.Write(Expressions.Count);

        foreach (string expression in Expressions)
            writer.WriteStringOffset(expression);

        writer.WriteNulls((9 - Expressions.Count) * writer.AddressSpace.GetByteSize());
    }

    public ExpressionBlock()
    {
        Expressions = new List<string>();
    }
}