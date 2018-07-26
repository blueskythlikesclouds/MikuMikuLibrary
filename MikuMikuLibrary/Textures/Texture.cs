using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public class Texture
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<TextureMipMap> MipMaps { get; }

        public bool IsYCbCr
        {
            get
            {
                return MipMaps.Count == 2 &&
                  ( MipMaps[ 0 ].Format == TextureFormat.ATI2 && MipMaps[ 1 ].Format == TextureFormat.ATI2 ) &&
                  ( MipMaps[ 0 ].Width == MipMaps[ 1 ].Width << 1 ) && ( MipMaps[ 0 ].Height == MipMaps[ 1 ].Height << 1 );
            }
        }

        internal void Read( EndianBinaryReader reader )
        {
            reader.PushBaseOffset();

            var signature = reader.ReadString( StringBinaryFormat.FixedLength, 3 );
            if ( signature != "TXP" )
                throw new InvalidDataException( "Invalid signature (expected TXP)" );

            byte typeNum = reader.ReadByte();
            if ( typeNum != 4 )
                throw new InvalidDataException( "Invalid type number (expected 4)" );

            int mipMapCount = reader.ReadInt32();
            int mipMapCountWithRubbish = reader.ReadInt32();

            MipMaps.Capacity = mipMapCount;
            for ( int i = 0; i < mipMapCount; i++ )
            {
                reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
                {
                    var mipMap = new TextureMipMap();
                    mipMap.Read( reader );
                    MipMaps.Add( mipMap );
                } );
            }

            reader.PopBaseOffset();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.PushBaseOffset();

            writer.Write( "TXP", StringBinaryFormat.FixedLength, 3 );
            writer.Write( ( byte )4 );
            writer.Write( MipMaps.Count );
            writer.Write( MipMaps.Count | 0x00010101 );

            foreach ( var mipMap in MipMaps )
            {
                writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
                {
                    mipMap.Write( writer );
                } );
            }

            writer.PopBaseOffset();
        }

        public Texture()
        {
            ID = -1;
            MipMaps = new List<TextureMipMap>();
        }
    }
}
