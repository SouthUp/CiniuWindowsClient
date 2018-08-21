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
    public class ConsumeSearchViewModel : NotificationObject
    {
        private string _currentMonthDateTime = DateTime.Now.ToString("yyyy-MM月");
        public string CurrentMonthDateTime
        {
            get { return _currentMonthDateTime; }
            set
            {
                _currentMonthDateTime = value;
                RaisePropertyChanged("CurrentMonthDateTime");
            }
        }
        private UserMonthConsumeInfo _wordsConsumeCount = new UserMonthConsumeInfo();
        public UserMonthConsumeInfo CurrentMonthConsumeInfo
        {
            get { return _wordsConsumeCount; }
            set
            {
                if (_wordsConsumeCount != value)
                {
                    _wordsConsumeCount = value;
                    RaisePropertyChanged("CurrentMonthConsumeInfo");
                }
            }
        }
        private ObservableCollection<UserConsumeInfo> _userConsumeInfoList = new ObservableCollection<UserConsumeInfo>();
        public ObservableCollection<UserConsumeInfo> UserConsumeInfoList
        {
            get { return _userConsumeInfoList; }
            set
            {
                _userConsumeInfoList = value;
                RaisePropertyChanged("UserConsumeInfoList");
            }
        }
    }
}
