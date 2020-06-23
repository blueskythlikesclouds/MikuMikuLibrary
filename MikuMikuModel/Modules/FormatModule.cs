using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MikuMikuModel.Modules
{
    public abstract class FormatModule<T> : IFormatModule
    {
        public Type ModelType => typeof( T );
        public abstract IReadOnlyList<FormatExtension> Extensions { get; }

        public virtual bool Match( string fileName )
        {
            string extension = Path.GetExtension( fileName ).Trim( '.' );

            return Extensions.Any( x => x.Extension == "*" ) || Extensions.Any(
                x => x.Extension.Equals( extension, StringComparison.OrdinalIgnoreCase ) );
        }

        public virtual bool Match( byte[] buffer )
        {
            return true;
        }

        public virtual T Import( Stream source, string fileName = null )
        {
            return ImportCore( source, fileName );
        }

        public virtual T Import( string filePath )
        {
            using ( var stream = File.OpenRead( filePath ) )
                return ImportCore( stream, Path.GetFileName( filePath ) );
        }

        public virtual void Export( T model, Stream destination, string fileName = null )
        {
            ExportCore( model, destination, fileName );
        }

        public virtual void Export( T model, string filePath )
        {
            using ( var stream = File.Create( filePath ) )
                ExportCore( model, stream, Path.GetFileName( filePath ) );
        }

        protected abstract T ImportCore( Stream source, string fileName );
        protected abstract void ExportCore( T model, Stream destination, string fileName );

        #region Explicit IFormatModule Implementation

        object IFormatModule.Import( string filePath )
        {
            return Import( filePath );
        }

        object IFormatModule.Import( Stream source, string fileName )
        {
            return Import( source, fileName );
        }

        void IFormatModule.Export( object model, string filePath )
        {
            Export( ( T ) model, filePath );
        }

        void IFormatModule.Export( object model, Stream destination, string fileName )
        {
            Export( ( T ) model, destination, fileName );
        }

        #endregion
    }
}