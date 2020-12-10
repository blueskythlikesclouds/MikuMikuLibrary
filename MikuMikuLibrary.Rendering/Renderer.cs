using System;
using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.Rendering.Buffers;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Passes;
using MikuMikuLibrary.Rendering.Primitives;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using MikuMikuLibrary.Textures.Processing;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering
{
    public sealed class Renderer : IDisposable
    {
        public const int ShadowMapSize = 1024;

        public const int SSSWidth = 1280;
        public const int SSSHeight = 720;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public State State { get; }
        public Scheduler Scheduler { get; }
        public ShaderLibrary ShaderLibrary { get; }

        public IReadOnlyList<Pass> Passes { get; } = new Pass[]
        {
            new ShadowMapPass(),
            new SSSPass(),
            new ScenePass(),
            new ToneMapPass(),
            new BackgroundPass(),
            new GridPass(),
            new EndPass()
        };

        public Quad Quad { get; }

        public UniformBuffer<CameraData> CameraUniformBuffer { get; }
        public UniformBuffer<SceneData> SceneUniformBuffer { get; }
        public UniformBuffer<MaterialData> MaterialUniformBuffer { get; }
        public UniformBuffer<ToneMapData> ToneMapUniformBuffer { get; }

        public Matrix4x4 ShadowLightViewProjection { get; set; }

        public Framebuffer ShadowMapFramebuffer { get; private set; }
        public Texture ShadowMapTexture { get; private set; }

        public Framebuffer ShadowMapFilteredFramebuffer { get; private set; }
        public Texture ShadowMapFilteredTexture { get; private set; }

        public Texture SSSDepthTexture { get; private set; }

        public Framebuffer SSSHighFramebuffer { get; private set; }
        public Texture SSSHighTexture { get; private set; }      
        
        public Framebuffer SSSMiddleFramebuffer { get; private set; }
        public Texture SSSMiddleTexture { get; private set; }      
        
        public Framebuffer SSSLowFramebuffer { get; private set; }
        public Texture SSSLowTexture { get; private set; }        
        
        public Framebuffer SSSLowFilteredFramebuffer { get; private set; }
        public Texture SSSLowFilteredTexture { get; private set; }

        public Framebuffer SceneFramebuffer { get; private set; }
        public Texture SceneColorTexture { get; private set; }
        public Texture SceneDepthTexture { get; private set; }

        public Framebuffer ToneMapFramebuffer { get; private set; }
        public Texture ToneMapColorTexture { get; private set; }

        public Shader CreateShader( string name )
        {
            var shader = ShaderLibrary.Create( name );

            shader.Use( State );
            shader.BindUniformBuffer( "CameraData", CameraUniformBuffer );
            shader.BindUniformBuffer( "SceneData", SceneUniformBuffer );
            shader.BindUniformBuffer( "MaterialData", MaterialUniformBuffer );
            shader.BindUniformBuffer( "ToneMapData", ToneMapUniformBuffer );

            shader.SetUniform( "uUseIBL", true );

            foreach ( var samplerInfo in Sampler.All )
                samplerInfo.Bind( shader );

            return shader;
        }

        public void Initialize( int width, int height )
        {
            if ( ShadowMapFramebuffer == null )
            {
                {
                    ShadowMapFramebuffer = new Framebuffer( State, ShadowMapSize, ShadowMapSize );
                    ShadowMapTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.DepthComponent,
                        ShadowMapSize, ShadowMapSize, PixelFormat.DepthComponent, PixelType.Float );

                    GL.DrawBuffer( DrawBufferMode.None );
                    GL.ReadBuffer( ReadBufferMode.None );

                    ShadowMapFramebuffer.Attach( FramebufferAttachment.DepthAttachment, ShadowMapTexture );
                }

                {
                    ShadowMapFilteredFramebuffer = new Framebuffer( State, ShadowMapSize, ShadowMapSize );
                    ShadowMapFilteredTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.DepthComponent,
                        ShadowMapSize, ShadowMapSize, PixelFormat.DepthComponent, PixelType.Float );

                    GL.DrawBuffer( DrawBufferMode.None );
                    GL.ReadBuffer( ReadBufferMode.None );

                    ShadowMapFilteredFramebuffer.Attach( FramebufferAttachment.DepthAttachment, ShadowMapFilteredTexture );
                }

                {
                    SSSDepthTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.DepthComponent,
                        SSSWidth, SSSHeight, PixelFormat.DepthComponent, PixelType.Float );

                    {
                        SSSHighFramebuffer = new Framebuffer( State, SSSWidth, SSSHeight );
                        SSSHighTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f,
                            SSSWidth, SSSHeight, PixelFormat.Rgba, PixelType.HalfFloat );

                        SSSHighFramebuffer.Attach( FramebufferAttachment.ColorAttachment0, SSSHighTexture );
                        SSSHighFramebuffer.Attach( FramebufferAttachment.DepthAttachment, SSSDepthTexture );
                    }           
                    
                    {
                        SSSMiddleFramebuffer = new Framebuffer( State, SSSWidth / 2, SSSHeight / 2 );
                        SSSMiddleTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f,
                            SSSWidth / 2, SSSHeight / 2, PixelFormat.Rgba, PixelType.HalfFloat );

                        SSSMiddleFramebuffer.Attach( FramebufferAttachment.ColorAttachment0, SSSMiddleTexture );
                    }     
                    
                    {
                        SSSLowFramebuffer = new Framebuffer( State, SSSWidth / 4, SSSHeight / 4 );
                        SSSLowTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f,
                            SSSWidth / 4, SSSHeight / 4, PixelFormat.Rgba, PixelType.HalfFloat );

                        SSSLowFramebuffer.Attach( FramebufferAttachment.ColorAttachment0, SSSLowTexture );
                    }          
                    
                    {
                        SSSLowFilteredFramebuffer = new Framebuffer( State, SSSWidth / 4, SSSHeight / 4 );
                        SSSLowFilteredTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f,
                            SSSWidth / 4, SSSHeight / 4, PixelFormat.Rgba, PixelType.HalfFloat );

                        SSSLowFilteredFramebuffer.Attach( FramebufferAttachment.ColorAttachment0, SSSLowFilteredTexture );
                    }
                }
            }

            Width = Math.Max( 1, width );
            Height = Math.Max( 1, height );

            if ( SceneFramebuffer != null )
            {
                SceneFramebuffer.Dispose();
                SceneColorTexture.Dispose();
                SceneDepthTexture.Dispose();
                ToneMapFramebuffer.Dispose();
                ToneMapColorTexture.Dispose();
            }

            {
                SceneFramebuffer = new Framebuffer( State, Width, Height );

                SceneColorTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f,
                    Width, Height, PixelFormat.Rgba, PixelType.HalfFloat );

                SceneDepthTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.DepthComponent,
                    Width, Height, PixelFormat.DepthComponent, PixelType.Float );

                SceneFramebuffer.Attach( FramebufferAttachment.ColorAttachment0, SceneColorTexture );
                SceneFramebuffer.Attach( FramebufferAttachment.DepthAttachment, SceneDepthTexture );
            }

            {
                ToneMapFramebuffer = new Framebuffer( State, Width, Height );

                ToneMapColorTexture = new Texture( State, TextureTarget.Texture2D, PixelInternalFormat.Rgba8,
                    Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte );

                ToneMapFramebuffer.Attach( FramebufferAttachment.ColorAttachment0, ToneMapColorTexture );
                ToneMapFramebuffer.Attach( FramebufferAttachment.DepthAttachment, SceneDepthTexture );
            }

            foreach ( var pass in Passes )
                pass.Initialize( this );
        }

        public void Render( Camera camera, Scene scene, Effect effect )
        {
            effect.Bind( this );

            foreach ( var renderPass in Passes )
                renderPass.Render( this, camera, scene, effect );
        }

        public void RenderQuad()
        {
            Quad.VertexArray.Bind( State );
            Quad.ElementArray.Bind( State );
            Quad.ElementArray.Render();
        }

        public void Dispose()
        {
            ShaderLibrary.Dispose();

            foreach ( var pass in Passes )
                pass.Dispose();

            Quad.Dispose();

            CameraUniformBuffer.Dispose();
            SceneUniformBuffer.Dispose();
            MaterialUniformBuffer.Dispose();
            ToneMapUniformBuffer.Dispose();

            ShadowMapFramebuffer.Dispose();
            ShadowMapTexture.Dispose();

            ShadowMapFilteredFramebuffer.Dispose();
            ShadowMapFilteredTexture.Dispose();

            SSSDepthTexture.Dispose();
            SSSHighFramebuffer.Dispose();
            SSSHighTexture.Dispose();          
            SSSMiddleFramebuffer.Dispose();
            SSSMiddleTexture.Dispose();         
            SSSLowFramebuffer.Dispose();
            SSSLowTexture.Dispose();
            SSSLowFilteredFramebuffer.Dispose();
            SSSLowFilteredTexture.Dispose();

            SceneFramebuffer.Dispose();
            SceneColorTexture.Dispose();
            SceneDepthTexture.Dispose();

            ToneMapFramebuffer.Dispose();
            ToneMapColorTexture.Dispose();
        }

        public Renderer( ShaderLibrary shaderLibrary, int width, int height )
        {
            State = new State();
            Scheduler = new Scheduler();
            ShaderLibrary = shaderLibrary;

            CameraUniformBuffer = new UniformBuffer<CameraData>( State, 0 );
            SceneUniformBuffer = new UniformBuffer<SceneData>( State, 1 );
            MaterialUniformBuffer = new UniformBuffer<MaterialData>( State, 2 );
            ToneMapUniformBuffer = new UniformBuffer<ToneMapData>( State, 3 );

            Quad = new Quad( State );

            Initialize( width, height );
        }
    }
}