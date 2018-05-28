using CheckWordModel;
using CheckWordUtil;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json;
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
    public class MainResultViewModel : NotificationObject
    {
        private ObservableCollection<MyFolderDataViewModel> dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
        public ObservableCollection<MyFolderDataViewModel> DealDataResultList
        {
            get { return dealDataResultList; }
            set
            {
                dealDataResultList = value;
                RaisePropertyChanged("DealDataResultList");
            }
        }
    }
}
