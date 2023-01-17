using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Objects.Extra;
using MikuMikuLibrary.Objects.Extra.Blocks;
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