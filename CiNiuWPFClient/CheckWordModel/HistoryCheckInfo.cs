using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordModel
{
    public class HistoryCheckInfo : ViewModelBase
    {
        public DateTime LastWriteTime { get; set; }
        public string Type { get; set; }

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
        private bool isDelete = false;
        public bool IsDelete
        {
            get { return isDelete; }
            set
            {
                isDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }
        private bool isModify = false;
        public bool IsModify
        {
            get { return isModify; }
            set
            {
                isModify = value;
                RaisePropertyChanged("IsModify");
            }
        }
    }
}
