using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Test.Pojo;

namespace Test
{
    class CharacterTbl
    {
        public CosBean[] cosList;
        public int cosLength;
        public int lastCosId = -1;
        public int lastCosIndex = -1;
        public DbgsetBean[] dbgsetList;
        public int dbgsetLength;
        public CharacterItemBean[] itemList;
        public int itemLength;
        public int lastItemNo = -1;
        public int lastItemIndex = -1;
        public CharacterTbl(string[] ori)
        {
            //寻找长度行
            foreach (string line in ori)
            {
                if (line.Contains("cos.length"))
                {
                    cosLength = Int32.Parse(StringCut.splitAfterEqual(line));
                    cosList = new CosBean[cosLength + 500];
                }
                if (line.Contains("dbgset.length"))
                {
                    dbgsetLength = Int32.Parse(StringCut.splitAfterEqual(line));
                    dbgsetList = new DbgsetBean[dbgsetLength + 500];
                }
                if (line.Contains("item.length"))
                {
                    itemLength = Int32.Parse(StringCut.splitAfterEqual(line));
                    itemList = new CharacterItemBean[itemLength + 500];
                }
            }
            foreach (string line in ori)
            {
                //注释
                if (line[0].Equals('#')) { continue; }
                //长度行
                if (line.Contains("cos.length")) { continue; }
                if (line.Contains("dbgset.length")) { continue; }
                if (line.Contains("item.length")&&!line.Contains("cos")&&!line.Contains("dbgset")) { continue; }
                switch (StringCut.splitPoint(line, 1))
                {
                    case "cos":
                        doCos(line);
                        break;
                    case "dbgset":
                        doDbgset(line);
                        break;
                    case "item":
                        doItem(line);
                        break;
                    default:
                        Console.WriteLine(line);
                        break;
                }
            }
            docol();
            if (cosLength < lastCosIndex + 1) cosLength = lastCosIndex + 1;
            if (itemLength < lastItemIndex + 1) itemLength = lastItemIndex + 1;
        }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            CosBean[] notNullCosList = (from str in cosList where str != null select str).ToArray();
            CosBean[] dicSortCos = (from objDic in notNullCosList
                                    orderby objDic.index.ToString()
                                    select objDic).ToArray();
            DbgsetBean[] notNullDbgsetList = (from str in dbgsetList where str != null select str).ToArray();
            DbgsetBean[] dicSortDbgset = (from objDic in notNullDbgsetList
                                          orderby objDic.index.ToString()
                                          select objDic).ToArray();
            CharacterItemBean[] notNullitemList = (from str in itemList where str != null select str).ToArray();
            CharacterItemBean[] dicSortItem = (from objDic in notNullitemList
                                               orderby objDic.index.ToString()
                                               select objDic).ToArray();
            foreach (CosBean c in dicSortCos)
                result.AddRange(c.toString());
            result.Add("cos.length=" + cosLength.ToString());
            foreach (DbgsetBean d in dicSortDbgset)
                result.AddRange(d.toString());
            result.Add("dbgset.length="+dbgsetLength.ToString());
            foreach (CharacterItemBean cib in dicSortItem)
                result.AddRange(cib.toString());
            result.Add("item.length=" + itemLength.ToString());
            return result; 
        }
        private void doItem(string line)
        {
            int index = Int32.Parse(StringCut.splitPoint(line, 2));
            if (itemList[index] == null) itemList[index] = new CharacterItemBean();
            itemList[index].index = index;
            if (index > lastItemIndex) lastItemIndex = index;
            String key = StringCut.splitBeforeEqual(line);
            String value = StringCut.splitAfterEqual(line);
            //等号前第二段
            String index1 = StringCut.splitPoint(StringCut.splitBeforeEqual(line, 2), 1);
            //处理col
            if (line.Contains(".col.")) 
            {
                if (key.Equals("length"))
                {
                    itemList[index].haveColLength = true;
                    itemList[index].colLength = Int32.Parse(value);
                    itemList[index].col = new ColBean[Int32.Parse(value)];
                    return;
                }
                itemList[index].haveCol = true;
                itemList[index].colOri.Add(line);
                return;
            }
            switch (key)
            {
                case "attr":
                    itemList[index].attr = Int32.Parse(value);
                    itemList[index].haveAttr = true;
                    break;
                case "rpk":
                    ObjRpkBean objrpk = new ObjRpkBean();
                    objrpk.index = Int32.Parse(index1);
                    objrpk.rpk =Int32.Parse(value);
                    itemList[index].dataObjRpk.Add(objrpk);
                    itemList[index].haveRpk = true;
                    break;
                case "uid":
                    ObjUidBean objuid = new ObjUidBean();
                    objuid.index = Int32.Parse(index1);
                    objuid.uid = value;
                    itemList[index].dataObjUid.Add(objuid);
                    itemList[index].haveUid = true;
                    break;
                case "length":
                    String lengthKey = StringCut.splitBeforeEqual(line, 2);
                    switch (lengthKey)
                    {
                        case "obj.length":
                            itemList[index].dataObjLength = Int32.Parse(value);
                            itemList[index].haveObjLength = true;
                            break;
                        case "tex.length":
                            itemList[index].dataTexLength = Int32.Parse(value);
                            itemList[index].haveTexLength = true;
                            break;
                        case "objset.length":
                            itemList[index].objsetLength = Int32.Parse(value);
                            itemList[index].haveObjsetLength = true;
                            break;
                        default:
                            Console.WriteLine(line);
                            break;
                    }
                    break;
                case "chg":
                    TexChgBean texchg = new TexChgBean();
                    texchg.index = Int32.Parse(index1);
                    texchg.chg = value;
                    itemList[index].dataTexChg.Add(texchg);
                    itemList[index].haveTexChg = true;
                    break;
                case "org":
                    TexOrgBean texorg = new TexOrgBean();
                    texorg.index = Int32.Parse(index1);
                    texorg.org = value;
                    itemList[index].dataTexOrg.Add(texorg);
                    itemList[index].haveTexOrg = true;
                    break;
                case "des_id":
                    itemList[index].des_id = Int32.Parse(value);
                    itemList[index].haveDes_id = true;
                    break;
                case "exclusion":
                    itemList[index].exclusion = Int32.Parse(value);
                    itemList[index].haveExclusion = true;
                    break;
                case "face_depth":
                    itemList[index].face_depth = double.Parse(value);
                    itemList[index].haveFace_depth = true;
                    break;
                case "flag":
                    itemList[index].flag = Int32.Parse(value); 
                    itemList[index].haveFlag = true;
                    break;
                case "name":
                    itemList[index].name = value;
                    itemList[index].haveName = true;
                    break;
                case "no":
                    itemList[index].no = Int32.Parse(value);
                    itemList[index].haveNo = true;
                    if (itemList[index].no > lastItemNo) lastItemNo = itemList[index].no;
                    break;
                case "org_itm":
                    itemList[index].org_itm = Int32.Parse(value);
                    itemList[index].haveOrg_itm = true;
                    break;
                case "point":
                    itemList[index].point = Int32.Parse(value);
                    itemList[index].havePoint = true;
                    break;
                case "sub_id":
                    itemList[index].sub_id = Int32.Parse(value);
                    itemList[index].haveSub_id = true;
                    break;
                case "type":
                    itemList[index].type = Int32.Parse(value);
                    itemList[index].haveType = true;
                    break;
                case "npr_flag":
                    itemList[index].npr_flag = Int32.Parse(value);
                    itemList[index].haveNpr_flag = true;
                    break;
                default:
                    //数字
                    if(index1 == "objset")
                    {
                        ObjsetBean objset = new ObjsetBean();
                        //格式和别的行不同
                        objset.index = Int32.Parse(key);
                        objset.objset = value;
                        itemList[index].objset.Add(objset);
                        itemList[index].haveObjset = true;
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                    break;
            }
        }

        public void addCos(CosBean newCos)
        {
            newCos.index = lastCosIndex + 1;
            if (cosList[newCos.index] != null) throw new Exception("ModuleIndexUsed");
            else cosList[newCos.index] = new CosBean();
            cosList[newCos.index] = newCos;
            cosLength++;
            if (newCos.index > lastCosIndex) lastCosIndex = newCos.index;
            if (newCos.id > lastCosId) lastCosId = newCos.id;
        }

        private void doDbgset(string line)
        {
            int index = Int32.Parse(StringCut.splitPoint(line, 2));
            if (dbgsetList[index] == null) dbgsetList[index] = new DbgsetBean();
            dbgsetList[index].index = index;
            String key = StringCut.splitBeforeEqual(line);
            String value = StringCut.splitAfterEqual(line);
            switch (key)
            {
                case "name":
                    dbgsetList[index].name = value;
                    dbgsetList[index].haveName = true;
                    break;
                case "length":
                    dbgsetList[index].length = Int32.Parse(value);
                    dbgsetList[index].haveLength = true;
                    break;
                default:
                    dbgsetList[index].haveItem = true;
                    ItemBean item = new ItemBean();
                    item.index = Int32.Parse(key);
                    item.item = value;
                    dbgsetList[index].item.Add(item);
                    break;
            }
        }

        private void doCos(string line)
        {
            int index = Int32.Parse(StringCut.splitPoint(line, 2));
            if (cosList[index] == null) cosList[index] = new CosBean();
            cosList[index].index = index;
            if (index > lastCosIndex) lastCosIndex = index;
            String key = StringCut.splitBeforeEqual(line);
            String value = StringCut.splitAfterEqual(line);
            switch (key)
            {
                case "id":
                    cosList[index].haveId = true;
                    cosList[index].id = Int32.Parse(value);
                    if (cosList[index].id > lastCosId) lastCosId = cosList[index].id;
                    break;
                case "length":
                    cosList[index].haveLength = true;
                    cosList[index].length = Int32.Parse(value);
                    break;
                default:
                    cosList[index].haveItem = true;
                    ItemBean item = new ItemBean();
                    item.index = Int32.Parse(key);
                    item.item = value;
                    cosList[index].item.Add(item);
                    break;
            }
        }
        private void docol()
        {
            foreach(CharacterItemBean cib in itemList) 
                if ((cib != null)&&(cib.haveCol))
                {
                    foreach(String line in cib.colOri)
                    {
                        String key = StringCut.splitBeforeEqual(line);
                        String value = StringCut.splitAfterEqual(line);
                        //等号前第二段
                        String index1 =StringCut.splitPoint(StringCut.splitBeforeEqual(line, 2), 1);
                        String index2 = "";
                        if (index1.Equals("blend") || (index1.Equals("offset")))
                        {
                            index2 = key;
                            key = index1;
                            index1 = StringCut.splitPoint(StringCut.splitBeforeEqual(line, 3), 1);
                        }
                        int index = Int32.Parse(index1);
                        if (cib.col[index] == null) cib.col[index] = new ColBean();
                        cib.col[index].index = index;
                        switch (key)
                        {
                            case "contrast":
                                cib.col[index].contrast = double.Parse(value);
                                break;
                            case "flag":
                                cib.col[index].flag = Int32.Parse(value);
                                break;
                            case "hue":
                                cib.col[index].hue = Int32.Parse(value);
                                break;
                            case "inverse":
                                cib.col[index].inverse = Int32.Parse(value);
                                break;
                            case "saturation":
                                cib.col[index].saturation = double.Parse(value);
                                break;
                            case "tex":
                                cib.col[index].tex = value;
                                break;
                            case "value":
                                cib.col[index].value = double.Parse(value);
                                break;
                            case "blend":
                                switch (index2)
                                {
                                    case "0":
                                        cib.col[index].blend0 = double.Parse(value);
                                        break;
                                    case "1":
                                        cib.col[index].blend1 = double.Parse(value);
                                        break;
                                    case "2":
                                        cib.col[index].blend2 = double.Parse(value);
                                        break;
                                    default:
                                        Console.WriteLine(line);
                                        break;
                                }
                                break;
                            case "offset":
                                switch (index2)
                                {
                                    case "0":
                                        cib.col[index].offset0 = Double.Parse(value);
                                        break;
                                    case "1":
                                        cib.col[index].offset1 = Double.Parse(value);
                                        break;
                                    case "2":
                                        cib.col[index].offset2 = Double.Parse(value);
                                        break;
                                    default:
                                        Console.WriteLine(line);
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine(line);
                                break;
                        }
                    }
                }
            
        }
        public CosBean findCosById(int id)
        {
            foreach (CosBean c in cosList)
                if (c.id == id) return c;
            return new CosBean();
        }
        public List<CosBean> findCosByItemId(int id)
        {
            List<CosBean> result = new List<CosBean>(); 
            foreach (CosBean c in cosList)
                if(c != null)
                    foreach(ItemBean i in c.item)
                      if (Int32.Parse(i.item) == id) result.Add(c);
            return result;
        }
        public int findBodyNo(CosBean cos)
        {
            foreach (ItemBean i in cos.item)
                foreach(CharacterItemBean c in itemList)
                    if(c.no == Int32.Parse(i.item))
                    {
                        if (c.type == 1) return c.no;
                        break;
                    }
            throw new Exception("BodyNotFound");
        }
        public CharacterItemBean findItemByNo(int no)
        {
            foreach (CharacterItemBean ci in itemList)
                if (ci.no == no) return ci;
            throw new Exception("ItemNoNotFound");
        }
        public void addItem(CharacterItemBean newItem)
        {
            newItem.index = lastItemIndex + 1;
            if (itemList[newItem.index] != null) throw new Exception("ModuleIndexUsed");
            else itemList[newItem.index] = new CharacterItemBean();
            itemList[newItem.index] = newItem;
            itemLength++;
            if (newItem.index > lastItemIndex) lastItemIndex = newItem.index;
            if (newItem.no > lastItemNo) lastItemNo = newItem.no;
        }
    }
}
