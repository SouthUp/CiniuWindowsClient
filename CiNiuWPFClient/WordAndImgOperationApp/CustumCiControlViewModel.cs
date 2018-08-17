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
    public class CustumCiControlViewModel : NotificationObject
    {
        private System.Windows.Visibility _custumCiGridVisibility = System.Windows.Visibility.Visible;
        public System.Windows.Visibility CustumCiGridVisibility
        {
            get { return _custumCiGridVisibility; }
            set
            {
                _custumCiGridVisibility = value;
                RaisePropertyChanged("CustumCiGridVisibility");
            }
        }
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
