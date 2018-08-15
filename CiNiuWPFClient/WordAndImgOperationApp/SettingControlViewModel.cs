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
        private bool _isUseCustumCi = false;
        public bool IsUseCustumCi
        {
            get { return _isUseCustumCi; }
            set
            {
                _isUseCustumCi = value;
                RaisePropertyChanged("IsUseCustumCi");
            }
        }
    }
}
