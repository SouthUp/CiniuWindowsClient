using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class ConsumeResponse
    {
        public int points { get; set; }
        public int consumedPoints { get; set; }
        public int toAccountWords { get; set; }
        public int accountedWords { get; set; }
    }
}
