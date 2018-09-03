using CheckWordEvent;
using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// FindPsw.xaml 的交互逻辑
    /// </summary>
    public partial class FindPsw : UserControl
    {
        FindPswViewModel viewModel = new FindPswViewModel();
        public FindPsw()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (timersTimer != null)
                {
                    timersTimer.Stop();
                }
            }
            catch (Exception ex)
            { }
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
            viewModel.MessageInfo = "";
            viewModel.UserName = UserNameTextBox.Text;
            viewModel.YZMStr = YZMTextBox.Text;
            if (!FindPswCheckPhoneAndCodePass(viewModel.UserName, viewModel.YZMStr, viewModel.PassWord, viewModel.NewPassWord)) return;
            EventAggregatorRepository.EventAggregator.GetEvent<AppBusyIndicatorEvent>().Publish(new AppBusyIndicator() { IsBusy = true });
            System.Threading.ThreadStart startLogin = delegate ()
            {
                APIService service = new APIService();
                string messageInfo = "";
                string token = service.FindPsw(viewModel.UserName, viewModel.PassWord, viewModel.YZMStr, out messageInfo);
                if (!string.IsNullOrEmpty(token))
                {
                    UtilSystemVar.UserToken = token;
                    UtilSystemVar.UserName = viewModel.UserName;
                    EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginIn");
                    EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("MainWindow");
                    EventAggregatorRepository.EventAggregator.GetEvent<CloseLoginWindowViewEvent>().Publish(true);
                }
                else
                {
                    bool netState = GetCurrentNetState();
                    if (!netState)
                    {
                        viewModel.MessageInfo = "网络异常";
                    }
                    else
                    {
                        viewModel.MessageInfo = messageInfo;
                    }
                }
                EventAggregatorRepository.EventAggregator.GetEvent<AppBusyIndicatorEvent>().Publish(new AppBusyIndicator() { IsBusy = false });
            };
            System.Threading.Thread t = new System.Threading.Thread(startLogin);
            t.IsBackground = true;
            t.Start();
        }
        private bool GetCurrentNetState()
        {
            bool result = true;
            try
            {
                using (Ping ping = new Ping())
                {
                    int timeout = 3000;
                    PingReply reply = ping.Send("www.baidu.com", timeout);
                    if (reply == null || reply.Status != IPStatus.Success)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public System.Timers.Timer timersTimer;
        int countTime = 60;
        private void SendYZMBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.MessageInfo = "";
            viewModel.UserName = UserNameTextBox.Text;
            if (!CheckPhonePass(viewModel.UserName)) return;
            viewModel.IsSendYZMBtnEnabled = false;
            System.Threading.ThreadStart startLogin = delegate ()
            {
                APIService service = new APIService();
                string resultSendYZM = service.RegisterSendYZM(viewModel.UserName, "FindPsw");
                if (resultSendYZM.ToLower() == "ok")
                {
                    viewModel.SendYZMBtnContentTime = "60s";
                    viewModel.SendYZMBtnContent = "重新发送验证码";
                    viewModel.MessageInfo = "验证码已发送,请注意查收";
                    timersTimer = new System.Timers.Timer();
                    timersTimer.Interval = 1000;
                    timersTimer.Elapsed += TimersTimer_Elapsed;
                    timersTimer.Start();
                }
                else
                {
                    viewModel.MessageInfo = resultSendYZM;
                    viewModel.IsSendYZMBtnEnabled = true;
                }
            };
            System.Threading.Thread t = new System.Threading.Thread(startLogin);
            t.IsBackground = true;
            t.Start();
        }
        private void TimersTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            countTime--;
            viewModel.SendYZMBtnContentTime = countTime.ToString() + "s";
            if (countTime == 0)
            {
                timersTimer.Stop();
                viewModel.IsSendYZMBtnEnabled = true;
                viewModel.SendYZMBtnContentTime = "";
                viewModel.SendYZMBtnContent = "重新发送验证码";
                countTime = 60;
            }
        }
        private bool CheckPhonePass(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                viewModel.MessageInfo = "请输入手机号码";
                return false;
            }
            else
            {
                Regex regex = new Regex(@"^1\d{10}$");// new Regex(@"^1(3|4|5|7|8)\d{9}$");
                if (!regex.IsMatch(userName))
                {
                    viewModel.MessageInfo = "请输入正确的手机号";
                    return false;
                }
            }
            return true;
        }
        private bool FindPswCheckPhoneAndCodePass(string userName, string code,string psw,string newPsw)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                viewModel.MessageInfo = "请输入手机号码";
                return false;
            }
            else
            {
                Regex regex = new Regex(@"^1\d{10}$");// new Regex(@"^1(3|4|5|7|8)\d{9}$");
                if (!regex.IsMatch(userName))
                {
                    viewModel.MessageInfo = "请输入正确的手机号";
                    return false;
                }
            }
            if (String.IsNullOrWhiteSpace(code))
            {
                viewModel.MessageInfo = "请输入验证码";
                return false;
            }
            if (String.IsNullOrWhiteSpace(newPsw))
            {
                viewModel.MessageInfo = "请输入重置密码";
                return false;
            }
            else
            {
                if (newPsw.Length < 6 || newPsw.Length > 20)
                {
                    viewModel.MessageInfo = "密码长度为6-20位";
                    return false;
                }
            }
            if (String.IsNullOrWhiteSpace(psw))
            {
                viewModel.MessageInfo = "请输入确认密码";
                return false;
            }
            if (psw != newPsw)
            {
                viewModel.MessageInfo = "确认密码与重置密码不一致";
                return false;
            }
            return true;
        }

        private void LoginBtn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginIn();
            }
        }

        private void UserNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UserNameTextBoxBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673ab7"));
            UserNameTextBoxBorder.Opacity = 0.54;
        }

        private void UserNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UserNameTextBoxBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            UserNameTextBoxBorder.Opacity = 0.06;
        }

        private void YZMTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            YZMTextBoxBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673ab7"));
            YZMTextBoxBorder.Opacity = 0.54;
        }

        private void YZMTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            YZMTextBoxBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            YZMTextBoxBorder.Opacity = 0.06;
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673ab7"));
            PasswordBorder.Opacity = 0.54;
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            PasswordBorder.Opacity = 0.06;
        }
        private void NewPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            NewPasswordBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673ab7"));
            NewPasswordBorder.Opacity = 0.54;
        }

        private void NewPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            NewPasswordBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            NewPasswordBorder.Opacity = 0.06;
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoadLoginContentGridViewEvent>().Publish("LoginControl");
        }
    }
}
