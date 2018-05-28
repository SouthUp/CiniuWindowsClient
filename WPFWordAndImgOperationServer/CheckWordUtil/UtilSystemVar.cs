using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordUtil
{
    public class UtilSystemVar
    {
        public static string UserToken { get; set; }
        public static string UserName { get; set; }

        public static bool IsCallWebApi = Boolean.Parse(ConfigurationManager.AppSettings["IsCallWebApi"].ToString());
    }
}
