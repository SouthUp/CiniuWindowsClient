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
    public class WPFOfficeWindowViewModel : NotificationObject
    {
        private Visibility _titleLogoVisibility = Visibility.Visible;
        public Visibility TitleLogoVisibility
        {
            get { return _titleLogoVisibility; }
            set
            {
                _titleLogoVisibility = value;
                RaisePropertyChanged("TitleLogoVisibility");
            }
        }
        private Visibility _returnBackBtnVisibility = Visibility.Collapsed;
        public Visibility ReturnBackBtnVisibility
        {
            get { return _returnBackBtnVisibility; }
            set
            {
                _returnBackBtnVisibility = value;
                RaisePropertyChanged("ReturnBackBtnVisibility");
            }
        }
        private System.Windows.Visibility _busyWindowVisibility = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility BusyWindowVisibility
        {
            get { return _busyWindowVisibility; }
            set
            {
                _busyWindowVisibility = value;
                RaisePropertyChanged("BusyWindowVisibility");
            }
        }
        /// <summary>
        /// 繁忙的内容提示
        /// </summary>
        private string busyContent = "正在加载中...";
        public string BusyContent
        {
            get { return busyContent; }
            set
            {
                if (busyContent != value)
                {
                    busyContent = value;
                    RaisePropertyChanged("BusyContent");
                }
            }
        }
        private string selectExcuteFilePathInfo = "";
        public string SelectExcuteFilePathInfo
        {
            get { return selectExcuteFilePathInfo; }
            set
            {
                selectExcuteFilePathInfo = value;
                RaisePropertyChanged("SelectExcuteFilePathInfo");
            }
        }
        private bool openFloatWindowEnable = true;
        public bool OpenFloatWindowEnable
        {
            get { return openFloatWindowEnable; }
            set
            {
                openFloatWindowEnable = value;
                RaisePropertyChanged("OpenFloatWindowEnable");
            }
        }
        private string openFloatWindowContent = "显示浮动窗口";
        public string OpenFloatWindowContent
        {
            get { return openFloatWindowContent; }
            set
            {
                openFloatWindowContent = value;
                RaisePropertyChanged("OpenFloatWindowContent");
            }
        }
        private System.Windows.Visibility _userInfoGridVisibility = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility UserInfoGridVisibility
        {
            get { return _userInfoGridVisibility; }
            set
            {
                _userInfoGridVisibility = value;
                RaisePropertyChanged("UserInfoGridVisibility");
            }
        }
        private string userName = "";
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged("UserName");
            }
        }
        private Visibility menueUnLoginVisibility = Visibility.Visible;
        public Visibility MenueUnLoginVisibility
        {
            get { return menueUnLoginVisibility; }
            set
            {
                menueUnLoginVisibility = value;
                RaisePropertyChanged("MenueUnLoginVisibility");
            }
        }
        private Visibility menueLoginVisibility = Visibility.Collapsed;
        public Visibility MenueLoginVisibility
        {
            get { return menueLoginVisibility; }
            set
            {
                menueLoginVisibility = value;
                RaisePropertyChanged("MenueLoginVisibility");
            }
        }
    }
}
