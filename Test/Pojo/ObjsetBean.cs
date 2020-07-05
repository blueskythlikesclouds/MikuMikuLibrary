using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class ObjsetBean
    {
        //item.812.objset.0=MIKITM481
        public String objset;
        public int index;
        public String toString()
        {
            String str = ".objset." + index.ToString() + "=" + objset;
            return str;
        }
        public ObjsetBean() { }
        public ObjsetBean(ObjsetBean old)
        {
            objset = old.objset;
            index = old.index;
        }
    }
}
