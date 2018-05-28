using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class ChekedWordSettingsInfo : ViewModelBase
    {
        private bool _IsCanDelete = false;
        public bool IsCanDelete
        {
            get { return _IsCanDelete; }
            set
            {
                _IsCanDelete = value;
                RaisePropertyChanged("IsCanDelete");
            }
        }
        private bool _IsChecked = true;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        private bool _IsChecking = false;
        public bool IsChecking
        {
            get { return _IsChecking; }
            set
            {
                _IsChecking = value;
                RaisePropertyChanged("IsChecking");
            }
        }
        private bool _IsCheckedFinished = false;
        public bool IsCheckedFinished
        {
            get { return _IsCheckedFinished; }
            set
            {
                _IsCheckedFinished = value;
                RaisePropertyChanged("IsCheckedFinished");
            }
        }
        private int currentIndex = 0;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                currentIndex = value;
                RaisePropertyChanged("CurrentIndex");
            }
        }
        private int totalCount = 0;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                RaisePropertyChanged("TotalCount");
            }
        }
        private string fileFullPath = "";
        public string FileFullPath
        {
            get { return fileFullPath; }
            set
            {
                fileFullPath = value;
                RaisePropertyChanged("FileFullPath");
            }
        }
        public List<string> FilePathsList = new List<string>();
    }
}
