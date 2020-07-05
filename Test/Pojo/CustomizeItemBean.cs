using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class CustomizeItemBean
    {
        public int index;
        public String chara;
        public int id;
        public String name;
        public int ng;
        public int obj_id;
        public String parts;
        public int sell_type;
        public int shop_ed_day;
        public int shop_ed_month;
        public int shop_ed_year;
        public int shop_price;
        public int shop_st_day;
        public int shop_st_month;
        public int shop_st_year;
        public int sort_index;
        public List<String> toString()
        {
            List<String> result = new List<string>();
            String header = "cstm_item." + index.ToString();
            //cstm_item.0.chara=ALL
            result.Add(header + ".chara=" + chara);
            //cstm_item.0.id=0
            result.Add(header + ".id=" + id.ToString());
            //cstm_item.0.name=縁なしメガネ（銀）
            result.Add(header + ".name=" + name.ToString());
            //cstm_item.0.ng=0
            result.Add(header + ".ng=" + ng.ToString());
            //cstm_item.0.obj_id=1001
            result.Add(header + ".obj_id=" + obj_id.ToString());
            //cstm_item.0.parts=FACE
            result.Add(header + ".parts=" + parts.ToString());
            //cstm_item.0.sell_type=1
            result.Add(header + ".sell_type=" + sell_type.ToString());
            //cstm_item.0.shop_ed_day=1
            result.Add(header + ".shop_ed_day=" + shop_ed_day.ToString());
            //cstm_item.0.shop_ed_month=1
            result.Add(header + ".shop_ed_month=" + shop_ed_month.ToString());
            //cstm_item.0.shop_ed_year=2029
            result.Add(header + ".shop_ed_year=" + shop_ed_year.ToString());
            //cstm_item.0.shop_price=50
            result.Add(header + ".shop_price=" + shop_price.ToString());
            //cstm_item.0.shop_st_day=13
            result.Add(header + ".shop_st_day=" + shop_st_day.ToString());
            //cstm_item.0.shop_st_month=9
            result.Add(header + ".shop_st_month=" + shop_st_month.ToString());
            //cstm_item.0.shop_st_year=2013
            result.Add(header + ".shop_st_year=" + shop_st_year.ToString());
            //cstm_item.0.sort_index=0
            result.Add(header + ".sort_index=" + sort_index.ToString());
            return result;
        }
    }
}
