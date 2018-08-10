using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace CheckWordModel
{
    public class MyFolderDataViewModel : ViewModelBase
    {
        MyFolderDataViewModel _parent;
        public MyFolderDataViewModel(string fileName, string filePath)
        {
            this.FileName = fileName;
            this.FilePath = filePath;
        }
        public SelectFileType _typeSelectFile = SelectFileType.Img;
        public SelectFileType TypeSelectFile
        {
            get
            {
                return _typeSelectFile;
            }
            set
            {
                _typeSelectFile = value;
                if (_typeSelectFile == SelectFileType.Docx)
                {
                    ShowDocTitleLogo = true;
                    ShowImgTitleLogo = false;
                    ShowXlsxTitleLogo = false;
                }
                else if (_typeSelectFile == SelectFileType.Img)
                {
                    ShowDocTitleLogo = false;
                    ShowImgTitleLogo = true;
                    ShowXlsxTitleLogo = false;
                }
                else if (_typeSelectFile == SelectFileType.Xlsx)
                {
                    ShowDocTitleLogo = false;
                    ShowImgTitleLogo = false;
                    ShowXlsxTitleLogo = true;
                }
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
        private string _fileName = "";
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged("FileName");
            }
        }
        private string _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaisePropertyChanged("FilePath");
            }
        }
        private string _dealImageFilePath = "";
        public string DealImageFilePath
        {
            get { return _dealImageFilePath; }
            set
            {
                _dealImageFilePath = value;
                RaisePropertyChanged("DealImageFilePath");
            }
        }
        private string _fileToolTip = "";
        public string FileToolTip
        {
            get { return _fileToolTip; }
            set
            {
                _fileToolTip = value;
                RaisePropertyChanged("FileToolTip");
            }
        }
        private bool _showWeiJinTitleLogo = false;
        public bool ShowWeiJinTitleLogo
        {
            get { return _showWeiJinTitleLogo; }
            set
            {
                _showWeiJinTitleLogo = value;
                RaisePropertyChanged("ShowWeiJinTitleLogo");
            }
        }
        private bool _showNoWeiJinTitleLogo = false;
        public bool ShowNoWeiJinTitleLogo
        {
            get { return _showNoWeiJinTitleLogo; }
            set
            {
                _showNoWeiJinTitleLogo = value;
                RaisePropertyChanged("ShowNoWeiJinTitleLogo");
            }
        }
        private bool _showNoCheckTitleLogo = false;
        public bool ShowNoCheckTitleLogo
        {
            get { return _showNoCheckTitleLogo; }
            set
            {
                _showNoCheckTitleLogo = value;
                RaisePropertyChanged("ShowNoCheckTitleLogo");
            }
        }
        public string _checkResultInfo = "0";//0:无违禁词,1:有违禁词，2:未检测
        public string CheckResultInfo
        {
            get
            {
                return _checkResultInfo;
            }
            set
            {
                _checkResultInfo = value;
                if (_checkResultInfo == "0")
                {
                    ShowNoWeiJinTitleLogo = true;
                    ShowWeiJinTitleLogo = false;
                    ShowNoCheckTitleLogo = false;
                    FileToolTip = "不包含违禁词";
                }
                else if (_checkResultInfo == "1")
                {
                    ShowNoWeiJinTitleLogo = false;
                    ShowWeiJinTitleLogo = true;
                    ShowNoCheckTitleLogo = false;
                    FileToolTip = "点击查看详情";
                }
                else if (_checkResultInfo == "2")
                {
                    ShowNoWeiJinTitleLogo = false;
                    ShowWeiJinTitleLogo = false;
                    ShowNoCheckTitleLogo = true;
                    FileToolTip = "文件已打开，未能检测";
                }
            }
        }
    }
}
