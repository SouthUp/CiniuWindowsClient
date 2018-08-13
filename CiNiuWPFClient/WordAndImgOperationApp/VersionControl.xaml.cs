using CheckWordEvent;
using CheckWordUtil;
using System;
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
                APIService service = new APIService();
                string apiMinVersion = "";
                string versionInfo = service.GetVersion(out apiMinVersion);
            });
            task.Start();
            await task;
        }
    }
}
