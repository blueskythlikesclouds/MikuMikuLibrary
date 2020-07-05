using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Test.Pojo;
using System.Collections;

namespace Test
{
    class CharacterItemBean
    {
        public int index;
        public Boolean haveAttr = false;
        public int attr;
        public Boolean haveRpk = false;
        public List<ObjRpkBean> dataObjRpk = new List<ObjRpkBean>();
        public Boolean haveUid = false;
        public List<ObjUidBean> dataObjUid = new List<ObjUidBean>();
        public Boolean haveObjLength = false;
        public int dataObjLength;
        public Boolean haveTexChg = false;
        public List<TexChgBean> dataTexChg = new List<TexChgBean>();
        public Boolean haveTexOrg = false;
        public List<TexOrgBean> dataTexOrg = new List<TexOrgBean>();
        public Boolean haveTexLength = false;
        public int dataTexLength;
        public Boolean haveDes_id = false;
        public int des_id;
        public Boolean haveExclusion = false;
        public int exclusion;
        public Boolean haveFace_depth = false;
        public double face_depth;
        public Boolean haveFlag = false;
        public int flag;
        public Boolean haveName = false;
        public String name;
        public Boolean haveNo = false;
        public int no;
        public Boolean haveObjset = false;
        public List<ObjsetBean> objset = new List<ObjsetBean>();
        public Boolean haveObjsetLength = false;
        public int objsetLength;
        public Boolean haveOrg_itm = false;
        public int org_itm;
        public Boolean havePoint = false;
        public int point;
        public Boolean haveSub_id = false;
        public int sub_id;
        public Boolean haveType = false;
        public int type;
        public Boolean haveNpr_flag = false;
        public int npr_flag;
        public Boolean haveCol = false;
        public List<String> colOri = new List<string>();
        public ColBean[] col;
        public Boolean haveColLength = false;
        public int colLength;
        public CharacterItemBean() { }
        public CharacterItemBean(CharacterItemBean old)
        {
            index = old.index;
            haveAttr = old.haveAttr;
            attr = old.attr;
            haveRpk = old.haveRpk;
            foreach (ObjRpkBean orb in old.dataObjRpk) dataObjRpk.Add(new ObjRpkBean(orb));
            haveUid = old.haveUid;
            foreach (ObjUidBean oub in old.dataObjUid) dataObjUid.Add(new ObjUidBean(oub));
            haveObjLength = old.haveObjLength;
            dataObjLength = old.dataObjLength;
            haveTexChg = old.haveTexChg;
            foreach (TexChgBean tcb in old.dataTexChg) dataTexChg.Add(new TexChgBean(tcb));
            haveTexOrg = old.haveTexOrg;
            foreach (TexOrgBean tob in old.dataTexOrg) dataTexOrg.Add(new TexOrgBean(tob));
            haveTexLength = old.haveTexLength;
            dataTexLength = old.dataTexLength;
            haveDes_id = old.haveDes_id;
            des_id = old.des_id;
            haveExclusion = old.haveExclusion;
            exclusion = old.exclusion;
            haveFace_depth = old.haveFace_depth;
            face_depth = old.face_depth;
            haveFlag = old.haveFlag;
            flag = old.flag;
            haveName = old.haveName;
            name = old.name;
            haveNo = old.haveNo;
            no = old.no;
            haveObjset = old.haveObjset;
            foreach (ObjsetBean ob in old.objset) objset.Add(new ObjsetBean(ob));
            haveObjsetLength = old.haveObjsetLength;
            objsetLength = old.objsetLength;
            haveOrg_itm = old.haveOrg_itm;
            org_itm = old.org_itm;
            havePoint = old.havePoint;
            point = old.point;
            haveSub_id = old.haveSub_id;
            sub_id = old.sub_id;
            haveType = old.haveType;
            type = old.type;
            haveNpr_flag = old.haveNpr_flag;
            npr_flag = old.npr_flag;
            haveCol = old.haveCol;
            foreach (String s in old.colOri) colOri.Add(s);
            
            if (old.haveCol)
            {
                col = new ColBean[old.col.Length];
                for (int i = 0; i <= old.col.Length - 1; i++)
                    col[i] = new ColBean(old.col[i]);
            }

            haveColLength = old.haveColLength;
            colLength = old.colLength;

    }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            String header = "item." + index.ToString();
            //item.598.attr=5
            if (haveAttr) result.Add(header + ".attr=" + attr.ToString());
            //item.316.data.col.*
            if (haveCol)
            {
                List<ColBean> dicSortCol = (from objDic in col
                                            orderby objDic.index.ToString()
                                            select objDic).ToList<ColBean>();
                foreach (ColBean c in dicSortCol)
                    result.AddRange(c.toString(index));
            }
            //item.316.data.col.length=4
            if (haveColLength) result.Add(header + ".data.col.length=" + colLength.ToString());
            //item.598.data.obj.0.rpk=-1
            //item.598.data.obj.0.uid=CMNITM1001_FACE_FACE_01__DIVSKN
            ObjRpkBean[] dicSortRpk = (from objDic in dataObjRpk
                                      orderby objDic.index.ToString()
                                      select objDic).ToArray();
            ObjUidBean[] dicSortUid = (from objDic in dataObjUid
                                           orderby objDic.index.ToString()
                                           select objDic).ToArray();
            for(int i = 0;i <= dataObjLength - 1; i++)
            {
                if (haveRpk) result.Add(header + dicSortRpk[i].toString());
                if (haveUid) result.Add(header + dicSortUid[i].toString());
            }
            //item.598.data.obj.length=1
            if (haveObjLength) result.Add(header + ".data.obj.length=" + dataObjLength.ToString());
            //item.598.data.tex.0.chg=F_DIVA_CMN1003_REDMEGANE01
            //item.598.data.tex.0.org=F_DIVA_CMN1001_MEGANE01
            TexChgBean[] dicSortChg = (from objDic in dataTexChg
                                       orderby objDic.index.ToString()
                                       select objDic).ToArray();
            TexOrgBean[] dicSortOrg = (from objDic in dataTexOrg
                                       orderby objDic.index.ToString()
                                       select objDic).ToArray();
            for(int i = 0; i <= dataTexLength - 1; i++)
            {
                if (haveTexChg) result.Add(header + dicSortChg[i].toString());
                if (haveTexOrg) result.Add(header + dicSortOrg[i].toString());
            }
            //item.598.data.tex.length=2
            if (haveTexLength) result.Add(header + ".data.tex.length=" + dataTexLength.ToString());
            //item.598.des_id=1
            if (haveDes_id) result.Add(header + ".des_id=" + des_id.ToString());
            //item.598.exclusion=0
            if (haveExclusion) result.Add(header + ".exclusion=" + exclusion.ToString());
            //item.598.face_depth=0
            if (haveFace_depth) result.Add(header + ".face_depth=" + face_depth.ToString());
            //item.598.flag=4
            if (haveFlag) result.Add(header + ".flag=" + flag.ToString());
            //item.598.name=縁なしメガネ（赤）
            if (haveName) result.Add(header + ".name=" + name.ToString());
            //item.598.no=1003
            if (haveNo) result.Add(header + ".no=" + no.ToString());
            //item.144.npr_flag=1
            if (haveNpr_flag) result.Add(header + ".npr_flag=" + npr_flag.ToString());
            //item.598.objset.0=CMNITM1001
            var dicSortObjset = from objDic in objset
                                orderby objDic.index.ToString()
                                select objDic;
            foreach(ObjsetBean o in dicSortObjset)
            {
                if (haveObjset) result.Add(header + o.toString());
            }
            //item.598.objset.length=2
            if (haveObjsetLength) result.Add(header + ".objset.length=" + objsetLength.ToString());
            //item.598.org_itm=1001
            if (haveOrg_itm) result.Add(header + ".org_itm=" + org_itm.ToString());
            //item.598.point=0
            if (havePoint) result.Add(header + ".point=" + point.ToString());
            //item.598.sub_id=4
            if (haveSub_id) result.Add(header + ".sub_id=" + sub_id.ToString());
            //item.598.type=0
            if (haveType) result.Add(header + ".type=" + type.ToString());
            return result;
        }
    }
}
