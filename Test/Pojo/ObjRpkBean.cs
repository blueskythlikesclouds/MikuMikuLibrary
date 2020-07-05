using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class ObjRpkBean
    {
        //item.813.data.obj.0.rpk=1
        public int rpk;
        public int index;
        public String toString()
        {
            String str = ".data.obj." + index.ToString() + ".rpk=" + rpk.ToString();
            return str;
        }
        public ObjRpkBean() { }
        public ObjRpkBean(ObjRpkBean old) 
        {
            rpk = old.rpk;
            index = old.index;
        }
    }
}
