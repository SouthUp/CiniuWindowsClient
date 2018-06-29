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
                RaisePropertyChanged("PointCount");
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
