using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class VersionInfo : ViewModelBase
    {
        private string discriptionInfo = "";
        public string DiscriptionInfo
        {
            get { return discriptionInfo; }
            set
            {
                discriptionInfo = value;
                RaisePropertyChanged("DiscriptionInfo");
            }
        }
    }
}
