using MikuMikuLibrary.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MikuMikuLibrary.Archives.Farc
{
    public class FarcArchive : BinaryFile, IArchive<string>
    {
        private readonly List<InternalEntry> entries;

        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public bool CanAdd
        {
            get { return true; }
        }

        public bool CanRemove
        {
            get { return true; }
        }

        public void Add( string handle, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError )
        {
            var entry = entries.FirstOrDefault( x => x.Handle.Equals( handle, StringComparison.OrdinalIgnoreCase ) );

            if ( entry != null )
            {
                switch ( conflictPolicy )
                {
                    case ConflictPolicy.RaiseError:
                        throw new InvalidOperationException( $"Entry already exists ({handle})" );

                    case ConflictPolicy.Replace:
                        entry.Stream = source;
                        entry.OwnsStream = !leaveOpen;
                        break;

                    case ConflictPolicy.Ignore:
                        break;
                }
            }

            entries.Add( new InternalEntry( handle, source, !leaveOpen ) );
        }

        public void Add( string handle, string fileName, ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError )
        {
            Add( handle, File.OpenRead( fileName ), false, conflictPolicy );
        }

        public void Remove( string handle )
        {
            var entry = entries.FirstOrDefault( x => x.Handle.Equals(
                handle, StringComparison.OrdinalIgnoreCase ) );

            if ( entry != null )
            {
                entry.Dispose();
                entries.Remove( entry );
            }
        }

        public bool Contains( string handle )
        {
            return entries.Any( x => x.Handle.Equals(
                handle, StringComparison.OrdinalIgnoreCase ) );
        }

        public IEnumerable<string> EnumerateEntries()
        {
            return entries.Select( x => x.Handle );
        }

        public EntryStream<string> Open( string handle )
        {
            return new EntryStream<string>( handle,
                entries.First( x => x.Handle.Equals( handle, StringComparison.OrdinalIgnoreCase ) ).Stream );
        }

        public IEnumerator<string> GetEnumerator()
        {
            return EnumerateEntries().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateEntries().GetEnumerator();
        }

        protected override void InternalRead( Stream source )
        {
            var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.BigEndian );

            string signature = reader.ReadString( StringBinaryFormat.FixedLength, 4 );
            if ( !( signature == "FARC" || signature == "FArC" || signature == "FArc" ) )
                throw new InvalidDataException( "Invalid signature, excepted FARC/FArC/FArc" );

            uint headerSize = reader.ReadUInt32() + 0x08;

            if ( signature == "FARC" )
            {
                int flags = reader.ReadInt32();
                reader.SeekCurrent( 4 );

                bool isCompressed = ( flags & 2 ) == 2;
                bool isEncrypted = ( flags & 4 ) == 4;

                ICryptoTransform decryptor = null;

                int futureToneCheck = reader.ReadInt32();
                if ( isEncrypted && ( futureToneCheck & ( futureToneCheck - 1 ) ) != 0 )
                {
                    isEncrypted = false;

                    reader.SeekBegin( 16 );
                    var ivBytes = reader.ReadBytes( 16 );

                    decryptor = new AesManaged
                    {
                        KeySize = 128,
                        Key = new byte[]
                        {
                            0x13, 0x72, 0xD5, 0x7B, 0x6E, 0x9E, 0x31, 0xEB, 0xA2, 0x39, 0xB8, 0x3C, 0x15, 0x57, 0xC6, 0xBB,
                        },
                        BlockSize = 128,
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.Zeros,
                        IV = ivBytes,
                    }.CreateDecryptor();

                    var decryptedArchive = new MemoryStream();
                    decryptedArchive.Write( ivBytes, 0, 16 );

                    using ( var crypto = new CryptoStream(
                        source, decryptor, CryptoStreamMode.Read ) )
                    {
                        crypto.CopyTo( decryptedArchive );
                    }

                    reader = new EndianBinaryReader( decryptedArchive, Encoding.UTF8, Endianness.BigEndian );
                    reader.SeekBegin( 20 );
                }

                else if ( isEncrypted )
                {
                    decryptor = new AesManaged
                    {
                        KeySize = 128,
                        Key = new byte[]
                        {
                            0x70, 0x72, 0x6F, 0x6A, 0x65, 0x63, 0x74, 0x5F, 0x64, 0x69, 0x76, 0x61, 0x2E, 0x62, 0x69, 0x6E
                        },
                        BlockSize = 128,
                        Mode = CipherMode.ECB,
                        Padding = PaddingMode.Zeros,
                        IV = new byte[ 16 ],
                    }.CreateDecryptor();
                }

                bool futureToneMode = reader.ReadInt32() == 1;
                int entryCount = reader.ReadInt32();

                if ( futureToneMode )
                    reader.SeekCurrent( 4 );

                entries.Capacity = entryCount;
                while ( reader.Position < headerSize )
                {
                    string entryName = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint entryOffset = reader.ReadUInt32();
                    uint entryCompressedSize = reader.ReadUInt32();
                    uint entryUncompressedSize = reader.ReadUInt32();

                    if ( futureToneMode )
                        reader.SeekCurrent( 4 );

                    reader.ReadAtOffsetAndSeekBack( entryOffset, () =>
                    {
                        long entrySize = 0;
                        if ( isEncrypted )
                        {
                            if ( isCompressed )
                                entrySize = AlignmentUtilities.Align( entryCompressedSize, 16 );
                            else
                                entrySize = AlignmentUtilities.Align( entryUncompressedSize, 16 );
                        }

                        else if ( isCompressed )
                            entrySize = entryCompressedSize;

                        else
                            entrySize = entryUncompressedSize;

                        if ( entryOffset + entrySize > reader.BaseStreamLength )
                            entrySize = reader.BaseStreamLength - entryOffset;

                        Stream entryStream = reader.BaseStream.CreateSubView( entryOffset, entrySize );

                        if ( isEncrypted )
                        {
                            entryStream = new CryptoStream( entryStream, decryptor, CryptoStreamMode.Read );
                            if ( isCompressed && ( entryCompressedSize != entryUncompressedSize ) )
                            {
                                entryStream = new GZipStream( entryStream, CompressionMode.Decompress );
                            }
                        }

                        else if ( isCompressed && ( entryUncompressedSize != entryCompressedSize ) )
                        {
                            entryStream = new GZipStream( entryStream, CompressionMode.Decompress );
                        }

                        entries.Add( new InternalEntry( entryName, entryStream, true ) );
                    } );

                    if ( futureToneMode && ( --entryCount ) < 1 )
                        break;
                }
            }

            else if ( signature == "FArC" )
            {
                reader.SeekCurrent( 4 );

                while ( reader.Position < headerSize )
                {
                    string entryName = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint entryOffset = reader.ReadUInt32();
                    uint entryCompressedSize = reader.ReadUInt32();
                    uint entryUncompressedSize = reader.ReadUInt32();

                    reader.ReadAtOffsetAndSeekBack( entryOffset, () =>
                    {
                        long entrySize = entryCompressedSize;
                        if ( entryOffset + entrySize > reader.BaseStreamLength )
                            entrySize = reader.BaseStreamLength - entryOffset;

                        Stream entryStream = source.CreateSubView( entryOffset, entryCompressedSize );
                        if ( entryUncompressedSize != entryCompressedSize )
                            entryStream = new GZipStream( entryStream, CompressionMode.Decompress );

                        entries.Add( new InternalEntry( entryName, entryStream, true ) );
                    } );
                }
            }

            else if ( signature == "FArc" )
            {
                reader.SeekCurrent( 4 );

                while ( reader.Position < headerSize )
                {
                    string entryName = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint entryOffset = reader.ReadUInt32();
                    uint entrySize = reader.ReadUInt32();

                    reader.ReadAtOffsetAndSeekBack( entryOffset, () =>
                    {
                        long entrySizeLong = entrySize;
                        if ( entryOffset + entrySizeLong > reader.BaseStreamLength )
                            entrySizeLong = reader.BaseStreamLength - entryOffset;

                        var entryStream = reader.BaseStream.CreateSubView( entryOffset, entrySizeLong );
                        entries.Add( new InternalEntry( entryName, entryStream, true ) );
                    } );
                }
            }
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.BigEndian ) )
            {
                writer.Write( "FArc", StringBinaryFormat.FixedLength, 4 );
                writer.PushOffset();
                writer.Write( 0 );
                writer.Write( 16 );

                foreach ( var entry in entries )
                {
                    writer.Write( entry.Handle, StringBinaryFormat.NullTerminated );
                    writer.EnqueueOffsetWriteAligned( 16, 0x78, AlignmentKind.Left, () => entry.Stream.CopyTo( destination ) );
                    writer.Write( ( uint )entry.Stream.Length );
                }

                long headerEnd = destination.Position;
                writer.WriteAtOffsetAndSeekBack( writer.PeekOffset(), () => writer.Write( ( uint )( headerEnd - writer.PopOffset() - 4 ) ) );
                writer.DoEnqueuedOffsetWrites();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach ( var entry in entries )
                entry.Dispose();
        }

        public FarcArchive()
        {
            entries = new List<InternalEntry>();
        }
    }
}
