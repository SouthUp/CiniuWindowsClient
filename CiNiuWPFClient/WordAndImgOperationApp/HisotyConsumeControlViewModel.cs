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
    public class HisotyConsumeControlViewModel : NotificationObject
    {
        private ObservableCollection<UserMonthConsumeInfo> _historyConsumeInfoList = new ObservableCollection<UserMonthConsumeInfo>();
        public ObservableCollection<UserMonthConsumeInfo> HistoryConsumeInfoList
        {
            get { return _historyConsumeInfoList; }
            set
            {
                _historyConsumeInfoList = value;
                RaisePropertyChanged("HistoryConsumeInfoList");
            }
        }
    }
}
