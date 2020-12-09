using System;
using System.IO;
using System.Linq;
using System.Text;
using MikuMikuLibrary.Cryptography;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.IO
{
    public abstract class BinaryFile : IBinaryFile
    {
        protected Stream mStream;
        protected bool mOwnsStream;

        public abstract BinaryFileFlags Flags { get; }
        public virtual BinaryFormat Format { get; set; }
        public virtual Endianness Endianness { get; set; }
        public virtual Encoding Encoding => Encoding.UTF8;

        public void Load( Stream source, bool leaveOpen = false )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
            {
                mStream = source;
                mOwnsStream = !leaveOpen;
            }

            if ( !( Flags.HasFlag( BinaryFileFlags.HasSectionFormat ) && ReadModern() ) )
                ReadClassic();

            if ( !leaveOpen && !Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
                source.Dispose();

            bool ReadModern()
            {
                var stream = source;

                var bytes = new byte[ 4 ];

                string ReadSignature()
                {
                    stream.Read( bytes, 0, bytes.Length );
                    return Encoding.UTF8.GetString( bytes );
                }

                long current = source.Position;
                {
                    string signature = ReadSignature();

                    if ( signature == "DIVA" )
                    {
                        string signatureOtherHalf = ReadSignature();

                        if ( signatureOtherHalf != "FILE" )
                            throw new InvalidDataException( $"Invalid signature (expected DIVAFILE)" );

                        stream = DivafileDecryptor.DecryptToMemoryStream( source, true, true );
                        signature = ReadSignature();
                    }

                    if ( SectionRegistry.SectionInfosBySignature.TryGetValue( signature, out var sectionInfo ) )
                    {
                        using ( var section = sectionInfo.Create( SectionMode.Read, this ) )
                        {
                            section.Read( stream, true );

                            while ( source.Position < source.Length )
                            {
                                signature = ReadSignature();
                                sectionInfo = SectionRegistry.SectionInfosBySignature[ signature ];

                                using ( var siblingSection = sectionInfo.Create( SectionMode.Read ) )
                                {
                                    siblingSection.Read( stream, true );

                                    if ( siblingSection is EndOfFileSection )
                                        break;

                                    if ( section.SectionInfo.SubSectionInfos.TryGetValue( sectionInfo, out var subSectionInfo ) )
                                        subSectionInfo.ProcessPropertyForReading( siblingSection, section );
                                }
                            }

                            section.ProcessData();
                        }

                        return true;
                    }

                    if ( stream != source )
                        stream.Close();
                }

                source.Seek( current, SeekOrigin.Begin );

                return false;
            }

            void ReadClassic()
            {
                using ( var reader = new EndianBinaryReader( source, Encoding, Endianness, true ) )
                {
                    reader.PushBaseOffset();
                    {
                        Read( reader );
                    }
                }
            }
        }

        public virtual void Load( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            Load( File.OpenRead( filePath ) );
        }

        public void Save( Stream destination, bool leaveOpen = false )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            if ( Flags.HasFlag( BinaryFileFlags.HasSectionFormat ) && BinaryFormatUtilities.IsModern( Format ) )
                WriteModern();

            else
                WriteClassic();

            // Adopt this stream
            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
            {
                if ( mOwnsStream )
                    mStream.Close();

                mStream = destination;
                mOwnsStream = !leaveOpen;

                mStream.Flush();
            }

            else if ( !leaveOpen )
            {
                destination.Close();
            }

            void WriteModern()
            {
                using ( var section = GetSectionInstanceForWriting() )
                {
                    section.Write( destination );

                    foreach ( var subSection in section.Sections.Where( x => x.SectionInfo.IsBinaryFile ) )
                        subSection.Write( destination );

                    using ( var eofSection = new EndOfFileSection( SectionMode.Write, this ) ) 
                        eofSection.Write( destination );
                }
            }

            void WriteClassic()
            {
                using ( var writer = new EndianBinaryWriter( destination, Encoding, Endianness, true ) )
                {
                    writer.PushBaseOffset();
                    {
                        // Push a string table
                        writer.PushStringTable( 16, AlignmentMode.Center, StringBinaryFormat.NullTerminated );
                        {
                            Write( writer );
                        }
                        writer.PerformScheduledWrites();
                        writer.PopStringTablesReversed();
                    }
                }
            }
        }

        public virtual void Save( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            // Prevent any kind of conflict.
            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) && mStream is FileStream fileStream )
            {
                filePath = Path.GetFullPath( filePath );
                string thisFilePath = Path.GetFullPath( fileStream.Name );

                if ( filePath.Equals( thisFilePath, StringComparison.OrdinalIgnoreCase ) )
                {
                    do
                    {
                        thisFilePath += "_";
                    } while ( File.Exists( thisFilePath ) );

                    using ( var destination = File.Create( thisFilePath ) )
                        Save( destination );

                    fileStream.Close();

                    File.Delete( filePath );
                    File.Move( thisFilePath, filePath );

                    mStream = new FileStream( filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite );
                    mOwnsStream = true;

                    return;
                }
            }

            Save( new FileStream( filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite ) );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public abstract void Read( EndianBinaryReader reader, ISection section = null );
        public abstract void Write( EndianBinaryWriter writer, ISection section = null );

        public static T Load<T>( Stream source, bool leaveOpen = false ) where T : IBinaryFile, new()
        {
            var instance = new T();
            instance.Load( source, leaveOpen );
            return instance;
        }

        public static T Load<T>( string filePath ) where T : IBinaryFile, new()
        {
            var instance = new T();
            instance.Load( filePath );
            return instance;
        }

        public static T LoadIfExist<T>( string filePath ) where T : IBinaryFile, new()
        {
            var instance = new T();

            if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) )
                return instance;

            instance.Load( filePath );
            return instance;
        }

        public void LoadIfExist( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) )
                return;

            Load( filePath );
        }

        protected virtual ISection GetSectionInstanceForWriting()
        {
            var type = GetType();

            if ( !SectionRegistry.SingleSectionInfosByDataType.TryGetValue( type, out var sectionInfo ) )
                throw new NotImplementedException();

            return sectionInfo.Create( SectionMode.Write, this );
        }

        /// <summary>
        ///     Cleans up resources used by the object.
        /// </summary>
        /// <param name="disposing">Whether the managed objects are going to be disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposing && Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) && mOwnsStream )
                mStream?.Close();
        }

        ~BinaryFile()
        {
            Dispose( false );
        }
    }
}