using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class TexOrgBean
    {
        //item.813.data.tex.4.org=F_DIVA_MIK000_EYELASHES
        public String org;
        public int index;
        public String toString()
        {
            
            String str = ".data.tex."+index.ToString()+".org="+org;
            return str;
        }
        public TexOrgBean() { }
        public TexOrgBean(TexOrgBean old)
        {
            org = old.org;
            index = old.index;
        }
    }
    
}
