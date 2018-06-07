using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedExcelWordInfo : ViewModelBase
    {
        private string id = "";
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }
        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        private ObservableCollection<UnChekedExcelWordInfo> children = new ObservableCollection<UnChekedExcelWordInfo>();
        public ObservableCollection<UnChekedExcelWordInfo> Children
        {
            get { return children; }
            set
            {
                children = value;
                RaisePropertyChanged("Children");
            }
        }
        public Range UnCheckWordRange { get; set; }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private int warningCount = 0;
        public int WarningCount
        {
            get
            {
                warningCount = Children.Count;
                return warningCount;
            }
            set
            {
                warningCount = value;
                RaisePropertyChanged("WarningCount");
            }
        }
        private string typeTextFrom = "Text";
        public string TypeTextFrom
        {
            get { return typeTextFrom; }
            set
            {
                typeTextFrom = value;
                RaisePropertyChanged("TypeTextFrom");
            }
        }
        private ObservableCollection<UnChekedDetailWordInfo> _unChekedWordDetailInfos = new ObservableCollection<UnChekedDetailWordInfo>();
        public ObservableCollection<UnChekedDetailWordInfo> UnChekedWordDetailInfos
        {
            get { return _unChekedWordDetailInfos; }
            set
            {
                _unChekedWordDetailInfos = value;
                RaisePropertyChanged("UnChekedWordDetailInfos");
            }
        }
        public void Initialize()
        {
            foreach (UnChekedExcelWordInfo child in this.Children)
            {
                child.Initialize();
            }
        }
    }
}
