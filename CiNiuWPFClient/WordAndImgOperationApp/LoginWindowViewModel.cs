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
    public class LoginWindowViewModel : NotificationObject
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
        /// <summary>
        /// 繁忙的内容提示
        /// </summary>
        private string busyContent = "正在加载中...";
        public string BusyContent
        {
            get { return busyContent; }
            set
            {
                if (busyContent != value)
                {
                    busyContent = value;
                    RaisePropertyChanged("BusyContent");
                }
            }
        } 
    }
}
