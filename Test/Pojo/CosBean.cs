using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Test.Pojo;
using System.Linq;
namespace Test
{
    class CosBean
    {
        public int index;
        public Boolean haveId = false;
        public int id;
        public Boolean haveItem = false;
        public List<ItemBean> item = new List<ItemBean>();
        public Boolean haveLength = false;
        public int length;
        public CosBean() { }
        public CosBean(CosBean old)
        {
            index = old.index;
            haveId = old.haveId;
            id = old.id;
            haveItem = old.haveItem;
            foreach (ItemBean i in old.item)
                item.Add(new ItemBean(i));
            haveLength = old.haveLength;
            length = old.length;
        }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            String header = "cos." + index.ToString();
            String line = header + ".id=" + id.ToString();
            if (haveId)
            {
                result.Add(line);
            }
            //字典排序
            if (haveItem)
            {
                var dicSort = from objDic in item
                              orderby objDic.index.ToString()
                              select objDic;
                foreach (ItemBean i in dicSort)
                {
                    line = header + i.toString();
                    result.Add(line);
                }
            }
            if (haveLength)
            {
                line = header + ".item.length=" + length.ToString();
                result.Add(line);
            }
            return result;
        }
    }
}
