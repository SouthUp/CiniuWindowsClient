using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class ReplaceWordInfo : ViewModelBase
    {
        private int index = 0;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                RaisePropertyChanged("Index");
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
    }
}
