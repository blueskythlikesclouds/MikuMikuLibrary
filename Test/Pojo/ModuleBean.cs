using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    class ModuleBean
    {
        public int index;
        public int attr;
        public string chara;
        public string cos;
        public int id;
        public string name;
        public int ng;
        public int shop_ed_day;
        public int shop_ed_month;
        public int shop_ed_year;
        public int shop_price;
        public int shop_st_day;
        public int shop_st_month;
        public int shop_st_year;
        public int sort_index;
        public ModuleBean(ModuleBean old)
        {
            index = old.index;
            attr = old.attr;
            chara = old.chara;
            cos = old.cos;
            id = old.id;
            name = old.name;
            ng = old.ng;
            shop_ed_day = old.shop_ed_day;
            shop_ed_month = old.shop_ed_month;
            shop_ed_year = old.shop_ed_year;
            shop_price = old.shop_price;
            shop_st_day = old.shop_st_day;
            shop_st_month = old.shop_st_month;
            shop_st_year = old.shop_st_year;
            sort_index = old.sort_index;
        }
        public ModuleBean() { }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            String header = "module." + index.ToString();
            //module.0.attr=0
            result.Add(header + ".attr=" + attr.ToString());
            //module.0.chara=MIKU
            result.Add(header + ".chara=" + chara.ToString());
            //module.0.cos=COS_001
            result.Add(header + ".cos=" + cos.ToString());
            //module.0.id=0
            result.Add(header + ".id=" + id.ToString());
            //module.0.name=初音ミク
            result.Add(header + ".name=" + name.ToString());
            //module.0.ng=0
            result.Add(header + ".ng=" + ng.ToString());
            //module.0.shop_ed_day=1
            result.Add(header + ".shop_ed_day=" + shop_ed_day.ToString());
            //module.0.shop_ed_month=1
            result.Add(header + ".shop_ed_month=" + shop_ed_month.ToString());
            //module.0.shop_ed_year=2029
            result.Add(header + ".shop_ed_year=" + shop_ed_year.ToString());
            //module.0.shop_price=0
            result.Add(header + ".shop_price=" + shop_price.ToString());
            //module.0.shop_st_day=1
            result.Add(header + ".shop_st_day=" + shop_st_day.ToString());
            //module.0.shop_st_month=1
            result.Add(header + ".shop_st_month=" + shop_st_month.ToString());
            //module.0.shop_st_year=2009
            result.Add(header + ".shop_st_year=" + shop_st_year.ToString());
            //module.0.sort_index=1
            result.Add(header + ".sort_index=" + sort_index.ToString());
            return result;
        }
    }
}
