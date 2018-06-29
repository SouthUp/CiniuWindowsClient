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
    }
}
