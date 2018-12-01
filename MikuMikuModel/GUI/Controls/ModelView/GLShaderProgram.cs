using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLShaderProgram : IDisposable
    {
        private static readonly Dictionary<string, GLShaderProgram> shaderPrograms;
        private readonly Dictionary<string, int> uniforms;

        public static string ShaderDirectoryPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Shaders" );

        public static IReadOnlyDictionary<string, GLShaderProgram> ShaderPrograms
        {
            get { return shaderPrograms; }
        }

        public string Name { get; }
        public int ID { get; }

        public void Use()
        {
            GL.UseProgram( ID );
        }

        public void RegisterUniform( string name )
        {
            Debug.WriteLine( $"RegisterUniform: {name}" );
            uniforms[ name ] = GL.GetUniformLocation( ID, name );
        }

        public void RegisterUniforms()
        {
            GL.GetProgram( ID, GetProgramParameterName.ActiveUniforms, out int count );
            for ( int i = 0; i < count; i++ )
            {
                GL.GetActiveUniform( ID, i, 32, out _, out _, out _, out string name );
                uniforms[ name ] = GL.GetUniformLocation( ID, name );
                Debug.WriteLine( $"RegisterUniforms: {name}" );
            }
        }

        public void SetUniform( string name, Matrix4 value )
        {
            GL.UniformMatrix4( GetUniformLocation( name ), false, ref value );
        }

        public void SetUniform( string name, bool value )
        {
            GL.Uniform1( GetUniformLocation( name ), value ? 1 : 0 );
        }

        public void SetUniform( string name, Vector4 value )
        {
            GL.Uniform4( GetUniformLocation( name ), value );
        }

        public void SetUniform( string name, Color4 value )
        {
            GL.Uniform4( GetUniformLocation( name ), value );
        }

        public void SetUniform( string name, Vector3 value )
        {
            GL.Uniform3( GetUniformLocation( name ), value );
        }

        public void SetUniform( string name, float value )
        {
            GL.Uniform1( GetUniformLocation( name ), value );
        }

        public void SetUniform( string name, int value )
        {
            GL.Uniform1( GetUniformLocation( name ), value );
        }

        public int GetUniformLocation( string name )
        {
            if ( uniforms.TryGetValue( name, out int location ) )
                return location;
            else
            {
                Debug.WriteLine( $"GetUniformLocation: {name}" );

                location = GL.GetUniformLocation( ID, name );
                uniforms.Add( name, location );
                return location;
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected void Dispose( bool disposing )
        {
            if ( disposing ) { }

            GL.DeleteProgram( ID );
        }

        public static GLShaderProgram Create( string shaderName )
        {
            if ( shaderPrograms.TryGetValue( shaderName, out GLShaderProgram shaderProgram ) )
                return shaderProgram;

            var fragmentShaderFilePath = Path.Combine( ShaderDirectoryPath, shaderName + ".frag" );
            var vertexShaderFilePath = Path.Combine( ShaderDirectoryPath, shaderName + ".vert" );

            if ( !File.Exists( fragmentShaderFilePath ) || !File.Exists( vertexShaderFilePath ) )
                return null;

            int fragmentShader = CreateShader( ShaderType.FragmentShader, File.ReadAllText( fragmentShaderFilePath ) );
            if ( fragmentShader == -1 )
                return null;

            int vertexShader = CreateShader( ShaderType.VertexShader, File.ReadAllText( vertexShaderFilePath ) );
            if ( vertexShader == -1 )
                return null;

            var shaderProgramID = GL.CreateProgram();
            GL.AttachShader( shaderProgramID, fragmentShader );
            GL.AttachShader( shaderProgramID, vertexShader );
            GL.LinkProgram( shaderProgramID );

            GL.DeleteShader( fragmentShader );
            GL.DeleteShader( vertexShader );

            shaderProgram = new GLShaderProgram( shaderName, shaderProgramID );
            shaderPrograms.Add( shaderName, shaderProgram );
            return shaderProgram;
        }

        private static int CreateShader( ShaderType shaderType, string shaderSource )
        {
            int shader = GL.CreateShader( shaderType );
            GL.ShaderSource( shader, shaderSource );
            GL.CompileShader( shader );

            GL.GetShader( shader, ShaderParameter.CompileStatus, out int compileStatus );
            if ( compileStatus == 0 )
            {
                Debug.WriteLine( $"Shader compile failed for {shaderType}, error message: {GL.GetShaderInfoLog( shader )}" );
                GL.DeleteShader( shader );
                return -1;
            }

            return shader;
        }

        ~GLShaderProgram()
        {
            Dispose( false );
        }

        private GLShaderProgram( string name, int shaderProgram )
        {
            uniforms = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );

            Name = name;
            ID = shaderProgram;
            RegisterUniforms();
        }

        static GLShaderProgram()
        {
            shaderPrograms = new Dictionary<string, GLShaderProgram>( StringComparer.OrdinalIgnoreCase );
        }
    }
}
