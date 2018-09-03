using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordModel
{
    public class CategorySelectInfo : ViewModelBase
    {
        private bool checkedState = true;
        public bool CheckedState
        {
            get { return checkedState; }
            set
            {
                checkedState = value;
                RaisePropertyChanged("CheckedState");
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
        private string code = "";
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                RaisePropertyChanged("Code");
            }
        }
    }
}
