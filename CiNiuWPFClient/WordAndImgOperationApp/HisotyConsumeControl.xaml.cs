using CheckWordEvent;
using CheckWordModel;
using CheckWordUtil;
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
    /// HisotyConsumeControl.xaml 的交互逻辑
    /// </summary>
    public partial class HisotyConsumeControl : UserControl
    {
        HisotyConsumeControlViewModel viewModel = new HisotyConsumeControlViewModel();
        public HisotyConsumeControl()
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
            Task<List<UserMonthConsumeInfo>> task = new Task<List<UserMonthConsumeInfo>>(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                List<UserMonthConsumeInfo> list = new List<UserMonthConsumeInfo>();
                try
                {
                    APIService serviceApi = new APIService();
                    list = serviceApi.GetUserAllHistoryConsumeByToken(UtilSystemVar.UserToken);
                }
                catch (Exception ex)
                { }
                System.Threading.Thread.Sleep(500);
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                return list;
            });
            task.Start();
            await task;
            viewModel.HistoryConsumeInfoList = new System.Collections.ObjectModel.ObservableCollection<UserMonthConsumeInfo>(task.Result.ToList());
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CloseSettingWindowPopGridViewEvent>().Publish(true);
        }
    }
}
