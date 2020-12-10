using System;
using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.Lights;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using OpenTK.Graphics.OpenGL;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace MikuMikuLibrary.Rendering.Passes
{
    public class ToneMapPass : Pass
    {
        private readonly Vector2[] mGaussianDirection = new Vector2[ 6 ];
        private readonly Vector4[] mGaussianKernel = new Vector4[ 7 ];

        private Shader mReduceTexShader;
        private Shader mReduceTexExtractShader;
        private Shader mGaussUsualShader;
        private Shader mGaussConeShader;
        private Shader mReduceTexCompositeShader;

        private Shader mExposureMinifyShader;
        private Shader mExposureMeasureShader;
        private Shader mExposureAverageShader;

        private Shader mToneMapShader;

        private List<FramebufferTexture> mColorTextures0;
        private FramebufferTexture mColorFramebuffer1;
        private FramebufferTexture mColorFramebuffer2Level0;
        private FramebufferTexture mColorFramebuffer2Level1;
        private FramebufferTexture mColorFramebuffer2Level2;

        private FramebufferTexture mExposureFramebuffer0;

        private FramebufferTexture mColorFramebuffer2Level0HorizontalBlur;
        private FramebufferTexture mColorFramebuffer2Level1HorizontalBlur;
        private FramebufferTexture mColorFramebuffer2Level2HorizontalBlur;
        
        private FramebufferTexture mColorFramebuffer2Level0VerticalBlur;
        private FramebufferTexture mColorFramebuffer2Level1VerticalBlur;
        private FramebufferTexture mColorFramebuffer2Level2VerticalBlur;

        private FramebufferTexture mColorFramebuffer3;

        private FramebufferTexture mColorFramebuffer4;

        private FramebufferTexture mExposureFramebuffer1;

        private Texture mExposureFramebuffer2;

        private FramebufferTexture mExposureFramebuffer3;

        private Texture mToneMapLookupTexture;

        private int mDstPixel;

        private void InitializeShaders( Renderer renderer )
        {
            mReduceTexShader = renderer.CreateShader( "PostProcess/ReduceTex/ReduceTex" );
            mReduceTexExtractShader = renderer.CreateShader( "PostProcess/ReduceTex/ReduceTexExtract" );
            mGaussUsualShader = renderer.CreateShader( "PostProcess/Gauss/GaussUsual" );
            mGaussConeShader = renderer.CreateShader( "PostProcess/Gauss/GaussCone" );
            mReduceTexCompositeShader = renderer.CreateShader( "PostProcess/ReduceTex/ReduceTexComposite" );

            mExposureMinifyShader = renderer.CreateShader( "PostProcess/Exposure/ExposureMinify" );
            mExposureMeasureShader = renderer.CreateShader( "PostProcess/Exposure/ExposureMeasure" );
            mExposureAverageShader = renderer.CreateShader( "PostProcess/Exposure/ExposureAverage" );

            mToneMapShader = renderer.CreateShader( "PostProcess/ToneMap" );
            mToneMapShader.SetUniform( "uColorTexture", 0 );
            mToneMapShader.SetUniform( "uGlareTexture", 1 );
            mToneMapShader.SetUniform( "uLookupTexture", 2 );
            mToneMapShader.SetUniform( "uExposureTexture", 3 );
        }

        private FramebufferTexture CreateLumaFB( Renderer renderer, int w, int h )
        {
            return new FramebufferTexture( renderer.State, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D,
                PixelInternalFormat.R16f, w, h, PixelFormat.Red, PixelType.HalfFloat );
        }

        private FramebufferTexture CreateColorFB( Renderer renderer, int w, int h )
        {
            return new FramebufferTexture( renderer.State, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D,
                PixelInternalFormat.Rgba16f, w, h, PixelFormat.Rgba, PixelType.HalfFloat );
        }

        private void InitializeStaticFramebuffers( Renderer renderer )
        {
            mColorFramebuffer1 = CreateColorFB( renderer, 256, 144 );

            mColorFramebuffer2Level0 = CreateColorFB( renderer, 128, 72 );
            mColorFramebuffer2Level1 = CreateColorFB( renderer, 64, 36 );
            mColorFramebuffer2Level2 = CreateColorFB( renderer, 32, 18 );

            mColorFramebuffer2Level0HorizontalBlur = CreateColorFB( renderer, 128, 72 );
            mColorFramebuffer2Level1HorizontalBlur = CreateColorFB( renderer, 64, 36 );
            mColorFramebuffer2Level2HorizontalBlur = CreateColorFB( renderer, 32, 18 );

            mColorFramebuffer2Level0VerticalBlur = CreateColorFB( renderer, 128, 72 );
            mColorFramebuffer2Level1VerticalBlur = CreateColorFB( renderer, 64, 36 );
            mColorFramebuffer2Level2VerticalBlur = CreateColorFB( renderer, 32, 18 );

            mColorFramebuffer3 = CreateColorFB( renderer, 256, 144 );

            mColorFramebuffer4 = CreateColorFB( renderer, 256, 144 );

            mExposureFramebuffer0 = CreateLumaFB( renderer, 8, 8 );
            mExposureFramebuffer1 = CreateLumaFB( renderer, 1, 1 );
            mExposureFramebuffer2 = new Texture( renderer.State, TextureTarget.Texture2D,
                PixelInternalFormat.R16f, 32, 1, PixelFormat.Red, PixelType.HalfFloat );

            mExposureFramebuffer3 = CreateLumaFB( renderer, 1, 1 );
        }

        private void InitializeDynamicFramebuffers( Renderer renderer )
        {
            foreach ( var fb in mColorTextures0 )
                fb.Dispose();

            mColorTextures0.Clear();

            int downscaledWidth = renderer.Width;
            int downscaledHeight = renderer.Height;

            while ( downscaledWidth > 512 && downscaledHeight > 288 )
            {
                downscaledWidth /= 2;
                downscaledHeight /= 2;

                mColorTextures0.Add( CreateColorFB( renderer, downscaledWidth, downscaledHeight ) );
            }

            if ( mColorTextures0.Count == 0 )
                mColorTextures0.Add( CreateColorFB( renderer, 512, 288 ) );
        }

        private unsafe void CalculateGaussianKernel( Vector3 radius, Vector3 intensity )
        {
            fixed ( Vector4* gaussianKernel = mGaussianKernel )
            {
                CalculateGaussianKernelChannels( ( float* ) gaussianKernel, radius.X, intensity.X, 0 );
                CalculateGaussianKernelChannels( ( float* ) gaussianKernel, radius.Y, intensity.Y, 1 );
                CalculateGaussianKernelChannels( ( float* ) gaussianKernel, radius.Z, intensity.Z, 2 );
                CalculateGaussianKernelChannels( ( float* ) gaussianKernel, 1.0f, 1.0f, 3 );
            }
        }

        private unsafe void CalculateGaussianKernelChannels( float* gaussianKernel, float radius, float intensity, int channelIndex )
        {
            float sum = 0.5f;
            float scale = radius * 0.8f;
            scale = -1.0f / ( 2.0f * radius * radius );

            SetGaussianKernelChannel( 0, 1.0f );
            for ( int i = 1; i < 7; i++ )
            {
                float channel = ( float ) Math.Exp( i * i * scale );
                SetGaussianKernelChannel( i, channel );

                sum += channel;
            }

            sum = 0.5f / sum;

            for ( int i = 0; i < 7; i++ )
                SetGaussianKernelChannel( i, GetGaussianKernelChannel( i ) * sum * intensity );

            float GetGaussianKernelChannel( int index ) => gaussianKernel[ index * 4 + channelIndex ];
            void SetGaussianKernelChannel( int index, float value ) => gaussianKernel[ index * 4 + channelIndex ] = value;
        }

        public override void Initialize( Renderer renderer )
        {
            if ( mReduceTexShader == null )
            {
                InitializeShaders( renderer );
                InitializeStaticFramebuffers( renderer );
            }

            InitializeDynamicFramebuffers( renderer );
        }

        public override void Render( Renderer renderer, Camera camera, Scene scene, Effect effect )
        {
            renderer.State.Blend( false );
            renderer.State.CullFace( false );
            renderer.State.DepthTest( false );

            // Step 1
            {
                mReduceTexShader.Use( renderer.State );

                for ( int i = 0; i < mColorTextures0.Count; i++ )
                {
                    var sourceTexture = i == 0 ? renderer.SceneColorTexture : mColorTextures0[ i - 1 ].Texture;

                    BindTexture( mReduceTexShader, sourceTexture );
                    mColorTextures0[ i ].Framebuffer.Bind( renderer.State );
                    renderer.RenderQuad();
                }
            }

            // Step 2
            {
                mReduceTexExtractShader.Use( renderer.State );

                BindTexture( mReduceTexExtractShader, mColorTextures0[ mColorTextures0.Count - 1 ].Texture );
                mColorFramebuffer1.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 3
            {
                mReduceTexShader.Use( renderer.State );

                BindTexture( mReduceTexShader, mColorFramebuffer1.Texture );
                mColorFramebuffer2Level0.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();

                BindTexture( mReduceTexShader, mColorFramebuffer2Level0.Texture );
                mColorFramebuffer2Level1.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();

                BindTexture( mReduceTexShader, mColorFramebuffer2Level1.Texture );
                mColorFramebuffer2Level2.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 4
            {
                mExposureMinifyShader.Use( renderer.State );

                BindTexture( mExposureMinifyShader, mColorFramebuffer2Level2.Texture );
                mExposureFramebuffer0.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 5
            {
                CalculateGaussianKernel( effect.GlowParameter.GlareRadius, effect.GlowParameter.GlareIntensity );

                mGaussUsualShader.Use( renderer.State );
                mGaussUsualShader.SetUniform( "uGaussianKernel", mGaussianKernel );

                RenderGauss( mColorFramebuffer2Level0, mColorFramebuffer2Level0HorizontalBlur, mColorFramebuffer2Level0VerticalBlur );
                RenderGauss( mColorFramebuffer2Level1, mColorFramebuffer2Level1HorizontalBlur, mColorFramebuffer2Level1VerticalBlur );
                RenderGauss( mColorFramebuffer2Level2, mColorFramebuffer2Level2HorizontalBlur, mColorFramebuffer2Level2VerticalBlur );

                void RenderGauss( FramebufferTexture pair, FramebufferTexture horizontal, FramebufferTexture vertical )
                {
                    for ( int i = 0; i < 6; i++ )
                        mGaussianDirection[ i ] = new Vector2( ( i + 1 ) / ( float ) pair.Framebuffer.Width, 0 );

                    mGaussUsualShader.SetUniform( "uDirection", mGaussianDirection );
                    horizontal.Framebuffer.Bind( renderer.State );
                    renderer.RenderQuad();

                    for ( int i = 0; i < 6; i++ )
                        mGaussianDirection[ i ] = new Vector2( 0, ( i + 1 ) / ( float ) pair.Framebuffer.Height );

                    mGaussUsualShader.SetUniform( "uDirection", mGaussianDirection );
                    vertical.Framebuffer.Bind( renderer.State );
                    renderer.RenderQuad();
                }
            }

            // Step 6
            {
                mGaussConeShader.Use( renderer.State );

                BindTexture( mGaussConeShader, mColorFramebuffer1.Texture );
                mColorFramebuffer3.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 7
            {
                mReduceTexCompositeShader.Use( renderer.State );

                BindTexture( mReduceTexCompositeShader, mColorFramebuffer3.Texture );

                mColorFramebuffer2Level0VerticalBlur.Texture.Bind( renderer.State, mReduceTexCompositeShader, "uTexture1", 1 );
                mColorFramebuffer2Level1VerticalBlur.Texture.Bind( renderer.State, mReduceTexCompositeShader, "uTexture2", 2 );
                mColorFramebuffer2Level2VerticalBlur.Texture.Bind( renderer.State, mReduceTexCompositeShader, "uTexture3", 3 );

                mColorFramebuffer4.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 8
            {
                mExposureMeasureShader.Use( renderer.State );
                mExposureFramebuffer0.Texture.Bind( renderer.State, 0 );
                mExposureFramebuffer1.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 8.1
            {
                mExposureFramebuffer2.Bind( renderer.State );
                GL.CopyTexSubImage2D( TextureTarget.Texture2D, 0, ( mDstPixel++ ) % 32, 0, 0, 0, 1, 1 );
            }

            // Step 9
            {
                mExposureAverageShader.Use( renderer.State );
                mExposureFramebuffer2.Bind( renderer.State, 0 );
                mExposureFramebuffer3.Framebuffer.Bind( renderer.State );
                renderer.RenderQuad();
            }

            // Step 10
            {
                GenerateToneMapLookupTexture( renderer.State, effect.GlowParameter.Gamma, 1.0f,
                    ( int ) effect.GlowParameter.SaturatePower, effect.GlowParameter.SaturateCoefficient );

                var toneScale = Vector3.One / ( effect.GlowParameter.ToneTransEnd - effect.GlowParameter.ToneTransStart );

                renderer.ToneMapUniformBuffer.SetData( renderer.State, new ToneMapData
                {
                    Exposure = new Vector4( effect.GlowParameter.Exposure, 0.0625f, effect.GlowParameter.Exposure * 0.5f, 1.0f ),
                    FadeColorAndFunc = new Vector4( effect.GlowParameter.FadeColor, ( int ) effect.GlowParameter.FadeBlendFunc ),
                    ToneScale = new Vector4( toneScale, 0 ),
                    ToneOffset = new Vector4( -toneScale * effect.GlowParameter.ToneTransStart, 0 )
                } );

                mToneMapShader.Use( renderer.State );
                renderer.SceneColorTexture.Bind( renderer.State, 0 );
                mColorFramebuffer4.Texture.Bind( renderer.State, 1 );
                mToneMapLookupTexture.Bind( renderer.State, 2 );
                mExposureFramebuffer3.Texture.Bind( renderer.State, 3 );

                renderer.State.BindFramebuffer( renderer.ToneMapFramebuffer );
                renderer.RenderQuad();
            }

            void BindTexture( Shader shaderProgram, Texture texture )
            {
                texture.Bind( renderer.State, 0 );
                shaderProgram.SetUniform( "uResolution", new Vector4( texture.Width, texture.Height, 1.0f / texture.Width, 1.0f / texture.Height ) );
            }
        }

        private void GenerateToneMapLookupTexture( State state, float gamma, float gammaRate, int saturatePower, float saturateCoef )
        {
            const int samples = 32;
            const float scale = 1.0f / samples;
            const int size = 16 * samples;

            double v9 = gamma * gammaRate * 1.5f;

            var array = new Vector2[ size ];
            for ( int i = 1; i < size; i++ )
            {
                double v11 = Math.Exp( -i * scale );
                double v10 = Math.Pow( 1.0f - v11, v9 );
                v11 = v10 * 2.0f - 1.0f;

                for ( int j = 0; j < saturatePower; j++ )
                {
                    v11 *= v11;
                    v11 *= v11;
                    v11 *= v11;
                    v11 *= v11;
                }

                array[ i ] = new Vector2( ( float ) v10, ( float ) ( v10 * saturateCoef * ( samples / ( double ) i ) * ( 1.0 - v11 ) ) );
            }

            if ( mToneMapLookupTexture == null )
                mToneMapLookupTexture = new Texture( state, TextureTarget.Texture2D, PixelInternalFormat.Rg16f, size, 1, PixelFormat.Rg, PixelType.HalfFloat );

            GL.TexSubImage2D( TextureTarget.Texture2D, 0, 0, 0, size, 1, PixelFormat.Rg, PixelType.Float, array );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                foreach ( var pair in mColorTextures0 )
                    pair.Dispose();

                mColorFramebuffer1.Dispose();
                mColorFramebuffer2Level0.Dispose();
                mColorFramebuffer2Level1.Dispose();
                mColorFramebuffer2Level2.Dispose();
                mExposureFramebuffer0.Dispose();
                mColorFramebuffer2Level0HorizontalBlur.Dispose();
                mColorFramebuffer2Level1HorizontalBlur.Dispose();
                mColorFramebuffer2Level2HorizontalBlur.Dispose();              
                mColorFramebuffer2Level0VerticalBlur.Dispose();
                mColorFramebuffer2Level1VerticalBlur.Dispose();
                mColorFramebuffer2Level2VerticalBlur.Dispose();
                mColorFramebuffer3.Dispose();
                mColorFramebuffer4.Dispose();
                mExposureFramebuffer1.Dispose();
                mExposureFramebuffer2.Dispose();
                mExposureFramebuffer3.Dispose();
                mToneMapLookupTexture.Dispose();
            }

            base.Dispose( disposing );
        }

        public ToneMapPass()
        {
            mColorTextures0 = new List<FramebufferTexture>();
        }
    }
}