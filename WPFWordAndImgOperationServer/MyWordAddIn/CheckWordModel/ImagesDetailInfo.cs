using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class ImagesDetailInfo
    {
        public Microsoft.Office.Interop.Excel.Shape UnCheckWordExcelRange { get; set; }
        public Range UnCheckWordRange { get; set; }
        public string ImgResultPath { get; set; }
    }
}
