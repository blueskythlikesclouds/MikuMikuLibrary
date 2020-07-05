using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using Test.Pojo;

namespace Test
{
    class CustomizeItems
    {
        public CustomizeItemBean[] itemList;
        public int length;
        public int patch;
        public int version;
        public CustomizeItems(String[] ori)
        {
            //寻找长度行
            int flag = 0;
            foreach (string line in ori.Reverse())
            {
                switch (StringCut.splitBeforeEqual(line))
                {
                    case "version":
                        version =Int32.Parse(StringCut.splitAfterEqual(line));
                        flag++;
                        break;
                    case "patch":
                        patch = Int32.Parse(StringCut.splitAfterEqual(line));
                        flag++;
                        break;
                    case "length":
                        length = Int32.Parse(StringCut.splitAfterEqual(line));
                        itemList = new CustomizeItemBean[length + 500];
                        flag++;
                        break;
                }
                if (flag == 3) break;
            }
            foreach (string line in ori)
            {
                //读取文件
                //注释
                if (line[0].Equals('#')) { continue; }
                //最后一行
                if (line.Contains("data_list.length")) { break; }
                //切分
                int index = Int32.Parse(StringCut.splitPoint(line,2));
                String key = StringCut.splitBeforeEqual(line);
                String value = StringCut.splitAfterEqual(line);
                if (itemList[index] == null) {itemList[index] = new CustomizeItemBean(); }
                itemList[index].index = index;
                switch (key)
                {
                    case "chara":
                        itemList[index].chara = value;
                        break;
                    case "id":
                        itemList[index].id = Int32.Parse(value);
                        break;
                    case "name":
                        itemList[index].name = value;
                        break;
                    case "ng":
                        itemList[index].ng = Int32.Parse(value);
                        break;
                    case "obj_id":
                        itemList[index].obj_id = Int32.Parse(value);
                        break;
                    case "parts":
                        itemList[index].parts = value;
                        break;
                    case "sell_type":
                        itemList[index].sell_type = Int32.Parse(value);
                        break;
                    case "shop_ed_day":
                        itemList[index].shop_ed_day = Int32.Parse(value);
                        break;
                    case "shop_ed_month":
                        itemList[index].shop_ed_month = Int32.Parse(value);
                        break;
                    case "shop_ed_year":
                        itemList[index].shop_ed_year = Int32.Parse(value);
                        break;
                    case "shop_price":
                        itemList[index].shop_price = Int32.Parse(value);
                        break;
                    case "shop_st_day":
                        itemList[index].shop_st_day = Int32.Parse(value);
                        break;
                    case "shop_st_month":
                        itemList[index].shop_st_month = Int32.Parse(value);
                        break;
                    case "shop_st_year":
                        itemList[index].shop_st_year = Int32.Parse(value);
                        break;
                    case "sort_index":
                        itemList[index].sort_index = Int32.Parse(value);
                        break;
                    default:
                        Console.WriteLine(value);
                        break;
                }
            }
        }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            for (int i = 1; i <= 214; i++)
            {
                result.Add("#---------------------------------------------");
            }
            CustomizeItemBean[] notNullItemList = (from str in itemList where str != null select str).ToArray();
            CustomizeItemBean[] dicSortItem = (from objDic in notNullItemList
                                          orderby objDic.index.ToString()
                                          select objDic).ToArray();
            foreach (CustomizeItemBean i in dicSortItem)
                result.AddRange(i.toString());
            result.Add("cstm_item.data_list.length=" + length.ToString());
            result.Add("patch=" + patch.ToString());
            result.Add("version=" + version.ToString());
            return result;
        }
    }
}
