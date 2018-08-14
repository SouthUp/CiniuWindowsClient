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
    public class SettingFindPswViewModel : NotificationObject
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
        private string _newPassWord;
        public string NewPassWord
        {
            get { return _newPassWord; }
            set
            {
                if (_newPassWord != value)
                {
                    _newPassWord = value;
                    RaisePropertyChanged("NewPassWord");
                }
            }
        }
        private string _yzmStr;
        public string YZMStr
        {
            get { return _yzmStr; }
            set
            {
                if (_yzmStr != value)
                {
                    _yzmStr = value;
                    RaisePropertyChanged("YZMStr");
                }
            }
        }
        private string _sendYZMBtnContentTime = "";
        public string SendYZMBtnContentTime
        {
            get { return _sendYZMBtnContentTime; }
            set
            {
                if (_sendYZMBtnContentTime != value)
                {
                    _sendYZMBtnContentTime = value;
                    RaisePropertyChanged("SendYZMBtnContentTime");
                }
            }
        }
        private string _sendYZMBtnContent = "发送验证码";
        public string SendYZMBtnContent
        {
            get { return _sendYZMBtnContent; }
            set
            {
                if (_sendYZMBtnContent != value)
                {
                    _sendYZMBtnContent = value;
                    RaisePropertyChanged("SendYZMBtnContent");
                }
            }
        }
        private bool _isSendYZMBtnEnabled = true;
        public bool IsSendYZMBtnEnabled
        {
            get { return _isSendYZMBtnEnabled; }
            set
            {
                if (_isSendYZMBtnEnabled != value)
                {
                    _isSendYZMBtnEnabled = value;
                    RaisePropertyChanged("IsSendYZMBtnEnabled");
                }
            }
        }
        private Visibility _findPswGridVisibility = Visibility.Visible;
        public Visibility FindPswGridVisibility
        {
            get { return _findPswGridVisibility; }
            set
            {
                if (_findPswGridVisibility != value)
                {
                    _findPswGridVisibility = value;
                    RaisePropertyChanged("FindPswGridVisibility");
                }
            }
        }
        private Visibility _findPswResultGridVisibility = Visibility.Collapsed;
        public Visibility FindPswResultGridVisibility
        {
            get { return _findPswResultGridVisibility; }
            set
            {
                if (_findPswResultGridVisibility != value)
                {
                    _findPswResultGridVisibility = value;
                    RaisePropertyChanged("FindPswResultGridVisibility");
                }
            }
        }
    }
}
