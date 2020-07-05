using System;
using System.Collections.Generic;
using System.Text;
using Test.Pojo;

namespace Test
{
    class TexOrgBeanSort : IComparer<TexOrgBean>/*实现 IComparer<T> 接口中的 Compare 方法，
                                       在使用Sort排序时会根据Compare方法体的规定进行排序*/
    {
        public int Compare(TexOrgBean x, TexOrgBean y)
        {
            return (x.index.CompareTo(y.index));//（-x.age.CompareTo(y.age）降序
        }
    }
}
