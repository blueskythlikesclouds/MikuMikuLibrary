using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Models;
using MikuMikuLibrary.Textures;
using MikuMikuModel.GUI.Controls.ModelView;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Controls
{
    public partial class ModelViewControl : GLControl
    {
        private static ModelViewControl sInstance;

        public static ModelViewControl Instance => sInstance ?? ( sInstance = new ModelViewControl() );

        public static void DisposeInstance()
        {
            sInstance?.Dispose();
        }

        private IGLDraw mModel;
        private GLShaderProgram mDefaultShader;
        private GLShaderProgram mGridShader;

        private Vector3 mCamPos = Vector3.Zero;
        private Vector3 mCamRot = new Vector3( -90, 0, 0 );
        private Vector3 mCamDir = new Vector3( 0, 0, -1 );
        private Point mPrevMousePos;
        private static readonly Vector3 sCamUp = new Vector3( 0, 1, 0 );
        private static readonly float sFieldOfView = MathHelper.DegreesToRadians( 65 );

        private bool mLeft, mRight, mUp, mDown;

        private int mGridVertexArrayID;
        private GLBuffer<Vector3> mGridVertexBuffer;

        public void SetModel( Model model, TextureSet textureSet )
        {
            if ( mDefaultShader == null || mGridShader == null )
                return;

            mModel?.Dispose();
            ResetCamera();

            mModel = new GLModel( model, textureSet );

            var bSphere = new BoundingSphere();
            foreach ( var mesh in model.Meshes )
                bSphere.Merge( mesh.BoundingSphere );

            var distance = ( float )( ( bSphere.Radius * 2f ) / Math.Tan( sFieldOfView ) ) + 0.75f;

            mCamPos = new Vector3(
                bSphere.Center.X,
                bSphere.Center.Y,
                bSphere.Center.Z + distance );
        }

        public void SetModel( Mesh mesh, TextureSet textureSet )
        {
            if ( mDefaultShader == null || mGridShader == null )
                return;

            mModel?.Dispose();
            ResetCamera();

            mModel = new GLMesh( mesh, new Dictionary<int, GLTexture>(), textureSet );

            var distance = ( float )( ( mesh.BoundingSphere.Radius * 2f ) / Math.Tan( sFieldOfView ) ) + 0.75f;

            mCamPos = new Vector3(
                mesh.BoundingSphere.Center.X,
                mesh.BoundingSphere.Center.Y,
                mesh.BoundingSphere.Center.Z + distance );
        }

        public void SetModel( SubMesh subMesh, Mesh mesh, TextureSet textureSet )
        {
            if ( mDefaultShader == null || mGridShader == null )
                return;

            mModel?.Dispose();
            ResetCamera();

            var materials = new List<GLMaterial>( new GLMaterial[ mesh.Materials.Count ] );
            var dictionary = new Dictionary<int, GLTexture>();

            foreach ( var indexTable in subMesh.IndexTables )
            {
                if ( materials[ indexTable.MaterialIndex ] == null )
                    materials[ indexTable.MaterialIndex ] = new GLMaterial( mesh.Materials[ indexTable.MaterialIndex ], dictionary, textureSet );
            }

            mModel = new GLSubMesh( subMesh, materials );

            var distance = ( float )( ( subMesh.BoundingSphere.Radius * 2f ) / Math.Tan( sFieldOfView ) ) + 0.75f;

            mCamPos = new Vector3(
                subMesh.BoundingSphere.Center.X,
                subMesh.BoundingSphere.Center.Y,
                subMesh.BoundingSphere.Center.Z + distance );
        }

        public void ResetCamera()
        {
            mCamPos = Vector3.Zero;
            mCamRot = new Vector3( -90, 0, 0 );
            mCamDir = new Vector3( 0, 0, -1 );
        }

        private void UpdateCamera()
        {
            float x = MathHelper.DegreesToRadians( mCamRot.X );
            float y = MathHelper.DegreesToRadians( mCamRot.Y );
            float yCos = ( float )Math.Cos( y );

            var front = new Vector3()
            {
                X = ( float )Math.Cos( x ) * yCos,
                Y = ( float )Math.Sin( y ),
                Z = ( float )Math.Sin( x ) * yCos
            };

            mCamDir = Vector3.Normalize( front );

            float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.8f : ModifierKeys.HasFlag( Keys.Control ) ? 0.025f : 0.1f;

            if ( mUp && !mDown )
                mCamPos += mCamDir * cameraSpeed;
            else if ( mDown && !mUp )
                mCamPos -= mCamDir * cameraSpeed;

            if ( mLeft && !mRight )
                mCamPos -= Vector3.Normalize( Vector3.Cross( mCamDir, sCamUp ) ) * cameraSpeed;
            else if ( mRight && !mLeft )
                mCamPos += Vector3.Normalize( Vector3.Cross( mCamDir, sCamUp ) ) * cameraSpeed;
        }

        private Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt( mCamPos, mCamPos + mCamDir, sCamUp );
        }

        private Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView( sFieldOfView, ( float )Width / Height, 0.1f, 1000000f );
        }

        protected override void OnLoad( EventArgs e )
        {
            Application.Idle += OnApplicationIdle;

            CreateGrid();

            base.OnLoad( e );
        }

        private void OnApplicationIdle( object sender, EventArgs e )
        {
            Invalidate();
        }

        private Vector3 RotatePoint( Vector3 point, Vector3 origin, Vector3 angles )
        {
            return Quaternion.FromEulerAngles( angles ) * ( point - origin ) + origin;
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

            mGridVertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray( mGridVertexArrayID );

            mGridVertexBuffer = new GLBuffer<Vector3>( BufferTarget.ArrayBuffer, vertices.ToArray(), 12, BufferUsageHint.StaticDraw );

            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, mGridVertexBuffer.Stride, 0 );
            GL.EnableVertexAttribArray( 0 );
        }

        private void DrawGrid( Matrix4 view, Matrix4 projection )
        {
            mGridShader.Use();
            mGridShader.SetUniform( "view", view );
            mGridShader.SetUniform( "projection", projection );
            mGridShader.SetUniform( "color", new Vector4( 0.15f, 0.15f, 0.15f, 1f ) );

            GL.BindVertexArray( mGridVertexArrayID );
            GL.DrawArrays( PrimitiveType.Lines, 0, mGridVertexBuffer.Length );
        }

        protected override void OnPaint( PaintEventArgs pe )
        {
            if ( mModel != null && mDefaultShader != null && mGridShader != null )
            {
                GL.ClearColor( Color4.LightGray );
                GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

                UpdateCamera();
                var view = GetViewMatrix();
                var projection = GetProjectionMatrix();

                mDefaultShader.Use();
                mDefaultShader.SetUniform( "view", view );
                mDefaultShader.SetUniform( "projection", projection );
                mDefaultShader.SetUniform( "viewPosition", mCamPos );
                mDefaultShader.SetUniform( "lightPosition", mCamPos );
                mModel.Draw( mDefaultShader );

                // Draw a grid
                DrawGrid( view, projection );

                SwapBuffers();
            }
        }

        protected override void OnMouseMove( MouseEventArgs e )
        {
            float deltaX = e.Location.X - mPrevMousePos.X;
            float deltaY = e.Location.Y - mPrevMousePos.Y;

            if ( e.Button == MouseButtons.Left )
            {
                float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.04f : ModifierKeys.HasFlag( Keys.Control ) ? 0.00125f : 0.005f;
                var dirRight = Vector3.Normalize( Vector3.Cross( mCamDir, sCamUp ) );
                var dirUp = Vector3.Normalize( Vector3.Cross( mCamDir, dirRight ) );
                mCamPos -= ( ( dirRight * deltaX ) + ( dirUp * deltaY ) ) * cameraSpeed;
            }

            else if ( e.Button == MouseButtons.Right )
            {
                mCamRot.X += deltaX * 0.1f;
                mCamRot.Y -= deltaY * 0.1f;
            }

            mPrevMousePos = e.Location;

            base.OnMouseMove( e );
        }

        protected override void OnMouseWheel( MouseEventArgs e )
        {
            float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.04f : ModifierKeys.HasFlag( Keys.Control ) ? 0.00125f : 0.005f;
            mCamPos += mCamDir * cameraSpeed * e.Delta;

            base.OnMouseWheel( e );
        }

        protected override void OnKeyDown( KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.W:
                    mUp = true;
                    e.Handled = true;
                    break;

                case Keys.A:
                    mLeft = true;
                    e.Handled = true;
                    break;

                case Keys.S:
                    mDown = true;
                    e.Handled = true;
                    break;

                case Keys.D:
                    mRight = true;
                    e.Handled = true;
                    break;
            }

            base.OnKeyDown( e );
        }

        protected override void OnKeyUp( KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.W:
                    mUp = false;
                    e.Handled = true;
                    break;

                case Keys.A:
                    mLeft = false;
                    e.Handled = true;
                    break;

                case Keys.S:
                    mDown = false;
                    e.Handled = true;
                    break;

                case Keys.D:
                    mRight = false;
                    e.Handled = true;
                    break;
            }

            base.OnKeyUp( e );
        }

        protected override void OnLostFocus( EventArgs e )
        {
            mUp = mLeft = mDown = mRight = false;
            base.OnLostFocus( e );
        }

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );
            GL.Viewport( ClientRectangle );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                mDefaultShader?.Dispose();
                mGridShader?.Dispose();
                mModel?.Dispose();
                mGridVertexBuffer?.Dispose();

                Application.Idle -= OnApplicationIdle;
            }

            GL.DeleteVertexArray( mGridVertexArrayID );
            base.Dispose( disposing );
        }

        ~ModelViewControl()
        {
            Dispose( false );
        }

        private ModelViewControl() : base( new GraphicsMode( 32, 24, 0, 4 ), 3, 3, GraphicsContextFlags.ForwardCompatible )
        {
            InitializeComponent();
            MakeCurrent();
            VSync = true;

            mDefaultShader = GLShaderProgram.Create( "Default" );
            if ( mDefaultShader == null )
                mDefaultShader = GLShaderProgram.Create( "DefaultBasic" );

            mGridShader = GLShaderProgram.Create( "Grid" );
            if ( mDefaultShader == null || mGridShader == null )
            {
                Debug.WriteLine( "Shader compile failed. GL rendering will be disabled." );

                Visible = false;
                return;
            }

            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.CullFace( CullFaceMode.Back );
            GL.Enable( EnableCap.CullFace );
            GL.Enable( EnableCap.DepthTest );
            GL.Enable( EnableCap.PrimitiveRestartFixedIndex );
            GL.Enable( EnableCap.FramebufferSrgb );
        }
    }
}
