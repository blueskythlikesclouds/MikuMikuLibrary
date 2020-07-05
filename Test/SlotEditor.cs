using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Test.Pojo;

namespace Test
{
    class SlotEditor
    {       
        Modules modules;
        CustomizeItems cstmItems;
        CharacterTbl hakTbl;
        CharacterTbl kaiTbl;
        CharacterTbl lenTbl;
        CharacterTbl lukTbl;
        CharacterTbl meiTbl;
        CharacterTbl mikTbl;
        CharacterTbl nerTbl;
        CharacterTbl rinTbl;
        CharacterTbl sakTbl;
        CharacterTbl tetTbl;
        ObjDb objdb;
        SprDb sprdb;
        public SlotEditor()
        {
            string[] moduleid = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_gm_module_tbl\gm_module_id.bin");
            string[] itemid = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_gm_customize_item_tbl\gm_customize_item_id.bin");
            string[] hak = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\hakitm_tbl.txt");
            string[] kai = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\kaiitm_tbl.txt");
            string[] len = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\lenitm_tbl.txt");
            string[] luk = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\lukitm_tbl.txt");
            string[] mei = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\meiitm_tbl.txt");
            string[] mik = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\mikitm_tbl.txt");
            string[] ner = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\neritm_tbl.txt");
            string[] rin = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\rinitm_tbl.txt");
            string[] sak = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\sakitm_tbl.txt");
            string[] tet = System.IO.File.ReadAllLines(@"C:\MZZZ\rom\mdata_chritm_prop\tetitm_tbl.txt");


            objdb = new ObjDb(@"C:\MZZZ\rom\objset\mdata_obj_db.xml");
            sprdb = new SprDb(@"C:\MZZZ\rom\2d\mdata_spr_db.xml");

            modules = new Modules(moduleid);
            cstmItems = new CustomizeItems(itemid);
            hakTbl = new CharacterTbl(hak);
            kaiTbl = new CharacterTbl(kai);
            lenTbl = new CharacterTbl(len);
            lukTbl = new CharacterTbl(luk);
            meiTbl = new CharacterTbl(mei);
            mikTbl = new CharacterTbl(mik);
            nerTbl = new CharacterTbl(ner);
            rinTbl = new CharacterTbl(rin);
            sakTbl = new CharacterTbl(sak);
            tetTbl = new CharacterTbl(tet);


        }
        public void output(String str)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@str+@"MZZZ\rom\objset");
            di.Create();
            di = new System.IO.DirectoryInfo(@str + @"MZZZ\rom\mdata_gm_customize_item_id");
            di.Create();
            di = new System.IO.DirectoryInfo(@str + @"MZZZ\rom\mdata_chritm_prop");
            di.Create();
            di = new System.IO.DirectoryInfo(@str + @"MZZZ\rom\mdata_gm_module_tbl");
            di.Create();
            di = new System.IO.DirectoryInfo(@str + @"MZZZ\rom\2d");
            di.Create();
            di = new System.IO.DirectoryInfo(@str + @"MZZZ\rom\skin_param");
            di.Create();
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_gm_module_tbl\gm_module_id.bin", modules.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_gm_customize_item_id\gm_customize_item_id.bin", cstmItems.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\mikitm_tbl.txt", mikTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\hakitm_tbl.txt", hakTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\kaiitm_tbl.txt", kaiTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\lenitm_tbl.txt", lenTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\mikitm_tbl.txt", mikTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\lukitm_tbl.txt", lukTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\meiitm_tbl.txt", meiTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\neritm_tbl.txt", nerTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\rinitm_tbl.txt", rinTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\sakitm_tbl.txt", sakTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\mdata_chritm_prop\tetitm_tbl.txt", tetTbl.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\objset\mdata_obj_db.xml", objdb.toString(), Encoding.UTF8);
            System.IO.File.WriteAllLines(@str + @"MZZZ\rom\2d\mdata_spr_db.xml", sprdb.toString(), Encoding.UTF8);
        }
        public void addBaseModuleWithNewBody(String charactor,String name)
        {
            copyModuleWithNewBody(charactor, 1,name);
        }
        public void copyModuleWithNewBody(int moduleId,String name)
        {
            ModuleBean mb = new ModuleBean(modules.findModuleById(moduleId));
            CharacterTbl chaTbl = findCharactor(mb.chara);
            //更新module
            mb.id = modules.lastModuleId + 1;
            mb.sort_index = modules.lastSortIndex + 1;
            if (name.Equals("")) mb.name = mb.name + " NEW";
            else mb.name =mb.name + name;
            int oriCosId =StringCut.cosString2Id(mb.cos);
            mb.cos = StringCut.cosId2String(chaTbl.lastCosId + 1);
            modules.add(mb);
            //更新cos
            CosBean newCos = new CosBean(chaTbl.findCosById(oriCosId));
            int bodyno = chaTbl.findBodyNo(newCos);
            newCos.id = chaTbl.lastCosId + 1;
            foreach (ItemBean i in newCos.item)
                if (Int32.Parse(i.item) == bodyno) i.item = (chaTbl.lastItemNo + 1).ToString();
            chaTbl.addCos(newCos);
            //新增身体
            copyItemByNo(mb.chara,bodyno);
        }
        public void copyModuleWithNewBody(String charactor,int bodyId,String name)
        {
            CharacterTbl chaTbl = findCharactor(charactor);
            List<CosBean> coslist = chaTbl.findCosByItemId(bodyId);
            if (coslist.Count == 0) throw new Exception("BodyNotFound");
            ModuleBean mb = modules.findModuleByCosId(charactor,coslist[0].id + 1);
            copyModuleWithNewBody(mb.id,name);
        }
        public void copyModuleWithNewBody(String farcName)
        {

        }
        public void copyItemByNo(String charactor,int no)
            //复制角色零件
        {
            //复制item
            CharacterTbl chaTbl = findCharactor(charactor);
            CharacterItemBean newitem = new CharacterItemBean(chaTbl.findItemByNo(no));
            if (newitem.attr != 1) throw new Exception("CanNotCopyTextureReplacedItem");
            newitem.name = newitem.name + " NEW";
            newitem.objset[0].objset = objdb.getCharacterNameUpper(charactor, 1);
            newitem.dataObjUid[0].uid = objdb.getCharacterNameUpper(charactor, 1) 
                + newitem.dataObjUid[0].uid.Substring(newitem.dataObjUid[0].uid.IndexOf('_'));
            newitem.no = chaTbl.lastItemNo + 1;
            chaTbl.addItem(newitem);
            //复制objdb
            objdb.add(newitem);
        }
        public void copyCustomizeItemByNo(int no)
            //复制配件
        {

        }
        private CharacterTbl findCharactor(String charactor)
        {
            switch (charactor.ToLower())
            {
                case "miku":
                    return mikTbl;
                case "luka":
                    return lukTbl;
                case "meiko":
                    return meiTbl;
                case "rin":
                    return rinTbl;
                case "sakine":
                    return sakTbl;
                case "haku":
                    return hakTbl;
                case "teto":
                    return tetTbl;
                case "neru":
                    return nerTbl;
                case "kaito":
                    return kaiTbl;
                case "len":
                    return lenTbl;
                default:
                    throw new Exception("NonStandardCharactorName");
            }
        }
        private String shortName(String charactor)
        {
            return charactor.Substring(0, 3).ToLower();
        }
    }
}
