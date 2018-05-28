using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordUtil
{
    public class SystemVar
    {
        public static string FolderPath= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\";
        public static bool IsLoginIn { get; set; }
    }
}
