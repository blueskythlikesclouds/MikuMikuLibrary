using System;
using System.Linq;
class Test
{
    static void Main()
    {

         [] arr = new string[] {
         "100",
         "1",
         "2",
      };
        var sort = from a in arr
                   orderby a
                   select a;

        foreach (string res in sort)
        {
            Console.WriteLine(res);
        }
    }
}