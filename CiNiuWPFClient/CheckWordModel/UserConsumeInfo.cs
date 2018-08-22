using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordModel
{
    public class UserConsumeInfo : ViewModelBase
    {
        private string dataTimeStr = "";
        public string DataTimeStr
        {
            get { return dataTimeStr; }
            set
            {
                dataTimeStr = value;
                RaisePropertyChanged("DataTimeStr");
            }
        }
        private string name = "";
        public string FileName
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("FileName");
            }
        }
        private string wordConsumeCount = "";
        public string WordConsumeCount
        {
            get { return wordConsumeCount; }
            set
            {
                wordConsumeCount = value;
                RaisePropertyChanged("WordConsumeCount");
            }
        }
        private string picConsumeCount = "";
        public string PicConsumeCount
        {
            get { return picConsumeCount; }
            set
            {
                picConsumeCount = value;
                RaisePropertyChanged("PicConsumeCount");
            }
        }
        private bool _showImgTitleLogo = false;
        public bool ShowImgTitleLogo
        {
            get { return _showImgTitleLogo; }
            set
            {
                _showImgTitleLogo = value;
                RaisePropertyChanged("ShowImgTitleLogo");
            }
        }
        private bool _showDocTitleLogo = false;
        public bool ShowDocTitleLogo
        {
            get { return _showDocTitleLogo; }
            set
            {
                _showDocTitleLogo = value;
                RaisePropertyChanged("ShowDocTitleLogo");
            }
        }
        private bool _showXlsxTitleLogo = false;
        public bool ShowXlsxTitleLogo
        {
            get { return _showXlsxTitleLogo; }
            set
            {
                _showXlsxTitleLogo = value;
                RaisePropertyChanged("ShowXlsxTitleLogo");
            }
        }
    }
}
