using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UserStateInfos : ViewModelBase
    {
        private bool active = false;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                RaisePropertyChanged("Active");
            }
        }
        private string activeName = "";
        public string ActiveName
        {
            get { return activeName; }
            set
            {
                activeName = value;
                RaisePropertyChanged("ActiveName");
            }
        }
        private int pointCount = 0;
        public int PointCount
        {
            get { return pointCount; }
            set
            {
                pointCount = value;
                if (pointCount != 0)
                {
                    PointCountStr = pointCount.ToString();
                }
                RaisePropertyChanged("PointCount");
            }
        }
        private string pointCountStr = "";
        public string PointCountStr
        {
            get { return pointCountStr; }
            set
            {
                pointCountStr = value;
                RaisePropertyChanged("PointCountStr");
            }
        }
        private int picCount = 0;
        public int PicCount
        {
            get { return picCount; }
            set
            {
                picCount = value;
                RaisePropertyChanged("PicCount");
            }
        }
        private int wordCount = 0;
        public int WordCount
        {
            get { return wordCount; }
            set
            {
                wordCount = value;
                RaisePropertyChanged("WordCount");
            }
        }
        private DateTime expiredDate;
        public DateTime ExpiredDate
        {
            get { return expiredDate; }
            set
            {
                expiredDate = value;
                RaisePropertyChanged("ExpiredDate");
            }
        }
        private string expiredDateStr;
        public string ExpiredDateStr
        {
            get { return expiredDateStr; }
            set
            {
                expiredDateStr = value;
                RaisePropertyChanged("ExpiredDateStr");
            }
        }
    }
}
