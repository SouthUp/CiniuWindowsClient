﻿using CheckWordModel;
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
        private ObservableCollection<UnChekedExcelWordInfo> uncheckedWordLists = new ObservableCollection<UnChekedExcelWordInfo>();
        public ObservableCollection<UnChekedExcelWordInfo> UncheckedWordLists
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
    }
}
