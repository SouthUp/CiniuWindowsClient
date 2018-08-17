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
    /// EditCustumCiControl.xaml 的交互逻辑
    /// </summary>
    public partial class EditCustumCiControl : UserControl
    {
        EditCustumCiControlViewModel viewModel = new EditCustumCiControlViewModel();
        public EditCustumCiControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }

        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<ReturnToCustumCiViewEvent>().Publish(true);
        }
        private async void InitData()
        {
            Task<List<CustumCiInfo>> task = new Task<List<CustumCiInfo>>(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                List<CustumCiInfo> list = new List<CustumCiInfo>();
                try
                {
                    APIService serviceApi = new APIService();
                    list = serviceApi.GetUserCustumCiByToken(UtilSystemVar.UserToken);
                }
                catch (Exception ex)
                { }
                System.Threading.Thread.Sleep(500);
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                return list;
            });
            task.Start();
            await task;
            viewModel.CustumCiInfoList = new System.Collections.ObjectModel.ObservableCollection<CustumCiInfo>(task.Result.ToList());
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var custumCiInfo = btn.Tag as CustumCiInfo;
                custumCiInfo.IsSelected = true;
                foreach (var item in viewModel.CustumCiInfoList)
                {
                    if (item.ID != custumCiInfo.ID)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var custumCiInfo = btn.Tag as CustumCiInfo;
                custumCiInfo.IsSelected = true;
                foreach (var item in viewModel.CustumCiInfoList)
                {
                    if (item.ID != custumCiInfo.ID)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var custumCiInfo = grid.Tag as CustumCiInfo;
                custumCiInfo.IsSelected = true;
                foreach (var item in viewModel.CustumCiInfoList)
                {
                    if (item.ID != custumCiInfo.ID)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }
    }
}
