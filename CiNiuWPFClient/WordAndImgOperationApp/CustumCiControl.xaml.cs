using CheckWordEvent;
using CheckWordModel;
using CheckWordUtil;
using Microsoft.Win32;
using Newtonsoft.Json;
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
    /// CustumCiControl.xaml 的交互逻辑
    /// </summary>
    public partial class CustumCiControl : UserControl
    {
        CustumCiControlViewModel viewModel = new CustumCiControlViewModel();
        public CustumCiControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<ReturnToCustumCiViewEvent>().Subscribe(ReturnToCustumCiView);
        }
        private void ReturnToCustumCiView(bool b)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    ContentGrid.Children.Clear();
                    viewModel.CustumCiGridVisibility = Visibility.Visible;
                    viewModel.ContentGridVisibility = Visibility.Collapsed;
                }));
            }
            catch (Exception ex)
            { }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void EditCustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    EditCustumCiControl editCustumCiControl = new EditCustumCiControl();
                    ContentGrid.Children.Add(editCustumCiControl);
                    viewModel.CustumCiGridVisibility = Visibility.Collapsed;
                    viewModel.ContentGridVisibility = Visibility.Visible;
                }
                catch (Exception ex)
                { }
            }));
        }

        private async void SureToCustumCiTiaoBtn_Click(object sender, RoutedEventArgs e)
        {
            //调用接口添加自建词库
            Task<bool> task = new Task<bool>(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                bool b = false;
                try
                {
                    APIService service = new APIService();
                    b = service.AddCustumCiTiaoByToken(UtilSystemVar.UserToken, viewModel.SearchText, viewModel.DiscriptionSearchText);
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex)
                { }
                if (b)
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                }
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                return b;
            });
            task.Start();
            await task;
            if (task.Result)
            {
                ShowTipsInfo("添加自建词条成功");
            }
            else
            {
                ShowTipsInfo("添加自建词条失败");
            }
        }
        private void ShowTipsInfo(string tipsInfo)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    this.viewModel.MessageTipInfo = tipsInfo;
                    viewModel.MessageTipVisibility = Visibility.Visible;
                    Task task = new Task(() => {
                        System.Threading.Thread.Sleep(2000);
                        viewModel.MessageTipVisibility = Visibility.Collapsed;
                    });
                    task.Start();
                }));
            }
            catch (Exception ex)
            { }
        }

        private void DownLoadBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ImportCustumCiBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
