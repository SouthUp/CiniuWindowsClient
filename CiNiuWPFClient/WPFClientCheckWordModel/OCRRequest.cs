using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class OCRRequest
    {
        public string image { get; set; }
        public string recognize_granularity = "small";
        public string vertexes_location = "true";
        public string fileName { get; set; }
        public string taskId { get; set; }
    }
}
