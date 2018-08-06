using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
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
using WPFClientService;

namespace ConsoleWPFClientServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceHost hostHeart = new ServiceHost(typeof(WPFClientCheckWordService));
                hostHeart.Open();
                ServiceHost hostMessage = new ServiceHost(typeof(MessageService));
                hostMessage.Open();
                this.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            { }
        }
    }
}
