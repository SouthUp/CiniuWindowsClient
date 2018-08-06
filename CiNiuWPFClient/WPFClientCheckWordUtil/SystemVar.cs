using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordUtil
{
    public class SystemVar
    {
        public static string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\";
        public static string UrlStr { get; set; }
        public static string UserToken { get; set; }
    }
}
