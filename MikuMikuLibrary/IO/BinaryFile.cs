using System;
using System.IO;

namespace MikuMikuLibrary.IO
{
    public abstract class BinaryFile : IDisposable
    {
        protected Stream stream;
        protected bool ownsStream;

        public abstract bool CanLoad { get; }
        public abstract bool CanSave { get; }

        public static T Load<T>( Stream source, bool leaveOpen = false ) where T : BinaryFile
        {
            var instance = Activator.CreateInstance<T>() as T;
            instance.Load( source, leaveOpen );
            return instance;
        }

        public static T Load<T>( string fileName ) where T : BinaryFile
        {
            var instance = Activator.CreateInstance<T>() as T;
            instance.Load( fileName );
            return instance;
        }

        public static T LoadIfExist<T>( string fileName ) where T : BinaryFile
        {
            var instance = Activator.CreateInstance<T>() as T;
            instance.LoadIfExist( fileName );
            return instance;
        }

        public virtual void Dispose()
        {
            if ( ownsStream && stream != null )
                stream.Dispose();
        }

        public virtual void Load( string fileName )
        {
            if ( !CanLoad )
                throw new NotSupportedException( "Loading is not supported" );

            Load( File.OpenRead( fileName ), false );
        }

        public virtual void LoadIfExist( string fileName )
        {
            if ( !CanLoad )
                throw new NotSupportedException( "Loading is not supported" );

            if ( !File.Exists( fileName ) )
                return;

            Load( fileName );
        }

        public virtual void Load( Stream source, bool leaveOpen = false )
        {
            if ( !CanLoad )
                throw new NotSupportedException( "Loading is not supported" );

            stream = source;
            ownsStream = !leaveOpen;

            if ( !source.CanSeek ) //|| source is FileStream )
            {
                var memoryStream = new MemoryStream();
                source.CopyTo( memoryStream );
                memoryStream.Seek( 0, SeekOrigin.Begin );
                InternalRead( memoryStream );
            }
            else
            {
                InternalRead( source );
            }
        }

        public virtual void Save( string fileName )
        {
            if ( !CanSave )
                throw new NotSupportedException( "Saving is not supported" );

            Save( File.Create( fileName ), false );
        }

        public virtual void Save( Stream destination, bool leaveOpen = false )
        {
            if ( !CanSave )
                throw new NotSupportedException( "Saving is not supported" );

            //stream = destination;
            //ownsStream = !leaveOpen;

            //if ( destination is FileStream )
            //{
            //    var memoryStream = new MemoryStream();
            //    InternalWrite( memoryStream );
            //    memoryStream.Seek( 0, SeekOrigin.Begin );
            //    memoryStream.CopyTo( destination );
            //}

            InternalWrite( destination );

            if ( !leaveOpen )
                destination.Close();
        }

        protected abstract void InternalRead( Stream source );
        protected abstract void InternalWrite( Stream destination );
    }
}
