using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class ReplaceWordResponseResult
    {
        public bool Result { get; set; }
        public List<ReplaceWordInfo> ReplaceWordModels { get; set; }
        public string Message { get; set; }
    }
}
