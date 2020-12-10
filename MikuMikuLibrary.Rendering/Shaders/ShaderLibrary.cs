using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.Rendering.Shaders
{
    public sealed class ShaderLibrary : IDisposable
    {
        public const string VertexShaderExtension = ".vert.glsl";
        public const string FragmentShaderExtension = ".frag.glsl";

        private readonly Dictionary<string, Shader> mShaders;

        public string DirectoryPath { get; }

        public IReadOnlyDictionary<string, Shader> Shaders => mShaders;

        private string ResolveShader( string shaderFilePath, string directoryPath, bool fixVersionDirective = true )
        {
            int index;

            string shader = File.ReadAllText( shaderFilePath );

            // Search for all include directives and insert the corresponding files recursively
            while ( ( index = shader.IndexOf( "#include", StringComparison.Ordinal ) ) != -1 )
            {
                int firstQuoteIndex = shader.IndexOf( '"', index + 8 );
                int secondQuoteIndex = shader.IndexOf( '"', firstQuoteIndex + 1 );

                if ( firstQuoteIndex == -1 || secondQuoteIndex == -1 )
                    throw new InvalidDataException( "Include directive formatted incorrectly" );

                string filePath = Path.GetFullPath( Path.Combine( directoryPath,
                    shader.Substring( firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1 ) ) );

                if ( !File.Exists( filePath ) )
                    throw new FileNotFoundException( "File specified in include directive does not exist", filePath );

                string begin = shader.Substring( 0, index );
                string end = shader.Substring( secondQuoteIndex + 1, shader.Length - secondQuoteIndex - 1 );

                shader = begin + ResolveShader( filePath, Path.GetDirectoryName( filePath ), false ) + end;
            }

            if ( !fixVersionDirective )
                return shader;

            // Remove possibly duplicate version directives so we can insert our own
            while ( ( index = shader.IndexOf( "#version", StringComparison.Ordinal ) ) != -1 )
            {
                int endIndex = shader.IndexOf( '\n', index + 8 );

                if ( endIndex == -1 )
                    endIndex = shader.IndexOf( '\r', index + 8 );

                if ( endIndex == -1 )
                    break;

                shader = shader.Substring( 0, index ) +
                         shader.Substring( endIndex + 1, shader.Length - endIndex - 1 );
            }

            shader = "#version 330\n" + shader;

#if DEBUG
            File.WriteAllText(
                Path.Combine( DirectoryPath, "Dump", Path.GetFileName( shaderFilePath ) ), shader );
#endif

            return shader;
        }

        public Shader Create( string name )
        {
            if ( mShaders.TryGetValue( name, out var shaderProgram ) )
                return shaderProgram;

            string vertexShaderFilePath = Path.Combine( DirectoryPath, name + VertexShaderExtension );
            string fragmentShaderFilePath = Path.Combine( DirectoryPath, name + FragmentShaderExtension );

            if ( !File.Exists( vertexShaderFilePath ) || !File.Exists( fragmentShaderFilePath ) )
                throw new FileNotFoundException( $"Failed to find shader files of {name}" );

            string directoryPath = Path.Combine( DirectoryPath, Path.GetDirectoryName( name ) );

            shaderProgram = Shader.Create( 
                ResolveShader( vertexShaderFilePath, directoryPath ),
                ResolveShader( fragmentShaderFilePath, directoryPath ) );

            if ( shaderProgram == null )
                return null;

            mShaders.Add( name, shaderProgram );

            return shaderProgram;
        }

        public void Dispose()
        {
            foreach ( var shaderProgram in mShaders.Values )
                shaderProgram.Dispose();
        }

        public ShaderLibrary( string directoryPath )
        {
            mShaders = new Dictionary<string, Shader>( StringComparer.OrdinalIgnoreCase );

            DirectoryPath = directoryPath;

#if DEBUG
            Directory.CreateDirectory( Path.Combine( DirectoryPath, "Dump" ) );
#endif
        }
    }
}