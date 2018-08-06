using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class DBWordModel
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<CommonNameAndYearModel> type { get; set; }
        public List<CommonNameModel> category { get; set; }
    }
}
