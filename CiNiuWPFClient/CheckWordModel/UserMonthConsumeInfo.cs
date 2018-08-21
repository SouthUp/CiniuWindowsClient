using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordModel
{
    public class UserMonthConsumeInfo : ViewModelBase
    {
        private int totalCount = 0;
        public int TotalConsumeCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                RaisePropertyChanged("TotalCount");
            }
        }
        private int wordConsumeCount = 0;
        public int WordConsumeCount
        {
            get { return wordConsumeCount; }
            set
            {
                wordConsumeCount = value;
                RaisePropertyChanged("WordConsumeCount");
            }
        }
        private int picConsumeCount = 0;
        public int PicConsumeCount
        {
            get { return picConsumeCount; }
            set
            {
                picConsumeCount = value;
                RaisePropertyChanged("PicConsumeCount");
            }
        }
    }
}
