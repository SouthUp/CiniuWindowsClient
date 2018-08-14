using CheckWordEvent;
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
    /// VersionControl.xaml 的交互逻辑
    /// </summary>
    public partial class VersionControl : UserControl
    {
        VersionControlViewModel viewModel = new VersionControlViewModel();
        public VersionControl()
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
                    string version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).ProductVersion;
                    viewModel.CurrentVersionInfo = version;
                    viewModel.CurrentVersionTimeInfo = System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetEntryAssembly().Location).ToString("yyyy.MM.dd");
                    APIService service = new APIService();
                    VersionResponse versionResponse = service.GetVersionInfo();
                    if (versionResponse != null && !string.IsNullOrEmpty(versionResponse.latestClient))
                    {
                        viewModel.NewVersionInfo = versionResponse.latestClient;
                        if(versionResponse.descriptionInfos !=null)
                        {
                            foreach (var itemInfo in versionResponse.descriptionInfos)
                            {
                                Dispatcher.Invoke(new Action(() => {
                                    viewModel.DiscriptionInfos.Add(new CheckWordModel.VersionInfo { DiscriptionInfo = itemInfo });
                                }));
                            }
                        }
                        if (versionResponse.time != null)
                        {
                            viewModel.NewVersionTimeInfo = versionResponse.time.ToString("yyyy.MM.dd");
                        }
                        if (new Version(viewModel.NewVersionInfo) > new Version(version))
                        {
                            viewModel.UpdateBtnVisibility = Visibility.Visible;
                            viewModel.UpdateTipsVisibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("60020");
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

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.ciniuwang.com/download");
            }
            catch (Exception ex)
            { }
        }
    }
}
