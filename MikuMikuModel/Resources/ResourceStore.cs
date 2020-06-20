using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MikuMikuModel.Resources
{
    public static class ResourceStore
    {
        private static readonly Dictionary<string, object> sResources =
            new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );

        private static readonly string sResourcesDirectory =
            Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Resources" );

        public static string GetPath( string relativePath ) => 
            Path.GetFullPath( Path.Combine( sResourcesDirectory, relativePath ) );

        public static string GetPathIfExist( string relativePath )
        {
            string fullPath = GetPath( relativePath );

            if ( !File.Exists( fullPath ) )
                throw new FileNotFoundException( "Requested resource path does not exist", fullPath );

            return fullPath;
        }

        public static T Load<T>( string relativePath, Func<string, T> loadFunc )
        {
            string fullPath = GetPath( relativePath );

            if ( sResources.TryGetValue( fullPath, out var obj ) )
                return ( T ) obj;

            if ( !File.Exists( fullPath ) )
                throw new FileNotFoundException( "Requested resource could not be found", fullPath );

            obj = loadFunc( fullPath );

            sResources[ fullPath ] = obj;
            return ( T ) obj;
        }

        public static Bitmap LoadBitmap( string relativePath ) => 
            Load( relativePath, path => new Bitmap( path ) );

        public static Icon LoadIcon( string relativePath ) =>
            Load( relativePath, path => new Icon( path ) );

        static ResourceStore()
        {
            if ( !Directory.Exists( sResourcesDirectory ) )
                Directory.CreateDirectory( sResourcesDirectory );
        }
    }
}