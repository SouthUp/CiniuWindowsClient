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
    public class LoginViewModel : NotificationObject
    {
        private string messageInfo = "";
        public string MessageInfo
        {
            get { return messageInfo; }
            set
            {
                messageInfo = value;
                RaisePropertyChanged("MessageInfo");
            }
        }
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }
        private string _passWord;
        public string PassWord
        {
            get { return _passWord; }
            set
            {
                if (_passWord != value)
                {
                    _passWord = value;
                    RaisePropertyChanged("PassWord");
                }
            }
        }
        private bool _isAutoLogin;
        public bool IsAutoLogin
        {
            get { return _isAutoLogin; }
            set
            {
                if (_isAutoLogin != value)
                {
                    _isAutoLogin = value;
                    RaisePropertyChanged("IsAutoLogin");
                }
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
    }
}
