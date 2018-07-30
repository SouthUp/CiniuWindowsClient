using CheckWordModel;
using Microsoft.Office.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWordAddIn
{
    public class HostSystemVar
    {
        public static Dictionary<string, List<UnChekedWordInfo>> CurrentImgsDictionary { get; set; }
    }
}
