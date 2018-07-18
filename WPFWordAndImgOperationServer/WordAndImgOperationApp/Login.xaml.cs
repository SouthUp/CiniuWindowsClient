using CheckWordEvent;
using CheckWordUtil;
using Newtonsoft.Json;
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
using WPFClientCheckWordModel;

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
            try
            {
                string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).ProductVersion;
                viewModel.CurrentVersionInfo = version;
                string loginInOutInfos = string.Format(@"{0}\UserLoginInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                if (ui != null && ui.ToString() != "")
                {
                    try
                    {
                        var userLoginInfo = JsonConvert.DeserializeObject<UserLoginInfo>(ui.ToString());
                        if (userLoginInfo != null)
                        {
                            viewModel.IsAutoLogin = userLoginInfo.IsAutoLogin;
                            if (viewModel.IsAutoLogin)
                            {
                                viewModel.UserName = userLoginInfo.UserName;
                                viewModel.PassWord = userLoginInfo.PassWord;
                                LoginIn();
                            }
                        }
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        private void CheckVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CheckVersionMessageEvent>().Publish(true);
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
                UserLoginInfo userLoginInfo = new UserLoginInfo();
                userLoginInfo.UserName = userName;
                userLoginInfo.PassWord = pwd;
                userLoginInfo.IsAutoLogin = isAutoLogin;
                //保存用户登录信息到本地
                string userLoginInfos = string.Format(@"{0}\UserLoginInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(userLoginInfo), userLoginInfos);
            }
            catch (Exception ex)
            { }
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginIn();
            }
        }

        private void FindPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.ciniuwang.com/password");
            }
            catch (Exception ex)
            { }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.ciniuwang.com/register");
            }
            catch (Exception ex)
            { }
        }

        private void LoginBtn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginIn();
            }
        }

        private void CheckSelectToggleBtn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.IsAutoLogin = !viewModel.IsAutoLogin;
            }
        }
    }
}
