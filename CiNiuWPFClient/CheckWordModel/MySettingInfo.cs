﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordModel
{
    public class MySettingInfo : ViewModelBase
    {
        private bool isCheckPicInDucument = true;
        public bool IsCheckPicInDucument
        {
            get { return isCheckPicInDucument; }
            set
            {
                isCheckPicInDucument = value;
                RaisePropertyChanged("IsCheckPicInDucument");
            }
        }
        private bool isUseCustumCi = false;
        public bool IsUseCustumCi
        {
            get { return isUseCustumCi; }
            set
            {
                isUseCustumCi = value;
                RaisePropertyChanged("IsUseCustumCi");
            }
        }
    }
}