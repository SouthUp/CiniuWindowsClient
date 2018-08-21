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
        private int _wordsConsumeCount = 200;
        public int WordsConsumeCount
        {
            get { return _wordsConsumeCount; }
            set
            {
                if (_wordsConsumeCount != value)
                {
                    _wordsConsumeCount = value;
                    RaisePropertyChanged("WordsConsumeCount");
                }
            }
        }
        private int _picturesConsumeCount = 100;
        public int PicturesConsumeCount
        {
            get { return _picturesConsumeCount; }
            set
            {
                _picturesConsumeCount = value;
                RaisePropertyChanged("PicturesConsumeCount");
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
