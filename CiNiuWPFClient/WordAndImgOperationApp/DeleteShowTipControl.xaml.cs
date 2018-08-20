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
    /// DeleteShowTipControl.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteShowTipControl : UserControl
    {
        CustumCiInfo info;
        DeleteShowTipControlViewModel viewModel = new DeleteShowTipControlViewModel();
        public DeleteShowTipControl(CustumCiInfo info)
        {
            InitializeComponent();
            this.info = info;
            this.DataContext = viewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.NameInfo = info.Name;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CloseSettingWindowPopGridViewEvent>().Publish(true);
        }
        private void SureBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CloseSettingWindowPopGridViewEvent>().Publish(true);
            EventAggregatorRepository.EventAggregator.GetEvent<DeleteCustumCiEvent>().Publish(info);
        }
    }
}
