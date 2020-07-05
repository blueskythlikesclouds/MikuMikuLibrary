using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class ItemBean
    {
        //*.0.item.0=500
        public String item;
        public int index;
        public ItemBean() { }
        public ItemBean(ItemBean old)
        {
            item = old.item;
            index = old.index;
        }
        public String toString()
        {
            String str = ".item." + index.ToString() + "=" + item.ToString();
            return str;
        }
    }
}
