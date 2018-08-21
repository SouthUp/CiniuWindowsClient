using CheckWordEvent;
using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// ConsumeSearch.xaml 的交互逻辑
    /// </summary>
    public partial class ConsumeSearch : UserControl
    {
        ConsumeSearchViewModel viewModel = new ConsumeSearchViewModel();
        public ConsumeSearch()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadSettingWindowGridViewEvent>().Publish("UserInfoControl");
        }

        private void ConsumeHistoryBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ConsumeStandardBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
