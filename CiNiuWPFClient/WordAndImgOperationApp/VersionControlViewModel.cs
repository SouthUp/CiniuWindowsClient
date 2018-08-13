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
    public class VersionControlViewModel : NotificationObject
    {
        private string _currentVersionInfo = "";
        public string CurrentVersionInfo
        {
            get { return _currentVersionInfo; }
            set
            {
                _currentVersionInfo = value;
                RaisePropertyChanged("CurrentVersionInfo");
            }
        }
        private string _newVersionInfo = "";
        public string NewVersionInfo
        {
            get { return _newVersionInfo; }
            set
            {
                _newVersionInfo = value;
                RaisePropertyChanged("NewVersionInfo");
            }
        }
        private string _currentVersionTimeInfo = "";
        public string CurrentVersionTimeInfo
        {
            get { return _currentVersionTimeInfo; }
            set
            {
                _currentVersionTimeInfo = value;
                RaisePropertyChanged("CurrentVersionTimeInfo");
            }
        }
        private string _newVersionTimeInfo = "";
        public string NewVersionTimeInfo
        {
            get { return _newVersionTimeInfo; }
            set
            {
                _newVersionTimeInfo = value;
                RaisePropertyChanged("NewVersionTimeInfo");
            }
        }
        private ObservableCollection<VersionInfo> _discriptionInfos = new ObservableCollection<VersionInfo>();
        public ObservableCollection<VersionInfo> DiscriptionInfos
        {
            get { return _discriptionInfos; }
            set
            {
                _discriptionInfos = value;
                RaisePropertyChanged("DiscriptionInfos");
            }
        }
    }
}
