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
        private string monthName = "";
        public string MonthName
        {
            get { return monthName; }
            set
            {
                monthName = value;
                RaisePropertyChanged("MonthName");
            }
        }
        private int totalCount = 0;
        public int TotalConsumeCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                RaisePropertyChanged("TotalConsumeCount");
            }
        }
        private Visibility showBackground = Visibility.Collapsed;
        public Visibility ShowBackground
        {
            get { return showBackground; }
            set
            {
                showBackground = value;
                RaisePropertyChanged("ShowBackground");
            }
        }
        private int gridHeight = 32;
        public int GridHeight
        {
            get { return gridHeight; }
            set
            {
                gridHeight = value;
                RaisePropertyChanged("GridHeight");
            }
        }
    }
}
