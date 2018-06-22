using CheckWordModel;
using CheckWordModel.Communication;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrowseSearchTXT
{
    public class MainWindowViewModel : NotificationObject
    {
        private Visibility _titleLogoVisibility = Visibility.Visible;
        public Visibility TitleLogoVisibility
        {
            get { return _titleLogoVisibility; }
            set
            {
                _titleLogoVisibility = value;
                RaisePropertyChanged("TitleLogoVisibility");
            }
        }
        private Visibility _returnBtnVisibility = Visibility.Collapsed;
        public Visibility ReturnBtnVisibility
        {
            get { return _returnBtnVisibility; }
            set
            {
                _returnBtnVisibility = value;
                RaisePropertyChanged("ReturnBtnVisibility");
            }
        }
        private Visibility _inputCheckGridVisibility = Visibility.Visible;
        public Visibility InputCheckGridVisibility
        {
            get { return _inputCheckGridVisibility; }
            set
            {
                _inputCheckGridVisibility = value;
                RaisePropertyChanged("InputCheckGridVisibility");
            }
        }
        private Visibility _dataProcessGridVisibility = Visibility.Collapsed;
        public Visibility DataProcessGridVisibility
        {
            get { return _dataProcessGridVisibility; }
            set
            {
                _dataProcessGridVisibility = value;
                RaisePropertyChanged("DataProcessGridVisibility");
            }
        }
        private Visibility _dataProcessResultGridVisibility = Visibility.Collapsed;
        public Visibility DataProcessResultGridVisibility
        {
            get { return _dataProcessResultGridVisibility; }
            set
            {
                _dataProcessResultGridVisibility = value;
                RaisePropertyChanged("DataProcessResultGridVisibility");
            }
        }
        private string _checkResultText = "";
        public string CheckResultText
        {
            get { return _checkResultText; }
            set
            {
                _checkResultText = value;
                RaisePropertyChanged("CheckResultText");
            }
        }
        private Visibility _commonCheckResultVisibility = Visibility.Collapsed;
        public Visibility CommonCheckResultVisibility
        {
            get { return _commonCheckResultVisibility; }
            set
            {
                _commonCheckResultVisibility = value;
                RaisePropertyChanged("CommonCheckResultVisibility");
            }
        }
        private Visibility _tongJiCheckResultVisibility = Visibility.Collapsed;
        public Visibility TongJiCheckResultVisibility
        {
            get { return _tongJiCheckResultVisibility; }
            set
            {
                _tongJiCheckResultVisibility = value;
                RaisePropertyChanged("TongJiCheckResultVisibility");
            }
        }
        private Visibility _singgleWordCheckResultVisibility = Visibility.Collapsed;
        public Visibility SinggleWordCheckResultVisibility
        {
            get { return _singgleWordCheckResultVisibility; }
            set
            {
                _singgleWordCheckResultVisibility = value;
                RaisePropertyChanged("SinggleWordCheckResultVisibility");
            }
        }
        private Visibility _singgleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
        public Visibility SinggleWordCheckResultNoUncheckVisibility
        {
            get { return _singgleWordCheckResultNoUncheckVisibility; }
            set
            {
                _singgleWordCheckResultNoUncheckVisibility = value;
                RaisePropertyChanged("SinggleWordCheckResultNoUncheckVisibility");
            }
        }
        private UnChekedDetailWordInfo _currentWordInfo = new UnChekedDetailWordInfo();
        public UnChekedDetailWordInfo CurrentWordInfo
        {
            get { return _currentWordInfo; }
            set
            {
                _currentWordInfo = value;
                RaisePropertyChanged("CurrentWordInfo");
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
        private int _hasUnChekedWordInfoCount = 0;
        public int HasUnChekedWordInfoCount
        {
            get { return _hasUnChekedWordInfoCount; }
            set
            {
                _hasUnChekedWordInfoCount = value;
                RaisePropertyChanged("HasUnChekedWordInfoCount");
            }
        }
        private ExchangeBrowseTxTProcessingInfo _currentCurrentProcessingInfo = new ExchangeBrowseTxTProcessingInfo();
        public ExchangeBrowseTxTProcessingInfo CurrentProcessingInfo
        {
            get { return _currentCurrentProcessingInfo; }
            set
            {
                _currentCurrentProcessingInfo = value;
                RaisePropertyChanged("CurrentProcessingInfo");
            }
        }
        private Visibility _fileReadFailTipsVisibility = Visibility.Collapsed;
        public Visibility FileReadFailTipsVisibility
        {
            get { return _fileReadFailTipsVisibility; }
            set
            {
                _fileReadFailTipsVisibility = value;
                RaisePropertyChanged("FileReadFailTipsVisibility");
            }
        }
        private string _fileReadFailTips = "";
        public string FileReadFailTips
        {
            get { return _fileReadFailTips; }
            set
            {
                _fileReadFailTips = value;
                RaisePropertyChanged("FileReadFailTips");
            }
        }
        private string _fileReadFailTipsExtention = "";
        public string FileReadFailTipsExtention
        {
            get { return _fileReadFailTipsExtention; }
            set
            {
                _fileReadFailTipsExtention = value;
                RaisePropertyChanged("FileReadFailTipsExtention");
            }
        }
        private Visibility _hidePopWindowVisibility = Visibility.Collapsed;
        public Visibility HidePopWindowVisibility
        {
            get { return _hidePopWindowVisibility; }
            set
            {
                _hidePopWindowVisibility = value;
                RaisePropertyChanged("HidePopWindowVisibility");
            }
        }
        private bool _isPopWindowOpen = false;
        public bool IsPopWindowOpen
        {
            get { return _isPopWindowOpen; }
            set
            {
                _isPopWindowOpen = value;
                RaisePropertyChanged("IsPopWindowOpen");
            }
        }
        private bool _isDetailPopWindowOpen = false;
        public bool IsDetailPopWindowOpen
        {
            get { return _isDetailPopWindowOpen; }
            set
            {
                _isDetailPopWindowOpen = value;
                RaisePropertyChanged("IsDetailPopWindowOpen");
            }
        }
    }
}
