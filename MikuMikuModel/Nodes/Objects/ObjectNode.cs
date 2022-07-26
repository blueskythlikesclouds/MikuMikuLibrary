using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Archives;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.TypeConverters;
using MikuMikuModel.Resources;
using MikuMikuLibrary.Objects.Extra.Blocks;
using Newtonsoft.Json;
using MikuMikuModel.Configurations;
using MikuMikuLibrary.Extensions;

namespace MikuMikuModel.Nodes.Objects
{
    public class ObjectNode : Node<Object>
    {
        public override NodeFlags Flags =>
            NodeFlags.Import | NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/Object.png" );

        public override Control Control
        {
            get
            {
                var objectSetParent = FindParent<ObjectSetNode>();

                if ( objectSetParent == null )
                    return null;

                ModelViewControl.Instance.SetModel( Data, objectSetParent.Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        [Category( "General" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Flags" )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint ObjectFlags
        {
            get => GetProperty<uint>( nameof( Object.Flags ) );
            set => SetProperty( value, nameof( Object.Flags ) );
        }

        protected override void Initialize()
        {

            AddCustomHandler("Auto Create Constraints", () =>
            {
                string jsonFilePath = null;
                string objectFarcFilePath = null;
                ObjectSet baseObjectSet = new ObjectSet();
                Dictionary<string, string> boneMap = null;

                // open json
                using (var jsonFileDialog = new OpenFileDialog()
                {
                    Title = "Select BoneMap json file.",
                    Filter = "JSON files (*.json)|*.json|All files(*.*)|*.*",
                    FilterIndex = 0,
                    RestoreDirectory = true,
                })
                {
                    if (jsonFileDialog.ShowDialog() == DialogResult.OK)
                        jsonFilePath = jsonFileDialog.FileName;
                }

                if (jsonFilePath != null)
                {
                    var text = File.ReadAllText(jsonFilePath);
                    boneMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                }
                else
                {
                    MessageBox.Show("BoneMap JSON file path is null!");
                    return;
                }
                    
                // open base object farc
                using (var objectFarcFileDialog = new OpenFileDialog()
                {
                    Title = "Select reference DIVA Object Set.",
                    Filter = "FARC Archive (*.farc)|*.farc|Object Set (Classic) (*.bin)|*.bin|Object Set (Modern)|*.osd|All files(*.*)|*.*",
                    FilterIndex = 0,
                    RestoreDirectory = true,
                })
                {
                    if (objectFarcFileDialog.ShowDialog() == DialogResult.OK)
                        objectFarcFilePath = objectFarcFileDialog.FileName;
                }
                if (objectFarcFilePath != null)
                {
                    if (objectFarcFilePath.EndsWith(".farc"))
                    {
                        var baseObjFarc = BinaryFile.Load<FarcArchive>(objectFarcFilePath);
                        var baseObjBinSrc = baseObjFarc.Open(baseObjFarc.First(x => x.EndsWith("_obj.bin")), EntryStreamMode.MemoryStream);
                        baseObjectSet = BinaryFile.Load<ObjectSet>(baseObjBinSrc);
                    }
                    else
                    {
                        baseObjectSet = BinaryFile.Load<ObjectSet>(objectFarcFilePath);
                    }
                }
                else
                {
                    MessageBox.Show("Object Set file not found!");
                    return;
                }
                    

                // fix bone orientations
                foreach ( var bone in Data.Skin.Bones )
                {
                    boneMap.TryGetValue(bone.Name, out var srcbonename);
                    var srcbone = baseObjectSet.Objects[0].Skin.Bones.FirstOrDefault(x => x.Name == srcbonename);

                    if (srcbone == null)
                        continue;

                    Matrix4x4.Invert(bone.InverseBindPoseMatrix, out var bibpm);
                    Matrix4x4.Decompose(bibpm, out var boneScale, out var boneRot, out var boneTrans);
                    Matrix4x4.Invert(srcbone.InverseBindPoseMatrix, out var sbibpm);
                    Matrix4x4.Decompose(sbibpm, out var srcboneScale, out var srcboneRot, out var srcboneTrans);

                    var newbon = Matrix4x4.CreateTranslation(boneTrans);
                    var transed = newbon * sbibpm;
                    transed.Translation = boneTrans;

                    Matrix4x4.Invert(transed, out var outbon);
                    bone.InverseBindPoseMatrix = outbon;
                }

                // clear existing blocks
                Data.Skin.Blocks.Clear();

                // root bone expression block
                var exp_Root = new ExpressionBlock()
                {
                    Name = "RootBone",
                    ParentName = "n_hara_cp",
                    Position = new Vector3(0, -1.03f, 0),
                    Rotation = new Vector3(0, 0, 0),
                    Scale = Vector3.One,
                };
                exp_Root.Expressions.Add("= 0 v 0.RootBone");
                exp_Root.Expressions.Add("= 1 v 1.RootBone");
                exp_Root.Expressions.Add("= 2 v 2.RootBone");
                exp_Root.Expressions.Add("= 3 v 3.RootBone");
                exp_Root.Expressions.Add("= 4 v 4.RootBone");
                exp_Root.Expressions.Add("= 5 v 5.RootBone");
                exp_Root.Expressions.Add("= 6 v 6.RootBone");
                exp_Root.Expressions.Add("= 7 v 7.RootBone");
                exp_Root.Expressions.Add("= 8 v 8.RootBone");
                Data.Skin.Blocks.Add(exp_Root);

                foreach (var bone in Data.Skin.Bones)
                {

                    Matrix4x4.Invert(bone.InverseBindPoseMatrix, out var bindPoseMatrix);
                    var matrix = Matrix4x4.Multiply(bindPoseMatrix,
                        bone.Parent?.InverseBindPoseMatrix ?? Matrix4x4.Identity);

                    Matrix4x4.Decompose(matrix, out var scale, out var rotation, out var translation);
                    rotation = Quaternion.Normalize(rotation);

                    if (bone.Parent == null)
                        bone.Parent = new BoneInfo()
                        {
                            Name = "RootBone"
                        };

                    if (boneMap.TryGetValue(bone.Name, out string sourceNodeName))
                    {
                        var oriConstraintBlock = new ConstraintBlock
                        {
                            Name = bone.Name,
                            Data = new OrientationConstraintData(),
                            Coupling = Coupling.Rigid,
                            ParentName = bone.Parent.Name,
                            Position = translation,
                            Rotation = rotation.ToEulerAngles(),
                            Scale = scale,
                            SourceNodeName = sourceNodeName
                        };
                        Data.Skin.Blocks.Add(oriConstraintBlock);
                    }
                    else // If bone is not in constraint map, make dummy expression block instead
                    {
                        var dummyExpBlock = new ExpressionBlock
                        {
                            Name = bone.Name,
                            ParentName = bone.Parent.Name,
                            Position = translation,
                            Rotation = rotation.ToEulerAngles(),
                            Scale = scale,
                        };
                        dummyExpBlock.Expressions.Add("");
                        Data.Skin.Blocks.Add(dummyExpBlock);
                    }
                }

                // sort all blocks
                var sortedBlocks = new List<NodeBlock>();
                AddRecursively((NodeBlock)Data.Skin.Blocks.FirstOrDefault(x => x is NodeBlock nodeBlock && nodeBlock.ParentName == "n_hara_cp"));

                void AddRecursively(NodeBlock block)
                {
                    if (block is ConstraintBlock cnsBlock)
                        cnsBlock.Coupling = Coupling.Soft;

                    sortedBlocks.Add(block);

                    foreach (var alsoBlock in Data.Skin.Blocks.OfType<NodeBlock>()
                                 .Where(x => x.ParentName == block.Name || (block is OsageBlock osgBlock && x.ParentName == osgBlock.Nodes[0].Name))
                                 .OrderBy(x => x.Name))
                        AddRecursively(alsoBlock);
                }

                Data.Skin.Blocks.Clear();
                Data.Skin.Blocks.AddRange(sortedBlocks);

                NotifyModified(NodeModifyFlags.Property);
            });

        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<Mesh>( "Meshes", Data.Meshes, x => x.Name ) );
            Nodes.Add( new ListNode<Material>( "Materials", Data.Materials, x => x.Name ) );

            if ( Data.Skin != null )
                Nodes.Add( new SkinNode( "Skin", Data.Skin ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public ObjectNode( string name, Object data ) : base( name, data )
        {
        }
    }
}