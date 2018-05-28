﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedDetailWordInfo : ViewModelBase
    {
        private bool isUnCheckWord;
        public bool IsUnCheckWord
        {
            get { return isUnCheckWord; }
            set
            {
                isUnCheckWord = value;
                RaisePropertyChanged("IsUnCheckWord");
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
        private string sourceDBImgPath = "";
        public string SourceDBImgPath
        {
            get { return sourceDBImgPath; }
            set
            {
                sourceDBImgPath = value;
                RaisePropertyChanged("SourceDBImgPath");
            }
        }
    }
}
