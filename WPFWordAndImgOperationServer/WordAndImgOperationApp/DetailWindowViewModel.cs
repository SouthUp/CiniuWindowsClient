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
    public class DetailWindowViewModel : NotificationObject
    {
        private MyFolderDataViewModel currentMyFolderData;
        public MyFolderDataViewModel CurrentMyFolderData
        {
            get { return currentMyFolderData; }
            set
            {
                currentMyFolderData = value;
                RaisePropertyChanged("CurrentMyFolderData");
            }
        }
        private Visibility _busyWindowVisibility = Visibility.Collapsed;
        public Visibility BusyWindowVisibility
        {
            get { return _busyWindowVisibility; }
            set
            {
                _busyWindowVisibility = value;
                RaisePropertyChanged("BusyWindowVisibility");
            }
        }
        private Visibility axFramerControlVisibility = Visibility.Collapsed;
        public Visibility AxFramerControlVisibility
        {
            get { return axFramerControlVisibility; }
            set
            {
                axFramerControlVisibility = value;
                RaisePropertyChanged("AxFramerControlVisibility");
            }
        }
        private Visibility picGridVisibility = Visibility.Collapsed;
        public Visibility PicGridVisibility
        {
            get { return picGridVisibility; }
            set
            {
                picGridVisibility = value;
                RaisePropertyChanged("PicGridVisibility");
            }
        }
    }
}
