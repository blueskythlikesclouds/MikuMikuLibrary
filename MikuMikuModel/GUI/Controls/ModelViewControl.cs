using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures;
using MikuMikuModel.GUI.Controls.ModelView;
using MikuMikuModel.Resources.Styles;
using OpenTK.Graphics.OpenGL;
using OpenTK.WinForms;

using MathHelper = OpenTK.Mathematics.MathHelper;

namespace MikuMikuModel.GUI.Controls;

public class ModelViewControl : GLControl
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
    private static readonly float sFieldOfView = MathHelper.DegreesToRadians(65);
        
    private readonly GLShaderProgram mDefaultShader;
    private readonly GLShaderProgram mGridShader;

    private Vector4 mBackgroundColor = new(0.827f, 0.827f, 0.827f, 1.0f);

    // 
    // Orbit Camera
    //
    private Vector3 mCamViewPoint = Vector3.Zero;
    private Vector3 mCamInterest = Vector3.Zero;

    private Vector2 mCamOrbitRotation = Vector2.Zero;
    private float mCamOrbitDistance = 0.0f;

    private float mCamOrbitDistanceMin = 0.1f;
    private float mCamOrbitDistanceMax = 100000.0f;

    //
    // Free Camera
    //
    private Vector3 mCamDirection = new Vector3(0, 0, -1);

    private Vector3 mCamPosition = Vector3.Zero;
    private Vector3 mCamRotation = new Vector3(-90, 0, 0);

    private bool mComputeProjectionMatrix = true;
    private bool mFocused = true;

    private Vector4 mGridInnerColor = new(0.55f, 0.55f, 0.55f, 1);
    private Vector4 mGridOuterColor = new(0.65f, 0.65f, 0.65f, 1);
    private Vector4 mGridXColor = new(0.75f, 0.25f, 0.25f, 1);
    private Vector4 mGridZColor = new(0.25f, 0.75f, 0.25f, 1);

    private int mGridVertexArrayId;
    private GLBuffer<Vector4> mGridVertexBuffer;

    private bool mLeft, mRight, mUp, mDown, mFront, mBack;
    private bool mPreviousMouseLeft, mPreviousMouseRight;

    private readonly Stopwatch mStopwatch;
    private long mPreviousTime;
    private float mSpeedFactor;

    private BoundingSphere mBoundingSphere;
    private IDrawable mModel;
    private Point mPreviousMousePosition;
    private Matrix4x4 mProjectionMatrix;
    private Matrix4x4 mViewMatrix;

    private readonly List<DrawCommand> mOpaqueDrawCommands;
    private readonly List<DrawCommand> mTransparentDrawCommands;

    public static ModelViewControl Instance => sInstance ??= new ModelViewControl();

    private bool CanRender => mDefaultShader != null && mGridShader != null;

    private static bool sUseOrbitCamera = true;

    public static bool UseOrbitCamera
    {
        get => sUseOrbitCamera;
        set
        {
            sUseOrbitCamera = value;
            sInstance?.SetCamera(sInstance.mBoundingSphere);
        }
    }

    public static void ResetInstance()
    {
        sInstance?.Reset();
    }

    public static void DisposeInstance()
    {
        sInstance?.Dispose();
    }

    public Color GridInnerColor
    {
        get => mGridInnerColor.ToColor();
        set => mGridInnerColor = new Vector4(value.R, value.G, value.B, value.A) / 255.0f;
    }

    public Color GridOuterColor
    {
        get => mGridOuterColor.ToColor();
        set => mGridOuterColor = new Vector4(value.R, value.G, value.B, value.A) / 255.0f;
    }

    public Color GridXColor
    {
        get => mGridXColor.ToColor();
        set => mGridXColor = new Vector4(value.R, value.G, value.B, value.A) / 255.0f;
    }

    public Color GridZColor
    {
        get => mGridZColor.ToColor();
        set => mGridZColor = new Vector4(value.R, value.G, value.B, value.A) / 255.0f;
    }

    public Color BackgroundColor
    {
        get => mBackgroundColor.ToColor();
        set => mBackgroundColor = new Vector4(value.R, value.G, value.B, value.A) / 255.0f;
    }

    public void SetModel(ObjectSet objectSet, TextureSet textureSet)
    {
        if (!CanRender)
            return;

        Reset();

        mModel = new GLObjectSet(objectSet, textureSet);

        mBoundingSphere = new BoundingSphere();

        foreach (var mesh in objectSet.Objects)
        {
            mBoundingSphere.Center += mesh.BoundingSphere.Center;
            mBoundingSphere.Radius = Math.Max(mBoundingSphere.Radius, mesh.BoundingSphere.Radius);
        }

        mBoundingSphere.Center /= objectSet.Objects.Count;

        SetCamera(mBoundingSphere);
    }

    public void SetModel(Object obj, TextureSet textureSet)
    {
        if (!CanRender)
            return;

        Reset();
        mBoundingSphere = obj.BoundingSphere;
        mModel = new GLObject(obj, new Dictionary<uint, GLTexture>(), textureSet);
        SetCamera(obj.BoundingSphere);
    }

    public void SetModel(Mesh mesh, Object obj, TextureSet textureSet)
    {
        if (!CanRender)
            return;

        Reset();

        var materials = new List<GLMaterial>(new GLMaterial[obj.Materials.Count]);
        var dictionary = new Dictionary<uint, GLTexture>();

        foreach (var subMesh in mesh.SubMeshes)
        {
            if (materials[(int)subMesh.MaterialIndex] == null)
                materials[(int)subMesh.MaterialIndex] = new GLMaterial(obj.Materials[(int)subMesh.MaterialIndex], dictionary, textureSet);
        }

        mBoundingSphere = mesh.BoundingSphere;
        mModel = new GLMesh(mesh, materials);
        SetCamera(mesh.BoundingSphere);
    }

    private void SetCamera(BoundingSphere boundingSphere)
    {
        float distance = (float)(boundingSphere.Radius * 2f / Math.Tan(sFieldOfView)) + 0.75f;

        if (UseOrbitCamera)
        {
            mCamInterest = boundingSphere.Center;
            mCamOrbitRotation = Vector2.Zero;
            mCamOrbitDistance = distance;
            mCamViewPoint = mCamInterest + CalculateCameraOrbitPosition();
        }

        else
        {
            mCamPosition = new Vector3(
                boundingSphere.Center.X,
                boundingSphere.Center.Y,
                boundingSphere.Center.Z + distance);
        }

        Invalidate();
    }

    private void Reset()
    {
        mModel?.Dispose();
        mModel = null;

        GL.Finish();

        ResetCamera();
    }

    private void ResetCamera()
    {
        if (UseOrbitCamera)
        {
            mCamViewPoint = new Vector3(3.45f, 1.0f, 0.0f);
            mCamInterest = new Vector3(0.0f, 1.0f, 0.0f);
            mCamOrbitRotation = new Vector2(0.0f, 5.0f);
            mCamOrbitDistance = 5.0f;
        }

        else
        {
            mCamPosition = Vector3.Zero;
            mCamRotation = new Vector3(-90, 0, 0);
            mCamDirection = new Vector3(0, 0, -1);
        }
    }

    private Vector3 CalculateCameraOrbitPosition()
    {
        return Vector3.Transform(new Vector3(0.0f, 0.0f, mCamOrbitDistance),
            Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(-mCamOrbitRotation.Y)) *
            Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(-mCamOrbitRotation.X)));
    }

    private void UpdateCamera()
    {
        if (UseOrbitCamera)
        {
            var frontDirection = mCamInterest - mCamViewPoint;
            frontDirection.Y = 0.0f;
            frontDirection = Vector3.Normalize(frontDirection);

            float cameraSpeed = ((ModifierKeys & SPEED_UP_KEY) != 0 ? CAMERA_SPEED_FAST :
                (ModifierKeys & SLOW_DOWN_KEY) != 0 ? CAMERA_SPEED_SLOW : CAMERA_SPEED) * mSpeedFactor;

            if (mFront && !mBack)
                mCamInterest += frontDirection * cameraSpeed;
            else if (mBack && !mFront)
                mCamInterest -= frontDirection * cameraSpeed;

            if (mLeft && !mRight)
                mCamInterest -= Vector3.Normalize(Vector3.Cross(frontDirection, sCamUp)) * cameraSpeed;
            else if (mRight && !mLeft)
                mCamInterest += Vector3.Normalize(Vector3.Cross(frontDirection, sCamUp)) * cameraSpeed;

            if (mUp && !mDown)
                mCamInterest += Vector3.UnitY * cameraSpeed / 2;
            else if (!mUp && mDown)
                mCamInterest -= Vector3.UnitY * cameraSpeed / 2;

            mCamViewPoint = mCamInterest + CalculateCameraOrbitPosition();
        }

        else
        {
            float x = MathHelper.DegreesToRadians(mCamRotation.X);
            float y = MathHelper.DegreesToRadians(mCamRotation.Y);
            float yCos = (float)Math.Cos(y);

            var front = new Vector3
            {
                X = (float)Math.Cos(x) * yCos,
                Y = (float)Math.Sin(y),
                Z = (float)Math.Sin(x) * yCos
            };

            mCamDirection = Vector3.Normalize(front);

            float cameraSpeed = ((ModifierKeys & SPEED_UP_KEY) != 0 ? CAMERA_SPEED_FAST :
                (ModifierKeys & SLOW_DOWN_KEY) != 0 ? CAMERA_SPEED_SLOW : CAMERA_SPEED) * mSpeedFactor;

            if (mFront && !mBack)
                mCamPosition += mCamDirection * cameraSpeed;
            else if (mBack && !mFront)
                mCamPosition -= mCamDirection * cameraSpeed;

            if (mLeft && !mRight)
                mCamPosition -= Vector3.Normalize(Vector3.Cross(mCamDirection, sCamUp)) * cameraSpeed;
            else if (mRight && !mLeft)
                mCamPosition += Vector3.Normalize(Vector3.Cross(mCamDirection, sCamUp)) * cameraSpeed;

            if (mUp && !mDown)
                mCamPosition += Vector3.UnitY * cameraSpeed / 2;
            else if (!mUp && mDown)
                mCamPosition -= Vector3.UnitY * cameraSpeed / 2;
        }

        if (mLeft || mRight || mUp || mDown || mFront || mBack)
            Invalidate();
    }

    private void GetViewMatrix(out Matrix4x4 view)
    {
        view = UseOrbitCamera
            ? Matrix4x4.CreateLookAt(mCamViewPoint, mCamInterest, sCamUp)
            : Matrix4x4.CreateLookAt(mCamPosition, mCamPosition + mCamDirection, sCamUp);
    }

    private void GetProjectionMatrix(out Matrix4x4 projection)
    {
        projection = Matrix4x4.CreatePerspectiveFieldOfView(sFieldOfView, (float)Width / Height, 0.1f, 10000f);
    }

    private void OnStyleChanged(object sender, StyleChangedEventArgs eventArgs)
    {
        StyleHelpers.ApplyStyle(this, eventArgs.Style);
        Refresh();
    }

    protected override void OnLoad(EventArgs e)
    {
        CreateGrid();

        StyleHelpers.StoreDefaultStyle(this);

        if (StyleSet.CurrentStyle != null)
            StyleHelpers.ApplyStyle(this, StyleSet.CurrentStyle);

        StyleSet.StyleChanged += OnStyleChanged;

        base.OnLoad(e);
    }

    private void CreateGrid()
    {
        const float gridSize = 100.0f;
        const float gridSpacing = 0.5f;

        var vertices = new List<Vector4>((int)(gridSize / gridSpacing * 4));

        for (float i = -gridSize; i <= gridSize; i += gridSpacing)
        {
            int attrX;
            int attrZ;

            // TODO: What's an actual good way to do this?

            if (Math.Abs(i) < 0.001f)
            {
                attrX = 0;
                attrZ = 1;
            }

            else if (Math.Abs(i % (gridSpacing * 5.0f)) < 0.001f)
                attrX = attrZ = 2;

            else
                attrX = attrZ = 3;

            vertices.Add(new Vector4(i, 0, -gridSize, attrX));
            vertices.Add(new Vector4(i, 0, gridSize, attrX));
            vertices.Add(new Vector4(-gridSize, 0, i, attrZ));
            vertices.Add(new Vector4(gridSize, 0, i, attrZ));
        }

        mGridVertexArrayId = GL.GenVertexArray();
        GL.BindVertexArray(mGridVertexArrayId);

        mGridVertexBuffer = new GLBuffer<Vector4>(BufferTarget.ArrayBuffer, vertices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, mGridVertexBuffer.Stride, 0);
        GL.EnableVertexAttribArray(0);
    }

    private void Draw(List<DrawCommand> drawCommands)
    {
        GLMesh prevMesh = null;
        GLSubMesh prevSubMesh = null;
        GLMaterial prevMaterial = null;

        foreach (var drawCommand in drawCommands)
        {
            if (prevMesh != drawCommand.Mesh)
            {
                mDefaultShader.SetUniform("uHasNormal", drawCommand.Mesh.NormalBuffer != null);
                mDefaultShader.SetUniform("uHasTexCoord0", drawCommand.Mesh.TexCoord0Buffer != null);
                mDefaultShader.SetUniform("uHasTexCoord1", drawCommand.Mesh.TexCoord1Buffer != null);
                mDefaultShader.SetUniform("uHasColor0", drawCommand.Mesh.Color0Buffer != null);
                mDefaultShader.SetUniform("uHasTangent", drawCommand.Mesh.TangentBuffer != null);

                GL.BindVertexArray(drawCommand.Mesh.VertexArrayId);
            }

            if (prevMaterial != drawCommand.SubMesh.Material)
                drawCommand.SubMesh.Material.Bind(mDefaultShader);

            if (prevSubMesh != drawCommand.SubMesh)
                drawCommand.SubMesh.ElementBuffer.Bind();

            GL.DrawElements((OpenTK.Graphics.OpenGL.PrimitiveType)drawCommand.SubMesh.PrimitiveType,
                drawCommand.SubMesh.ElementBuffer.Length, DrawElementsType.UnsignedInt, 0);

            prevMesh = drawCommand.Mesh;
            prevMaterial = drawCommand.SubMesh.Material;
            prevSubMesh = drawCommand.SubMesh;
        }
    }

    private void DrawModel(ref Matrix4x4 view, ref Matrix4x4 projection)
    {
        mDefaultShader.Use();
        mDefaultShader.SetUniform("uView", view);
        mDefaultShader.SetUniform("uProjection", projection);

        if (UseOrbitCamera)
        {
            mDefaultShader.SetUniform("uViewPosition", mCamViewPoint);
            mDefaultShader.SetUniform("uLightPosition", mCamViewPoint);
        }

        else
        {
            mDefaultShader.SetUniform("uViewPosition", mCamPosition);
            mDefaultShader.SetUniform("uLightPosition", mCamPosition);
        }

        mOpaqueDrawCommands.Clear();
        mTransparentDrawCommands.Clear();

        mModel.Submit(mOpaqueDrawCommands, mTransparentDrawCommands);

        if (mOpaqueDrawCommands.Count > 0)
        {
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
            Draw(mOpaqueDrawCommands);
        }

        if (mTransparentDrawCommands.Count > 0)
        {
            mTransparentDrawCommands.Sort((x, y) =>
                Vector3.DistanceSquared(y.SubMesh.Center, mCamPosition)
                    .CompareTo(Vector3.DistanceSquared(x.SubMesh.Center, mCamPosition)));

            // Transparent
            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            Draw(mTransparentDrawCommands);
        }

        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);
    }

    private void DrawGrid(ref Matrix4x4 view, ref Matrix4x4 projection)
    {
        mGridShader.Use();
        mGridShader.SetUniform("uView", view);
        mGridShader.SetUniform("uProjection", projection);
        mGridShader.SetUniform("uInnerColor", mGridInnerColor);
        mGridShader.SetUniform("uOuterColor", mGridOuterColor);
        mGridShader.SetUniform("uXColor", mGridXColor);
        mGridShader.SetUniform("uZColor", mGridZColor);

        GL.Enable(EnableCap.Blend);
        GL.DepthMask(false);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.BindVertexArray(mGridVertexArrayId);
        GL.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.Lines, 0, mGridVertexBuffer.Length);
        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        if (!CanRender)
            return;

        long time = mStopwatch.ElapsedMilliseconds;
        mSpeedFactor = MathF.Min(1.0f, (time - mPreviousTime) / 33.33f) * 2.0f;
        mPreviousTime = time;

        if (mFocused)
            UpdateCamera();

        GL.ClearColor(mBackgroundColor.X, mBackgroundColor.Y, mBackgroundColor.Z, mBackgroundColor.W);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (mComputeProjectionMatrix)
        {
            GetProjectionMatrix(out mProjectionMatrix);
            mComputeProjectionMatrix = false;
        }

        GetViewMatrix(out mViewMatrix);

        DrawGrid(ref mViewMatrix, ref mProjectionMatrix);
        DrawModel(ref mViewMatrix, ref mProjectionMatrix);

        SwapBuffers();
    }

    // Clicking on the control doesn't get it focused for some reason, so do it manually.
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            Focus();
        
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        mPreviousMouseLeft &= e.Button != MouseButtons.Left;
        mPreviousMouseRight &= e.Button != MouseButtons.Right;

        base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        float deltaX = e.Location.X - mPreviousMousePosition.X;
        float deltaY = e.Location.Y - mPreviousMousePosition.Y;

        switch (e.Button)
        {

            case MouseButtons.Left when UseOrbitCamera && mPreviousMouseLeft:
                {
                    const float cameraSpeed = 0.18f;
                    mCamOrbitRotation.X += deltaX * cameraSpeed;
                    mCamOrbitRotation.Y += deltaY * cameraSpeed;
                    mCamOrbitRotation.Y = MathHelper.Clamp(mCamOrbitRotation.Y, -89.9f, +89.9f);
                    break;
                }

            case MouseButtons.Left when !UseOrbitCamera && mPreviousMouseLeft:
                {
                    float cameraSpeed = (ModifierKeys.HasFlag(Keys.Shift) ? 0.04f :
                        ModifierKeys.HasFlag(Keys.Control) ? 0.00125f : 0.005f) * mSpeedFactor;

                    var dirRight = Vector3.Normalize(Vector3.Cross(mCamDirection, sCamUp));
                    var dirUp = Vector3.Normalize(Vector3.Cross(mCamDirection, dirRight));
                    mCamPosition -= (dirRight * deltaX + dirUp * deltaY) * cameraSpeed;
                    break;
                }

            case MouseButtons.Right when !UseOrbitCamera && mPreviousMouseRight:
                mCamRotation.X += deltaX * 0.1f;
                mCamRotation.Y -= deltaY * 0.1f;
                mCamRotation.Y = MathHelper.Clamp(mCamRotation.Y, -89.9f, +89.9f);
                break;

            default:
                {
                    mPreviousMouseLeft |= e.Button == MouseButtons.Left;
                    mPreviousMouseRight |= e.Button == MouseButtons.Right;
                    break;
                }
        }

        if (e.Button != MouseButtons.None)
            Invalidate();

        mPreviousMousePosition = e.Location;

        base.OnMouseMove(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        float cameraSpeed = ((ModifierKeys & SPEED_UP_KEY) != 0 ? WHEEL_CAMERA_SPEED_FAST :
            (ModifierKeys & SLOW_DOWN_KEY) != 0 ? WHEEL_CAMERA_SPEED_SLOW : WHEEL_CAMERA_SPEED) * mSpeedFactor;

        if (UseOrbitCamera)
        {
            mCamOrbitDistance = MathHelper.Clamp(mCamOrbitDistance - (cameraSpeed * e.Delta), mCamOrbitDistanceMin,
                mCamOrbitDistanceMax);
        }
        else
        {
            mCamPosition += mCamDirection * cameraSpeed * e.Delta;
        }

        Invalidate();

        base.OnMouseWheel(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        bool keyHandled = true;

        switch (e.KeyCode)
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

        if (keyHandled)
        {
            e.Handled = true;
            Invalidate();
        }

        base.OnKeyUp(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        bool keyHandled = true;

        switch (e.KeyCode)
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

        if (keyHandled)
            e.Handled = true;

        base.OnKeyUp(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        mFocused = true;
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        mFocused = false;
        mFront = mLeft = mUp = mDown = mBack = mRight = false;
        mPreviousMouseLeft = mPreviousMouseRight = false;

        base.OnLostFocus(e);
    }

    protected override void OnResize(EventArgs e)
    {
        if (CanRender)
        {
            GL.Viewport(ClientRectangle);
            mComputeProjectionMatrix = true;
        }

        base.OnResize(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            mDefaultShader?.Dispose();
            mGridShader?.Dispose();
            mModel?.Dispose();
            mGridVertexBuffer?.Dispose();
            StyleSet.StyleChanged -= OnStyleChanged;
        }

        GL.DeleteVertexArray(mGridVertexArrayId);
        base.Dispose(disposing);
    }

    ~ModelViewControl()
    {
        Dispose(false);
    }

    private ModelViewControl() : base(new GLControlSettings { NumberOfSamples = 2, Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible })
    {
        MakeCurrent();
        
        mGridShader = GLShaderProgram.Create("Grid");
        mDefaultShader = GLShaderProgram.Create("Default");

        if (!CanRender)
        {
            MessageBox.Show("Shader compilation failed. GL rendering will be disabled.", Program.Name,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            Visible = false;
            return;
        }

        mOpaqueDrawCommands = new List<DrawCommand>(32);
        mTransparentDrawCommands = new List<DrawCommand>(32);

        mStopwatch = new Stopwatch();
        mStopwatch.Start();

        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.CullFace(CullFaceMode.Back);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.PrimitiveRestart);
        GL.PrimitiveRestartIndex(~0u);

        Context.SwapInterval = 1;
    }
}
