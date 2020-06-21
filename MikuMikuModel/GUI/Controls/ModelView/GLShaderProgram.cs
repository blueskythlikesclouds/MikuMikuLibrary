using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MikuMikuModel.Resources;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLShaderProgram : IDisposable
    {
        private static readonly Dictionary<string, GLShaderProgram> sShaderPrograms;
        private readonly Dictionary<string, int> mUniforms;

        private bool mDisposed;

        public static IReadOnlyDictionary<string, GLShaderProgram> ShaderPrograms => sShaderPrograms;

        public string Name { get; }
        public int Id { get; }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public void Use()
        {
            GL.UseProgram( Id );
        }

        public void RegisterUniform( string name )
        {
            Debug.WriteLine( $"RegisterUniform: {name}" );
            mUniforms[ name ] = GL.GetUniformLocation( Id, name );
        }

        public void RegisterUniforms()
        {
            GL.GetProgram( Id, GetProgramParameterName.ActiveUniforms, out int count );

            for ( int i = 0; i < count; i++ )
            {
                GL.GetActiveUniform( Id, i, 32, out _, out _, out _, out string name );
                mUniforms[ name ] = GL.GetUniformLocation( Id, name );
                Debug.WriteLine( $"RegisterUniforms: {name}" );
            }
        }

        public void SetUniform( string name, Matrix4 value ) => 
            GL.UniformMatrix4( GetUniformLocation( name ), false, ref value );

        public void SetUniform( string name, bool value ) => 
            GL.Uniform1( GetUniformLocation( name ), value ? 1 : 0 );

        public void SetUniform( string name, Vector4 value ) => 
            GL.Uniform4( GetUniformLocation( name ), value );

        public void SetUniform( string name, Color4 value ) => 
            GL.Uniform4( GetUniformLocation( name ), value );

        public void SetUniform( string name, Vector3 value ) => 
            GL.Uniform3( GetUniformLocation( name ), value );

        public void SetUniform( string name, float value ) => 
            GL.Uniform1( GetUniformLocation( name ), value );

        public void SetUniform( string name, int value ) => 
            GL.Uniform1( GetUniformLocation( name ), value );

        public int GetUniformLocation( string name )
        {
            if ( mUniforms.TryGetValue( name, out int location ) ) 
                return location;

            Debug.WriteLine( $"GetUniformLocation: {name}" );

            location = GL.GetUniformLocation( Id, name );
            mUniforms.Add( name, location );
            return location;
        }

        protected void Dispose( bool disposing )
        {
            if ( mDisposed )
                return;

            GL.DeleteProgram( Id );
            GL.Finish();

            mDisposed = true;
        }

        public static GLShaderProgram Create( string shaderName )
        {
            if ( sShaderPrograms.TryGetValue( shaderName, out var shaderProgram ) )
                return shaderProgram;

            string fragmentShaderFilePath = ResourceStore.GetPath( Path.Combine( "Shaders", shaderName + ".frag" ) );
            string vertexShaderFilePath = ResourceStore.GetPath( Path.Combine( "Shaders", shaderName + ".vert" ) );

            if ( !File.Exists( fragmentShaderFilePath ) || !File.Exists( vertexShaderFilePath ) )
                return null;

            int fragmentShader = CreateShader( ShaderType.FragmentShader, File.ReadAllText( fragmentShaderFilePath ) );

            if ( fragmentShader == -1 )
                return null;

            int vertexShader = CreateShader( ShaderType.VertexShader, File.ReadAllText( vertexShaderFilePath ) );

            if ( vertexShader == -1 )
                return null;

            int shaderProgramId = GL.CreateProgram();

            GL.AttachShader( shaderProgramId, fragmentShader );
            GL.AttachShader( shaderProgramId, vertexShader );
            GL.LinkProgram( shaderProgramId );

            GL.DeleteShader( fragmentShader );
            GL.DeleteShader( vertexShader );

            shaderProgram = new GLShaderProgram( shaderName, shaderProgramId );

            sShaderPrograms.Add( shaderName, shaderProgram );

            return shaderProgram;
        }

        private static int CreateShader( ShaderType shaderType, string shaderSource )
        {
            int shader = GL.CreateShader( shaderType );
            GL.ShaderSource( shader, shaderSource );
            GL.CompileShader( shader );

            GL.GetShader( shader, ShaderParameter.CompileStatus, out int compileStatus );

            if ( compileStatus != 0 ) 
                return shader;

            Debug.WriteLine( $"Shader compilation failed for {shaderType}, error message: {GL.GetShaderInfoLog( shader )}" );
            GL.DeleteShader( shader );

            return -1;

        }

        ~GLShaderProgram()
        {
            Dispose( false );
        }

        static GLShaderProgram()
        {
            sShaderPrograms = new Dictionary<string, GLShaderProgram>( StringComparer.OrdinalIgnoreCase );
        }

        private GLShaderProgram( string name, int shaderProgram )
        {
            mUniforms = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );

            Name = name;
            Id = shaderProgram;
            RegisterUniforms();
        }
    }
}