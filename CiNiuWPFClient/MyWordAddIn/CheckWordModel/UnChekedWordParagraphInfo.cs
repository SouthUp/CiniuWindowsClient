using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedWordParagraphInfo : ViewModelBase
    {
        public Paragraph Paragraph { get; set; }
        public List<UnChekedWordInfo> UnChekedWordLists { get; set; }
    }
}
