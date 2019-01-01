using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.IO
{
    public abstract class BinaryFile : IBinaryFile
    {
        protected Stream stream;
        protected bool ownsStream;

        public abstract BinaryFileFlags Flags { get; }
        public virtual BinaryFormat Format { get; set; }
        public virtual Endianness Endianness { get; set; }

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

        public void Load( Stream source, bool leaveOpen = false )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
            {
                stream = source;
                ownsStream = !leaveOpen;
            }

            bool readAsSection = false;

            // Attempt to detect the section format and read with that
            if ( Flags.HasFlag( BinaryFileFlags.HasSectionFormat ) )
            {
                long position = source.Position;
                var signatureBytes = new byte[ 4 ];
                source.Read( signatureBytes, 0, signatureBytes.Length );
                source.Seek( position, SeekOrigin.Begin );

                var signature = Encoding.ASCII.GetString( signatureBytes );
                if ( SectionManager.SectionInfosBySignature.TryGetValue( signature, out SectionInfo sectionInfo ) )
                {
                    sectionInfo.Read( source, this );
                    readAsSection = true;
                }
            }

            if ( !readAsSection )
            {
                // Or try to read in the old fashioned way
                using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness ) )
                {
                    reader.PushBaseOffset();
                    {
                        Read( reader );
                    }
                    reader.PopBaseOffset();
                }
            }

            if ( !leaveOpen && !Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
                source.Dispose();
        }

        public virtual void Load( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            Load( File.OpenRead( filePath ), false );
        }

        public void LoadIfExist( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) )
                return;

            Load( filePath );
        }

        public void Save( Stream destination, bool leaveOpen = false )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            // See if we are supposed to write in sectioned format
            if ( Flags.HasFlag( BinaryFileFlags.HasSectionFormat ) && BinaryFormatUtilities.IsModern( Format ) )
                GetSectionInstanceForWriting().Write( destination );

            else
            {
                // Or try to write in the old fashioned way
                using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness ) )
                {
                    writer.PushBaseOffset();
                    {
                        // Push a string table
                        writer.PushStringTable( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                        {
                            Write( writer );
                        }
                        // Do the enqueued offset writes & string tables
                        writer.DoEnqueuedOffsetWrites();
                        writer.PopStringTablesReversed();
                    }
                    writer.PopBaseOffset();
                }
            }

            // Adopt this stream
            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
            {
                if ( ownsStream )
                    stream.Dispose();

                stream = destination;
                ownsStream = !leaveOpen;

                stream.Flush();
            }
            else if ( !leaveOpen )
            {
                destination.Close();
            }
        }

        public virtual void Save( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            // Prevent any kind of conflict.
            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) && stream is FileStream fileStream )
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
                        Save( destination, false );

                    fileStream.Close();

                    File.Delete( filePath );
                    File.Move( thisFilePath, filePath );

                    stream = new FileStream( filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite );
                    ownsStream = true;

                    return;
                }
            }

            Save( new FileStream( filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite ), false );
        }

        protected virtual Section GetSectionInstanceForWriting()
        {
            if ( SectionManager.SingleSectionInfosByDataType.TryGetValue( GetType(), out SectionInfo sectionInfo ) )
                return sectionInfo.Create( this, Endianness );
            else
                throw new NotImplementedException( "Section writing is not yet implemented" );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Cleans up resources used by the object.
        /// </summary>
        /// <param name="disposing">Whether or not the managed objects are going to be disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposing && Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) && ownsStream )
                stream?.Dispose();
        }

        ~BinaryFile()
        {
            Dispose( false );
        }

        public abstract void Read( EndianBinaryReader reader, Section section = null );
        public abstract void Write( EndianBinaryWriter writer, Section section = null );
    }
}
