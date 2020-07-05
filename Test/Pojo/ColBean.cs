using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class ColBean
    {
        public int index;
        public double blend0;
        public double blend1;
        public double blend2;
        public double contrast;
        public int flag;
        public int hue;
        public int inverse;
        public double offset0;
        public double offset1;
        public double offset2;
        public double saturation;
        public String tex;
        public double value;
        public List<String> toString(int superIndex)
        {
            List<String> result = new List<string>();
            String header = "item." + superIndex.ToString() + ".data.col." + this.index.ToString();
            //item.316.data.col.0.blend.0=0.1
            result.Add(header + ".blend.0=" + blend0.ToString());
            //item.316.data.col.0.blend.1=0.1
            result.Add(header + ".blend.1=" + blend1.ToString());
            //item.316.data.col.0.blend.2=0.12
            result.Add(header + ".blend.2=" + blend2.ToString());
            //item.316.data.col.0.contrast=2.7
            result.Add(header + ".contrast=" + contrast.ToString());
            //item.316.data.col.0.flag=1
            result.Add(header + ".flag=" + flag.ToString());
            //item.316.data.col.0.hue=0
            result.Add(header + ".hue=" + hue.ToString());
            //item.316.data.col.0.inverse=0
            result.Add(header + ".inverse=" + inverse.ToString());
            //item.316.data.col.0.offset.0=0
            result.Add(header + ".offset.0=" + offset0.ToString());
            //item.316.data.col.0.offset.1=0
            result.Add(header + ".offset.1=" + offset1.ToString());
            //item.316.data.col.0.offset.2=0
            result.Add(header + ".offset.2=" + offset2.ToString());
            //item.316.data.col.0.saturation=0.1
            result.Add(header + ".saturation=" + saturation.ToString());
            //item.316.data.col.0.tex=F_DIVA_MIK500_HAIRBASE
            result.Add(header + ".tex=" + tex.ToString());
            //item.316.data.col.0.value=0.4
            result.Add(header + ".value=" + value.ToString());
            return result;
        }
        public ColBean() { }
        public ColBean(ColBean old)
        {
            index = old.index;
            blend0 = old.blend0;
            blend1 = old.blend1;
            blend2 = old.blend2;
            contrast = old.contrast;
            flag = old.flag;
            hue = old.hue;
            inverse = old.inverse;
            offset0 = old.offset0;
            offset1 = old.offset1;
            offset2 = old.offset2;
            saturation = old.saturation;
            tex = old.tex;
            value = old.value;
        }
    }
}
