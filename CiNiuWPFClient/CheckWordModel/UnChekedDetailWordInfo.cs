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
    }
}
