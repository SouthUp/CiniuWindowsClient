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
    }
}
