using CheckWordEvent;
using CheckWordModel;
using CheckWordModel.Communication;
using CheckWordUtil;
using IWPFClientService;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using WPFClientCheckWordModel;
using WPFClientCheckWordUtil;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, ICallBackServices, IShell
    {
        WindowState windowState;
        private string CheckWordTempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTemp";
        NotifyIcon notifyIcon;
        WPFOfficeWindowViewModel viewModel = new WPFOfficeWindowViewModel();
        System.Threading.Thread thCheckNetConn;
        public MainWindow()
        {
            InitializeComponent();
            SetIcon();
            windowState = this.WindowState;
            this.DataContext = viewModel;
            this.Left = SystemParameters.WorkArea.Width - 352;
            this.Top = SystemParameters.WorkArea.Height - 130;
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Subscribe(InitContentGridView);
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Subscribe(LoginInOrOut);
            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Subscribe(SendNotifyMessage);
            EventAggregatorRepository.EventAggregator.GetEvent<CheckVersionMessageEvent>().Subscribe(CheckVersionMessage);
            EventAggregatorRepository.EventAggregator.GetEvent<CloseMyAppEvent>().Subscribe(CloseMyApp);
            RegisterWcfService();
            GetVersionInfo();
        }
        private void CloseMyApp(bool b)
        {
            MenuExit_Click(null, null);
        }
        private async void CheckVersionMessage(bool b)
        {
            try
            {
                string newVersion = await GetNewVersionInfo();
            }
            catch (Exception ex)
            { }
        }
        private void SendNotifyMessage(string errorCode)
        {
            try
            {
                string bodyText = "";
                switch (errorCode)
                {
                    case "100":
                        bodyText = "内存数据异常";
                        break;
                    case "200":
                        bodyText = "服务器异常";
                        break;
                    case "300":
                        bodyText = "网络异常";
                        SetIconToolTip("词牛（网络异常）", "MyAppError.ico");
                        break;
                    case "4001":
                        bodyText = "服务器数据获取失败";
                        break;
                    case "4002":
                        bodyText = "数据解析错误";
                        break;
                    case "4003":
                        bodyText = "词库获取错误";
                        SetIconToolTip("词牛（数据异常）", "MyAppError.ico");
                        break;
                    case "4004":
                        bodyText = "词库解析错误";
                        break;
                    case "500":
                        bodyText = "剩余点数不足";
                        break;
                    case "60010":
                        bodyText = "已是最新版本";
                        break;
                    case "60020":
                        bodyText = "获取最新版本失败";
                        break;
                    case "60030":
                        bodyText = "检测到新版本，请升级";
                        break;
                    case "60040":
                        bodyText = "当前版本已不可用，请升级";
                        break;
                }
                if (!string.IsNullOrEmpty(bodyText))
                {
                    var notifyMessage = new CheckWordControl.Notify.NotifyMessage(bodyText, errorCode, null);
                    CheckWordControl.Notify.NotifyMessageManager.Current.EnqueueMessage(notifyMessage);
                }
            }
            catch (Exception ex)
            { }
        }
        private void InitContentGridView(string typeName)
        {
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    if (typeName == "Login")
                    {
                        CloseBtn_Click(null, null);
                        CloseLoginView();
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        loginWindow.Activate();
                    }
                    else if (typeName == "MainWindow")
                    {
                        this.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            this.Show();
                            System.Windows.Application.Current.MainWindow.Activate();
                            this.WindowState = windowState;
                        }));
                        viewModel.MenueUnLoginVisibility = Visibility.Collapsed;
                        viewModel.MenueLoginVisibility = Visibility.Visible;
                        viewModel.UserName = UtilSystemVar.UserName;
                        try
                        {
                            if (thCheckNetConn == null)
                            {
                                thCheckNetConn = new System.Threading.Thread(CheckNetConn);
                                thCheckNetConn.IsBackground = true;
                                thCheckNetConn.Start();
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                catch (Exception ex)
                { }
            }));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CloseBtn_Click(null, null);
        }
        private void GetVersionInfo()
        {
            Task task = new Task(() => {
                try
                {
                    string version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).ProductVersion;
                    viewModel.CurrentVersionInfo = version;
                    APIService service = new APIService();
                    string apiMinVersion = "";
                    string newVersion = service.GetVersion(out apiMinVersion);
                    if (!string.IsNullOrEmpty(newVersion))
                    {
                        System.Threading.Thread.Sleep(1500);
                        if (new Version(apiMinVersion) > new Version(ConfigurationManager.AppSettings["APIVersion"].ToString()))
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("60040");
                        }
                        else
                        {
                            if (new Version(newVersion) > new Version(version))
                            {
                                viewModel.NewVersionInfo = newVersion;
                                EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("60030");
                            }
                        }
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("60020");
                    }
                }
                catch (Exception ex)
                { }
            });
            task.Start();
        }
        private async Task<string> GetNewVersionInfo()
        {
            string result = "";
            Task<string> task = new Task<string>(() => {
                APIService service = new APIService();
                string apiMinVersion = "";
                string versionInfo = service.GetVersion(out apiMinVersion);
                return versionInfo;
            });
            task.Start();
            await task;
            if (!string.IsNullOrEmpty(task.Result))
            {
                result = task.Result;
            }
            return result;
        }
        private void CheckNetConn()
        {
            while (true)
            {
                try
                {
                    bool result = GetCurrentNetState();
                    if (result)
                    {
                        if (this.notifyIcon.Text.Contains("网络异常"))
                        {
                            SetIconToolTip("词牛（已登录）");
                            try
                            {
                                Dispatcher.Invoke(new Action(() => {
                                    foreach (Window win in App.Current.Windows)
                                    {
                                        if (win != this && win.Title == "NotifyMessageView")
                                        {
                                            var viewModel = win.DataContext as CheckWordControl.Notify.NotifyMessageViewModel;
                                            if (viewModel != null && viewModel.Message.ErrorCode == "300")
                                            {
                                                viewModel._closeAction();
                                                win.Close();
                                                break;
                                            }
                                        }
                                    }
                                }));
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(500);
                        result = GetCurrentNetState();
                        if (!result)
                        {
                            System.Threading.Thread.Sleep(500);
                            result = GetCurrentNetState();
                            if (!result)
                            {
                                EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("300");
                            }
                        }
                    }
                }
                catch (Exception ex)
                { }
                System.Threading.Thread.Sleep(1000);
            }
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
        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.Hide();
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            CancelCheckNet();
        }
        private void CancelCheckNet()
        {
            try
            {
                if (thCheckNetConn != null)
                {
                    thCheckNetConn.Abort();
                    thCheckNetConn = null;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    thCheckNetConn = null;
                }
                catch
                { }
            }
        }
        private void SetIcon()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Text = "词牛（未登录）";
            this.notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "Resources/MyApp.ico");//程序图标
            this.notifyIcon.Visible = true;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(OnNotifyIconClick);
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        }
        /// <summary>
        /// 鼠标双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果鼠标左键双击    
            if (e.Button == MouseButtons.Left
                && viewModel.MenueLoginVisibility == Visibility.Visible)
            {
                this.Show();
                this.Activate();
                this.WindowState = windowState;
            }
        }
        /// <summary>    
        /// 鼠标单击    
        /// </summary>    
        /// <param name="sender"></param>    
        /// <param name="e"></param>
        private void OnNotifyIconClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果鼠标左键单击    
            if (e.Button == MouseButtons.Right)
            {
                if(viewModel.MenueLoginVisibility == Visibility.Visible)
                {
                    new Task(() => {
                        try
                        {
                            APIService service = new APIService();
                            var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                            if (userStateInfos != null)
                            {
                                viewModel.CurrentUserInfo = userStateInfos;
                            }
                        }
                        catch
                        { }
                    }).Start();
                }
                viewModel.IsSysMenuePopWindowOpen = true;
                try
                {
                    this.Activate();
                }
                catch (Exception ex)
                { }
            }
            else if (e.Button == MouseButtons.Left && viewModel.MenueLoginVisibility == Visibility.Collapsed)
            {
                MenuLogin_Click(null, null);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
            LeaveWcfService();
            CloseConsoleWPFClientServer();
            CloseNotifyMessageView();
            CloseLoginView();
        }
        private void MenuSetting_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsSysMenuePopWindowOpen = false;
        }
        private void MenuLoginOut_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsSysMenuePopWindowOpen = false;
            MessageBoxResult confirmToDel = System.Windows.MessageBox.Show("确认要注销登录吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmToDel != MessageBoxResult.Yes)
            {
                return;
            }
            CancelCheckNet();
            try
            {
                UserLoginInfo userLoginInfo = new UserLoginInfo();
                userLoginInfo.UserName = "";
                userLoginInfo.PassWord = "";
                userLoginInfo.IsAutoLogin = false;
                //保存用户登录信息到本地
                string userLoginInfos = string.Format(@"{0}\UserLoginInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(userLoginInfo), userLoginInfos);
            }
            catch (Exception ex)
            { }
            UtilSystemVar.UserToken = "";
            UtilSystemVar.UserName = "";
            viewModel.MenueUnLoginVisibility = Visibility.Visible;
            viewModel.MenueLoginVisibility = Visibility.Collapsed;
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginOut");
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("Login");
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsSysMenuePopWindowOpen = false;
            this.Close();
        }
        /// <summary>  
        /// 重写WndProc函数，类型为虚保护，响应窗体消息事件  
        /// </summary>  
        /// <param name="hwnd"></param>  
        /// <param name="msg">消息内容</param>  
        /// <param name="wParam"></param>  
        /// <param name="lParam"></param>  
        /// <param name="handled">是否相应完成</param>  
        /// <returns></returns>  
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                //0x0312表示事件消息为按下快捷键  
                case 0x0312:
                    
                    break;
                case CheckWordUtil.Win32Helper.WM_COPYDATA:
                    try
                    {
                        CheckWordUtil.Win32Helper.CopyDataStruct cds = (CheckWordUtil.Win32Helper.CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CheckWordUtil.Win32Helper.CopyDataStruct));
                        string jsonData = cds.lpData;
                        System.Threading.ThreadStart start = delegate ()
                        {
                            var result = JsonConvert.DeserializeObject<CommonExchangeInfo>(jsonData);
                            if (result.Code == "ShowWordAndImgOperationApp")
                            {
                                MenuLogin_Click(null, null);
                            }
                            else if (result.Code == "ShowNotifyMessageView")
                            {
                                EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish(result.Data);
                            }
                            else if (result.Code == "HideNotifyMessageView")
                            {
                                if (result.Data == "4003" && this.notifyIcon.Text.Contains("词库获取错误"))
                                {
                                    SetIconToolTip("词牛（已登录）");
                                    try
                                    {
                                        Dispatcher.Invoke(new Action(() => {
                                            foreach (Window win in App.Current.Windows)
                                            {
                                                if (win != this && win.Title == "NotifyMessageView")
                                                {
                                                    var viewModel = win.DataContext as CheckWordControl.Notify.NotifyMessageViewModel;
                                                    if (viewModel != null && viewModel.Message.ErrorCode == "4003")
                                                    {
                                                        viewModel._closeAction();
                                                        win.Close();
                                                        break;
                                                    }
                                                }
                                            }
                                        }));
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                            }
                        };
                        System.Threading.Thread t = new System.Threading.Thread(start);
                        t.IsBackground = true;
                        t.Start();
                    }
                    catch (Exception ex)
                    { }
                    break;
            }
            return IntPtr.Zero;
        }
        private void CloseLoginView()
        {
            try
            {
                foreach (Window win in App.Current.Windows)
                {
                    if (win != this && win.Title == "LoginWindow")
                    {
                        win.Close();
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void CloseNotifyMessageView()
        {
            try
            {
                foreach (Window win in App.Current.Windows)
                {
                    if (win != this && win.Title == "NotifyMessageView")
                    {
                        win.Close();
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void CloseConsoleWPFClientServer()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("ConsoleWPFClientServer");
                foreach (var p in processes)
                {
                    p.Kill();
                }
            }
            catch (Exception ex)
            { }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }
        private MessageServiceClient mService = null;
        private void RegisterWcfService()
        {
            try
            {
                InstanceContext context = new InstanceContext(this);
                mService = new MessageServiceClient(context);
                mService.Register("WordAndImgOperationApp");
            }
            catch (Exception ex)
            { }
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginOut");
        }

        private void LeaveWcfService()
        {
            try
            {
                mService.Leave("WordAndImgOperationApp");
            }
            catch (Exception ex)
            { }
        }
        public void SendMessage(string str)
        {

        }
        private void LoginInOrOut(string typeInfo)
        {
            try
            {
                if (typeInfo == "LoginIn")
                {
                    SetIconToolTip("词牛（已登录）");
                }
                else
                {
                    SetIconToolTip("词牛（未登录）");
                }
                LoginInOutInfo loginInOutInfo = new LoginInOutInfo();
                loginInOutInfo.Type = typeInfo;
                loginInOutInfo.UrlStr = UtilSystemVar.UrlStr;
                loginInOutInfo.Token = UtilSystemVar.UserToken;
                try
                {
                    //保存用户登录信息到本地
                    string loginInOutInfos = string.Format(@"{0}\LoginInOutInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    DataParse.WriteToXmlPath(JsonConvert.SerializeObject(loginInOutInfo), loginInOutInfos);
                }
                catch (Exception ex)
                {
                    WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                }
                string json = JsonConvert.SerializeObject(loginInOutInfo);
                mService.ClientSendMessage(json);
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
        private void SetIconToolTip(string toolTips, string icoName = "MyApp.ico")
        {
            try
            {
                this.notifyIcon.Text = toolTips;
                this.notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "Resources/" + icoName);
            }
            catch (Exception ex)
            { }
        }

        private void RechargeBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsSysMenuePopWindowOpen = false;
            try
            {
                System.Diagnostics.Process.Start("http://www.ciniuwang.com/pay");
            }
            catch (Exception ex)
            { }
        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsSysMenuePopWindowOpen = false;
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    foreach (Window win in App.Current.Windows)
                    {
                        if (win != this && win.Title == "LoginWindow")
                        {
                            win.Show();
                            try
                            {
                                var loginWin = win as LoginWindow;
                                loginWin.WindowState = loginWin.windowState;
                            }
                            catch (Exception)
                            { }
                            win.Activate();
                        }
                    }
                }));
            }
            catch (Exception ex)
            { }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                windowState = this.WindowState;
            }
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                
            }
        }

        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MoreMenueBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = true;
        }

        private void SelectHistoryBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectHistoryBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        private void GoUserInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = false;
        }
        private void GoCustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = false;
        }
        private void GoSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = false;
        }

        private void PinBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckedPinBtn.Visibility = Visibility.Visible;
            PinBtn.Visibility = Visibility.Collapsed;
            this.Topmost = true;
            viewModel.PinBtnToolTip = "取消固定";
        }

        private void CheckedPinBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckedPinBtn.Visibility = Visibility.Collapsed;
            PinBtn.Visibility = Visibility.Visible;
            this.Topmost = false;
            viewModel.PinBtnToolTip = "点击固定";
        }
    }
}
