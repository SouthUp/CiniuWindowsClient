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
            EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Subscribe(SettingWindowBusyIndicator);
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Subscribe(LoadSettingWindowGridView);
        }
        private void SettingWindowBusyIndicator(AppBusyIndicator busyindicator)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    if (busyindicator.IsBusy)
                    {
                        viewModel.BusyWindowVisibility = Visibility.Visible;
                    }
                    else
                    {
                        viewModel.BusyWindowVisibility = Visibility.Collapsed;
                    }
                }));
            }
            catch (Exception ex)
            { }
        }
        private void LoadSettingWindowGridView(string typeName)
        {
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    ContentGrid.Children.Clear();
                    if (typeName == "VersionControl")
                    {
                        VersionControl versionControl = new VersionControl();
                        ContentGrid.Children.Add(versionControl);
                    }
                    else if (typeName == "SettingControl")
                    {
                        SettingControl settingControl = new SettingControl();
                        ContentGrid.Children.Add(settingControl);
                    }
                    else if (typeName == "UserInfoControl")
                    {
                        UserInfoControl userInfoControl = new UserInfoControl();
                        ContentGrid.Children.Add(userInfoControl);
                    }
                }
                catch (Exception ex)
                { }
            }));
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
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("UserInfoControl");
        }

        private void CustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("CustumCiControl");
        }
        private void VersionBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("VersionControl");
        }
        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("AboutControl");
        }
        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("SettingControl");
        }
    }
}
