using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Test
{
    class SprDb
    {
        public XElement spr;
        public IEnumerable<XElement> sprList;
        public int lastId = -1;
        public int lastId1 = -1;
        public SprDb(String str)
        {
            spr = XElement.Load(@str);
            this.sprList = from el in spr.Element("SpriteSets").Elements("SpriteSetEntry")
                           select el;
            foreach (XElement x in sprList)
            {
                if (Int32.Parse(x.Element("Id").Value) > lastId) lastId = Int32.Parse(x.Element("Id").Value);
                if (Int32.Parse(x.Element("Sprites").Element("SpriteEntry").Element("Id").Value) > lastId1)
                    lastId1 = Int32.Parse(x.Element("Sprites").Element("SpriteEntry").Element("Id").Value);
                if (Int32.Parse(x.Element("Textures").Element("SpriteTextureEntry").Element("Id").Value) > lastId1)
                    lastId1 = Int32.Parse(x.Element("Textures").Element("SpriteTextureEntry").Element("Id").Value);
            }
        }
        public void add(int id,String name,String fileName,int spriteEntryId,String spriteEntryName,int spriteTextureEntryId,String spriteTextureEntryName)
        {
            sprList.Last().AddAfterSelf(new XElement("SpriteSetEntry",
                                            new XElement("Id", id),
                                            new XElement("Name", name),
                                            new XElement("FileName", fileName),
                                            new XElement("Sprites",
                                                new XElement("SpriteEntry",
                                                    new XElement("Id", spriteEntryId),
                                                    new XElement("Name", spriteEntryName),
                                                    new XElement("Index", 0)
                                                            )
                                                        ),
                                            new XElement("Textures",
                                                new XElement("SpriteTextureEntry",
                                                    new XElement("Id", spriteTextureEntryId),
                                                    new XElement("Name", spriteTextureEntryName),
                                                    new XElement("Index", 0)
                                                            )
                                                        )
                                                    )
                                       );
            if (id > lastId) lastId = id;
            if (spriteEntryId > lastId1) lastId1 = spriteEntryId;
            if (spriteTextureEntryId > lastId1) lastId1 = spriteTextureEntryId;
        }
        public List<String> toString()
        {
            return new List<string>() { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", spr.ToString() };
        }
    }
}
