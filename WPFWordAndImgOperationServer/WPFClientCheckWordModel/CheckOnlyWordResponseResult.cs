using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class CheckOnlyWordResponseResult
    {
        public bool Result { get; set; }
        public WordModel WordInfo { get; set; }
        public string Message { get; set; }
    }
}
