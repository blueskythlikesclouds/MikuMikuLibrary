﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.CharacterItem
{
    public class Costume
    {
        public int CostumeID { get; set; }
        public List<int> Parts { get; }

        internal void Read(EndianBinaryReader reader)
        {
            CostumeID = reader.ReadInt32();
            for (int i = 0; i < 25; i++)
            {
                Parts.Add(reader.ReadInt32());
            }
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(CostumeID);
            for (int i = 0; i < 25; i++)
            {
                writer.Write(Parts[i]);
            }
        }

        public Costume()
        {
            Parts = new List<int>();
        }
    }
}
