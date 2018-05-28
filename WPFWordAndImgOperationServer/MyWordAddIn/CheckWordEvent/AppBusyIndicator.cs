using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.PubSubEvents;

namespace CheckWordEvent
{
    /// <summary>
    /// 应用程序繁忙指示器
    /// </summary>
    public class AppBusyIndicator : NotificationObject
    {
        /// <summary>
        /// 是否繁忙
        /// </summary>
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    RaisePropertyChanged("IsBusy");
                }
            }
        }

        /// <summary>
        /// 正在执行的内容提示
        /// </summary>
        private string busyContent;
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
    }

}
