using CheckWordEvent;
using CheckWordUtil;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// UserInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfoControl : UserControl
    {
        UserInfoControlViewModel viewModel = new UserInfoControlViewModel();
        public UserInfoControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }
        private async void InitData()
        {
            Task task = new Task(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                try
                {
                    viewModel.UserName = UtilSystemVar.UserName;
                    APIService service = new APIService();
                    var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                    if (userStateInfos != null)
                    {
                        viewModel.PointCount = userStateInfos.PointCountStr;
                    }
                }
                catch (Exception ex)
                { }
                System.Threading.Thread.Sleep(500);
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
            });
            task.Start();
            await task;
        }

        private void FindPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("SettingFindPsw");
        }

        private void ConsumeSeachBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("ConsumeSearch");
        }
        private void RechargeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.ciniuwang.com/pay");
            }
            catch (Exception ex)
            { }
        }
    }
}
