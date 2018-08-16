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
        private string discriptionSearchText = "";
        public string DiscriptionSearchText
        {
            get { return discriptionSearchText; }
            set
            {
                discriptionSearchText = value;
                RaisePropertyChanged("DiscriptionSearchText");
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
        private bool _isMessageTipPopWindowOpen = false;
        public bool IsMessageTipPopWindowOpen
        {
            get { return _isMessageTipPopWindowOpen; }
            set
            {
                _isMessageTipPopWindowOpen = value;
                RaisePropertyChanged("IsMessageTipPopWindowOpen");
            }
        }
        private string _messageTipInfo= "";
        public string MessageTipInfo
        {
            get { return _messageTipInfo; }
            set
            {
                if (_messageTipInfo != value)
                {
                    _messageTipInfo = value;
                    RaisePropertyChanged("MessageTipInfo");
                }
            }
        }
        private string _checkFilesInfosText = "";
        public string CheckFilesInfosText
        {
            get { return _checkFilesInfosText; }
            set
            {
                if (_checkFilesInfosText != value)
                {
                    _checkFilesInfosText = value;
                    RaisePropertyChanged("CheckFilesInfosText");
                }
            }
        }
        private int _dealTotalCount = 1;
        public int DealTotalCount
        {
            get { return _dealTotalCount; }
            set
            {
                if (_dealTotalCount != value)
                {
                    _dealTotalCount = value;
                    RaisePropertyChanged("DealTotalCount");
                }
            }
        }
        private int _dealCurrentIndex = 0;
        public int DealCurrentIndex
        {
            get { return _dealCurrentIndex; }
            set
            {
                if (_dealCurrentIndex != value)
                {
                    _dealCurrentIndex = value;
                    RaisePropertyChanged("DealCurrentIndex");
                }
            }
        }
        private ObservableCollection<UnChekedWordInfo> _currentWordInfoResults = new ObservableCollection<UnChekedWordInfo>();
        public ObservableCollection<UnChekedWordInfo> CurrentWordInfoResults
        {
            get { return _currentWordInfoResults; }
            set
            {
                _currentWordInfoResults = value;
                RaisePropertyChanged("CurrentWordInfoResults");
            }
        }
        private ObservableCollection<MyFolderDataViewModel> _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
        public ObservableCollection<MyFolderDataViewModel> DealDataResultList
        {
            get { return _dealDataResultList; }
            set
            {
                _dealDataResultList = value;
                RaisePropertyChanged("DealDataResultList");
            }
        }
        private ObservableCollection<HistoryCheckInfo> _historyCheckInfoList = new ObservableCollection<HistoryCheckInfo>();
        public ObservableCollection<HistoryCheckInfo> HistoryCheckInfoList
        {
            get { return _historyCheckInfoList; }
            set
            {
                _historyCheckInfoList = value;
                RaisePropertyChanged("HistoryCheckInfoList");
            }
        }
        private Visibility historyFilesGridVisibility = Visibility.Collapsed;
        public Visibility HistoryFilesGridVisibility
        {
            get { return historyFilesGridVisibility; }
            set
            {
                historyFilesGridVisibility = value;
                RaisePropertyChanged("HistoryFilesGridVisibility");
            }
        }
        private bool isSelectHistoryChecked = false;
        public bool IsSelectHistoryChecked
        {
            get { return isSelectHistoryChecked; }
            set
            {
                isSelectHistoryChecked = value;
                RaisePropertyChanged("IsSelectHistoryChecked");
            }
        }
    }
}
