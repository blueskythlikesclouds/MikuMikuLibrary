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
        private static ModelViewControl instance;

        public static ModelViewControl Instance
        {
            get
            {
                if ( instance == null )
                    instance = new ModelViewControl();

                return instance;
            }
        }

        public static void DisposeInstance()
        {
            instance?.Dispose();
        }

        private IGLDraw model;
        private GLShaderProgram defaultShader;
        private GLShaderProgram lineShader;

        private Vector3 camPos = Vector3.Zero;
        private Vector3 camRot = new Vector3( -90, 0, 0 );
        private Vector3 camDir = new Vector3( 0, 0, -1 );
        private Point prevMousePos;
        private readonly Vector3 camUp = new Vector3( 0, 1, 0 );
        private readonly float fieldOfView = MathHelper.DegreesToRadians( 45 );

        private bool left, right, up, down;

        private int gridVertexArrayID;
        private GLBuffer<Vector3> gridVertexBuffer;

        public void SetModel( Model model, TextureSet textureSet )
        {
            if ( defaultShader == null || lineShader == null )
                return;

            this.model?.Dispose();
            ResetCamera();

            this.model = new GLModel( model, textureSet );

            var bSphere = new BoundingSphere();
            foreach ( var mesh in model.Meshes )
                bSphere.Merge( mesh.BoundingSphere );

            var distance = ( float )( ( bSphere.Radius * 2f ) / Math.Tan( fieldOfView ) ) + 0.5f;

            camPos = new Vector3(
                bSphere.Center.X,
                bSphere.Center.Y,
                bSphere.Center.Z + distance );
        }

        public void SetModel( Mesh mesh, TextureSet textureSet )
        {
            if ( defaultShader == null || lineShader == null )
                return;

            model?.Dispose();
            ResetCamera();

            model = new GLMesh( mesh, new Dictionary<int, GLTexture>(), textureSet );

            var distance = ( float )( ( mesh.BoundingSphere.Radius * 2f ) / Math.Tan( fieldOfView ) ) + 0.5f;

            camPos = new Vector3(
                mesh.BoundingSphere.Center.X,
                mesh.BoundingSphere.Center.Y,
                mesh.BoundingSphere.Center.Z + distance );
        }

        public void SetModel( SubMesh subMesh, Mesh mesh, TextureSet textureSet )
        {
            if ( defaultShader == null || lineShader == null )
                return;

            model?.Dispose();
            ResetCamera();

            var materials = new List<GLMaterial>( new GLMaterial[ mesh.Materials.Count ] );
            var dictionary = new Dictionary<int, GLTexture>();

            foreach ( var indexTable in subMesh.IndexTables )
            {
                if ( materials[ indexTable.MaterialIndex ] == null )
                    materials[ indexTable.MaterialIndex ] = new GLMaterial( mesh.Materials[ indexTable.MaterialIndex ], dictionary, textureSet );
            }

            model = new GLSubMesh( subMesh, materials );

            var distance = ( float )( ( subMesh.BoundingSphere.Radius * 2f ) / Math.Tan( fieldOfView ) ) + 0.5f;

            camPos = new Vector3(
                subMesh.BoundingSphere.Center.X,
                subMesh.BoundingSphere.Center.Y,
                subMesh.BoundingSphere.Center.Z + distance );
        }

        public void ResetCamera()
        {
            camPos = Vector3.Zero;
            camRot = new Vector3( -90, 0, 0 );
            camDir = new Vector3( 0, 0, -1 );
        }

        private void UpdateCamera()
        {
            float x = MathHelper.DegreesToRadians( camRot.X );
            float y = MathHelper.DegreesToRadians( camRot.Y );
            float yCos = ( float )Math.Cos( y );

            var front = new Vector3()
            {
                X = ( float )Math.Cos( x ) * yCos,
                Y = ( float )Math.Sin( y ),
                Z = ( float )Math.Sin( x ) * yCos
            };

            camDir = Vector3.Normalize( front );

            float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.8f : ModifierKeys.HasFlag( Keys.Control ) ? 0.025f : 0.1f;
            if ( up ) camPos += camDir * cameraSpeed;
            else if ( down ) camPos -= camDir * cameraSpeed;
            if ( left ) camPos -= Vector3.Normalize( Vector3.Cross( camDir, camUp ) ) * cameraSpeed;
            else if ( right ) camPos += Vector3.Normalize( Vector3.Cross( camDir, camUp ) ) * cameraSpeed;
        }

        private Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt( camPos, camPos + camDir, camUp );
        }

        private Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView( fieldOfView, ( float )Width / Height, 0.1f, 1000000f );
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

            gridVertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray( gridVertexArrayID );

            gridVertexBuffer = new GLBuffer<Vector3>( BufferTarget.ArrayBuffer, vertices.ToArray(), 12, BufferUsageHint.StaticDraw );

            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, gridVertexBuffer.Stride, 0 );
            GL.EnableVertexAttribArray( 0 );
        }

        private void DrawGrid( Matrix4 view, Matrix4 projection )
        {
            lineShader.Use();
            lineShader.SetUniform( "view", view );
            lineShader.SetUniform( "projection", projection );
            lineShader.SetUniform( "color", new Vector4( 0.15f, 0.15f, 0.15f, 1f ) );

            GL.BindVertexArray( gridVertexArrayID );
            GL.DrawArrays( PrimitiveType.Lines, 0, gridVertexBuffer.Length );
        }

        protected override void OnPaint( PaintEventArgs pe )
        {
            if ( model != null && defaultShader != null && lineShader != null )
            {
                GL.ClearColor( Color4.LightGray );
                GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

                UpdateCamera();
                var view = GetViewMatrix();
                var projection = GetProjectionMatrix();

                defaultShader.Use();
                defaultShader.SetUniform( "view", view );
                defaultShader.SetUniform( "projection", projection );
                defaultShader.SetUniform( "viewPosition", camPos );
                defaultShader.SetUniform( "lightPosition", camPos );
                model.Draw( defaultShader );

                // Draw a grid
                DrawGrid( view, projection );

                SwapBuffers();
            }
        }

        protected override void OnMouseMove( MouseEventArgs e )
        {
            float deltaX = e.Location.X - prevMousePos.X;
            float deltaY = e.Location.Y - prevMousePos.Y;

            if ( e.Button == MouseButtons.Left )
            {
                float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.04f : ModifierKeys.HasFlag( Keys.Control ) ? 0.00125f : 0.005f;
                var dirRight = Vector3.Normalize( Vector3.Cross( camDir, camUp ) );
                var dirUp = Vector3.Normalize( Vector3.Cross( camDir, dirRight ) );
                camPos -= ( ( dirRight * deltaX ) + ( dirUp * deltaY ) ) * cameraSpeed;
            }

            else if ( e.Button == MouseButtons.Right )
            {
                camRot.X += deltaX * 0.1f;
                camRot.Y -= deltaY * 0.1f;
            }

            prevMousePos = e.Location;

            base.OnMouseMove( e );
        }

        protected override void OnMouseWheel( MouseEventArgs e )
        {
            float cameraSpeed = ModifierKeys.HasFlag( Keys.Shift ) ? 0.04f : ModifierKeys.HasFlag( Keys.Control ) ? 0.00125f : 0.005f;
            camPos += camDir * cameraSpeed * e.Delta;

            base.OnMouseWheel( e );
        }

        protected override void OnKeyDown( KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.W:
                    up = true;
                    e.Handled = true;
                    break;

                case Keys.A:
                    left = true;
                    e.Handled = true;
                    break;

                case Keys.S:
                    down = true;
                    e.Handled = true;
                    break;

                case Keys.D:
                    right = true;
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
                    up = false;
                    e.Handled = true;
                    break;

                case Keys.A:
                    left = false;
                    e.Handled = true;
                    break;

                case Keys.S:
                    down = false;
                    e.Handled = true;
                    break;

                case Keys.D:
                    right = false;
                    e.Handled = true;
                    break;
            }

            base.OnKeyUp( e );
        }

        protected override void OnLostFocus( EventArgs e )
        {
            up = left = down = right = false;
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
                components?.Dispose();
                defaultShader?.Dispose();
                lineShader?.Dispose();
                model?.Dispose();
                gridVertexBuffer?.Dispose();

                Application.Idle -= OnApplicationIdle;
            }

            GL.DeleteVertexArray( gridVertexArrayID );
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

            defaultShader = GLShaderProgram.Create( "Default" );
            if ( defaultShader == null )
                defaultShader = GLShaderProgram.Create( "DefaultBasic" );

            lineShader = GLShaderProgram.Create( "Line" );
            if ( defaultShader == null || lineShader == null )
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
