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
    public class SettingWindowViewModel : NotificationObject
    {
        private System.Windows.Visibility _busyWindowVisibility = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility BusyWindowVisibility
        {
            get { return _busyWindowVisibility; }
            set
            {
                _busyWindowVisibility = value;
                RaisePropertyChanged("BusyWindowVisibility");
            }
        }
    }
}
