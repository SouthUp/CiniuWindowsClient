using CheckWordModel;
using CheckWordUtil;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WordAndImgOperationApp
{
    public class EditCustumCiControlViewModel : NotificationObject
    {
        private System.Windows.Visibility _contentGridVisibility = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ContentGridVisibility
        {
            get { return _contentGridVisibility; }
            set
            {
                _contentGridVisibility = value;
                RaisePropertyChanged("ContentGridVisibility");
            }
        }
    }
}
