﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Chritm
{
    public class ChritmSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public override Encoding Encoding { get; } =
            Encoding.GetEncoding("utf-8");

        public List<Item> Items { get; }
        public List<Cos> Costumes { get; }
        public List<Dbg> DebugSets { get; }

        public override void Read(EndianBinaryReader reader, ISection section = null)
        {
            int Unk = reader.ReadInt32();
            int ItemCount = reader.ReadInt32();
            int ItemOffset = reader.ReadInt32();
            int CosCount = reader.ReadInt32();
            int CosOffset = reader.ReadInt32();
            int DbgCount = reader.ReadInt32();
            int DbgOffset = reader.ReadInt32();
            reader.ReadAtOffset(ItemOffset, () =>
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    var item = new Item();
                    item.Read(reader);
                    Items.Add(item);
                }
            });
            reader.ReadAtOffset(CosOffset, () =>
            {
                for (int i = 0; i < CosCount; i++)
                {
                    var cos = new Cos();
                    cos.Read(reader);
                    Costumes.Add(cos);
                }
            });
            reader.ReadAtOffset(DbgOffset, () =>
            {
                for (int i = 0; i < DbgCount; i++)
                {
                    var dbg = new Dbg();
                    dbg.Read(reader);
                    DebugSets.Add(dbg);
                }
            });
        }

        public override void Write(EndianBinaryWriter writer, ISection section = null)
        {
            writer.WriteNulls(4);
            writer.Write(Items.Count);
            writer.ScheduleWriteOffsetIf(Items.Count != 0, 16, AlignmentMode.Left, () =>
            {
                foreach (var item in Items)
                {
                    item.Write(writer);
                }
            });
            writer.Write(Costumes.Count);
            writer.ScheduleWriteOffsetIf(Costumes.Count != 0, 16, AlignmentMode.Left, () =>
            {
                foreach (var cos in Costumes)
                {
                    cos.Write(writer);
                }
            });
            writer.Write(DebugSets.Count);
            writer.ScheduleWriteOffsetIf(DebugSets.Count != 0, 16, AlignmentMode.Left, () =>
            {
                foreach (var dbg in DebugSets)
                {
                    dbg.Write(writer);
                }
            });
            writer.WriteNulls(4);
        }

        public ChritmSet()
        {
            Items = new List<Item>();
            Costumes = new List<Cos>();
            DebugSets = new List<Dbg>();
        }
    }
}
