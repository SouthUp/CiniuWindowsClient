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
    public class SettingControlViewModel : NotificationObject
    {
        private bool _isCheckPicInDucument = true;
        public bool IsCheckPicInDucument
        {
            get { return _isCheckPicInDucument; }
            set
            {
                _isCheckPicInDucument = value;
                RaisePropertyChanged("IsCheckPicInDucument");
            }
        }
        private bool _isUseCustumCi = true;
        public bool IsUseCustumCi
        {
            get { return _isUseCustumCi; }
            set
            {
                _isUseCustumCi = value;
                RaisePropertyChanged("IsUseCustumCi");
            }
        }
        private ObservableCollection<CategorySelectInfo> categoryInfos = new ObservableCollection<CategorySelectInfo>();
        public ObservableCollection<CategorySelectInfo> CategoryInfos
        {
            get { return categoryInfos; }
            set
            {
                categoryInfos = value;
                RaisePropertyChanged("CategoryInfos");
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
