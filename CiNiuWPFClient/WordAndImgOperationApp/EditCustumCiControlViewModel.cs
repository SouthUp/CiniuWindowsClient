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
    public class EditCustumCiControlViewModel : NotificationObject
    {
        private System.Windows.Visibility _contentGridVisibility = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ContentGridVisibility
        {
            get { return _contentGridVisibility; }
            set
            {
                _contentGridVisibility = value;
                RaisePropertyChanged("ContentGridVisibility");
            }
        }
        private ObservableCollection<CustumCiInfo> _custumCiInfoList = new ObservableCollection<CustumCiInfo>();
        public ObservableCollection<CustumCiInfo> CustumCiInfoList
        {
            get { return _custumCiInfoList; }
            set
            {
                _custumCiInfoList = value;
                RaisePropertyChanged("CustumCiInfoList");
            }
        }
        private System.Windows.Visibility _messageTipVisibility = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility MessageTipVisibility
        {
            get { return _messageTipVisibility; }
            set
            {
                _messageTipVisibility = value;
                RaisePropertyChanged("MessageTipVisibility");
            }
        }
        private string _messageTipInfo = "";
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
    }
}
