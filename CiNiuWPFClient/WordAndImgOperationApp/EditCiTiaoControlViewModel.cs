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
    public class EditCiTiaoControlViewModel : NotificationObject
    {
        private string _nameInfo = "";
        public string NameInfo
        {
            get { return _nameInfo; }
            set
            {
                _nameInfo = value;
                RaisePropertyChanged("NameInfo");
            }
        }
        private string _descriptionInfo = "";
        public string DescriptionInfo
        {
            get { return _descriptionInfo; }
            set
            {
                _descriptionInfo = value;
                RaisePropertyChanged("DescriptionInfo");
            }
        }
    }
}
