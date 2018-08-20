using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class CustumWordRequest
    {
        public string wordId { get; set; }
        public string name { get; set; }
        public bool sensitive { get; set; }
        public bool official { get; set; }
        public string comment { get; set; }
    }
}
