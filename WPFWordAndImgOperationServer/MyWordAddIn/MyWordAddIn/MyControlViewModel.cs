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
        //////private ObservableCollection<UnChekedDetailWordInfo> uncheckedDetailWordLists = new ObservableCollection<UnChekedDetailWordInfo>();
        //////public ObservableCollection<UnChekedDetailWordInfo> UncheckedWordDetailLists
        //////{
        //////    get { return uncheckedDetailWordLists; }
        //////    set
        //////    {
        //////        uncheckedDetailWordLists = value;
        //////        RaisePropertyChanged("UncheckedWordDetailLists");
        //////    }
        //////}
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
        //////private int warningDetailCount = 0;
        //////public int WarningDetailCount
        //////{
        //////    get { return warningDetailCount; }
        //////    set
        //////    {
        //////        warningDetailCount = value;
        //////        RaisePropertyChanged("WarningDetailCount");
        //////    }
        //////}
        //////private Visibility detailVisibility = Visibility.Collapsed;
        //////public Visibility DetailVisibility
        //////{
        //////    get { return detailVisibility; }
        //////    set
        //////    {
        //////        detailVisibility = value;
        //////        RaisePropertyChanged("DetailVisibility");
        //////    }
        //////}
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
