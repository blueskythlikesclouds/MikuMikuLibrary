using System;
using System.Collections.Generic;
using System.Text;
using Test.Pojo;
using System.Linq;

namespace Test
{
    class DbgsetBean
    {
        public int index;
        public Boolean haveItem = false;
        public List<ItemBean> item = new List<ItemBean>();
        public Boolean haveLength = false;
        public int length;
        public Boolean haveName = false;
        public String name;
        public List<String> toString()
        {
            List<String> result = new List<string>();
            String header = "dbgset." + index.ToString();
            String line;
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
            if (haveName)
            {
                line = header + ".name=" + name;
                result.Add(line);
            }

            return result;
        }
    }
}
