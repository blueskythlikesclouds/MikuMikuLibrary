using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.IO
{
    public abstract class BinaryFile : IBinaryFile
    {
        public abstract BinaryFileFlags Flags { get; }
        public virtual BinaryFormat Format { get; set; }
        public virtual Endianness Endianness { get; set; }

        public static T Load<T>( Stream source ) where T : IBinaryFile
        {
            var instance = Activator.CreateInstance<T>();
            instance.Load( source );
            return instance;
        }

        public static T Load<T>( string filePath ) where T : IBinaryFile
        {
            var instance = Activator.CreateInstance<T>();
            instance.Load( filePath );
            return instance;
        }

        public void Load( Stream source )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            // Attempt to detect the section format and read with that
            if ( Flags.HasFlag( BinaryFileFlags.HasSectionFormat ) )
            {
                if ( SectionManager.SingleSectionInfosByDataType.TryGetValue( GetType(), out SectionInfo sectionInfo ) )
                {
                    long position = source.Position;
                    var signatureBytes = new byte[ 4 ];
                    source.Read( signatureBytes, 0, signatureBytes.Length );
                    source.Seek( position, SeekOrigin.Begin );

                    if ( Encoding.ASCII.GetString( signatureBytes ) == sectionInfo.Signature )
                    {
                        Format = BinaryFormat.F2nd;
                        sectionInfo.Create( source, this );
                        return;
                    }
                }
            }

            // Or try to read in the old fashioned way
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness ) )
                Read( reader );
        }

        public void Load( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            using ( var stream = File.OpenRead( filePath ) )
                Load( stream );
        }

        public void Save( Stream destination )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            // See if we are supposed to write in sectioned format
            if ( Flags.HasFlag( BinaryFileFlags.HasSectionFormat ) && BinaryFormatUtilities.IsModern( Format ) )
            {
                if ( SectionManager.SingleSectionInfosByDataType.TryGetValue( GetType(), out SectionInfo sectionInfo ) )
                {
                    sectionInfo.Create( this, Endianness ).Write( destination );
                    return;
                }
            }

            // Or try to write in the old fashioned way
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness ) )
            {
                // Push a string table
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );

                Write( writer );

                // Do the enqueued offset writes & string tables
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public void Save( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            using ( var stream = File.Create( filePath ) )
                Save( stream );
        }

        internal abstract void Read( EndianBinaryReader reader, Section section = null );
        internal abstract void Write( EndianBinaryWriter writer, Section section = null );
    }
}
