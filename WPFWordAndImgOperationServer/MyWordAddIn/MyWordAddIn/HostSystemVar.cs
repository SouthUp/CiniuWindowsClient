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
        public static CustomTaskPane CustomTaskPane { get; set; }
        public static CustomTaskPane MyWordsDBTaskPane { get; set; }
        public static CustomTaskPane MySynonymDBTaskPane { get; set; }
    }
}
