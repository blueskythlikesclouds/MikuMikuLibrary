using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Chritm
{
    public class CostumeItemObject
    {
        public uint ObjectID { get; set; }
        public int RPK { get; set; }

        internal void Read(EndianBinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            RPK = reader.ReadInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(RPK);
        }
    }
    public class CommonItemAdjust
    {
        public int Unk01 { get; set; }
        public int Unk02 { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }

        internal void Read(EndianBinaryReader reader)
        {
            Unk01 = reader.ReadInt32();
            Unk02 = reader.ReadInt32();
            PosX = reader.ReadSingle();
            PosY = reader.ReadSingle();
            PosZ = reader.ReadSingle();
            RotX = reader.ReadSingle();
            RotY = reader.ReadSingle();
            RotZ = reader.ReadSingle();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
            ScaleZ = reader.ReadSingle();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(Unk01);
            writer.Write(Unk02);
            writer.Write(PosX);
            writer.Write(PosY);
            writer.Write(PosZ);
            writer.Write(RotX);
            writer.Write(RotY);
            writer.Write(RotZ);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(ScaleZ);
        }
    }

    public class TextureChange
    {
        public uint OriginalTextureID { get; set; }
        public uint NewTextureID { get; set; }

        internal void Read(EndianBinaryReader reader)
        {
            OriginalTextureID = reader.ReadUInt32();
            NewTextureID = reader.ReadUInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(OriginalTextureID);
            writer.Write(NewTextureID);
        }
    }

    public class CostumeItem
    {
        public List<CostumeItemObject> Objects { get; }
        public List<CommonItemAdjust> CommonItemAdjustSettings { get; }
        public List<TextureChange> TextureChangeSettings { get; }
        public int Unk01 { get; set; }
        public int Unk02 { get; set; }
        public int Flag { get; set; }
        public string Name { get; set; }
        public List<uint> ObjectSetIDs { get; }
        public int Type { get; set; }
        public int Attribute { get; set; }
        public int ItemNumber { get; set; }
        public int DestinationID { get; set; }
        public int SubID { get; set; }
        public int Unk03 { get; set; }
        public int Unk04 { get; set; }
        public int OriginalItem { get; set; }

        internal void Read(EndianBinaryReader reader)
        {
            int ObjectCount = reader.ReadInt32();
            int ObjectOffset = reader.ReadInt32();
            reader.ReadAtOffset(ObjectOffset, () =>
            {
                for (int i = 0; i < ObjectCount; i++)
                {
                    var obj = new CostumeItemObject();
                    obj.Read(reader);
                    Objects.Add(obj);
                }
            });
            int CmnitmAdjustCount = reader.ReadInt32();
            int CmnitmAdjustOffset = reader.ReadInt32();
            reader.ReadAtOffset(CmnitmAdjustOffset, () =>
            {
                for (int i = 0; i < CmnitmAdjustCount; i++)
                {
                    var Adjust = new CommonItemAdjust();
                    Adjust.Read(reader);
                    CommonItemAdjustSettings.Add(Adjust);
                }
            });
            int TextureChangeCount = reader.ReadInt32();
            int TextureChangeOffset = reader.ReadInt32();
            reader.ReadAtOffset(TextureChangeOffset, () =>
            {
                for (int i = 0; i < TextureChangeCount; i++)
                {
                    var TexChange = new TextureChange();
                    TexChange.Read(reader);
                    TextureChangeSettings.Add(TexChange);
                }
            });
            Unk01 = reader.ReadInt32();
            Unk02 = reader.ReadInt32();
            Flag = reader.ReadInt32();
            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            int ObjectSetCount = reader.ReadInt32();
            int ObjectSetOffset = reader.ReadInt32();
            reader.ReadAtOffset(ObjectSetOffset, () =>
            {
                for (int i = 0; i < ObjectSetCount; i++)
                {
                    ObjectSetIDs.Add(reader.ReadUInt32());
                }
            });
            Type = reader.ReadInt32();
            Attribute = reader.ReadInt32();
            ItemNumber = reader.ReadInt32();
            DestinationID = reader.ReadInt32();
            SubID = reader.ReadInt32();
            Unk03 = reader.ReadInt32();
            Unk04 = reader.ReadInt32();
            OriginalItem = reader.ReadInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(Objects.Count);
            writer.ScheduleWriteOffsetIf(Objects.Count != 0, 4, AlignmentMode.Left, () =>
            {
                foreach (var obj in Objects)
                {
                    obj.Write(writer);
                }
            });
            writer.Write(CommonItemAdjustSettings.Count);
            writer.ScheduleWriteOffsetIf(CommonItemAdjustSettings.Count != 0, 4, AlignmentMode.Left, () =>
            {
                foreach (var Adjust in CommonItemAdjustSettings)
                {
                    Adjust.Write(writer);
                }
            });
            writer.Write(TextureChangeSettings.Count);
            writer.ScheduleWriteOffsetIf(TextureChangeSettings.Count != 0, 4, AlignmentMode.Left, () =>
            {
                foreach (var TexChange in TextureChangeSettings)
                {
                    TexChange.Write(writer);
                }
            });
            writer.Write(Unk01);
            writer.Write(Unk02);
            writer.Write(Flag);
            writer.AddStringToStringTable(Name);
            writer.Write(ObjectSetIDs.Count);
            writer.ScheduleWriteOffsetIf(ObjectSetIDs.Count != 0, 4, AlignmentMode.Left, () =>
            {
                foreach (var ObjectSetID in ObjectSetIDs)
                {
                    writer.Write(ObjectSetID);
                }
            });
            writer.Write(Type);
            writer.Write(Attribute);
            writer.Write(ItemNumber);
            writer.Write(DestinationID);
            writer.Write(SubID);
            writer.Write(Unk03);
            writer.Write(Unk04);
            writer.Write(OriginalItem);
        }

        public CostumeItem()
        {
            Objects = new List<CostumeItemObject>();
            CommonItemAdjustSettings = new List<CommonItemAdjust>();
            TextureChangeSettings = new List<TextureChange>();
            ObjectSetIDs = new List<uint>();
        }
    }
}
