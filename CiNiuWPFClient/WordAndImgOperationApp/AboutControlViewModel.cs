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
    public class AboutControlViewModel : NotificationObject
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
    }
}
