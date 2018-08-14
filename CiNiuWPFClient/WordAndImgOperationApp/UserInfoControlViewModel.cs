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
    public class UserInfoControlViewModel : NotificationObject
    {
        private string userName = "";
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged("UserName");
            }
        }
        private int pointCount = 0;
        public int PointCount
        {
            get { return pointCount; }
            set
            {
                pointCount = value;
                RaisePropertyChanged("PointCount");
            }
        }
    }
}
