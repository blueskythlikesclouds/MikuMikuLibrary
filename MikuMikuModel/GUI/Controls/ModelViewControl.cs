using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures;
using MikuMikuModel.GUI.Controls.ModelView;
using MikuMikuModel.Resources.Styles;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Object = MikuMikuLibrary.Objects.Object;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace MikuMikuModel.GUI.Controls
{
    public partial class ModelViewControl : GLControl
    {
        private const Keys SPEED_UP_KEY = Keys.Shift;
        private const Keys SLOW_DOWN_KEY = Keys.Alt;

        private const float CAMERA_SPEED = 0.1f;
        private const float CAMERA_SPEED_FAST = 0.8f;
        private const float CAMERA_SPEED_SLOW = 0.025f;

        private const float WHEEL_CAMERA_SPEED = 0.005f;
        private const float WHEEL_CAMERA_SPEED_FAST = 0.04f;
        private const float WHEEL_CAMERA_SPEED_SLOW = 0.00125f;
        private static ModelViewControl sInstance;

        private static readonly Vector3 sCamUp = Vector3.UnitY;
        private static readonly float sFieldOfView = MathHelper.DegreesToRadians( 65 );
        private readonly GLShaderProgram mDefaultShader;
        private readonly GLShaderProgram mGridShader;

        private Color4 mBackgroundColor = Color4.LightGray;
        private Vector3 mCamDirection = new Vector3( 0, 0, -1 );

        private Vector3 mCamPosition = Vector3.Zero;
        private Vector3 mCamRotation = new Vector3( -90, 0, 0 );
        private bool mComputeProjectionMatrix = true;
        private bool mFocused = true;
        private Color4 mGridColor = new Color4( 0.1f, 0.1f, 0.1f, 1 );

        private int mGridVertexArrayId;
        private GLBuffer<Vector3> mGridVertexBuffer;

        private bool mLeft, mRight, mUp, mDown, mFront, mBack;
        private IDrawable mModel;
        private Point mPreviousMousePosition;
        private Matrix4 mProjectionMatrix;
        private bool mShouldRedraw = true;

        private Timer mTimer;

        private Matrix4 mViewMatrix;

        public static ModelViewControl Instance => sInstance ?? ( sInstance = new ModelViewControl() );

        private bool CanRender => mDefaultShader != null && mGridShader != null;

        public static void DisposeInstance()
        {
            sInstance?.Dispose();
        }

        public void SetModel( ObjectSet objectSet, TextureSet textureSet )
        {
            if ( !CanRender )
                return;

            Reset();

            mModel = new GLObjectSet( objectSet, textureSet );

            var boundingSphere = new BoundingSphere();
            foreach ( var mesh in objectSet.Objects )
                boundingSphere.Merge( mesh.BoundingSphere );

            SetCamera( boundingSphere );
        }

        public void SetModel( Object obj, TextureSet textureSet )
        {
            if ( !CanRender )
                return;

            Reset();
            mModel = new GLObject( obj, new Dictionary<uint, GLTexture>(), textureSet );
            SetCamera( obj.BoundingSphere );
        }

        public void SetModel( Mesh mesh, Object obj, TextureSet textureSet )
        {
            if ( !CanRender )
                return;

            Reset();

            var materials = new List<GLMaterial>( new GLMaterial[ obj.Materials.Count ] );
            var dictionary = new Dictionary<uint, GLTexture>();

            foreach ( var subMesh in mesh.SubMeshes )
                if ( materials[ subMesh.MaterialIndex ] == null )
                    materials[ subMesh.MaterialIndex ] = new GLMaterial( obj.Materials[ subMesh.MaterialIndex ],
                        dictionary, textureSet );

            mModel = new GLMesh( mesh, materials );
            SetCamera( mesh.BoundingSphere );
        }

        private void SetCamera( BoundingSphere boundingSphere )
        {
            float distance = ( float ) ( boundingSphere.Radius * 2f / Math.Tan( sFieldOfView ) ) + 0.75f;

            mCamPosition = new Vector3(
                boundingSphere.Center.X,
                boundingSphere.Center.Y,
                boundingSphere.Center.Z + distance );
        }

        private void Reset()
        {
            mShouldRedraw = true;
            mModel?.Dispose();
            mModel = null;

            ResetCamera();
        }

        private void ResetCamera()
        {
            mCamPosition = Vector3.Zero;
            mCamRotation = new Vector3( -90, 0, 0 );
            mCamDirection = new Vector3( 0, 0, -1 );
        }

        private void UpdateCamera()
        {
            float x = MathHelper.DegreesToRadians( mCamRotation.X );
            float y = MathHelper.DegreesToRadians( mCamRotation.Y );
            float yCos = ( float ) Math.Cos( y );

            var front = new Vector3
            {
                X = ( float ) Math.Cos( x ) * yCos,
                Y = ( float ) Math.Sin( y ),
                Z = ( float ) Math.Sin( x ) * yCos
            };

            mCamDirection = Vector3.Normalize( front );

            float cameraSpeed = ( ModifierKeys & SPEED_UP_KEY ) != 0 ? CAMERA_SPEED_FAST :
                ( ModifierKeys & SLOW_DOWN_KEY ) != 0 ? CAMERA_SPEED_SLOW : CAMERA_SPEED;

            if ( mFront && !mBack )
                mCamPosition += mCamDirection * cameraSpeed;
            else if ( mBack && !mFront )
                mCamPosition -= mCamDirection * cameraSpeed;

            if ( mLeft && !mRight )
                mCamPosition -= Vector3.Normalize( Vector3.Cross( mCamDirection, sCamUp ) ) * cameraSpeed;
            else if ( mRight && !mLeft )
                mCamPosition += Vector3.Normalize( Vector3.Cross( mCamDirection, sCamUp ) ) * cameraSpeed;

            if ( mUp && !mDown )
                mCamPosition += Vector3.UnitY * cameraSpeed / 2;
            else if ( !mUp && mDown )
                mCamPosition -= Vector3.UnitY * cameraSpeed / 2;

            mShouldRedraw |= mLeft | mRight | mUp | mDown | mFront | mBack;
        }

        private void GetViewMatrix( out Matrix4 view )
        {
            view = Matrix4.LookAt( mCamPosition, mCamPosition + mCamDirection, sCamUp );
        }

        private void GetProjectionMatrix( out Matrix4 projection )
        {
            projection = Matrix4.CreatePerspectiveFieldOfView( sFieldOfView, ( float ) Width / Height, 0.1f, 1000000f );
        }

        protected override void OnLoad( EventArgs e )
        {
            CreateGrid();

            mTimer = new Timer();
            mTimer.Interval = ( int ) ( 1000.0 / 65.0 );
            mTimer.Tick += ( sender, args ) =>
            {
                if ( !CanRender )
                    return;

                Invalidate();
            };

            if ( StyleSet.CurrentStyle != null )
                ApplyStyle( StyleSet.CurrentStyle );

            StyleSet.StyleChanged += OnStyleChanged;

            base.OnLoad( e );
        }

        private void CreateGrid()
        {
            var vertices = new List<Vector3>();

            for ( float i = -10; i <= 10; i += 0.5f )
            {
                vertices.Add( new Vector3( i, 0, -10 ) );
                vertices.Add( new Vector3( i, 0, 10 ) );
                vertices.Add( new Vector3( -10, 0, i ) );
                vertices.Add( new Vector3( 10, 0, i ) );
            }

            mGridVertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray( mGridVertexArrayId );

            mGridVertexBuffer = new GLBuffer<Vector3>( BufferTarget.ArrayBuffer, vertices.ToArray(),
                BufferUsageHint.StaticDraw );

            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, mGridVertexBuffer.Stride, 0 );
            GL.EnableVertexAttribArray( 0 );
        }

        private void OnStyleChanged( object sender, StyleChangedEventArgs eventArgs )
        {
            ApplyStyle( eventArgs.Style );
        }

        private void ApplyStyle( Style style )
        {
            mBackgroundColor = style != null
                ? new Color4( style.ViewportBackground.R, style.ViewportBackground.G, style.ViewportBackground.B,
                    style.ViewportBackground.A )
                : Color4.LightGray;

            mGridColor = style != null
                ? new Color4( style.ViewportForeground.R, style.ViewportForeground.G, style.ViewportForeground.B,
                    style.ViewportForeground.A )
                : new Color4( 0.1f, 0.1f, 0.1f, 1 );

            mShouldRedraw = true;
        }

        private void DrawModel( ref Matrix4 view, ref Matrix4 projection )
        {
            mDefaultShader.Use();
            mDefaultShader.SetUniform( "uView", view );
            mDefaultShader.SetUniform( "uProjection", projection );
            mDefaultShader.SetUniform( "uViewPosition", mCamPosition );
            mDefaultShader.SetUniform( "uLightPosition", mCamPosition );

            mModel.Draw( mDefaultShader );
        }

        private void DrawGrid( ref Matrix4 view, ref Matrix4 projection )
        {
            mGridShader.Use();
            mGridShader.SetUniform( "uView", view );
            mGridShader.SetUniform( "uProjection", projection );
            mGridShader.SetUniform( "uColor", mGridColor );

            GL.BindVertexArray( mGridVertexArrayId );
            GL.DrawArrays( PrimitiveType.Lines, 0, mGridVertexBuffer.Length );
        }

        protected override void OnPaint( PaintEventArgs pe )
        {
            if ( !CanRender )
                return;

            if ( mFocused )
                UpdateCamera();

            if ( !mShouldRedraw )
                return;

            mShouldRedraw = false;

            GL.ClearColor( mBackgroundColor );
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            if ( mComputeProjectionMatrix )
                GetProjectionMatrix( out mProjectionMatrix );

            GetViewMatrix( out mViewMatrix );

            DrawModel( ref mViewMatrix, ref mProjectionMatrix );
            DrawGrid( ref mViewMatrix, ref mProjectionMatrix );

            SwapBuffers();
        }

        protected override void OnMouseMove( MouseEventArgs e )
        {
            float deltaX = e.Location.X - mPreviousMousePosition.X;
            float deltaY = e.Location.Y - mPreviousMousePosition.Y;

            switch ( e.Button )
            {
                case MouseButtons.Left:
                {
                    float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.04f :
                        ModifierKeys.HasFlag( Keys.Control ) ? 0.00125f : 0.005f;

                    var dirRight = Vector3.Normalize( Vector3.Cross( mCamDirection, sCamUp ) );
                    var dirUp = Vector3.Normalize( Vector3.Cross( mCamDirection, dirRight ) );
                    mCamPosition -= ( dirRight * deltaX + dirUp * deltaY ) * cameraSpeed;
                    break;
                }

                case MouseButtons.Right:
                    mCamRotation.X += deltaX * 0.1f;
                    mCamRotation.Y -= deltaY * 0.1f;
                    break;
            }

            if ( e.Button != MouseButtons.None )
                mShouldRedraw = true;

            mPreviousMousePosition = e.Location;

            base.OnMouseMove( e );
        }

        protected override void OnMouseWheel( MouseEventArgs e )
        {
            float cameraSpeed = ( ModifierKeys & SPEED_UP_KEY ) != 0 ? WHEEL_CAMERA_SPEED_FAST :
                ( ModifierKeys & SLOW_DOWN_KEY ) != 0 ? WHEEL_CAMERA_SPEED_SLOW : WHEEL_CAMERA_SPEED;

            mCamPosition += mCamDirection * cameraSpeed * e.Delta;
            mShouldRedraw = true;

            base.OnMouseWheel( e );
        }

        protected override void OnKeyDown( KeyEventArgs e )
        {
            bool keyHandled = true;

            switch ( e.KeyCode )
            {
                case Keys.W:
                    mFront = true;
                    break;

                case Keys.A:
                    mLeft = true;
                    break;

                case Keys.S:
                    mBack = true;
                    break;

                case Keys.D:
                    mRight = true;
                    break;

                case Keys.Space:
                    mUp = true;
                    break;

                case Keys.ControlKey:
                    mDown = true;
                    break;

                default:
                    keyHandled = false;
                    break;
            }

            if ( keyHandled )
                e.Handled = true;

            base.OnKeyDown( e );
        }

        protected override void OnKeyUp( KeyEventArgs e )
        {
            bool keyHandled = true;

            switch ( e.KeyCode )
            {
                case Keys.W:
                    mFront = false;
                    break;

                case Keys.A:
                    mLeft = false;
                    break;

                case Keys.S:
                    mBack = false;
                    break;

                case Keys.D:
                    mRight = false;
                    break;

                case Keys.Space:
                    mUp = false;
                    break;

                case Keys.ControlKey:
                    mDown = false;
                    break;

                default:
                    keyHandled = false;
                    break;
            }

            if ( keyHandled )
                e.Handled = true;

            base.OnKeyUp( e );
        }

        protected override void OnGotFocus( EventArgs e )
        {
            mTimer.Start();
            mFocused = true;
            base.OnGotFocus( e );
        }

        protected override void OnLostFocus( EventArgs e )
        {
            mTimer.Stop();
            mFocused = mFront = mLeft = mUp = mDown = mBack = mRight = false;
            base.OnLostFocus( e );
        }

        protected override void OnResize( EventArgs e )
        {
            if ( CanRender )
            {
                GL.Viewport( ClientRectangle );
                mComputeProjectionMatrix = true;
                mShouldRedraw = true;
            }

            base.OnResize( e );
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mTimer?.Stop();
                mTimer?.Dispose();
                mComponents?.Dispose();
                mDefaultShader?.Dispose();
                mGridShader?.Dispose();
                mModel?.Dispose();
                mGridVertexBuffer?.Dispose();
                StyleSet.StyleChanged -= OnStyleChanged;
            }

            GL.DeleteVertexArray( mGridVertexArrayId );
            base.Dispose( disposing );
        }

        ~ModelViewControl()
        {
            Dispose( false );
        }

        private ModelViewControl() : base( new GraphicsMode( 32, 24, 0, 4 ), 3, 3,
            GraphicsContextFlags.ForwardCompatible )
        {
            InitializeComponent();
            MakeCurrent();

            mGridShader = GLShaderProgram.Create( "Grid" );
            mDefaultShader = GLShaderProgram.Create( "Default" );

            if ( !CanRender )
            {
                MessageBox.Show( "Shader compilation failed. GL rendering will be disabled.", Program.Name,
                    MessageBoxButtons.OK, MessageBoxIcon.Error );

                Visible = false;
                return;
            }

            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.CullFace( CullFaceMode.Back );
            GL.Enable( EnableCap.CullFace );
            GL.Enable( EnableCap.DepthTest );
            GL.Enable( EnableCap.PrimitiveRestart );
            GL.PrimitiveRestartIndex( 0xFFFF );
        }
    }
}