using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedWordExcelRangeInfo : ViewModelBase
    {
        public Range Range { get; set; }
        public string RangeText { get; set; }
        public List<UnChekedWordInfo> UnChekedWordLists { get; set; }
    }
}
