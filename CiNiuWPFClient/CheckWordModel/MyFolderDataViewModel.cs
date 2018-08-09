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
        private string _fileImgShowPath = "";
        public string FileImgShowPath
        {
            get { return _fileImgShowPath; }
            set
            {
                _fileImgShowPath = value;
                RaisePropertyChanged("FileImgShowPath");
            }
        }
        private bool _hasError = false;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                _hasError = value;
                RaisePropertyChanged("HasError");
            }
        }
    }
}
