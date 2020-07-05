using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SlotEditor se = new SlotEditor();
            se.copyModuleWithNewBody(431,"");
            se.copyModuleWithNewBody(430, "");
            se.copyModuleWithNewBody(429, "");
            se.copyModuleWithNewBody(428, "");
            se.copyModuleWithNewBody("luka",1, "");
            se.copyModuleWithNewBody("rin", 1, "");
            se.copyModuleWithNewBody("meiko", 1, "");
            se.copyModuleWithNewBody("kaito", 1, "");
            se.copyModuleWithNewBody("sakine", 1, "");
            se.copyModuleWithNewBody("haku", 1, "");
            se.copyModuleWithNewBody("teto", 1, "");
            se.copyModuleWithNewBody("neru", 1, "");
            se.copyModuleWithNewBody("len", 1, "");
            se.output(@"D:\");
        }
    }
}
