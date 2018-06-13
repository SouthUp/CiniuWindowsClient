﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckWordControl
{
    /// <summary>
    /// ImageDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDetailControl : UserControl
    {
        string filePath = "";
        public ImageDetailControl(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Img.Source = CheckWordUtil.Util.GetBitmapImage(filePath);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Img.Source = null;
        }
    }
}