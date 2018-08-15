using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedDetailWordInfo : ViewModelBase
    {
        private string discription = "";
        public string Discription
        {
            get { return discription; }
            set
            {
                discription = value;
                RaisePropertyChanged("Discription");
            }
        }
        private string dateTime = "";
        public string DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                RaisePropertyChanged("DateTime");
            }
        }
        private string categoryName = "";
        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                RaisePropertyChanged("CategoryName");
            }
        }
        private string sourceName = "";
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                sourceName = value;
                RaisePropertyChanged("SourceName");
            }
        }
    }
}
