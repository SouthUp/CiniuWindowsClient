using CheckWordEvent;
using CheckWordUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : UserControl
    {
        LoginViewModel viewModel = new LoginViewModel();
        public Login()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(viewModel.IsAutoLogin)
            {
                viewModel.UserName = ConfigurationSettings.AppSettings["UserName"].ToString();
                viewModel.PassWord = ConfigurationSettings.AppSettings["PassWord"].ToString();
                LoginIn();
            }
        }

        private void CheckVersionBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginIn();
        }
        /// <summary>
        /// 登陆
        /// </summary>
        private void LoginIn()
        {
            EventAggregatorRepository.EventAggregator.GetEvent<AppBusyIndicatorEvent>().Publish(new AppBusyIndicator() { IsBusy = true });
            System.Threading.ThreadStart startLogin = delegate ()
            {
                APIService service = new APIService();
                string token = service.LoginIn(viewModel.UserName, viewModel.PassWord);
                if (!string.IsNullOrEmpty(token))
                {
                    UtilSystemVar.UserToken = token;
                    UtilSystemVar.UserName = viewModel.UserName;
                    EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginIn");
                    SaveLoginInfo(viewModel.UserName, viewModel.PassWord, viewModel.IsAutoLogin);
                    EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("MainSet");
                    EventAggregatorRepository.EventAggregator.GetEvent<IsCanOpenSearchPopWindowEvent>().Publish(true);
                }
                else
                {
                    viewModel.MessageInfo = "用户名或密码错误";
                }
                EventAggregatorRepository.EventAggregator.GetEvent<AppBusyIndicatorEvent>().Publish(new AppBusyIndicator() { IsBusy = false });
            };
            System.Threading.Thread t = new System.Threading.Thread(startLogin);
            t.IsBackground = true;
            t.Start();
        }
        private void SaveLoginInfo(string userName, string pwd, bool isAutoLogin)
        {
            try
            {
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection appsettings = config.AppSettings;
                if (isAutoLogin)
                {
                    foreach (KeyValueConfigurationElement item in appsettings.Settings)
                    {
                        if (item.Key == "UserName")
                            item.Value = userName;
                        if (item.Key == "PassWord")
                            item.Value = pwd;
                        if (item.Key == "IsAutoLogin")
                            item.Value = "true";
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                }
                else
                {
                    foreach (KeyValueConfigurationElement item in appsettings.Settings)
                    {
                        if (item.Key == "IsAutoLogin")
                            item.Value = "false";
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginIn();
            }
        }
    }
}
