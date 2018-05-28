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
        public string SourceDB { get; set; }
        public string NameType { get; set; }
        public List<CommonNameModel> SourceDBs { get; set; }
        public List<CommonNameModel> NameTypes { get; set; }
    }
}
