using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Objects.Extra;
using MikuMikuLibrary.Objects.Extra.Blocks;
using MikuMikuLibrary.Objects.Extra.Parameters;
using MikuMikuLibrary.Parameters;
using MikuMikuLibrary.Parameters.Extensions;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Forms;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.Objects;

public class SkinNode : Node<Skin>
{
    public override NodeFlags Flags => NodeFlags.Add;

    [Category("General")]
    [DisplayName("Ex data blocks")]
    public List<IBlock> Blocks => GetProperty<List<IBlock>>();

    private Skin PrompImportExData()
    {
        string filePath =
            ModuleImportUtilities.SelectModuleImport(new[]
                { typeof(FarcArchive), typeof(ObjectSet) });

        if (string.IsNullOrEmpty(filePath))
            return null;

        ObjectSet objSet;

        if (filePath.EndsWith(".farc", StringComparison.OrdinalIgnoreCase))
            objSet = BinaryFileNode<ObjectSet>.PromptFarcArchiveViewForm(filePath,
                "Select a file to replace with.",
                "This archive has no object set file.");

        else
            objSet = BinaryFile.Load<ObjectSet>(filePath);

        if (objSet == null)
            return null;

        if (objSet.Objects.Count == 0 || !objSet.Objects.Any(x => x.Skin != null && x.Skin.Blocks.Count > 0))
        {
            MessageBox.Show("This object set has no objects with ex data.", Program.Name, MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return null;
        }

        if (objSet.Objects.Count == 1)
            return objSet.Objects[0].Skin;

        using (var listNode = new ListNode<Object>("Objects", objSet.Objects, x => x.Name))
        using (var nodeSelectForm =
               new NodeSelectForm<Object>(listNode, obj => obj.Skin != null && obj.Skin.Blocks.Count > 0))
        {
            nodeSelectForm.Text = "Please select an object.";

            if (nodeSelectForm.ShowDialog() == DialogResult.OK)
                return ((Object)nodeSelectForm.TopNode.Data).Skin;
        }

        return null;
    }

    protected override void Initialize()
    {
        AddCustomHandler("Import ex data", () =>
        {
            var skin = PrompImportExData();

            if (skin == null)
                return;

            var nodeBlocks = skin.Blocks.OfType<NodeBlock>().ToList();

            using (var itemSelectForm = new ItemSelectForm<NodeBlock>(nodeBlocks.Select(
                       x => (x, $"{x.Signature} - {(x is OsageBlock osageBlock ? osageBlock.ExternalName : x.Name)}")).OrderBy(x => x.Item2))
                   {
                       Text = "Please select the blocks you want to import.",
                       GroupBoxText = "Blocks"
                   })
            {
                if (itemSelectForm.ShowDialog() != DialogResult.OK)
                    return;

                var importedBlocks = new List<NodeBlock>(skin.Blocks.Count);

                foreach (var nodeBlock in itemSelectForm.CheckedItems)
                {
                    importedBlocks.AddRange(nodeBlock.TraverseParents(nodeBlocks));
                    importedBlocks.Add(nodeBlock);
                }

                Data.Blocks.AddRange(importedBlocks.Distinct());
            }

            OnPropertyChanged(nameof(Data.Blocks));
        }, Keys.None, CustomHandlerFlags.ClearMementos | CustomHandlerFlags.Repopulate);

        AddCustomHandler("Replace ex data", () =>
        {
            var skin = PrompImportExData();

            if (skin == null)
                return;

            Data.Blocks.Clear();
            Data.Blocks.AddRange(skin.Blocks);

            OnPropertyChanged(nameof(Data.Blocks));
        }, Keys.None, CustomHandlerFlags.ClearMementos | CustomHandlerFlags.Repopulate);

        AddCustomHandlerSeparator();

        AddCustomHandler("Export Internal Skin Parameter", () =>
        {
            var configuration = ConfigurationList.Instance.CurrentConfiguration;
            using (SaveFileDialog dlg = new SaveFileDialog() { Filter = "Skin Parameter (*.txt)|*.txt" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ParameterTreeWriter externalSkinParam = new ParameterTreeWriter();
                    foreach (var block in Data.Blocks)
                    {
                        if (block is OsageBlock osgBlock)
                        {
                            if (osgBlock.InternalSkinParameter != null)
                            {
                                externalSkinParam.PushScope(osgBlock.ExternalName);
                                {
                                    externalSkinParam.Write("node", osgBlock.Nodes, (OsageNode x) =>
                                    {
                                        externalSkinParam.Write("coli_r", osgBlock.InternalSkinParameter.CollisionRadius);
                                        externalSkinParam.Write("hinge_ymin", -osgBlock.InternalSkinParameter.HingeY);
                                        externalSkinParam.Write("hinge_ymax", osgBlock.InternalSkinParameter.HingeY);
                                        externalSkinParam.Write("hinge_zmin", -osgBlock.InternalSkinParameter.HingeZ);
                                        externalSkinParam.Write("hinge_zmax", osgBlock.InternalSkinParameter.HingeZ);
                                        externalSkinParam.Write("weight", 1.0f);
                                        externalSkinParam.Write("inertial_cancel", 0.0f);
                                    });
                                    externalSkinParam.PushScope("root");
                                    {
                                        externalSkinParam.Write("air_res", osgBlock.InternalSkinParameter.AirResistance);
                                        externalSkinParam.Write("coli", osgBlock.InternalSkinParameter.Collisions, (OsageInternalCollisionParameter x) =>
                                        {
                                            externalSkinParam.Write("type", (int)x.CollisionType);
                                            externalSkinParam.Write("radius", x.CollisionRadius);
                                            externalSkinParam.PushScope("bone");
                                            {
                                                externalSkinParam.PushScope(0);
                                                {
                                                    // try to get the name
                                                    string boneName = configuration?.BoneData.Skeletons[0].ObjectBoneNames[(int)x.Head];
                                                    externalSkinParam.Write("name", boneName);
                                                    externalSkinParam.Write("posx", x.HeadPosition.X);
                                                    externalSkinParam.Write("posy", x.HeadPosition.Y);
                                                    externalSkinParam.Write("posz", x.HeadPosition.Z);
                                                }
                                                externalSkinParam.PopScope();

                                                externalSkinParam.PushScope(1);
                                                {
                                                    // try to get the name
                                                    string boneName = configuration?.BoneData.Skeletons[0].ObjectBoneNames[(int)(x.Tail == 0 ? x.Head : x.Tail)];
                                                    externalSkinParam.Write("name", boneName);
                                                    externalSkinParam.Write("posx", x.TailPosition.X);
                                                    externalSkinParam.Write("posy", x.TailPosition.Y);
                                                    externalSkinParam.Write("posz", x.TailPosition.Z);
                                                }
                                                externalSkinParam.PopScope();
                                            }
                                            externalSkinParam.PopScope();
                                        });
                                        externalSkinParam.Write("coli_type", 0);
                                        externalSkinParam.Write("force", osgBlock.InternalSkinParameter.Force);
                                        externalSkinParam.Write("force_gain", osgBlock.InternalSkinParameter.ForceGain);
                                        externalSkinParam.Write("friction", osgBlock.InternalSkinParameter.Friction);
                                        externalSkinParam.Write("init_rot_y", 0f);
                                        externalSkinParam.Write("init_rot_z", 0f);
                                        externalSkinParam.Write("rot_y", osgBlock.InternalSkinParameter.RotationY);
                                        externalSkinParam.Write("rot_z", osgBlock.InternalSkinParameter.RotationZ);
                                        externalSkinParam.Write("stiffness", 0f);
                                        externalSkinParam.Write("wind_afc", osgBlock.InternalSkinParameter.WindAffection);
                                    }
                                    externalSkinParam.PopScope();
                                }
                                externalSkinParam.PopScope();
                            }
                        }
                        else if (block is ClothBlock clsBlock)
                        {
                            if (clsBlock.InternalSkinParameter != null)
                            {
                                externalSkinParam.PushScope(clsBlock.Name);
                                {
                                    externalSkinParam.PushScope("root");
                                    {
                                        externalSkinParam.Write("air_res", clsBlock.InternalSkinParameter.AirResistance);
                                        externalSkinParam.Write("coli", clsBlock.InternalSkinParameter.Collisions, (OsageInternalCollisionParameter x) =>
                                        {
                                            externalSkinParam.Write("type", (int)x.CollisionType);
                                            externalSkinParam.Write("radius", x.CollisionRadius);
                                            externalSkinParam.PushScope("bone");
                                            {
                                                externalSkinParam.PushScope(0);
                                                {
                                                    // try to get the name
                                                    string boneName = configuration?.BoneData.Skeletons[0].ObjectBoneNames[(int)x.Head];
                                                    externalSkinParam.Write("name", boneName);
                                                    externalSkinParam.Write("posx", x.HeadPosition.X);
                                                    externalSkinParam.Write("posy", x.HeadPosition.Y);
                                                    externalSkinParam.Write("posz", x.HeadPosition.Z);
                                                }
                                                externalSkinParam.PopScope();

                                                externalSkinParam.PushScope(1);
                                                {
                                                    // try to get the name
                                                    string boneName = configuration?.BoneData.Skeletons[0].ObjectBoneNames[(int)x.Tail];
                                                    externalSkinParam.Write("name", boneName);
                                                    externalSkinParam.Write("posx", x.TailPosition.X);
                                                    externalSkinParam.Write("posy", x.TailPosition.Y);
                                                    externalSkinParam.Write("posz", x.TailPosition.Z);
                                                }
                                                externalSkinParam.PopScope();
                                            }
                                            externalSkinParam.PopScope();
                                        });
                                        externalSkinParam.Write("coli_type", 0);
                                        externalSkinParam.Write("force", clsBlock.InternalSkinParameter.Force);
                                        externalSkinParam.Write("force_gain", clsBlock.InternalSkinParameter.ForceGain);
                                        externalSkinParam.Write("friction", clsBlock.InternalSkinParameter.Friction);
                                        externalSkinParam.Write("init_rot_y", 0f);
                                        externalSkinParam.Write("init_rot_z", 0f);
                                        externalSkinParam.Write("rot_y", clsBlock.InternalSkinParameter.RotationY);
                                        externalSkinParam.Write("rot_z", clsBlock.InternalSkinParameter.RotationZ);
                                        externalSkinParam.Write("stiffness", 0f);
                                        externalSkinParam.Write("wind_afc", clsBlock.InternalSkinParameter.WindAffection);
                                    }
                                    externalSkinParam.PopScope();
                                }
                                externalSkinParam.PopScope();
                            }
                        }
                    }
                    externalSkinParam.Save(dlg.FileName);
                }
            }
        }, Keys.None, CustomHandlerFlags.None);

        AddCustomHandler("Create Internal Skin Parameter", () =>
        {
            var configuration = ConfigurationList.Instance.CurrentConfiguration;
            using (OpenFileDialog dlg = new OpenFileDialog() { Filter = "Skin Parameter (*.txt)|*.txt" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ParameterTree externalSkinParam = new ParameterTree(new EndianBinaryReader(File.OpenRead(dlg.FileName), Endianness.Little));
                    foreach (var block in Data.Blocks)
                    {
                        if (block is OsageBlock osgBlock)
                        {
                            if (externalSkinParam.OpenScope(osgBlock.ExternalName))
                            {
                                OsageInternalSkinParameter skp = new OsageInternalSkinParameter();

                                if (externalSkinParam.OpenScope("node"))
                                {
                                    if (externalSkinParam.OpenScope(0))
                                    {
                                        skp.CollisionRadius = externalSkinParam.Get<float>("coli_r");
                                        skp.HingeY = externalSkinParam.Get<float>("hinge_ymax");
                                        skp.HingeZ = externalSkinParam.Get<float>("hinge_zmax");
                                        externalSkinParam.CloseScope();
                                    }
                                    externalSkinParam.CloseScope();
                                }

                                if (externalSkinParam.OpenScope("root"))
                                {
                                    skp.AirResistance = externalSkinParam.Get<float>("air_res");
                                    externalSkinParam.Enumerate("coli", i =>
                                    {
                                        OsageInternalCollisionParameter coll = new OsageInternalCollisionParameter();
                                        coll.CollisionType = (OsageInternalCollisionType)externalSkinParam.Get<int>("type");
                                        coll.CollisionRadius = externalSkinParam.Get<float>("radius");
                                        if (externalSkinParam.OpenScope("bone"))
                                        {
                                            if (externalSkinParam.OpenScope(0))
                                            {
                                                coll.Head = (uint)configuration?.BoneData.Skeletons[0].ObjectBoneNames.FindIndex(x => x == externalSkinParam.Get<string>("name"));
                                                coll.HeadPosition = new Vector3(
                                                    externalSkinParam.Get<float>("posx"),
                                                    externalSkinParam.Get<float>("posy"),
                                                    externalSkinParam.Get<float>("posz"));
                                                externalSkinParam.CloseScope();
                                            }
                                            if (externalSkinParam.OpenScope(1))
                                            {
                                                coll.Tail = (uint)configuration?.BoneData.Skeletons[0].ObjectBoneNames.FindIndex(x => x == externalSkinParam.Get<string>("name"));
                                                coll.TailPosition = new Vector3(
                                                    externalSkinParam.Get<float>("posx"),
                                                    externalSkinParam.Get<float>("posy"),
                                                    externalSkinParam.Get<float>("posz"));
                                                externalSkinParam.CloseScope();
                                            }
                                            externalSkinParam.CloseScope();
                                        }
                                        skp.Collisions.Add(coll);
                                    });
                                    skp.Force = externalSkinParam.Get<float>("force");
                                    skp.ForceGain = externalSkinParam.Get<float>("force_gain");
                                    skp.Friction = externalSkinParam.Get<float>("friction");
                                    skp.Name = osgBlock.ExternalName;
                                    skp.RotationY = externalSkinParam.Get<float>("rot_y");
                                    skp.RotationZ = externalSkinParam.Get<float>("rot_z");
                                    skp.WindAffection = externalSkinParam.Get<float>("wind_afc");
                                    osgBlock.InternalSkinParameter = skp;
                                    externalSkinParam.CloseScope();
                                }
                                externalSkinParam.CloseScope();
                            }
                        }
                        else if (block is ClothBlock clsBlock)
                        {
                            if (externalSkinParam.OpenScope(clsBlock.Name))
                            {
                                OsageInternalSkinParameter skp = new OsageInternalSkinParameter();

                                if (externalSkinParam.OpenScope("root"))
                                {
                                    skp.AirResistance = externalSkinParam.Get<float>("air_res");
                                    externalSkinParam.Enumerate("coli", i =>
                                    {
                                        OsageInternalCollisionParameter coll = new OsageInternalCollisionParameter();
                                        coll.CollisionType = (OsageInternalCollisionType)externalSkinParam.Get<int>("type");
                                        coll.CollisionRadius = externalSkinParam.Get<float>("radius");
                                        if (externalSkinParam.OpenScope("bone"))
                                        {
                                            if (externalSkinParam.OpenScope(0))
                                            {
                                                coll.Head = (uint)configuration?.BoneData.Skeletons[0].ObjectBoneNames.FindIndex(x => x == externalSkinParam.Get<string>("name"));
                                                coll.HeadPosition = new Vector3(
                                                    externalSkinParam.Get<float>("posx"),
                                                    externalSkinParam.Get<float>("posy"),
                                                    externalSkinParam.Get<float>("posz"));
                                                externalSkinParam.CloseScope();
                                            }
                                            if (externalSkinParam.OpenScope(1))
                                            {
                                                coll.Tail = (uint)configuration?.BoneData.Skeletons[0].ObjectBoneNames.FindIndex(x => x == externalSkinParam.Get<string>("name"));
                                                coll.TailPosition = new Vector3(
                                                    externalSkinParam.Get<float>("posx"),
                                                    externalSkinParam.Get<float>("posy"),
                                                    externalSkinParam.Get<float>("posz"));
                                                externalSkinParam.CloseScope();
                                            }
                                            externalSkinParam.CloseScope();
                                        }
                                        skp.Collisions.Add(coll);
                                    });
                                    skp.CollisionRadius = externalSkinParam.Get<float>("coli_r");
                                    skp.Force = externalSkinParam.Get<float>("force");
                                    skp.ForceGain = externalSkinParam.Get<float>("force_gain");
                                    skp.Friction = externalSkinParam.Get<float>("friction");
                                    skp.HingeY = externalSkinParam.Get<float>("hinge_y");
                                    skp.HingeY = externalSkinParam.Get<float>("hinge_z");
                                    skp.Name = clsBlock.Name;
                                    skp.RotationY = externalSkinParam.Get<float>("rot_y");
                                    skp.RotationZ = externalSkinParam.Get<float>("rot_z");
                                    skp.WindAffection = externalSkinParam.Get<float>("wind_afc");
                                    clsBlock.InternalSkinParameter = skp;
                                    externalSkinParam.CloseScope();
                                }
                                externalSkinParam.CloseScope();
                            }
                        }
                    }
                }
            }
        }, Keys.None, CustomHandlerFlags.ClearMementos | CustomHandlerFlags.Repopulate);

        AddCustomHandlerSeparator();

        AddCustomHandler("Import ex data from JSON", () =>
        {
            //var skin = PrompImportExData();
            // Testing imports from Json
            // So this should just simply be...
            //taking this straight from keikei's part for testing. thanks oomf
            string jsonFilePath = null;
            // open json
            using (var jsonFileDialog = new OpenFileDialog()
                   {
                       Title = "Select NodeBlock JSON file.",
                       Filter = "JSON files (*.json)|*.json|All files(*.*)|*.*",
                       FilterIndex = 0,
                       RestoreDirectory = true,
                   })
            {
                if (jsonFileDialog.ShowDialog() == DialogResult.OK)
                    jsonFilePath = jsonFileDialog.FileName;
            }

            try
            {
                var placeholderImportedBlocks = File.ReadAllText(jsonFilePath);
                List<NodeBlock> skin = JsonSerializer.Deserialize<List<NodeBlock>>(
                    placeholderImportedBlocks, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                if (skin == null)
                    return;
                var nodeBlocks = skin;

                using (var itemSelectForm = new ItemSelectForm<NodeBlock>(nodeBlocks.Select(
                               x => (x,
                                   $"{x.Signature} - {(x is OsageBlock osageBlock ? osageBlock.ExternalName : x.Name)}"))
                           .OrderBy(x => x.Item2))
                       {
                           Text = "Please select the blocks you want to import.",
                           GroupBoxText = "Blocks"
                       })
                {
                    if (itemSelectForm.ShowDialog() != DialogResult.OK)
                        return;

                    var importedBlocks = skin;

                    foreach (var nodeBlock in itemSelectForm.CheckedItems)
                    {
                        importedBlocks.AddRange(nodeBlock.TraverseParents(nodeBlocks));
                        importedBlocks.Add(nodeBlock);
                    }

                    Data.Blocks.AddRange(importedBlocks.Distinct());
                }
            }
            catch (System.ArgumentNullException)
            {
                return;
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception.Message, Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OnPropertyChanged(nameof(Data.Blocks));
        }, Keys.None, CustomHandlerFlags.ClearMementos | CustomHandlerFlags.Repopulate);

        AddCustomHandler("Export ex data to JSON", () =>
        {
            var skin = PrompImportExData();

            if (skin == null)
                return;

            var nodeBlocks = skin.Blocks.OfType<NodeBlock>().ToList();

            using (var itemSelectForm = new ItemSelectForm<NodeBlock>(nodeBlocks.Select(
                       x => (x, $"{x.Signature} - {(x is OsageBlock osageBlock ? osageBlock.ExternalName : x.Name)}")).OrderBy(x => x.Item2))
                   {
                       Text = "Please select the blocks you want to export.",
                       GroupBoxText = "Blocks"
                   })
            {
                if (itemSelectForm.ShowDialog() != DialogResult.OK)
                    return;

                var importedBlocks = new List<NodeBlock>(skin.Blocks.Count);

                foreach (var nodeBlock in itemSelectForm.CheckedItems)
                {
                    importedBlocks.AddRange(nodeBlock.TraverseParents(nodeBlocks));
                    importedBlocks.Add(nodeBlock);
                }

                // Borrowing the Module Export Utilities to try this...
                // And this works!
                var filePath = ModuleExportUtilities.SelectModuleExport<Stream>();
                File.WriteAllText(filePath, JsonSerializer.Serialize(importedBlocks.Distinct(), new JsonSerializerOptions{ WriteIndented = true }));
            }

        }, Keys.None, CustomHandlerFlags.ClearMementos | CustomHandlerFlags.Repopulate);

        AddCustomHandler("Replace ex data from JSON", () =>
        {
            //taking this straight from keikei's part for testing. thanks oomf
            string jsonFilePath = null;
            // open json
            using (var jsonFileDialog = new OpenFileDialog()
                   {
                       Title = "Select NodeBlock json file.",
                       Filter = "JSON files (*.json)|*.json|All files(*.*)|*.*",
                       FilterIndex = 0,
                       RestoreDirectory = true,
                   })
            {
                if (jsonFileDialog.ShowDialog() == DialogResult.OK)
                    jsonFilePath = jsonFileDialog.FileName;
            }

            try
            {
                var placeholderImportedBlocks = File.ReadAllText(jsonFilePath);
                List<NodeBlock> importedBlocks = JsonSerializer.Deserialize<List<NodeBlock>>(
                    placeholderImportedBlocks, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });
                Data.Blocks.Clear();
                Data.Blocks.AddRange(importedBlocks.Distinct());
            }
            catch (System.ArgumentNullException)
            {
                return;
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception.Message, Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OnPropertyChanged(nameof(Data.Blocks));
        }, Keys.None, CustomHandlerFlags.ClearMementos | CustomHandlerFlags.Repopulate);

    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<BoneInfo>("Bones", Data.Bones, x => x.Name));
    }

    protected override void SynchronizeCore()
    {
    }

    public SkinNode(string name, Skin data) : base(name, data)
    {
    }
}