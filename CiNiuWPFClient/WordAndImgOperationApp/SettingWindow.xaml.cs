using CheckWordEvent;
using CheckWordModel;
using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        SettingWindowViewModel viewModel = new SettingWindowViewModel();
        string typeBtn = "";
        public SettingWindow(string type)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.typeBtn = type;
        }
        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
            if (typeBtn == "UserInfo")
            {
                UserInfoBtn.IsChecked = true;
            }
            else if (typeBtn == "CustumCi")
            {
                CustumCiBtn.IsChecked = true;
            }
            else if (typeBtn == "Setting")
            {
                SettingBtn.IsChecked = true;
            }
        }

        private void UserInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectInfoTextBlock.Text = "用户信息";
        }

        private void CustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectInfoTextBlock.Text = "自建词条";
        }
        private void VersionBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectInfoTextBlock.Text = "升级";
        }
        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectInfoTextBlock.Text = "关于";
        }
        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectInfoTextBlock.Text = "设置";
        }
    }
}
