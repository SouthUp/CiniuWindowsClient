using CheckWordModel;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyExcelAddIn
{
    public class MyControlViewModel : NotificationObject
    {
        private ObservableCollection<UnChekedWordInfo> uncheckedWordLists = new ObservableCollection<UnChekedWordInfo>();
        public ObservableCollection<UnChekedWordInfo> UncheckedWordLists
        {
            get { return uncheckedWordLists; }
            set
            {
                uncheckedWordLists = value;
                RaisePropertyChanged("UncheckedWordLists");
            }
        }
        private int warningTotalCount = 0;
        public int WarningTotalCount
        {
            get { return warningTotalCount; }
            set
            {
                warningTotalCount = value;
                RaisePropertyChanged("WarningTotalCount");
            }
        }
        private Visibility isBusyVisibility = Visibility.Hidden;
        public Visibility IsBusyVisibility
        {
            get { return isBusyVisibility; }
            set
            {
                isBusyVisibility = value;
                RaisePropertyChanged("IsBusyVisibility");
            }
        }
        private bool isUnLogin = false;
        public bool IsUnLogin
        {
            get { return isUnLogin; }
            set
            {
                isUnLogin = value;
                RaisePropertyChanged("IsUnLogin");
            }
        }
        private bool _isDetailInfoPopWindowOpen = false;
        public bool IsDetailInfoPopWindowOpen
        {
            get { return _isDetailInfoPopWindowOpen; }
            set
            {
                _isDetailInfoPopWindowOpen = value;
                RaisePropertyChanged("IsDetailInfoPopWindowOpen");
            }
        }
        private UnChekedWordInfo selectedUnChekedWordInfo = new UnChekedWordInfo();
        public UnChekedWordInfo SelectedUnChekedWordInfo
        {
            get { return selectedUnChekedWordInfo; }
            set
            {
                selectedUnChekedWordInfo = value;
                RaisePropertyChanged("SelectedUnChekedWordInfo");
            }
        }
    }
}
