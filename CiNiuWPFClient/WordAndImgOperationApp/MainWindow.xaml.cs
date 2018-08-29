using CheckWordEvent;
using CheckWordModel;
using CheckWordModel.Communication;
using CheckWordUtil;
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
    public partial class MainWindow : Window, IShell
    {
        private List<HistoryCheckInfo> listCheckHistory = new List<HistoryCheckInfo>();
        private static List<string> FilePathsList = new List<string>();
        private static List<string> UnCheckFilePathsList = new List<string>();
        private static List<string> UnReadFilePathsList = new List<string>();
        List<string> listClass = new List<string>() { ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx" };
        private int heightAddMax = 328;
        private bool IsDealingData = false;
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
            this.Top = SystemParameters.WorkArea.Height - 170;
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Subscribe(InitContentGridView);
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Subscribe(LoginInOrOut);
            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Subscribe(SendNotifyMessage);
            EventAggregatorRepository.EventAggregator.GetEvent<CloseMyAppEvent>().Subscribe(CloseMyApp);
            EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Subscribe(MainAppShowTipsInfo);
            EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Subscribe(WriteToSetting);
            EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Subscribe(GetWords);
            EventAggregatorRepository.EventAggregator.GetEvent<MainAppBusyIndicatorEvent>().Subscribe(MainAppBusyIndicator);
            RegisterWcfService();
            GetVersionInfo();
        }
        private void MainAppBusyIndicator(AppBusyIndicator busyindicator)
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
                }));
            }
            catch (Exception ex)
            { }
        }
        private void MainAppShowTipsInfo(AppBusyIndicator busyindicator)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    if (!string.IsNullOrEmpty(busyindicator.BusyContent))
                        this.viewModel.MessageTipInfo = busyindicator.BusyContent;
                    viewModel.IsMessageTipPopWindowOpen = busyindicator.IsBusy;
                    if (busyindicator.IsBusy)
                    {
                        Task task = new Task(() => {
                            System.Threading.Thread.Sleep(3000);
                            viewModel.IsMessageTipPopWindowOpen = false;
                        });
                        task.Start();
                    }
                }));
            }
            catch (Exception ex)
            { }
        }
        private void CloseMyApp(bool b)
        {
            MenuExit_Click(null, null);
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
                        CloseImgWindow();
                        CloseSettingWindow();
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
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
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
                    VersionResponse versionResponse = service.GetVersionInfo();
                    if (versionResponse != null && !string.IsNullOrEmpty(versionResponse.latestClient))
                    {
                        System.Threading.Thread.Sleep(1500);
                        viewModel.NewVersionInfo = versionResponse.latestClient;
                        apiMinVersion = versionResponse.minimumApi;
                        if (new Version(apiMinVersion) > new Version(ConfigurationManager.AppSettings["APIVersion"].ToString()))
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("60040");
                        }
                        else
                        {
                            if (new Version(viewModel.NewVersionInfo) > new Version(version))
                            {
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
            EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = false, BusyContent = "" });
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
            UtilSystemVar.UserToken = "";
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginOut");
            CloseNotifyMessageView();
            CloseLoginView();
            CloseImgWindow();
            CloseSettingWindow();
        }
        private void MenuSetting_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsSysMenuePopWindowOpen = false;
            OpenSettingWindow("Setting");
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
            Task task = new Task(() => {
                Dispatcher.Invoke(new Action(() => {
                    viewModel.IsSelectHistoryChecked = false;
                    viewModel.SearchText = "";
                    if(this.IsDealingData)
                    {
                        //取消检查数据
                        CloseDealingGrid();
                    }
                    MainGrid.Height = 80;
                    this.Height = 99;
                    this.Left = SystemParameters.WorkArea.Width - 352;
                    this.Top = SystemParameters.WorkArea.Height - 170;
                }));
            });
            task.Start();
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
                                if (result.Data == "4003" && this.notifyIcon.Text.Contains("数据异常"))
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
        private void RegisterWcfService()
        {
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginOut");
        }
        private void LoginInOrOut(string typeInfo)
        {
            try
            {
                LoginInOutInfo loginInOutInfo = new LoginInOutInfo();
                loginInOutInfo.Type = typeInfo;
                loginInOutInfo.UrlStr = UtilSystemVar.UrlStr;
                loginInOutInfo.Token = UtilSystemVar.UserToken;
                viewModel.UserName = UtilSystemVar.UserName;
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
                if (typeInfo == "LoginIn")
                {
                    new Task(() => {
                        EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                    }).Start();
                    SetIconToolTip("词牛（已登录）");
                    //获取用户设置
                    Task task = new Task(() => {
                        try
                        {
                            APIService service = new APIService();
                            MySettingInfo settingInfo = service.GetUserSettingByToken(UtilSystemVar.UserToken);
                            if (settingInfo != null)
                            {
                                EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Publish(settingInfo);
                            }
                            else
                            {
                                string mySettingInfo = string.Format(@"{0}\MySettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                                if (!File.Exists(mySettingInfo))
                                {
                                    var mySetting = new MySettingInfo { IsCheckPicInDucument = true, IsUseCustumCi = true };
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "通用类目", Code = "111" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "母婴", Code = "222" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "房地产", Code = "333" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "美妆", Code = "444" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "食品", Code = "555" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "医疗", Code = "666" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "教育", Code = "777" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "保健品", Code = "888" });
                                    mySetting.CategoryInfos.Add(new CategorySelectInfo { CheckedState = true, Name = "其它", Code = "999" });
                                    EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Publish(mySetting);
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    });
                    task.Start();
                }
                else
                {
                    new Task(() => {
                        try
                        {
                            List<WordModel> wordModels = new List<WordModel>();
                            string myWordModelsInfo = string.Format(@"{0}\MyWordModelsInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                            //保存用户设置信息到本地
                            DataParse.WriteToXmlPath(JsonConvert.SerializeObject(wordModels), myWordModelsInfo);
                        }
                        catch (Exception ex)
                        {
                            WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                        }
                    }).Start();
                    SetIconToolTip("词牛（未登录）");
                }
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
        private void GetWords(bool b)
        {
            try
            {
                LoginInOutInfo loginInOutInfo = new LoginInOutInfo();
                loginInOutInfo.Type = "LoginIn";
                loginInOutInfo.UrlStr = UtilSystemVar.UrlStr;
                loginInOutInfo.Token = UtilSystemVar.UserToken;
                viewModel.UserName = UtilSystemVar.UserName;
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
                CheckWordHelper.GetAllCheckWordByToken(UtilSystemVar.UserToken);
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
                viewModel.SearchText = SearchTextBox.Text;
                CheckInputText(viewModel.SearchText);
            }
        }

        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckInputText(viewModel.SearchText);
        }
        private async void CheckInputText(string textSearch)
        {
            if (!string.IsNullOrEmpty(textSearch))
            {
                int countWords = 0;
                countWords = textSearch.Count();
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                try
                {
                    APIService service = new APIService();
                    UserStateInfos userStateInfos = null;
                    Task task = new Task(() => {
                        userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                    });
                    task.Start();
                    await task;
                    if (userStateInfos != null)
                    {
                        if (userStateInfos.WordCount < countWords)
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("500");
                        }
                        else
                        {
                            ConsumeResponse consume = null;
                            Task taskConsume = new Task(() => {
                                consume = service.GetWordConsume(countWords, UtilSystemVar.UserToken);
                            });
                            taskConsume.Start();
                            await taskConsume;
                            if (consume != null)
                            {
                                try
                                {
                                    //处理逻辑
                                    List<UnChekedWordInfo> resultInfo = new List<UnChekedWordInfo>();
                                    Task taskGetUnChekedWord = new Task(() => {
                                        resultInfo = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(textSearch);
                                    });
                                    taskGetUnChekedWord.Start();
                                    await taskGetUnChekedWord;
                                    viewModel.CurrentWordInfoResults = new ObservableCollection<CheckWordModel.UnChekedWordInfo>(resultInfo);
                                    if (viewModel.CurrentWordInfoResults.Count > 0)
                                    {
                                        int heightAdd = 25;
                                        foreach (var item in viewModel.CurrentWordInfoResults)
                                        {
                                            List<UnChekedDetailWordInfo> _detailInfos = new List<UnChekedDetailWordInfo>();
                                            //查询违禁词描述
                                            Task taskGetWordDiscribe = new Task(() => {
                                                APIService serviceApi = new APIService();
                                                _detailInfos = serviceApi.GetWordDiscribeLists(UtilSystemVar.UserToken, item.ID);
                                            });
                                            taskGetWordDiscribe.Start();
                                            await taskGetWordDiscribe;
                                            item.UnChekedWordDetailInfos = new ObservableCollection<UnChekedDetailWordInfo>(_detailInfos);
                                            foreach (var detaiItem in item.UnChekedWordDetailInfos)
                                            {
                                                int rowsCount = detaiItem.Discription.Length / 25 + 1;
                                                heightAdd += 22 * rowsCount + 40;
                                            }
                                            heightAdd += 32 + 16;
                                        }
                                        if (heightAdd > heightAddMax)
                                        {
                                            heightAdd = heightAddMax;
                                        }
                                        //包含违禁词
                                        viewModel.WordNoUnchekResultVisibility = Visibility.Collapsed;
                                        viewModel.WordHasUnchekResultVisibility = Visibility.Visible;
                                        viewModel.DragFilesResultVisibility = Visibility.Collapsed;
                                        viewModel.HistoryFilesGridVisibility = Visibility.Collapsed;
                                        viewModel.IsSelectHistoryChecked = false;
                                        viewModel.AddToCustumCiTiaoVisibility = Visibility.Collapsed;
                                        MainGrid.Height = 80 + heightAdd;
                                        this.Height = 99 + heightAdd;
                                    }
                                    else
                                    {
                                        //不包含违禁词
                                        viewModel.WordNoUnchekResultVisibility = Visibility.Visible;
                                        viewModel.WordHasUnchekResultVisibility = Visibility.Collapsed;
                                        viewModel.DragFilesResultVisibility = Visibility.Collapsed;
                                        viewModel.HistoryFilesGridVisibility = Visibility.Collapsed;
                                        viewModel.IsSelectHistoryChecked = false;
                                        viewModel.AddToCustumCiTiaoVisibility = Visibility.Collapsed;
                                        MainGrid.Height = 80 + 75;
                                        this.Height = 99 + 75;
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                            else
                            {
                                EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                            }
                        }
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                    }
                }
                catch
                { }
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                System.Threading.ThreadStart start = delegate ()
                {
                    //记录历史
                    HistoryCheckInfo info = new HistoryCheckInfo { Type = "TxT", FileName = textSearch, FileFullPath = textSearch, CheckTime = DateTime.Now };
                    WriteToHistory(info);
                };
                System.Threading.Thread t = new System.Threading.Thread(start);
                t.IsBackground = true;
                t.Start();
            }
        }
        /// <summary>
        /// 用户记录写入历史
        /// </summary>
        /// <param name="list"></param>
        private void WriteToHistory(HistoryCheckInfo item)
        {
            try
            {
                string historyCheckInfos = string.Format(@"{0}\HistoryCheckInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(historyCheckInfos);
                if (ui != null && ui.ToString() != "")
                {
                    listCheckHistory = JsonConvert.DeserializeObject<List<HistoryCheckInfo>>(ui.ToString()).OrderByDescending(x => x.CheckTime).ToList();
                }
                var existHistory = listCheckHistory.FirstOrDefault(x => x.FileFullPath == item.FileFullPath && x.Type == item.Type);
                if (listCheckHistory.Count < 10)
                {
                    if (existHistory != null)
                    {
                        listCheckHistory.Remove(existHistory);
                    }
                    listCheckHistory.Insert(0, item);
                }
                else
                {
                    if (existHistory != null)
                    {
                        listCheckHistory.Remove(existHistory);
                    }
                    else
                    {
                        listCheckHistory.RemoveAt(9);
                    }
                    listCheckHistory.Insert(0, item);
                }
                viewModel.HistoryCheckInfoList = new ObservableCollection<HistoryCheckInfo>(listCheckHistory);
                //保存历史记录信息到本地
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(listCheckHistory), historyCheckInfos);
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
        /// <summary>
        /// 用户设置写入历史
        /// </summary>
        /// <param name="list"></param>
        private void WriteToSetting(MySettingInfo mySetting)
        {
            try
            {
                string mySettingInfo = string.Format(@"{0}\MySettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                //保存用户设置信息到本地
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(mySetting), mySettingInfo);
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
        private void MoreMenueBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = true;
        }
        double oldHeight = 99;
        double oldMainGrid = 80;
        private void SelectHistoryBtn_Checked(object sender, RoutedEventArgs e)
        {
            oldMainGrid = MainGrid.Height;
            oldHeight = this.Height;
            try
            {
                //检测变化
                string historyCheckInfos = string.Format(@"{0}\HistoryCheckInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(historyCheckInfos);
                if (ui != null && ui.ToString() != "")
                {
                    listCheckHistory = JsonConvert.DeserializeObject<List<HistoryCheckInfo>>(ui.ToString()).OrderByDescending(x => x.CheckTime).ToList();
                    foreach (var item in listCheckHistory)
                    {
                        if (item.Type == "File")
                        {
                            if (!File.Exists(item.FileFullPath))
                            {
                                item.IsDelete = true;
                            }
                            else
                            {
                                item.IsDelete = false;
                                if (new FileInfo(item.FileFullPath).LastWriteTime > item.LastWriteTime)
                                {
                                    item.IsModify = true;
                                }
                            }
                        }
                    }
                }
                viewModel.HistoryCheckInfoList = new ObservableCollection<HistoryCheckInfo>(listCheckHistory);
                //保存历史记录信息到本地
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(listCheckHistory), historyCheckInfos);
            }
            catch (Exception ex)
            { }
            viewModel.HistoryFilesGridVisibility = Visibility.Visible;
            MainGrid.Height = 80 + heightAddMax;
            this.Height = 99 + heightAddMax;
        }

        private void SelectHistoryBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            viewModel.HistoryFilesGridVisibility = Visibility.Collapsed;
            MainGrid.Height = oldMainGrid;
            this.Height = oldHeight;
        }
        private void GoUserInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = false;
            OpenSettingWindow("UserInfo");
        }
        private void GoCustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = false;
            OpenSettingWindow("CustumCi");
        }
        private void GoSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsMoreMenuePopWindowOpen = false;
            OpenSettingWindow("Setting");
        }
        private void OpenSettingWindow(string type)
        {
            CloseSettingWindow();
            SettingWindow settingWindow = new SettingWindow(type);
            settingWindow.Show();
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

        private void AddToCustumCiTiaoBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DiscriptionSearchText = "";
            viewModel.AddToCustumCiTiaoVisibility = Visibility.Visible;
            MainGrid.Height = 80 + 250;
            this.Height = 99 + 250;
        }

        private async void SureToCustumCiTiaoBtn_Click(object sender, RoutedEventArgs e)
        {
            //调用接口添加自建词库
            Task<bool> task = new Task<bool>(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                bool b = false;
                try
                {
                    APIService service = new APIService();
                    b = service.AddCustumCiTiaoByToken(UtilSystemVar.UserToken, viewModel.SearchText, viewModel.DiscriptionSearchText);
                }
                catch (Exception ex)
                { }
                if (b)
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                    System.Threading.Thread.Sleep(1000);
                }
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                return b;
            });
            task.Start();
            await task;
            if (task.Result)
            {
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = true, BusyContent = "添加自建词条成功" });
            }
            else
            {
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = true, BusyContent = "添加自建词条失败" });
            }
            viewModel.AddToCustumCiTiaoVisibility = Visibility.Collapsed;
            MainGrid.Height = 80 + 75;
            this.Height = 99 + 75;
            if (task.Result)
            {
                CheckInputText(viewModel.SearchText);
            }
        }
        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            DragDealingTipGrid.Visibility = Visibility.Collapsed;
            DragTipGrid.Visibility = Visibility.Collapsed;
            if (this.IsDealingData)
            {
                return;
            }
            this.IsDealingData = true;
            viewModel.DealingGridVisibility = Visibility.Visible;
            EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = true, BusyContent = "正在检测中，请稍后添加" });
            viewModel.CheckFilesInfosText = "正在解析中...";
            viewModel.DealCurrentIndex = 0;
            FilePathsList = new List<string>();
            UnCheckFilePathsList = new List<string>();
            UnReadFilePathsList = new List<string>();
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                foreach (var path in ((System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop)))
                {
                    if (File.Exists(path.ToString()))
                    {
                        if (listClass.Contains(System.IO.Path.GetExtension(path.ToString()).ToLower()))
                        {
                            if (!path.ToString().Contains("~$"))
                            {
                                FilePathsList.Add(path.ToString());
                                if (Win32Helper.IsFileOpen(path.ToString()) && ".doc,.docx".Contains(System.IO.Path.GetExtension(path.ToString()).ToLower()))
                                {
                                    UnReadFilePathsList.Add(path.ToString());
                                }
                            }
                        }
                        else
                        {
                            UnCheckFilePathsList.Add(path.ToString());
                        }
                    }
                    else if (Directory.Exists(path.ToString()))
                    {
                        DirectoryInfo dir = new DirectoryInfo(path.ToString());
                        GetAllFiles(dir);
                    }
                }
                DealDragDatas();
            }
        }
        private void GetAllFiles(DirectoryInfo dir)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                if (listClass.Contains(System.IO.Path.GetExtension(fi.FullName).ToLower()))
                {
                    if (!fi.FullName.Contains("~$"))
                    {
                        FilePathsList.Add(fi.FullName);
                        if (Win32Helper.IsFileOpen(fi.FullName) && ".doc,.docx".Contains(System.IO.Path.GetExtension(fi.FullName).ToLower()))
                        {
                            UnReadFilePathsList.Add(fi.FullName);
                        }
                    }
                }
                else
                {
                    UnCheckFilePathsList.Add(fi.FullName);
                }
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d);
            }
        }
        private void Window_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            DragDealingTipGrid.Visibility = Visibility.Collapsed;
            DragTipGrid.Visibility = Visibility.Collapsed;
        }

        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (this.IsDealingData)
            {
                DragDealingTipGrid.Visibility = Visibility.Visible;
                DragTipGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                DragTipGrid.Visibility = Visibility.Visible;
                DragDealingTipGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void CancelDealingBtn_Click(object sender, RoutedEventArgs e)
        {
            //取消检查数据
            CloseDealingGrid();
        }
        private void CloseDealingGrid()
        {
            viewModel.DealingGridVisibility = Visibility.Collapsed;
            this.IsDealingData = false;
            EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = false, BusyContent = "" });
        }
        private bool isCheckPicInDucument = true;
        private ObservableCollection<MyFolderDataViewModel> _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
        /// <summary>
        /// 处理拖拽文件
        /// </summary>
        private async void DealDragDatas()
        {
            try
            {
                if (FilePathsList.Count > 0)
                {
                    if (!IsDealingData)
                    {
                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                        return;
                    }
                    _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                    FileOperateHelper.DeleteFolder(CheckWordTempPath);
                    if (!Directory.Exists(CheckWordTempPath))
                    {
                        Directory.CreateDirectory(CheckWordTempPath);
                    }
                    isCheckPicInDucument = true;
                    string mySettingInfo = string.Format(@"{0}\MySettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(mySettingInfo);
                    if (ui != null && ui.ToString() != "")
                    {
                        try
                        {
                            var mySetting = JsonConvert.DeserializeObject<MySettingInfo>(ui.ToString());
                            if (mySetting != null)
                            {
                                isCheckPicInDucument = mySetting.IsCheckPicInDucument;
                            }
                        }
                        catch
                        { }
                    }
                    //解析数据
                    Task taskJieXi = new Task(() => {
                        int fileCount = FilePathsList.Count;
                        int imagesCount = 0;
                        foreach (var item in FilePathsList)
                        {
                            if (".png,.jpg,.jpeg".Contains(System.IO.Path.GetExtension(item).ToLower()))
                            {
                                imagesCount++;
                            }
                            else if (".doc,.docx".Contains(System.IO.Path.GetExtension(item).ToLower()))
                            {
                                try
                                {
                                    if (isCheckPicInDucument)
                                    {
                                        Aspose.Words.Document doc = new Aspose.Words.Document(item);
                                        //取得对象集合
                                        Aspose.Words.NodeCollection shapes = doc.GetChildNodes(Aspose.Words.NodeType.Shape, true);
                                        foreach (Aspose.Words.Drawing.Shape shape in shapes)
                                        {
                                            if (shape != null && shape.HasImage)
                                            {
                                                imagesCount++;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                            else if (".xls,.xlsx".Contains(System.IO.Path.GetExtension(item).ToLower()))
                            {
                                try
                                {
                                    if (isCheckPicInDucument)
                                    {
                                        Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(item);
                                        int sheetCount = workbook.Worksheets.Count;
                                        for (int k = 0; k < sheetCount; k++)
                                        {
                                            imagesCount += workbook.Worksheets[k].Pictures.Count;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                        viewModel.CheckFilesInfosText = "正在检测" + fileCount + "个文件，" + imagesCount + "张图片";
                    });
                    taskJieXi.Start();
                    await taskJieXi;
                    if (!IsDealingData)
                    {
                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                        return;
                    }
                    for (int i = 0; i < FilePathsList.Count; i++)
                    {
                        Task task = new Task(() => {
                            if (!IsDealingData)
                            {
                                Dispatcher.Invoke(new Action(() => {
                                    _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                }));
                                return;
                            }
                            viewModel.DealTotalCount = FilePathsList.Count;
                            viewModel.DealCurrentIndex = i;
                            DealMyPathsDataSource(FilePathsList[i]);
                        });
                        task.Start();
                        await task;
                    }
                    Task taskFinish = new Task(() => {
                        viewModel.DealTotalCount = FilePathsList.Count;
                        viewModel.DealCurrentIndex = FilePathsList.Count;
                        System.Threading.Thread.Sleep(300);
                    });
                    taskFinish.Start();
                    await taskFinish;
                    if (!IsDealingData)
                    {
                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                        return;
                    }
                    int heightAdd = 32 * FilePathsList.Count + 5;
                    if (heightAdd > heightAddMax)
                    {
                        heightAdd = heightAddMax;
                    }
                    MainGrid.Height = 80 + heightAdd;
                    this.Height = 99 + heightAdd;
                    //检查完成
                    viewModel.DealDataResultList = _dealDataResultList;
                    CloseDealingGrid();
                    viewModel.DragFilesResultVisibility = Visibility.Visible;
                    viewModel.HistoryFilesGridVisibility = Visibility.Collapsed;
                    viewModel.IsSelectHistoryChecked = false;
                    System.Threading.ThreadStart start = delegate ()
                    {
                        //记录历史
                        foreach (var item in FilePathsList)
                        {
                            HistoryCheckInfo info = new HistoryCheckInfo { Type = "File", FileFullPath = item, CheckTime = DateTime.Now };
                            if (!File.Exists(item))
                            {
                                info.IsDelete = true;
                            }
                            else
                            {
                                FileInfo fileInfo = new FileInfo(item);
                                try
                                {
                                    info.FileName = fileInfo.Name;
                                    info.LastWriteTime = fileInfo.LastWriteTime;
                                }
                                catch
                                { }
                            }
                            WriteToHistory(info);
                        }
                    };
                    System.Threading.Thread t = new System.Threading.Thread(start);
                    t.IsBackground = true;
                    t.Start();
                }
                else
                {
                    CloseDealingGrid();
                    if (UnCheckFilePathsList.Count > 0)
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = true, BusyContent = UnCheckFilePathsList.Count + "个文件类型不支持." });
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = true, BusyContent = "未发现支持的文件类型." });
                    }
                }
            }
            catch
            { }
        }
        /// <summary>
        /// 处理检查数据
        /// </summary>
        private void DealMyPathsDataSource(string dealFilePath)
        {
            try
            {
                MyFolderDataViewModel model = null;
                if (".doc,.docx".Contains(System.IO.Path.GetExtension(dealFilePath).ToLower()))
                {
                    model = LoadDocx(dealFilePath);
                }
                else if (".png,.jpg,.jpeg".Contains(System.IO.Path.GetExtension(dealFilePath).ToLower()))
                {
                    model = AutoExcutePicOCR(dealFilePath);
                }
                else if (".xls,.xlsx".Contains(System.IO.Path.GetExtension(dealFilePath).ToLower()))
                {
                    model = LoadXlsx(dealFilePath);
                }
                Dispatcher.Invoke(new Action(() => {
                    if (model != null)
                        _dealDataResultList.Add(model);
                }));
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 解析校验文档
        /// </summary>
        /// <param name="filePath"></param>
        private MyFolderDataViewModel LoadDocx(string dealFilePath)
        {
            MyFolderDataViewModel model = new MyFolderDataViewModel(System.IO.Path.GetFileName(dealFilePath), dealFilePath);
            model.TypeSelectFile = SelectFileType.Docx;
            model.CheckResultInfo = "0";
            try
            {
                string guid = Guid.NewGuid().ToString();
                string fileName = System.IO.Path.GetFileNameWithoutExtension(dealFilePath);
                string pathDir = CheckWordTempPath + "\\" + fileName + System.IO.Path.GetExtension(dealFilePath).Replace(".", "") + "-Docx\\";
                FileOperateHelper.DeleteFolder(pathDir);
                if (!Directory.Exists(pathDir))
                {
                    Directory.CreateDirectory(pathDir);
                }
                Aspose.Words.Document doc = new Aspose.Words.Document(dealFilePath);
                int countWords = 0;
                foreach (Aspose.Words.Section section in doc.Sections)
                {
                    if (!IsDealingData)
                    {
                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                        return null;
                    }
                    foreach (Aspose.Words.Paragraph paragraph in section.Body.Paragraphs)
                    {
                        if (!IsDealingData)
                        {
                            _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                            return null;
                        }
                        string textResult = paragraph.GetText();
                        if (!string.IsNullOrEmpty(textResult))
                        {
                            textResult = textResult.Replace("\f", "").Replace("\r", "").Replace("\n", "");
                            if (!string.IsNullOrEmpty(textResult))
                                countWords += textResult.Count();
                        }
                    }
                }
                try
                {
                    APIService service = new APIService();
                    var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                    if (userStateInfos != null)
                    {
                        if (userStateInfos.WordCount < countWords)
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("500");
                            model.CheckResultInfo = "2";
                            model.FileToolTip = "剩余点数不足,未能检测";
                            return model;
                        }
                        else
                        {
                            int index = 1;
                            //取得对象集合
                            Aspose.Words.NodeCollection shapes = doc.GetChildNodes(Aspose.Words.NodeType.Shape, true);
                            foreach (Aspose.Words.Drawing.Shape shape in shapes)
                            {
                                if (!IsDealingData)
                                {
                                    _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                    return null;
                                }
                                if (shape != null && shape.HasImage)
                                {
                                    string imageName = String.Format(pathDir + "照片-{0}.png", index);
                                    shape.ImageData.Save(imageName);
                                    index++;
                                }
                            }
                            if (countWords > 0)
                            {
                                if (!IsDealingData)
                                {
                                    _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                    return null;
                                }
                                ConsumeResponse consume = service.GetWordConsume(countWords, UtilSystemVar.UserToken, System.IO.Path.GetFileName(dealFilePath), guid);
                                if (consume != null)
                                {
                                    foreach (Aspose.Words.Section section in doc.Sections)
                                    {
                                        if (!IsDealingData)
                                        {
                                            _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                            return null;
                                        }
                                        foreach (Aspose.Words.Paragraph paragraph in section.Body.Paragraphs)
                                        {
                                            if (!IsDealingData)
                                            {
                                                _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                                return null;
                                            }
                                            string textResult = paragraph.GetText();
                                            var list = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(textResult).ToList();
                                            if (list.Count > 0)
                                            {
                                                model.CheckResultInfo = "1";
                                                return model;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!IsDealingData)
                                    {
                                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                        return null;
                                    }
                                    EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                                    model.CheckResultInfo = "2";
                                    model.FileToolTip = "服务器接口异常,未能检测";
                                    return model;
                                }
                            }
                            //如果包含图片，检测图片
                            if (isCheckPicInDucument)
                            {
                                if (Directory.Exists(pathDir))
                                {
                                    DirectoryInfo dirDoc = new DirectoryInfo(pathDir);
                                    var filePicInfos = dirDoc.GetFiles();
                                    FileOperateHelper.SortAsFileCreationTime(ref filePicInfos);
                                    foreach (var picInfo in filePicInfos)
                                    {
                                        if (!IsDealingData)
                                        {
                                            _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                            return null;
                                        }
                                        if (picInfo.FullName.Contains("png"))
                                        {
                                            var picResult = AutoExcutePicOCR(picInfo.FullName, dealFilePath, guid);
                                            if (picResult != null)
                                            {
                                                if (picResult.CheckResultInfo == "1")
                                                {
                                                    model.CheckResultInfo = "1";
                                                    return model;
                                                }
                                                else if (picResult.CheckResultInfo == "2")
                                                {
                                                    model.CheckResultInfo = "2";
                                                    model.FileToolTip = picResult.FileToolTip;
                                                    return model;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                        model.CheckResultInfo = "2";
                        model.FileToolTip = "服务器接口异常,未能检测";
                        return model;
                    }
                }
                catch
                {
                    model.CheckResultInfo = "2";
                    model.FileToolTip = "程序异常,未能检测";
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.CheckResultInfo = "2";
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return model;
        }
        /// <summary>
        /// 解析校验Xlsx
        /// </summary>
        /// <param name="filePath"></param>
        private MyFolderDataViewModel LoadXlsx(string dealFilePath)
        {
            MyFolderDataViewModel model = new MyFolderDataViewModel(System.IO.Path.GetFileName(dealFilePath), dealFilePath);
            model.TypeSelectFile = SelectFileType.Xlsx;
            model.CheckResultInfo = "0";
            try
            {
                string guid = Guid.NewGuid().ToString();
                string fileName = System.IO.Path.GetFileNameWithoutExtension(dealFilePath);
                string pathDir = CheckWordTempPath + "\\" + fileName + System.IO.Path.GetExtension(dealFilePath).Replace(".", "") + "-Xlsx\\";
                FileOperateHelper.DeleteFolder(pathDir);
                if (!Directory.Exists(pathDir))
                {
                    Directory.CreateDirectory(pathDir);
                }
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(dealFilePath);
                int countWords = 0;
                int sheetCount = workbook.Worksheets.Count;
                for (int k = 0; k < sheetCount; k++)
                {
                    if (!IsDealingData)
                    {
                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                        return null;
                    }
                    Aspose.Cells.Cells cells = workbook.Worksheets[k].Cells;
                    for (int i = 0; i < cells.MaxDataRow + 1; i++)
                    {
                        if (!IsDealingData)
                        {
                            _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                            return null;
                        }
                        for (int j = 0; j < cells.MaxDataColumn + 1; j++)
                        {
                            if (!IsDealingData)
                            {
                                _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                return null;
                            }
                            string s = cells[i, j].StringValue.Trim();
                            if (!string.IsNullOrEmpty(s))
                                countWords += s.Count();
                        }
                    }
                }
                try
                {
                    APIService service = new APIService();
                    var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                    if (userStateInfos != null)
                    {
                        if (userStateInfos.WordCount < countWords)
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("500");
                            model.CheckResultInfo = "2";
                            model.FileToolTip = "剩余点数不足,未能检测";
                            return model;
                        }
                        else
                        {
                            int index = 1;
                            for (int k = 0; k < sheetCount; k++)
                            {
                                if (!IsDealingData)
                                {
                                    _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                    return null;
                                }
                                foreach (var item in workbook.Worksheets[k].Pictures)
                                {
                                    if (!IsDealingData)
                                    {
                                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                        return null;
                                    }
                                    Aspose.Cells.Drawing.Picture pic = item;
                                    string imageName = String.Format(pathDir + "照片-{0}.jpg", index);
                                    Aspose.Cells.Rendering.ImageOrPrintOptions printoption = new Aspose.Cells.Rendering.ImageOrPrintOptions();
                                    printoption.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    pic.ToImage(imageName, printoption);
                                    index++;
                                }
                            }
                            if (countWords > 0)
                            {
                                if (!IsDealingData)
                                {
                                    _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                    return null;
                                }
                                ConsumeResponse consume = service.GetWordConsume(countWords, UtilSystemVar.UserToken, System.IO.Path.GetFileName(dealFilePath), guid);
                                if (consume != null)
                                {
                                    for (int k = 0; k < sheetCount; k++)
                                    {
                                        if (!IsDealingData)
                                        {
                                            _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                            return null;
                                        }
                                        Aspose.Cells.Cells cells = workbook.Worksheets[k].Cells;
                                        for (int i = 0; i < cells.MaxDataRow + 1; i++)
                                        {
                                            if (!IsDealingData)
                                            {
                                                _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                                return null;
                                            }
                                            for (int j = 0; j < cells.MaxDataColumn + 1; j++)
                                            {
                                                string textResult = cells[i, j].StringValue.Trim();
                                                if (!string.IsNullOrEmpty(textResult))
                                                {
                                                    var list = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(textResult).ToList();
                                                    if (list.Count > 0)
                                                    {
                                                        model.CheckResultInfo = "1";
                                                        return model;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!IsDealingData)
                                    {
                                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                        return null;
                                    }
                                    EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                                    model.CheckResultInfo = "2";
                                    model.FileToolTip = "服务器接口异常,未能检测";
                                    return model;
                                }
                            }
                            //如果包含图片，检测图片
                            if (isCheckPicInDucument)
                            {
                                if (Directory.Exists(pathDir))
                                {
                                    DirectoryInfo dirDoc = new DirectoryInfo(pathDir);
                                    var filePicInfos = dirDoc.GetFiles();
                                    FileOperateHelper.SortAsFileCreationTime(ref filePicInfos);
                                    foreach (var picInfo in filePicInfos)
                                    {
                                        if (!IsDealingData)
                                        {
                                            _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                            return null;
                                        }
                                        if (picInfo.FullName.Contains("jpg"))
                                        {
                                            var picResult = AutoExcutePicOCR(picInfo.FullName, dealFilePath, guid);
                                            if (picResult != null)
                                            {
                                                if (picResult.CheckResultInfo == "1")
                                                {
                                                    model.CheckResultInfo = "1";
                                                    return model;
                                                }
                                                else if (picResult.CheckResultInfo == "2")
                                                {
                                                    model.CheckResultInfo = "2";
                                                    model.FileToolTip = picResult.FileToolTip;
                                                    return model;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                        model.CheckResultInfo = "2";
                        model.FileToolTip = "服务器接口异常,未能检测";
                        return model;
                    }
                }
                catch
                {
                    model.CheckResultInfo = "2";
                    model.FileToolTip = "程序异常,未能检测";
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.CheckResultInfo = "2";
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return model;
        }
        /// <summary>
        /// 解析校验Img
        /// </summary>
        /// <param name="filePath"></param>
        private MyFolderDataViewModel AutoExcutePicOCR(string dealFilePath, string fromDucumentFileName = "", string taskId = "")
        {
            MyFolderDataViewModel model = new MyFolderDataViewModel(System.IO.Path.GetFileName(dealFilePath), dealFilePath);
            model.TypeSelectFile = SelectFileType.Img;
            model.CheckResultInfo = "0";
            try
            {
                try
                {
                    APIService service = new APIService();
                    var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                    if (userStateInfos != null)
                    {
                        if (userStateInfos.PicCount == 0)
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("500");
                            model.CheckResultInfo = "2";
                            model.FileToolTip = "剩余点数不足,未能检测";
                            return model;
                        }
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("200");
                        model.CheckResultInfo = "2";
                        model.FileToolTip = "服务器接口异常,未能检测";
                        return model;
                    }
                    ImgGeneralInfo resultImgGeneral = null;
                    var image = File.ReadAllBytes(dealFilePath);
                    //集成云处理OCR
                    APIService serviceOCR = new APIService();
                    string fileNameOCR = "";
                    if (string.IsNullOrEmpty(fromDucumentFileName))
                    {
                        fileNameOCR = System.IO.Path.GetFileName(dealFilePath);
                    }
                    else
                    {
                        fileNameOCR = System.IO.Path.GetFileName(fromDucumentFileName);
                    }
                    var result = serviceOCR.GetOCRResultByToken(UtilSystemVar.UserToken, image, fileNameOCR, taskId);
                    //反序列化
                    resultImgGeneral = JsonConvert.DeserializeObject<ImgGeneralInfo>(result.ToString().Replace("char", "Char"));
                    if (!IsDealingData)
                    {
                        _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                        return null;
                    }
                    if (resultImgGeneral != null && resultImgGeneral.words_result_num > 0)
                    {
                        foreach (var item in resultImgGeneral.words_result)
                        {
                            if (!IsDealingData)
                            {
                                _dealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                                return null;
                            }
                            string lineWord = item.words;
                            var listUnChekedWordInfo = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(lineWord);
                            if (listUnChekedWordInfo != null && listUnChekedWordInfo.Count > 0)
                            {
                                model.ResultImgGeneral = resultImgGeneral;
                                model.CheckResultInfo = "1";
                                return model;
                            }
                        }
                    }
                }
                catch
                {
                    model.CheckResultInfo = "2";
                    model.FileToolTip = "程序异常,未能检测";
                    return model;
                }
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return model;
        }
        private void listBox2_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                var listBox = sender as System.Windows.Controls.ListBox;
                if (listBox != null)
                {
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    listBox.RaiseEvent(eventArg);
                }
            }
            catch (Exception ex)
            { }
        }

        private void DealDataResultGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var myFolderDataViewModel = grid.Tag as MyFolderDataViewModel;
                if (myFolderDataViewModel.TypeSelectFile == SelectFileType.Img)
                {
                    //打开图片，加载框选标记
                    CloseImgWindow();
                    ImgWindow imgWindow = new ImgWindow(myFolderDataViewModel);
                    imgWindow.Show();
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process.Start(myFolderDataViewModel.FilePath); //打开此文件。
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }
        private void CloseImgWindow()
        {
            try
            {
                foreach (Window win in App.Current.Windows)
                {
                    if (win != this && win.Title == "ImgWindow")
                    {
                        win.Close();
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void CloseSettingWindow()
        {
            try
            {
                foreach (Window win in App.Current.Windows)
                {
                    if (win != this && win.Title == "SettingWindow")
                    {
                        win.Close();
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void HistoryGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var historyCheckInfo = grid.Tag as HistoryCheckInfo;
                if (historyCheckInfo.Type == "TxT")
                {
                    viewModel.IsSelectHistoryChecked = false;
                    viewModel.SearchText = historyCheckInfo.FileName;
                    CheckInputText(viewModel.SearchText);
                }
                else
                {
                    if (File.Exists(historyCheckInfo.FileFullPath))
                    {
                        viewModel.IsSelectHistoryChecked = false;
                        DealHistoryDragFiles(historyCheckInfo.FileFullPath);
                    }
                }
            }
        }
        private void DealHistoryDragFiles(string fileFullName)
        {
            try
            {
                this.IsDealingData = true;
                viewModel.DealingGridVisibility = Visibility.Visible;
                EventAggregatorRepository.EventAggregator.GetEvent<MainAppShowTipsInfoEvent>().Publish(new AppBusyIndicator() { IsBusy = true, BusyContent = "正在检测中，请稍后添加" });
                viewModel.CheckFilesInfosText = "正在解析中...";
                viewModel.DealCurrentIndex = 0;
                FilePathsList = new List<string>();
                UnCheckFilePathsList = new List<string>();
                UnReadFilePathsList = new List<string>();
                FilePathsList.Add(fileFullName);
                DealDragDatas();
            }
            catch (Exception ex)
            { }
        }
    }
}
