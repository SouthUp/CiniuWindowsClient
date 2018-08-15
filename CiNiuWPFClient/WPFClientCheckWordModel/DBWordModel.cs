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
        public bool iscustumci { get; set; }
        public bool isminganci { get; set; }
        public string username { get; set; }
        public List<CommonNameModel> discriptions { get; set; }
    }
}
