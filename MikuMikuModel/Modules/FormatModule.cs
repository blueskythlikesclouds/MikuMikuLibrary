using System;
using System.IO;
using System.Linq;

namespace MikuMikuModel.Modules
{
    public abstract class FormatModule<T> : IFormatModule
    {
        public abstract FormatModuleFlags Flags { get; }
        public Type ModelType => typeof( T );
        public abstract string Name { get; }
        public abstract string[] Extensions { get; }

        public virtual T Import( Stream source, string fileName = null )
        {
            if ( !Flags.HasFlag( FormatModuleFlags.Import ) )
                throw new NotSupportedException( "FormatModule can't import" );

            return ImportCore( source, fileName );
        }

        public virtual T Import( string filePath )
        {
            if ( !Flags.HasFlag( FormatModuleFlags.Import ) )
                throw new NotSupportedException( "FormatModule can't import" );

            using ( var stream = File.OpenRead( filePath ) )
                return ImportCore( stream, Path.GetFileName( filePath ) );
        }

        public virtual void Export( T obj, Stream destination, string fileName = null )
        {
            if ( !Flags.HasFlag( FormatModuleFlags.Export ) )
                throw new NotSupportedException( "FormatModule can't export" );

            ExportCore( obj, destination, fileName );
        }

        public virtual void Export( T obj, string filePath )
        {
            if ( !Flags.HasFlag( FormatModuleFlags.Export ) )
                throw new NotSupportedException( "FormatModule can't export" );

            using ( var stream = File.Create( filePath ) )
                ExportCore( obj, stream, Path.GetFileName( filePath ) );
        }

        public virtual bool Match( string fileName ) =>
            Extensions.Contains( "*" ) || Extensions.Contains( Path.GetExtension( fileName ).Trim( '.' ),
                StringComparer.OrdinalIgnoreCase );

        public virtual bool Match( byte[] buffer ) => true;

        protected abstract T ImportCore( Stream source, string fileName );
        protected abstract void ExportCore( T obj, Stream destination, string fileName );

        #region Explicit IFormatModule Implementation
        object IFormatModule.Import( string filePath ) => Import( filePath );
        object IFormatModule.Import( Stream source, string fileName ) => Import( source, fileName );

        void IFormatModule.Export( object obj, string filePath ) => Export( ( T ) obj, filePath );
        void IFormatModule.Export( object obj, Stream destination, string fileName ) => Export( ( T ) obj, destination, fileName );
        #endregion
    }
}