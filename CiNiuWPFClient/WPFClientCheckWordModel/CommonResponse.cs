using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class CommonResponse
    {
        public bool state { get; set; }
        public string result { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }
}
