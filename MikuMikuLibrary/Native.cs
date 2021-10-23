using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MikuMikuLibrary.IBLs.Processing.Interfaces;
using MikuMikuLibrary.Objects.Processing.Fbx.Interfaces;
using MikuMikuLibrary.Objects.Processing.Interfaces;
using MikuMikuLibrary.Textures.Processing.Interfaces;

namespace MikuMikuLibrary
{
    public static class Native
    {
        [DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool DeleteFile( string filePath );

        public static IFbxExporter FbxExporter { get; set; }
        public static ITextureDecoder TextureDecoder { get; }
        public static ITextureEncoder TextureEncoder { get; }
        public static ILightMapImporter LightMapImporter { get; }
        public static IStripifier Stripifier { get; }

        static Native()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            string dllFilePath = Path.Combine( Path.GetDirectoryName( executingAssembly.Location ),
                $"MikuMikuLibrary.Native.X{( IntPtr.Size == 8 ? "64" : "86" )}.dll" );

            // Unblock DLL when extracted through Windows (thanks Sewer)
            DeleteFile( dllFilePath + ":Zone.Identifier" );

            if ( !File.Exists( dllFilePath ) )
                throw new FileNotFoundException( "Native MML library could not be found", dllFilePath );

            var assembly = Assembly.LoadFile( dllFilePath );

            assembly.GetType( "MikuMikuLibrary.NativeContext" )
                .GetMethod( "Initialize", BindingFlags.Public | BindingFlags.Static ).Invoke( null, null );

            FbxExporter = ( IFbxExporter ) Activator.CreateInstance(
                assembly.GetType( "MikuMikuLibrary.Objects.Processing.Fbx.FbxExporterCore" ) );

            TextureDecoder = ( ITextureDecoder ) Activator.CreateInstance(
                assembly.GetType( "MikuMikuLibrary.Textures.Processing.TextureDecoderCore" ) );

            TextureEncoder = ( ITextureEncoder ) Activator.CreateInstance(
                assembly.GetType( "MikuMikuLibrary.Textures.Processing.TextureEncoderCore" ) );

            LightMapImporter = ( ILightMapImporter ) Activator.CreateInstance(
                assembly.GetType( "MikuMikuLibrary.IBLs.Processing.LightMapImporterCore" ) );

            Stripifier = ( IStripifier ) Activator.CreateInstance(
                assembly.GetType( "MikuMikuLibrary.Objects.Processing.StripifierCore" ) );
        }
    }
}