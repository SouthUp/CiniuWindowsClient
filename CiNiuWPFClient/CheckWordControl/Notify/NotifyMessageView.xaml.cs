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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CheckWordControl.Notify
{
    /// <summary>
    /// NotifyMessageView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyMessageView : Window
    {
        public NotifyMessageView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDownDrag(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            { }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = this.DataContext as NotifyMessageViewModel;
                if (viewModel != null && viewModel.Message.ErrorCode == "500")
                {
                    RechargeBtn.Visibility = Visibility.Visible;
                    ErrorCodeStackPanel.Visibility = Visibility.Collapsed;
                }
                else if (viewModel != null && viewModel.Message.ErrorCode == "60010")
                {
                    ErrorCodeStackPanel.Visibility = Visibility.Collapsed;
                }
                else if (viewModel != null && viewModel.Message.ErrorCode == "60030")
                {
                    DownLoadVersionBtn.Visibility = Visibility.Visible;
                    ErrorCodeStackPanel.Visibility = Visibility.Collapsed;
                }
            }
            catch
            { }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = this.DataContext as NotifyMessageViewModel;
                if (viewModel != null)
                {
                    viewModel._closeAction();
                }
            }
            catch (Exception ex)
            { }
            this.Close();
        }
        private void DownLoadVersionBtn_Click(object sender, RoutedEventArgs e)
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
