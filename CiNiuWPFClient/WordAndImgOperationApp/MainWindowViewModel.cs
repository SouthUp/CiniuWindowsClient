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
    public class WPFOfficeWindowViewModel : NotificationObject
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
        private Visibility menueUnLoginVisibility = Visibility.Visible;
        public Visibility MenueUnLoginVisibility
        {
            get { return menueUnLoginVisibility; }
            set
            {
                menueUnLoginVisibility = value;
                RaisePropertyChanged("MenueUnLoginVisibility");
            }
        }
        private Visibility menueLoginVisibility = Visibility.Collapsed;
        public Visibility MenueLoginVisibility
        {
            get { return menueLoginVisibility; }
            set
            {
                menueLoginVisibility = value;
                RaisePropertyChanged("MenueLoginVisibility");
            }
        }
        private bool _isSysMenuePopWindowOpen = false;
        public bool IsSysMenuePopWindowOpen
        {
            get { return _isSysMenuePopWindowOpen; }
            set
            {
                _isSysMenuePopWindowOpen = value;
                RaisePropertyChanged("IsSysMenuePopWindowOpen");
            }
        }
        private UserStateInfos _currentUserInfo = new UserStateInfos();
        public UserStateInfos CurrentUserInfo
        {
            get { return _currentUserInfo; }
            set
            {
                _currentUserInfo = value;
                RaisePropertyChanged("CurrentUserInfo");
            }
        }
        private bool _isVersionInfoPopWindowOpen = false;
        public bool IsVersionInfoPopWindowOpen
        {
            get { return _isVersionInfoPopWindowOpen; }
            set
            {
                _isVersionInfoPopWindowOpen = value;
                RaisePropertyChanged("IsVersionInfoPopWindowOpen");
            }
        }
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
    }
}
