using CheckWordEvent;
using CheckWordUtil;
using Newtonsoft.Json;
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
using System.Windows.Shapes;
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginWindowViewModel viewModel = new LoginWindowViewModel();
        public WindowState windowState;
        public LoginWindow()
        {
            InitializeComponent();
            windowState = this.WindowState;
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<AppBusyIndicatorEvent>().Subscribe(ReceiveBusyIndicator);
            EventAggregatorRepository.EventAggregator.GetEvent<LoadLoginContentGridViewEvent>().Subscribe(LoadLoginContentGridView);
            EventAggregatorRepository.EventAggregator.GetEvent<CloseLoginWindowViewEvent>().Subscribe(CloseLoginWindow);
        }
        private void ReceiveBusyIndicator(AppBusyIndicator busyindicator)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    if (busyindicator.IsBusy)
                    {
                        viewModel.BusyWindowVisibility = Visibility.Visible;
                    }
                    else
                    {
                        viewModel.BusyWindowVisibility = Visibility.Collapsed;
                    }
                    this.viewModel.BusyContent = busyindicator.BusyContent;
                }));
            }
            catch (Exception ex)
            { }
        }
        private void LoadLoginContentGridView(string typeName)
        {
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    ContentGrid.Children.Clear();
                    if (typeName == "LoginControl")
                    {
                        Login login = new Login();
                        ContentGrid.Children.Add(login);
                    }
                    else if (typeName == "RegisterControl")
                    {
                        Register register = new Register();
                        ContentGrid.Children.Add(register);
                    }
                    else if (typeName == "FindPswControl")
                    {
                        FindPsw findPsw = new FindPsw();
                        ContentGrid.Children.Add(findPsw);
                    }
                }
                catch (Exception ex)
                { }
            }));
        }
        private void CloseLoginWindow(bool b)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.Close();
            }));
        }
        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = true;
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CloseMyAppEvent>().Publish(true);
            //this.WindowState = WindowState.Minimized;
            //this.Hide();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                windowState = this.WindowState;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
            EventAggregatorRepository.EventAggregator.GetEvent<LoadLoginContentGridViewEvent>().Publish("LoginControl");
        }
    }
}
