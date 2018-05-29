using CheckWordModel;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyWordAddIn
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
        private int warningCount = 0;
        public int WarningCount
        {
            get { return warningCount; }
            set
            {
                warningCount = value;
                RaisePropertyChanged("WarningCount");
            }
        }
        private Visibility summaryVisibility = Visibility.Visible;
        public Visibility SummaryVisibility
        {
            get { return summaryVisibility; }
            set
            {
                summaryVisibility = value;
                RaisePropertyChanged("SummaryVisibility");
            }
        }
    }
}
