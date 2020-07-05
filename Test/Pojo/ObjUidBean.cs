using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class ObjUidBean
    {
        //item.813.data.obj.0.uid=MIKITM981_ATAM_HEAD_00_SP__DIVSKN
        public String uid;
        public int index;
        public String toString()
        {
            String str = ".data.obj."+index.ToString() + ".uid=" + uid;
            return str;
        }
        public ObjUidBean() { }
        public ObjUidBean(ObjUidBean old)
        {
            uid = old.uid;
            index = old.index;
        }
    }
}
