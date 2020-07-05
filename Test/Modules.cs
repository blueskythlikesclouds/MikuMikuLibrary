using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Modules
    {
        public ModuleBean[] moduleList;
        public int length;
        public int lastIndex = -1;
        public int lastModuleId = -1;
        public int lastSortIndex = -1;
        public Modules(string[] ori) {
            //寻找长度行
            foreach (string line in ori.Reverse())
            {
                if (line.Contains("data_list.length")) {
                    length = Int32.Parse(StringCut.splitAfterEqual(line));
                    moduleList = new ModuleBean[length+500];
                    break;
                }
            }
            foreach (string line in ori)
            {
                //读取文件
                //注释
                if (line[0].Equals('#')) { continue; }
                //最后一行
                if (line.Contains("data_list.length")) { break;}
                string[] linearr = new string[4];
                //切分
                linearr = linecut(line);
                int index = Int32.Parse(linearr[1]);
                String key = linearr[2];
                String value = linearr[3];
                if (moduleList[index] == null) { moduleList[index] = new ModuleBean(); }
                moduleList[index].index = index;
                if (index > lastIndex) lastIndex = index;
                switch (key)
                {
                    case "attr":
                        moduleList[index].attr = Int32.Parse(value);
                        break;
                    case "chara":
                        moduleList[index].chara = value;
                        break;
                    case "cos":
                        moduleList[index].cos = value;
                        break;
                    case "id":
                        moduleList[index].id = Int32.Parse(value);
                        if (moduleList[index].id > lastModuleId) lastModuleId = moduleList[index].id;
                        break;
                    case "name":
                        moduleList[index].name = value;
                        break;
                    case "ng":
                        moduleList[index].ng = Int32.Parse(value);
                        break;
                    case "shop_ed_day":
                        moduleList[index].shop_ed_day = Int32.Parse(value);
                        break;
                    case "shop_ed_month":
                        moduleList[index].shop_ed_month = Int32.Parse(value);
                        break;
                    case "shop_ed_year":
                        moduleList[index].shop_ed_year = Int32.Parse(value);
                        break;
                    case "shop_price":
                        moduleList[index].shop_price = Int32.Parse(value);
                        break;
                    case "shop_st_day":
                        moduleList[index].shop_st_day = Int32.Parse(value);
                        break;
                    case "shop_st_month":
                        moduleList[index].shop_st_month = Int32.Parse(value);
                        break;
                    case "shop_st_year":
                        moduleList[index].shop_st_year = Int32.Parse(value);
                        break;
                    case "sort_index":
                        moduleList[index].sort_index = Int32.Parse(value);
                        if (moduleList[index].sort_index > lastSortIndex) lastSortIndex = moduleList[index].sort_index;
                        break;
                    default:
                        Console.WriteLine(value);
                        break;
                }
            }
            if ((lastIndex + 1) > length) length = lastIndex + 1;
        }
        private string[] linecut(string line)
        {
            string[] linearr = new string[4];
            linearr[0] = StringCut.splitPoint(line, 1);
            linearr[1] = StringCut.splitPoint(line, 2);
            linearr[2] = StringCut.splitBeforeEqual(line);
            linearr[3] = StringCut.splitAfterEqual(line);
            return linearr;
        }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            for(int i = 1; i <= 400; i++)
            {
                result.Add("#---------------------------------------------");
            }
            ModuleBean[] notNullModuleList = (from str in moduleList where str != null select str).ToArray();
            ModuleBean[] dicSortModule = (from objDic in notNullModuleList
                                          orderby objDic.index.ToString()
                                          select objDic).ToArray();
            foreach (ModuleBean m in dicSortModule)
                result.AddRange(m.toString());
            result.Add("module.data_list.length=" + length.ToString());
            return result;
        }

        public void add(ModuleBean mb)
        {
            mb.index = lastIndex + 1;
            if (moduleList[mb.index] != null) throw new Exception("ModuleIndexUsed");
            else moduleList[mb.index] = new ModuleBean();
            moduleList[mb.index] = mb;
            length++;
            if (mb.index > lastIndex) lastIndex = mb.index;
            if (mb.id > lastModuleId) lastModuleId = mb.id;
            if (mb.sort_index > lastSortIndex) lastSortIndex = mb.sort_index;
        }

        public ModuleBean findModuleByCosId(String charactor,int id)
        {
            foreach (ModuleBean m in moduleList)
                if(m.chara.Equals(charactor.ToUpper()))
                    if (StringCut.cosString2Id(m.cos) == id) return m;
            throw new Exception("FindNoModuleWithCosId");
        }

        public ModuleBean findModuleById(int moduleId)
        {
            foreach (ModuleBean m in moduleList)
                if (m.id == moduleId) return m;
            throw new Exception("ModuleIdNotFound");
        }
    }
}
