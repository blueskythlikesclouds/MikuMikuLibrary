using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Numerics;
using MikuMikuLibrary.Objects;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Collections;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Objects;

public class MeshNode : Node<Mesh>
{
    public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

    public override Control Control
    {
        get
        {
            var objectSetParent = FindParent<ObjectSetNode>();
            var objectParent = FindParent<ObjectNode>();

            if (objectSetParent == null || objectParent == null)
                return null;

            ModelViewControl.Instance.SetModel(Data, objectParent.Data, objectSetParent.Data.TextureSet);
            return ModelViewControl.Instance;
        }
    }

    [Category("General")]
    [DisplayName("Bounding sphere")]
    public BoundingSphere BoundingSphere
    {
        get => GetProperty<BoundingSphere>();
        set => SetProperty(value);
    }

    [Category("General")]
    public Vector3[] Positions
    {
        get => GetProperty<Vector3[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    public Vector3[] Normals
    {
        get => GetProperty<Vector3[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    public Vector4[] Tangents
    {
        get => GetProperty<Vector4[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Texture coordinates 1")]
    public Vector2[] TexCoords0
    {
        get => GetProperty<Vector2[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Texture coordinates 2")]
    public Vector2[] TexCoords1
    {
        get => GetProperty<Vector2[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Texture coordinates 3")]
    public Vector2[] TexCoords2
    {
        get => GetProperty<Vector2[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Texture coordinates 4")]
    public Vector2[] TexCoords3
    {
        get => GetProperty<Vector2[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Colors 1")]
    public Vector4[] Colors0
    {
        get => GetProperty<Vector4[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Colors 2")]
    public Vector4[] Colors1
    {
        get => GetProperty<Vector4[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Blend weights")]
    public Vector4[] BlendWeights
    {
        get => GetProperty<Vector4[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Blend indices")]
    public Vector4Int[] BlendIndices
    {
        get => GetProperty<Vector4Int[]>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Flags")]
    public MeshFlags MeshFlags
    {
        get => GetProperty<MeshFlags>(nameof(Mesh.Flags));
        set => SetProperty(value, nameof(Mesh.Flags));
    }

    protected override void Initialize()
    {
        AddCustomHandler("Create color data", () =>
        {
            while (true)
            {
                using (var inputDialog = new InputDialog
                           { WindowTitle = "Please enter color values. (R, G, B, A)", Input = "1, 1, 1, 1" })
                {
                    if (inputDialog.ShowDialog() != DialogResult.OK)
                        break;

                    bool success = true;

                    var split = inputDialog.Input.Split(',').Select(x =>
                    {
                        success &= float.TryParse(x, out float value);
                        return value;
                    }).ToArray();

                    if (split.Length != 4 || !success)
                    {
                        MessageBox.Show("Please enter valid color values.", Program.Name, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        continue;
                    }

                    var color = new Vector4(split[0], split[1], split[2], split[3]);
                    var colors = new Vector4[Data.Positions.Length];
                    for (int i = 0; i < colors.Length; i++)
                        colors[i] = color;

                    Colors0 = colors;
                    break;
                }
            }
        });
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<SubMesh>("Submeshes", Data.SubMeshes));
    }

    protected override void SynchronizeCore()
    {
    }

    public MeshNode(string name, Mesh data) : base(name, data)
    {
    }
}