using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.Databases;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Textures;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Sprites;

public class SpriteSetNode : BinaryFileNode<SpriteSet>
{
    private TextureSetNode mTextureSetNode;

    public override NodeFlags Flags =>
        NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

    protected override void Initialize()
    {
        AddReplaceHandler<SpriteSet>(BinaryFile.Load<SpriteSet>);
        AddExportHandler<SpriteSet>(filePath => Data.Save(filePath));

        AddDirtyCustomHandler("Add dummy sprite", () =>
        {
            Data.Sprites.Add(new Sprite { Name = "DUMMY", ResolutionMode = ResolutionMode.HDTV720 });
            return true;
        }, Keys.None, CustomHandlerFlags.Repopulate);

        AddCustomHandlerSeparator();

        AddCustomHandler("Export All", () =>
        {
            using (var folderBrowseDialog = new VistaFolderBrowserDialog())
            {
                folderBrowseDialog.Description = "Select a folder to save sprites to.";
                folderBrowseDialog.UseDescriptionForTitle = true;

                if (folderBrowseDialog.ShowDialog() != DialogResult.OK)
                    return;

                foreach (var pair in SpriteCropper.Crop(Data))
                    pair.Value.Save(Path.Combine(folderBrowseDialog.SelectedPath, $"{pair.Key.Name}.png"));
            }
        }, Keys.Control | Keys.Shift | Keys.E);

        AddCustomHandlerSeparator();

        AddDirtyCustomHandler("Scale all sprites", () =>
        {
            string input = "2.0";

            while (true)
            {
                using (var inputDialog = new InputDialog { WindowTitle = "Scale all sprites", Input = input })
                {
                    if (inputDialog.ShowDialog() != DialogResult.OK)
                        break;

                    if (float.TryParse(inputDialog.Input,
                            NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                            out float factor))
                    {
                        foreach (var sprite in Data.Sprites)
                        {
                            sprite.Width *= factor;
                            sprite.Height *= factor;
                            sprite.X *= factor;
                            sprite.Y *= factor;
                        }

                        return true;
                    }
                    else
                        MessageBox.Show("Invalid factor.", Program.Name, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                    input = inputDialog.Input;
                }
            }

            return false;
        });

        AddDirtyCustomHandler("Set all resolution modes to...", () =>
        {
            string input = "HDTV720";

            while (true)
            {
                using (var inputDialog = new InputDialog { WindowTitle = "Set all resolution modes", Input = input })
                {
                    if (inputDialog.ShowDialog() != DialogResult.OK)
                        break;

                    if (!Enum.TryParse(input = inputDialog.Input, true, out ResolutionMode mode))
                    {
                        MessageBox.Show("Please enter a valid resolution mode.", Program.Name,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        continue;
                    }

                    foreach (var sprite in Data.Sprites)
                        sprite.ResolutionMode = mode;

                    return true;
                }
            }

            return false;
        });

        AddCustomHandlerSeparator();

        AddDirtyCustomHandler("Compute sprite rectangles", () =>
        {
            foreach (var sprite in Data.Sprites)
            {
                var texture = Data.TextureSet.Textures[(int)sprite.TextureIndex];

                sprite.RectangleBegin = new Vector2(
                    sprite.X / texture.Width,
                    sprite.Y / texture.Height);

                sprite.RectangleEnd = new Vector2(
                    (sprite.X + sprite.Width) / texture.Width,
                    (sprite.Y + sprite.Height) / texture.Height);
            }

            return true;
        });

        base.Initialize();
    }

    protected override void PopulateCore()
    {
        if (Parent != null && Name.EndsWith(".spr", StringComparison.OrdinalIgnoreCase))
        {
            var spriteDatabaseNode =
                Parent.FindNode<SpriteDatabaseNode>(Path.ChangeExtension(Name, "spi"));

            if (spriteDatabaseNode != null)
            {
                var spriteSetInfo = spriteDatabaseNode.Data.SpriteSets[0];

                string suffix = spriteSetInfo.Name.ToUpperInvariant() + "_";
                string suffixTex = suffix.Replace("SPR_", "SPRTEX_");

                foreach (var spriteInfo in spriteSetInfo.Sprites)
                {
                    string name = spriteInfo.Name;

                    if (name.StartsWith(suffix, StringComparison.OrdinalIgnoreCase))
                        name = name.Substring(suffix.Length);

                    if (spriteInfo.Index < Data.Sprites.Count)
                        Data.Sprites[spriteInfo.Index].Name = name;
                }

                foreach (var textureInfo in spriteSetInfo.Textures)
                {
                    string name = textureInfo.Name;

                    if (name.StartsWith(suffixTex, StringComparison.OrdinalIgnoreCase))
                        name = name.Substring(suffixTex.Length);

                    if (textureInfo.Index < Data.TextureSet.Textures.Count)
                        Data.TextureSet.Textures[textureInfo.Index].Name = name;
                }
            }
        }

        Nodes.Add(new ListNode<Sprite>("Sprites", Data.Sprites, x => x.Name));
        Nodes.Add(mTextureSetNode = new TextureSetNode("Texture Set", Data.TextureSet));
    }

    protected override void SynchronizeCore()
    {
        if (!IsPopulated || mTextureSetNode == null || Data.TextureSet == mTextureSetNode.Data)
            return;

        Data.TextureSet.Textures.Clear();
        Data.TextureSet.Textures.AddRange(mTextureSetNode.Data.Textures);
    }

    public SpriteSetNode(string name, SpriteSet data) : base(name, data)
    {
    }

    public SpriteSetNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
    {
    }
}