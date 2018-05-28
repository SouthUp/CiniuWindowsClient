using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel.Communication
{
    public class ExchangeBrowseTxTProcessingInfo : ViewModelBase
    {
        /// <summary>
        /// 是否处理完成
        /// </summary>
        public bool IsDealFinished { get; set; }

        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                currentIndex = value;
                RaisePropertyChanged("CurrentIndex");
            }
        }
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                RaisePropertyChanged("TotalCount");
            }
        }
        private string currentFileName = "";
        public string CurrentFileName
        {
            get { return currentFileName; }
            set
            {
                currentFileName = value;
                RaisePropertyChanged("CurrentFileName");
            }
        }
        private int unCheckWordsCount;
        public int UnCheckWordsCount
        {
            get { return unCheckWordsCount; }
            set
            {
                unCheckWordsCount = value;
                RaisePropertyChanged("UnCheckWordsCount");
            }
        }
    }
}
