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
        private bool _isMoreMenuePopWindowOpen = false;
        public bool IsMoreMenuePopWindowOpen
        {
            get { return _isMoreMenuePopWindowOpen; }
            set
            {
                _isMoreMenuePopWindowOpen = value;
                RaisePropertyChanged("IsMoreMenuePopWindowOpen");
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
        private string pinBtnToolTip = "点击固定";
        public string PinBtnToolTip
        {
            get { return pinBtnToolTip; }
            set
            {
                pinBtnToolTip = value;
                RaisePropertyChanged("PinBtnToolTip");
            }
        }
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    RaisePropertyChanged("SearchText");
                }
            }
        }
        private Visibility inputGridVisibility = Visibility.Visible;
        public Visibility InputGridVisibility
        {
            get { return inputGridVisibility; }
            set
            {
                inputGridVisibility = value;
                RaisePropertyChanged("InputGridVisibility");
            }
        }
        private Visibility dealingGridVisibility = Visibility.Collapsed;
        public Visibility DealingGridVisibility
        {
            get { return dealingGridVisibility; }
            set
            {
                dealingGridVisibility = value;
                RaisePropertyChanged("DealingGridVisibility");
            }
        }
        private Visibility wordHasUnchekResultVisibility = Visibility.Collapsed;
        public Visibility WordHasUnchekResultVisibility
        {
            get { return wordHasUnchekResultVisibility; }
            set
            {
                wordHasUnchekResultVisibility = value;
                RaisePropertyChanged("WordHasUnchekResultVisibility");
            }
        }
        private Visibility wordNoUnchekResultVisibility = Visibility.Collapsed;
        public Visibility WordNoUnchekResultVisibility
        {
            get { return wordNoUnchekResultVisibility; }
            set
            {
                wordNoUnchekResultVisibility = value;
                RaisePropertyChanged("WordNoUnchekResultVisibility");
            }
        }
        private Visibility dragFilesResultVisibility = Visibility.Collapsed;
        public Visibility DragFilesResultVisibility
        {
            get { return dragFilesResultVisibility; }
            set
            {
                dragFilesResultVisibility = value;
                RaisePropertyChanged("DragFilesResultVisibility");
            }
        }
        private Visibility addToCustumCiTiaoVisibility = Visibility.Collapsed;
        public Visibility AddToCustumCiTiaoVisibility
        {
            get { return addToCustumCiTiaoVisibility; }
            set
            {
                addToCustumCiTiaoVisibility = value;
                RaisePropertyChanged("AddToCustumCiTiaoVisibility");
            }
        }
    }
}
