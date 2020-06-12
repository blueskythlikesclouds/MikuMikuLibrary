using System;
using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects.Extra.Blocks;

namespace MikuMikuLibrary.Objects.Extra
{
    public abstract class Block
    {
        internal static readonly IReadOnlyDictionary<string, Func<Block>> BlockFactory =
            new Dictionary<string, Func<Block>>
            {
                { "OSG", () => new OsageBlock() },
                { "EXP", () => new ExpressionBlock() },
                { "MOT", () => new MotionBlock() },
                { "CNS", () => new ConstraintBlock() }
            };

        public abstract string Signature { get; }

        public string ParentName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        internal virtual void Read( EndianBinaryReader reader, StringSet stringSet )
        {
            ParentName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Position = reader.ReadVector3();
            Rotation = reader.ReadVector3();
            Scale = reader.ReadVector3();
            ReadBody( reader, stringSet );
        }

        internal virtual void Write( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( ParentName );
            writer.Write( Position );
            writer.Write( Rotation );
            writer.Write( Scale );
            WriteBody( writer, stringSet );
        }

        internal abstract void ReadBody( EndianBinaryReader reader, StringSet stringSet );
        internal abstract void WriteBody( EndianBinaryWriter writer, StringSet stringSet );
    }
}