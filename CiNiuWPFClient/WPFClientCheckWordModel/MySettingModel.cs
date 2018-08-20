using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class MySettingModel
    {
        public bool imageActive { get; set; }
        public bool customActive { get; set; }
        public List<CatagryTypesModel> types { get; set; }
    }
}
