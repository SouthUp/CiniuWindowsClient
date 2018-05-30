using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedDetailWordInfo : ViewModelBase
    {
        private string sourceDBYear = "";
        public string SourceDBPublishtime
        {
            get { return sourceDBYear; }
            set
            {
                sourceDBYear = value;
                RaisePropertyChanged("SourceDBPublishtime");
            }
        }
        private string sourceDBID = "";
        public string SourceDBID
        {
            get { return sourceDBID; }
            set
            {
                sourceDBID = value;
                RaisePropertyChanged("SourceDBID");
            }
        }
        private string id = "";
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }
        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        private string nameType = "";
        public string NameType
        {
            get { return nameType; }
            set
            {
                nameType = value;
                RaisePropertyChanged("NameType");
            }
        }
        private string numberLine = "";
        public string NumberLine
        {
            get { return numberLine; }
            set
            {
                numberLine = value;
                RaisePropertyChanged("NumberLine");
            }
        }
        private string sourceDB = "";
        public string SourceDB
        {
            get { return sourceDB; }
            set
            {
                sourceDB = value;
                RaisePropertyChanged("SourceDB");
            }
        }
        public Range UnCheckWordRange { get; set; }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isExpand = false;
        public bool IsExpand
        {
            get { return _isExpand; }
            set
            {
                _isExpand = value;
                RaisePropertyChanged("IsExpand");
            }
        }
    }
}
