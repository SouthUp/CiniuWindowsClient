using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class UpdateSettingRequest
    {
        public bool imageActive { get; set; }
        public bool customActive { get; set; }
        public List<string> notSelectedType { get; set; }
    }
}
