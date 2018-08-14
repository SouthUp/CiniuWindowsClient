using CheckWordModel;
using CheckWordUtil;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WordAndImgOperationApp
{
    public class SettingControlViewModel : NotificationObject
    {
        private string _wordOfficeVersion = "";
        public string WordOfficeVersion
        {
            get { return _wordOfficeVersion; }
            set
            {
                _wordOfficeVersion = value;
                RaisePropertyChanged("WordOfficeVersion");
            }
        }
        private string _excelOfficeVersion = "";
        public string ExcelOfficeVersion
        {
            get { return _excelOfficeVersion; }
            set
            {
                _excelOfficeVersion = value;
                RaisePropertyChanged("ExcelOfficeVersion");
            }
        }
        private bool _hasWordOffice = false;
        public bool HasWordOffice
        {
            get { return _hasWordOffice; }
            set
            {
                _hasWordOffice = value;
                RaisePropertyChanged("HasWordOffice");
            }
        }
        private bool _hasExcelOffice = false;
        public bool HasExcelOffice
        {
            get { return _hasExcelOffice; }
            set
            {
                _hasExcelOffice = value;
                RaisePropertyChanged("HasExcelOffice");
            }
        }
        private bool _hasWordOfficeAddIn = false;
        public bool HasWordOfficeAddIn
        {
            get { return _hasWordOfficeAddIn; }
            set
            {
                _hasWordOfficeAddIn = value;
                RaisePropertyChanged("HasWordOfficeAddIn");
            }
        }
        private bool _hasExcelOfficeAddIn = false;
        public bool HasExcelOfficeAddIn
        {
            get { return _hasExcelOfficeAddIn; }
            set
            {
                _hasExcelOfficeAddIn = value;
                RaisePropertyChanged("HasExcelOfficeAddIn");
            }
        }
        private bool _isCheckPicInDucument = true;
        public bool IsCheckPicInDucument
        {
            get { return _isCheckPicInDucument; }
            set
            {
                _isCheckPicInDucument = value;
                RaisePropertyChanged("IsCheckPicInDucument");
            }
        }
        private bool _isUseCustumCi = false;
        public bool IsUseCustumCi
        {
            get { return _isUseCustumCi; }
            set
            {
                _isUseCustumCi = value;
                RaisePropertyChanged("IsUseCustumCi");
            }
        }
    }
}
