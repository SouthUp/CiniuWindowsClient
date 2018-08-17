using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordModel
{
    public class CustumCiInfo : ViewModelBase
    {
        private string id = "";
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }
        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
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
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
    }
}
