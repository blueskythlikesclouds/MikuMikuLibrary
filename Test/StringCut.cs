using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class StringCut
    {
        //提取左数第num-1到num个.之间的字符串
        public static String splitPoint(String str,int num)
        {
            String result = "";
            String str1 = str;
            for (int i = 1; i <= num; i++)
            {
                result = str1.Substring(0, str1.IndexOf('.'));
                str1 = str1.Substring(str1.IndexOf('.') + 1);
            } 
            return result;
        }
        //提取左数第一个=号之前到.为止的字符串
        public static String splitBeforeEqual(String str) {
            return splitBeforeEqual(str, 1);
        }
        //提取左数第一个=号之前到第num个.为止的字符串
        public static String splitBeforeEqual(String str,int num)
        {
            String result = "";
            String str1 = str.Substring(0, str.IndexOf('='));
            int num1 = num;
            foreach (char c in str1.Reverse())
            {
                result += c;
                if (c == '.')
                {
                    num1--;
                    if (num1 <= 0)
                    {
                        result = result.Remove(result.Length - 1);
                        break;
                    }
                }
                
            }
            //反转
            char[] arr = result.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        //提取左数第一个=号后面的字符串
        public static String splitAfterEqual(String str) {
            String result = "";
            result = str.Substring(str.IndexOf('=')+1);
            return result;
        }
        public static int cosString2Id(String cos)
        {
            return Int32.Parse(cos.Substring(cos.IndexOf('_') + 1))-1;
        }
        public static String cosId2String(int cosId)
        {
            return "COS_" + (cosId+1).ToString();
        }
    }
}
