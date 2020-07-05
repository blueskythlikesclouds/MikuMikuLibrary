using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class TexChgBean
    {
        //item.813.data.tex.4.chg=F_DIVA_MIK981_EYELASHES
        public String chg;
        public int index;
        public String toString()
        {
            String str = ".data.tex."+index.ToString() + ".chg=" + chg;
            return str;
        }
        public TexChgBean() { }
        public TexChgBean(TexChgBean old)
        {
            chg = old.chg;
            index = old.index;
        }
    }
}
