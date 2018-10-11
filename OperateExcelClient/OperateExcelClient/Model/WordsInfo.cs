using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperateExcelClient.Model
{
    public class WordsInfo
    {
        public string wordId { get; set; }
        public string name { get; set; }
        public bool sensitive { get; set; }
        public bool official { get; set; }
        public string comment { get; set; }
    }
}
