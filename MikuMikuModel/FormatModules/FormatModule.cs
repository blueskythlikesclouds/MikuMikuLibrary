using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public abstract class FormatModule<T> : IFormatModule
    {
        public abstract FormatModuleFlags Flags { get; }

        public abstract string Name { get; }
        public abstract string[] Extensions { get; }

        public Type ModelType
        {
            get { return typeof( T ); }
        }

        public bool CanImport( Stream source, string fileName = null )
        {
            if ( !Flags.HasFlag( FormatModuleFlags.Import ) )
                return false;

            if ( !string.IsNullOrEmpty( fileName ) && !FormatModuleUtilities.HasAnyExtension( fileName, this ) )
                return false;

            return CanImportCore( source, fileName );
        }

        public bool CanImport( string filePath )
        {
            if ( !Flags.HasFlag( FormatModuleFlags.Import ) )
                return false;

            using ( var stream = File.OpenRead( filePath ) )
                return CanImport( stream, Path.GetFileName( filePath ) );
        }

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

        protected abstract bool CanImportCore( Stream source, string fileName );
        protected abstract T ImportCore( Stream source, string fileName );
        protected abstract void ExportCore( T obj, Stream destination, string fileName );

        object IFormatModule.Import( Stream source, string fileName ) => Import( source, fileName );
        object IFormatModule.Import( string filePath ) => Import( filePath );

        void IFormatModule.Export( object obj, Stream destination, string fileName ) => Export( ( T )obj, destination, fileName );
        void IFormatModule.Export( object obj, string filePath ) => Export( ( T )obj, filePath );
    }
}
