using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Rendering.Buffers;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Shaders
{
    public sealed class Shader : IDisposable
    {
        private readonly Dictionary<string, int> mUniforms;

        private bool mDisposed;

        public int Id { get; }

        public void Use( State state )
        {
            state.UseProgram( Id );
        }

        public void RegisterUniform( string name )
        {
            mUniforms[ name ] = GL.GetUniformLocation( Id, name );
        }

        public void RegisterUniforms()
        {
            GL.GetProgram( Id, GetProgramParameterName.ActiveUniforms, out int count );

            for ( int i = 0; i < count; i++ )
            {
                GL.GetActiveUniform( Id, i, 32, out _, out _, out _, out string name );
                mUniforms[ name ] = GL.GetUniformLocation( Id, name );
            }
        }

        public void SetUniform( string name, bool value ) => GL.Uniform1( GetUniformLocation( name ), value ? 1 : 0 );
        public void SetUniform( string name, int value ) => GL.Uniform1( GetUniformLocation( name ), value );
        public void SetUniform( string name, uint value ) => GL.Uniform1( GetUniformLocation( name ), value );
        public void SetUniform( string name, float value ) => GL.Uniform1( GetUniformLocation( name ), value );

        public unsafe void SetUniform( string name, Vector2 value ) => GL.Uniform2( GetUniformLocation( name ), 1, ( float* ) &value );
        public unsafe void SetUniform( string name, Vector3 value ) => GL.Uniform3( GetUniformLocation( name ), 1, ( float* ) &value );
        public unsafe void SetUniform( string name, Vector4 value ) => GL.Uniform4( GetUniformLocation( name ), 1, ( float* ) &value );
        public unsafe void SetUniform( string name, Color value ) => GL.Uniform4( GetUniformLocation( name ), 1, ( float* ) &value );

        public unsafe void SetUniform( string name, Matrix4x4 value ) => GL.UniformMatrix4( GetUniformLocation( name ), 1, false, ( float* ) &value );

        public unsafe void SetUniform( string name, float[] value )
        {
            fixed ( float* ptr = value )
                GL.Uniform1( GetUniformLocation( name ), value.Length, ptr );
        }        
        
        public unsafe void SetUniform( string name, Vector2[] value )
        {
            fixed ( Vector2* ptr = value )
                GL.Uniform2( GetUniformLocation( name ), value.Length, ( float* ) ptr );
        }       
        
        public unsafe void SetUniform( string name, Vector4[] value )
        {
            fixed ( Vector4* ptr = value )
                GL.Uniform4( GetUniformLocation( name ), value.Length, ( float* ) ptr );
        }

        public int GetUniformLocation( string name )
        {
            if ( mUniforms.TryGetValue( name, out int location ) ) 
                return location;

            location = GL.GetUniformLocation( Id, name );
            mUniforms.Add( name, location );
            return location;
        }

        public void BindUniformBuffer<T>( string name, UniformBuffer<T> buffer ) where T : unmanaged
        {
            GL.UniformBlockBinding( Id, GL.GetUniformBlockIndex( Id, name ), buffer.BindingIndex );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            GL.DeleteProgram( Id );
            GC.SuppressFinalize( this );
        }

        public Shader( int id )
        {
            mUniforms = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );

            Id = id;
        }

        ~Shader()
        {
            Dispose();
        }

        public static Shader Create( string vertexShader, string fragmentShader )
        {
            int vertexShaderId = CreateShader( ShaderType.VertexShader, vertexShader );

            if ( vertexShaderId == -1 )
                throw new Exception( "Failed to compile vertex shader" );

            int fragmentShaderId = CreateShader( ShaderType.FragmentShader, fragmentShader );

            if ( fragmentShaderId == -1 )
                throw new Exception( "Failed to compile fragment shader" );

            int shaderProgramId = GL.CreateProgram();

            GL.AttachShader( shaderProgramId, vertexShaderId );
            GL.AttachShader( shaderProgramId, fragmentShaderId );
            GL.LinkProgram( shaderProgramId );

            GL.DeleteShader( vertexShaderId );
            GL.DeleteShader( fragmentShaderId );

            var shaderProgram = new Shader( shaderProgramId );

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
    }
}