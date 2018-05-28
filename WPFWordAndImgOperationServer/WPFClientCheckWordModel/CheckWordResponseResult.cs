using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class CheckWordResponseResult
    {
        public bool Result { get; set; }
        public List<WordModel> UncheckWordModels { get; set; }
        public string Message { get; set; }
    }
}
