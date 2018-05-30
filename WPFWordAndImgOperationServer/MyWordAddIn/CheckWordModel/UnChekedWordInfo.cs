﻿using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedWordInfo : ViewModelBase
    {
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
        private ObservableCollection<UnChekedWordInfo> children = new ObservableCollection<UnChekedWordInfo>();
        public ObservableCollection<UnChekedWordInfo> Children
        {
            get { return children; }
            set
            {
                children = value;
                RaisePropertyChanged("Children");
            }
        }
        public Range Range { get; set; }
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
        private int warningCount = 0;
        public int WarningCount
        {
            get
            {
                warningCount = Children.Count;
                return warningCount;
            }
            set
            {
                warningCount = value;
                RaisePropertyChanged("WarningCount");
            }
        }
        private string typeTextFrom = "Text";
        public string TypeTextFrom
        {
            get { return typeTextFrom; }
            set
            {
                typeTextFrom = value;
                RaisePropertyChanged("TypeTextFrom");
            }
        }
        public void Initialize()
        {
            foreach (UnChekedWordInfo child in this.Children)
            {
                child.Initialize();
            }
        }
    }
}
