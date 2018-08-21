using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedWordInfo : ViewModelBase
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
        public Range Range { get; set; }
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
        private ObservableCollection<UnChekedInLineDetailWordInfo> _unChekedWordInLineDetailInfos = new ObservableCollection<UnChekedInLineDetailWordInfo>();
        public ObservableCollection<UnChekedInLineDetailWordInfo> UnChekedWordInLineDetailInfos
        {
            get { return _unChekedWordInLineDetailInfos; }
            set
            {
                _unChekedWordInLineDetailInfos = value;
                RaisePropertyChanged("UnChekedWordInLineDetailInfos");
            }
        }
        private int errorCount = 0;
        public int ErrorTotalCount
        {
            get { return errorCount; }
            set
            {
                errorCount = value;
                RaisePropertyChanged("ErrorTotalCount");
            }
        }
    }
}
