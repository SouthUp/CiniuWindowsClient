using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class WordModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsCustumCi { get; set; }
        public bool IsMinGanCi { get; set; }
        public string UserName { get; set; }
        public List<CommonNameModel> Discriptions { get; set; }
    }
}
