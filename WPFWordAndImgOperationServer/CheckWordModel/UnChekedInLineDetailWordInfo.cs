using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedInLineDetailWordInfo : ViewModelBase
    {
        private string typeTextFrom = "Text";
        public string TypeTextFrom
        {
            get { return typeTextFrom; }
            set
            {
                typeTextFrom = value;
                RaisePropertyChanged("TypeTextFrom");
            }
        }
        private string inLineText = "";
        public string InLineText
        {
            get { return inLineText; }
            set
            {
                inLineText = value;
                RaisePropertyChanged("InLineText");
            }
        }
        private string inLineKeyText = "";
        public string InLineKeyText
        {
            get { return inLineKeyText; }
            set
            {
                inLineKeyText = value;
                RaisePropertyChanged("InLineKeyText");
            }
        }
        public int InLineKeyTextRangeStart { get; set; }
        public int InLineKeyTextRangeEnd { get; set; }
    }
}
