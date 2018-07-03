using CheckWordEvent;
using CheckWordModel;
using CheckWordModel.Communication;
using CheckWordUtil;
using IWPFClientService;
using Newtonsoft.Json;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
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
    public partial class MainWindow : Window, ICallBackServices
    {
        private string CheckWordTempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTemp";
        WindowState windowState;
        NotifyIcon notifyIcon;
        WPFOfficeWindowViewModel viewModel = new WPFOfficeWindowViewModel();
        System.Threading.Thread thCheckNetConn;
        public MainWindow()
        {
            InitializeComponent();
            SetIcon();
            windowState = this.WindowState;
            App.Current.Deactivated += App_Deactivated;
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<AppBusyIndicatorEvent>().Subscribe(ReceiveBusyIndicator);
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Subscribe(InitContentGridView);
            EventAggregatorRepository.EventAggregator.GetEvent<DealCheckBtnDataEvent>().Subscribe(DealCheckBtnData);
            EventAggregatorRepository.EventAggregator.GetEvent<CancelDealCheckBtnDataEvent>().Subscribe(CancelDealCheckBtnData);
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Subscribe(LoginInOrOut);
            EventAggregatorRepository.EventAggregator.GetEvent<IsCanOpenSearchPopWindowEvent>().Subscribe(IsCanOpenSearchPopWindow);
            EventAggregatorRepository.EventAggregator.GetEvent<SendDealDataStateToSeachTxTEvent>().Subscribe(SendDealDataStateToSeachTxT);
            EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Subscribe(SendNotifyMessage);
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
        private void IsCanOpenSearchPopWindow(bool b)
        {
            if (b)
            {
                try
                {
                    bool isSearchPopStateOpen = true;
                    string loginInOutInfos = string.Format(@"{0}\SearchPopSettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                    if (ui != null && ui.ToString() != "")
                    {
                        try
                        {
                            var searchPopSettingInfo = JsonConvert.DeserializeObject<SearchPopSettingInfo>(ui.ToString());
                            if (searchPopSettingInfo != null)
                            {
                                isSearchPopStateOpen = searchPopSettingInfo.IsSearchPopStateOpen;
                            }
                        }
                        catch
                        { }
                    }
                    if (isSearchPopStateOpen)
                    {
                        ShowSearchPop();
                    }
                }
                catch (Exception ex)
                { }
            }
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
        private void InitContentGridView(string typeName)
        {
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    ContentGrid.Children.Clear();
                    if (typeName == "Login")
                    {
                        viewModel.MainSetVisibility = Visibility.Collapsed;

                        Login login = new Login();
                        ContentGrid.Children.Add(login);
                    }
                    else if (typeName == "MainSet")
                    {
                        CloseBtn_Click(null, null);
                        viewModel.MainSetVisibility = Visibility.Visible;

                        //MainSet mainSet = new MainSet();
                        //ContentGrid.Children.Add(mainSet);
                        viewModel.UserInfoGridVisibility = Visibility.Visible;
                        viewModel.UserName = UtilSystemVar.UserName;
                        viewModel.MenueUnLoginVisibility = Visibility.Collapsed;
                        viewModel.MenueLoginVisibility = Visibility.Visible;

                        viewModel.TitleLogoVisibility = Visibility.Visible;
                        viewModel.ReturnBackBtnVisibility = Visibility.Collapsed;
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
                    else if (typeName == "MainResult")
                    {
                        viewModel.MainSetVisibility = Visibility.Collapsed;

                        MainResult mainResult = new MainResult(DealDataResultList.ToList());
                        ContentGrid.Children.Add(mainResult);
                        viewModel.UserInfoGridVisibility = Visibility.Visible;
                        viewModel.UserName = UtilSystemVar.UserName;
                        viewModel.MenueUnLoginVisibility = Visibility.Collapsed;
                        viewModel.MenueLoginVisibility = Visibility.Visible;

                        viewModel.TitleLogoVisibility = Visibility.Collapsed;
                        viewModel.ReturnBackBtnVisibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                { }
            }));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Handle = new WindowInteropHelper(this).Handle;  //获取窗口句柄
            RunHotKey();
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
            CheckWordUtil.Win32Helper.ShowHideWindow("WPF服务程序");
            RegisterWcfService();
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("Login");
        }
        private void CheckNetConn()
        {
            int count = 0;
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
                    //检测会员状态
                    if (result)
                    {
                        if (count == 0 || count == 60)
                        {
                            try
                            {
                                APIService service = new APIService();
                                var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                                if (userStateInfos != null)
                                {
                                    if (userStateInfos.Active)
                                    {
                                        if (this.notifyIcon.Text.Contains("会员过期"))
                                        {
                                            SetIconToolTip("词牛（已登录）");
                                        }
                                    }
                                    else
                                    {
                                        if (!this.notifyIcon.Text.Contains("会员过期"))
                                        {
                                            SetIconToolTip("词牛（会员过期）", "MyAppError.ico");
                                        }
                                    }
                                    viewModel.CurrentUserInfo = userStateInfos;
                                }
                            }
                            catch
                            { }
                            count = 0;
                        }
                        count++;
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
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = true;
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.Hide();
        }
        /// <summary>
        /// 解析校验文档
        /// </summary>
        /// <param name="filePath"></param>
        private List<UnChekedWordInfo> LoadDocx(string filePath)
        {
            List<UnChekedWordInfo> listResult = new List<UnChekedWordInfo>();
            try
            {
                string _documentName = filePath;
                //获取文档内容进行解析
                try
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(_documentName);
                    string pathDir = CheckWordTempPath + "\\" + fileName + System.IO.Path.GetExtension(_documentName).Replace(".", "") + "-Docx\\";
                    FileOperateHelper.DeleteFolder(pathDir);
                    if (!Directory.Exists(pathDir))
                    {
                        Directory.CreateDirectory(pathDir);
                    }
                    Aspose.Words.Document doc = new Aspose.Words.Document(_documentName);
                    int index = 1;
                    //取得对象集合
                    Aspose.Words.NodeCollection shapes = doc.GetChildNodes(Aspose.Words.NodeType.Shape, true);
                    foreach (Aspose.Words.Drawing.Shape shape in shapes)
                    {
                        if (shape != null && shape.HasImage)
                        {
                            string imageName = String.Format(pathDir + "照片-{0}.png", index);
                            shape.ImageData.Save(imageName);
                            index++;
                        }
                    }
                    foreach (Aspose.Words.Section section in doc.Sections)
                    {
                        foreach (Aspose.Words.Paragraph paragraph in section.Body.Paragraphs)
                        {
                            string textResult = paragraph.GetText();
                            var list = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(textResult).ToList();
                            foreach (var item in list)
                            {
                                MatchCollection mc = Regex.Matches(textResult, item.Name, RegexOptions.IgnoreCase);
                                if (mc.Count > 0)
                                {
                                    foreach (Match m in mc)
                                    {
                                        var infoResult = listResult.FirstOrDefault(x => x.Name == item.Name);
                                        if (infoResult == null)
                                        {
                                            item.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { InLineKeyText = item.Name, InLineText = textResult });
                                            item.ErrorTotalCount++;
                                            listResult.Add(item);
                                        }
                                        else
                                        {
                                            infoResult.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { InLineKeyText = item.Name, InLineText = textResult });
                                            infoResult.ErrorTotalCount++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #region Spire.Doc
                    //Document document = new Document();
                    //document.LoadFromFileInReadMode(_documentName, FileFormat.Auto);
                    //int index = 1;
                    //foreach (Spire.Doc.Section section in document.Sections)
                    //{
                    //    foreach (Paragraph paragraph in section.Paragraphs)
                    //    {
                    //        string textResult = "";
                    //        foreach (DocumentObject docObject in paragraph.ChildObjects)
                    //        {
                    //            if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                    //            {
                    //                DocPicture picture = docObject as DocPicture;
                    //                string imageName = String.Format(pathDir + "照片-{0}.png", index);
                    //                picture.Image.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    //                index++;
                    //            }
                    //            if (docObject.DocumentObjectType == DocumentObjectType.TextRange)
                    //            {
                    //                TextRange textRange = docObject as TextRange;
                    //                textResult += textRange.Text;
                    //            }
                    //        }
                    //        var list = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(textResult).ToList();
                    //        foreach (var item in list)
                    //        {
                    //            MatchCollection mc = Regex.Matches(textResult, item.Name, RegexOptions.IgnoreCase);
                    //            if (mc.Count > 0)
                    //            {
                    //                foreach (Match m in mc)
                    //                {
                    //                    var infoResult = listResult.FirstOrDefault(x => x.Name == item.Name);
                    //                    if (infoResult == null)
                    //                    {
                    //                        item.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { InLineKeyText = item.Name, InLineText = textResult });
                    //                        item.ErrorTotalCount++;
                    //                        listResult.Add(item);
                    //                    }
                    //                    else
                    //                    {
                    //                        infoResult.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { InLineKeyText = item.Name, InLineText = textResult });
                    //                        infoResult.ErrorTotalCount++;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<SendNotifyMessageEvent>().Publish("4002");
                }
            }
            catch (Exception ex)
            { }
            return listResult;
        }
        #region ORC识别
        bool isInitCompleted = false;
        int countWhile = 0;
        double xScale = 1;
        double yScale = 1;
        BitmapImage bitmap = null;
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="fileName"></param>
        private void SavePic(string fileName)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if(ImgGrid.ActualWidth > 0 && ImgGrid.ActualHeight > 0)
                    {
                        RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)ImgGrid.ActualWidth, (int)ImgGrid.ActualHeight, 96, 96, PixelFormats.Default);
                        targetBitmap.Render(ImgGrid);
                        PngBitmapEncoder saveEncoder = new PngBitmapEncoder();
                        saveEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                        using (FileStream fs = System.IO.File.Open(fileName, System.IO.FileMode.Create))
                        {
                            saveEncoder.Save(fs);
                        }
                    }
                    img.Source = null;
                    TextOverlay.Children.Clear();
                }));
            }
            catch (Exception ex)
            { }
        }
        #endregion

        private void img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            xScale = img.ActualWidth / bitmap.PixelWidth;
            yScale = img.ActualHeight / bitmap.PixelHeight;
            isInitCompleted = true;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            WPFClientCheckWordUtil.CheckWordHelper.WordModels = new List<WPFClientCheckWordModel.WordModel>();
            RemoveHotKey();
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
        private void Window_StateChanged(object sender, EventArgs e)
        {
            viewModel.MainSetVisibility = Visibility.Collapsed;
            if (this.WindowState == WindowState.Minimized)
            {
                
            }
            else
            {
                windowState = this.WindowState;
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
                && viewModel.MenueLoginVisibility == Visibility.Visible
                && viewModel.OpenFloatWindowContent == "显示浮动窗口")
            {
                OpenFloatWindow_Click(null, null);
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
                System.Windows.Controls.ContextMenu NotifyIconMenu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifyIconMenu");
                NotifyIconMenu.DataContext = viewModel;
                NotifyIconMenu.IsOpen = true;
                //设置悬浮框异常关闭的显示
                if (viewModel.OpenFloatWindowEnable)
                {
                    try
                    {
                        Process[] processes = Process.GetProcessesByName("BrowseSearchTXT");
                        if (processes.Count() == 0 && viewModel.OpenFloatWindowContent == "隐藏浮动窗口")
                        {
                            viewModel.OpenFloatWindowContent = "显示浮动窗口";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                try
                {
                    this.Activate();
                }
                catch (Exception ex)
                { }
            }
            else if (e.Button == MouseButtons.Left && viewModel.MainSetVisibility == Visibility.Collapsed)
            {
                this.Show();
                this.Activate();
                this.WindowState = windowState;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CloseDetailWindowEvent>().Publish(true);
            this.notifyIcon.Visible = false;
            LeaveWcfService();
            CloseConsoleWPFClientServer();
            CloseSearchPop();
            CloseNotifyMessageView();
        }
        private void MenuCiKuManager_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckVersion_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = windowState;
        }
        private void MenuLoginOut_Click(object sender, RoutedEventArgs e)
        {
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
            viewModel.UserInfoGridVisibility = Visibility.Collapsed;
            viewModel.MenueUnLoginVisibility = Visibility.Visible;
            viewModel.MenueLoginVisibility = Visibility.Collapsed;

            viewModel.TitleLogoVisibility = Visibility.Visible;
            viewModel.ReturnBackBtnVisibility = Visibility.Collapsed;
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("Login");
            CloseSearchPop();
            EventAggregatorRepository.EventAggregator.GetEvent<HideDetailWindowEvent>().Publish(true);
            EventAggregatorRepository.EventAggregator.GetEvent<LoginInOrOutEvent>().Publish("LoginOut");
            this.Show();
            this.WindowState = windowState;
        }

        private void MenuUserInfo_Click(object sender, RoutedEventArgs e)
        {
            new Task(() => {
                try
                {
                    APIService service = new APIService();
                    var userStateInfos = service.GetUserStateByToken(UtilSystemVar.UserToken);
                    if (userStateInfos != null)
                    {
                        if (userStateInfos.Active)
                        {
                            if (this.notifyIcon.Text.Contains("会员过期"))
                            {
                                SetIconToolTip("词牛（已登录）");
                            }
                        }
                        else
                        {
                            if (!this.notifyIcon.Text.Contains("会员过期"))
                            {
                                SetIconToolTip("词牛（会员过期）", "MyAppError.ico");
                            }
                        }
                        viewModel.CurrentUserInfo = userStateInfos;
                    }
                }
                catch
                { }
            }).Start();
            viewModel.IsUserInfoPopWindowOpen = true;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OpenFloatWindow_Click(object sender, RoutedEventArgs e)
        {
            if(viewModel.OpenFloatWindowContent == "显示浮动窗口")
            {
                ShowSearchPop();
            }
            else
            {
                CloseSearchPop(true);
            }
        }
        private void App_Deactivated(object sender, EventArgs e)
        {
            System.Windows.Controls.ContextMenu NotifyIconMenu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifyIconMenu");
            if (NotifyIconMenu.IsOpen == true)
            {
                NotifyIconMenu.IsOpen = false;
            }
        }
        private IntPtr Handle;
        /// <summary>  
        /// 添加快捷键监听  
        /// </summary>  
        private void RunHotKey()
        {
            RegisterHotKey();  //注册快捷查询快捷键  
            //HwndSource source = HwndSource.FromHwnd(Handle);
            //if (source != null)
            //    source.AddHook(WndProc);  //添加Hook，监听窗口事件  
        }
        /// <summary>  
        /// 注册快捷键  
        /// </summary>  
        private void RegisterHotKey()
        {
            //10001为快捷键自定义ID，0x0002为Ctrl键, 0x0001为Alt键，或运算符|表同时按住两个键有效，0x41为A键。  
            ////////HotKey.RegisterHotKey(Handle, 10001, (0x0002 | 0x0001), 0x41);
            HotKey.RegisterHotKey(Handle, 10002, 0, 27);
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
                    if(wParam.ToString() == "10001")
                    {
                        if (viewModel.OpenFloatWindowContent == "显示浮动窗口" && viewModel.OpenFloatWindowEnable
                    && viewModel.MenueLoginVisibility == Visibility.Visible)
                        {
                            ShowSearchPop();
                        }
                    }
                    else if(wParam.ToString() == "10002")
                    {
                        CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                        commonExchangeInfo.Code = "ExchangeBrowseTxTReturnBack";
                        string jsonDataCommonExchange = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                        CheckWordUtil.Win32Helper.SendMessage("BrowseSearchTXT", jsonDataCommonExchange);
                    }
                    break;
                case CheckWordUtil.Win32Helper.WM_COPYDATA:
                    try
                    {
                        CheckWordUtil.Win32Helper.CopyDataStruct cds = (CheckWordUtil.Win32Helper.CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CheckWordUtil.Win32Helper.CopyDataStruct));
                        string jsonData = cds.lpData;
                        System.Threading.ThreadStart start = delegate ()
                        {
                            var result = JsonConvert.DeserializeObject<CommonExchangeInfo>(jsonData);
                            if (result.Code == "ExchangeBrowseTxTPaths")
                            {
                                IsCancelDeal = false;
                                string data = result.Data;
                                var exchangeBrowseTxTPathsInfo = JsonConvert.DeserializeObject<ExchangeBrowseTxTPathsInfo>(data);
                                //处理数据
                                DealData(exchangeBrowseTxTPathsInfo);
                            }
                            else if (result.Code == "ExchangeBrowseTxTCancelDeal")
                            {
                                IsCancelDeal = true;
                                UtilSystemVar.IsDealingData = false;
                            }
                            else if (result.Code == "ExchangeBrowseTxTGoLook")
                            {
                                EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("MainResult");
                                Dispatcher.Invoke(new Action(() => {
                                    this.Topmost = true;
                                    this.Show();
                                    this.WindowState = windowState;
                                    this.Topmost = false;
                                }));
                            }
                            else if (result.Code == "ExchangeBrowseTxTHide")
                            {
                                CloseSearchPop(true);
                            }
                            else if (result.Code == "DealDataProcessingState")
                            {
                                string data = result.Data;
                                var info = JsonConvert.DeserializeObject<DealDataProcessingStateInfo>(data);
                                UtilSystemVar.IsDealingData = info.IsDealingData;
                            }
                            else if(result.Code == "ShowWordAndImgOperationApp")
                            {
                                Dispatcher.Invoke(new Action(() => {
                                    this.Show();
                                    this.Activate();
                                    this.WindowState = windowState;
                                }));
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

        private void RemoveHotKey()
        {
            try
            {
                HwndSource source = HwndSource.FromHwnd(Handle);
                if (source != null)
                    source.RemoveHook(WndProc);  //添加Hook，监听窗口事件
                ////////HotKey.UnregisterHotKey(Handle, 10001);
                HotKey.UnregisterHotKey(Handle, 10002);
            }
            catch (Exception ex)
            { }
        }
        private void ShowSearchPop(bool isSaveState = true)
        {
            try
            {
                string pathBrowseSearchTXT = AppDomain.CurrentDomain.BaseDirectory + "\\BrowseSearchTXT.exe";
                if (File.Exists(pathBrowseSearchTXT))
                {
                    var procBrowseSearchTXT = System.Diagnostics.Process.GetProcessesByName("BrowseSearchTXT");
                    if (procBrowseSearchTXT.Length == 0)
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = pathBrowseSearchTXT;
                        proc.Start();
                        viewModel.OpenFloatWindowContent = "隐藏浮动窗口";
                        if (isSaveState)
                        {
                            SaveSearchPopState(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void CloseSearchPop(bool isSaveState = false)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("BrowseSearchTXT");
                foreach (var p in processes)
                {
                    p.Kill();
                }
                viewModel.OpenFloatWindowContent = "显示浮动窗口";
                if (isSaveState)
                {
                    SaveSearchPopState(false);
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
        private void SaveSearchPopState(bool isPopOpen)
        {
            try
            {
                SearchPopSettingInfo searchPopSettingInfo = new SearchPopSettingInfo();
                searchPopSettingInfo.IsSearchPopStateOpen = isPopOpen;
                //保存用户登录信息到本地
                string searchPopSettingInfos = string.Format(@"{0}\SearchPopSettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(searchPopSettingInfo), searchPopSettingInfos);
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
        private static bool IsCancelDeal = false;
        private static ObservableCollection<MyFolderDataViewModel> DealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="exchangeBrowseTxTPathsInfo"></param>
        public void DealData(ExchangeBrowseTxTPathsInfo pathsInfo)
        {
            try
            {
                int dealDataErrorCount = 0;
                DealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                FileOperateHelper.DeleteFolder(CheckWordTempPath);
                if (!Directory.Exists(CheckWordTempPath))
                {
                    Directory.CreateDirectory(CheckWordTempPath);
                }
                for (int i = 0; i < pathsInfo.Paths.Count; i++)
                {
                    if (IsCancelDeal)
                        return;
                    try
                    {
                        ExchangeBrowseTxTProcessingInfo info = new ExchangeBrowseTxTProcessingInfo();
                        info.IsDealFinished = false;
                        info.CurrentIndex = i;
                        info.TotalCount = pathsInfo.Paths.Count;
                        info.CurrentFileName = System.IO.Path.GetFileName(pathsInfo.Paths[i]);
                        SendProcessingMessageToBrowseSearchTXT(info);
                    }
                    catch (Exception ex)
                    { }
                    DealMyPathsDataSource(pathsInfo.Paths[i]);
                }
                foreach (var item in DealDataResultList)
                {
                    dealDataErrorCount += item.CountError;
                }
                System.Threading.ThreadStart start = delegate ()
                {
                    ExchangeBrowseTxTProcessingInfo exchangeBrowseTxTProcessingInfo = new ExchangeBrowseTxTProcessingInfo();
                    exchangeBrowseTxTProcessingInfo.CurrentIndex = pathsInfo.Paths.Count;
                    exchangeBrowseTxTProcessingInfo.TotalCount = pathsInfo.Paths.Count;
                    exchangeBrowseTxTProcessingInfo.CurrentFileName = System.IO.Path.GetFileName(pathsInfo.Paths.Last());
                    exchangeBrowseTxTProcessingInfo.IsDealFinished = true;
                    exchangeBrowseTxTProcessingInfo.UnCheckWordsCount = dealDataErrorCount;
                    SendProcessingMessageToBrowseSearchTXT(exchangeBrowseTxTProcessingInfo);
                    UtilSystemVar.IsDealingData = false;
                    EventAggregatorRepository.EventAggregator.GetEvent<SendDealDataStateToSeachTxTEvent>().Publish(true);
                };
                System.Threading.Thread t = new System.Threading.Thread(start);
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            { }
        }
        private void SendProcessingMessageToBrowseSearchTXT(ExchangeBrowseTxTProcessingInfo exchangeBrowseTxTProcessingInfo)
        {
            if(!IsCancelDeal)
            {
                CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                commonExchangeInfo.Code = "ExchangeBrowseTxTProcessing";
                commonExchangeInfo.Data = JsonConvert.SerializeObject(exchangeBrowseTxTProcessingInfo);
                string jsonDataCommonExchange = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                CheckWordUtil.Win32Helper.SendMessage("BrowseSearchTXT", jsonDataCommonExchange);
            }
        }
        /// <summary>
        /// 处理检查数据
        /// </summary>
        private void DealMyPathsDataSource(string dealFilePath)
        {
            try
            {
                if (".doc,.docx".Contains(System.IO.Path.GetExtension(dealFilePath).ToLower()))
                {
                    var listUncheckWordInfos = LoadDocx(dealFilePath);
                    MyFolderDataViewModel model = new MyFolderDataViewModel(System.IO.Path.GetFileName(dealFilePath), dealFilePath);
                    model.TypeSelectFile = SelectFileType.Docx;
                    model.FileImgShowPath = AppDomain.CurrentDomain.BaseDirectory + "Resources/WordIcon.png";
                    model.UnChekedWordInfos = new ObservableCollection<UnChekedWordInfo>(listUncheckWordInfos);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(dealFilePath);
                    string pathDir = CheckWordTempPath + "\\" + fileName + System.IO.Path.GetExtension(dealFilePath).Replace(".", "") + "-Docx\\";
                    if (Directory.Exists(pathDir))
                    {
                        DirectoryInfo dirDoc = new DirectoryInfo(pathDir);
                        var filePicInfos = dirDoc.GetFiles();
                        FileOperateHelper.SortAsFileCreationTime(ref filePicInfos);
                        foreach (var picInfo in filePicInfos)
                        {
                            if (picInfo.FullName.Contains("png"))
                            {
                                MyFolderDataViewModel modelPic = new MyFolderDataViewModel(System.IO.Path.GetFileName(picInfo.FullName), picInfo.FullName);
                                modelPic.TypeSelectFile = SelectFileType.Img;
                                var listResult = AutoExcutePicOCR(picInfo.FullName);
                                if (listResult != null)
                                {
                                    foreach (var item in listResult)
                                    {
                                        modelPic.CountError += item.ErrorTotalCount;
                                    }
                                    if (modelPic.CountError > 0)
                                    {
                                        modelPic.UnChekedWordInfos = new ObservableCollection<UnChekedWordInfo>(listResult);
                                        model.Children.Add(modelPic);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var child in model.Children)
                    {
                        foreach (var item in child.UnChekedWordInfos)
                        {
                            var unChekedWordInfoExsit = model.UnChekedWordInfos.FirstOrDefault(x => x.Name == item.Name);
                            if (unChekedWordInfoExsit == null)
                            {
                                model.UnChekedWordInfos.Add(item);
                            }
                            else
                            {
                                foreach (var itemInfo in item.UnChekedWordInLineDetailInfos)
                                {
                                    unChekedWordInfoExsit.UnChekedWordInLineDetailInfos.Add(itemInfo);
                                    unChekedWordInfoExsit.ErrorTotalCount++;
                                }
                            }
                        }
                    }
                    foreach (var item in model.UnChekedWordInfos)
                    {
                        model.CountError += item.ErrorTotalCount;
                    }
                    if (model.CountError > 0)
                    {
                        model.ErrorWordsInfos = string.Join("   ", model.UnChekedWordInfos.Select(x => x.Name).Distinct().ToList());
                        DealDataResultList.Add(model);
                    }
                }
                else if (".png,.jpg,.jpeg".Contains(System.IO.Path.GetExtension(dealFilePath).ToLower()))
                {
                    MyFolderDataViewModel model = new MyFolderDataViewModel(System.IO.Path.GetFileName(dealFilePath), dealFilePath);
                    model.TypeSelectFile = SelectFileType.Img;
                    var listResult = AutoExcutePicOCR(dealFilePath);
                    if (listResult != null)
                    {
                        foreach (var item in listResult)
                        {
                            model.CountError += item.ErrorTotalCount;
                        }
                        if (model.CountError > 0)
                        {
                            string errorImgPath = CheckWordTempPath + " \\" + System.IO.Path.GetFileNameWithoutExtension(dealFilePath) + System.IO.Path.GetExtension(dealFilePath).Replace(".", "") + "-Img\\" + System.IO.Path.GetFileName(dealFilePath);
                            model.FileImgShowPath = errorImgPath;
                            model.UnChekedWordInfos = new ObservableCollection<UnChekedWordInfo>(listResult);
                            model.ErrorWordsInfos = string.Join("   ", model.UnChekedWordInfos.Select(x => x.Name).Distinct().ToList());
                            DealDataResultList.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// ORC自动分析图片
        /// </summary>
        /// <param name="filePath"></param>
        private List<UnChekedWordInfo> AutoExcutePicOCR(string filePath)
        {
            List<UnChekedWordInfo> listResult = new List<UnChekedWordInfo>();
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
                            return null;
                        }
                    }
                }
                catch
                {
                    return null;
                }
                countWhile = 0;
                isInitCompleted = false;
                viewModel.SelectExcuteFilePathInfo = filePath;
                Dispatcher.Invoke(new Action(() => {
                    //清除框选
                    TextOverlay.Children.Clear();
                    //生成绑定图片
                    bitmap = Util.GetBitmapImageForBackUp(viewModel.SelectExcuteFilePathInfo);
                    img.Width = bitmap.PixelWidth;
                    img.Height = bitmap.PixelHeight;
                    img.Source = bitmap;
                }));
                ImgGeneralInfo resultImgGeneral = null;
                try
                {
                    var image = File.ReadAllBytes(filePath);
                    //集成云处理OCR
                    APIService service = new APIService();
                    var result = service.GetOCRResultByToken(UtilSystemVar.UserToken, image);
                    //反序列化
                    resultImgGeneral = JsonConvert.DeserializeObject<ImgGeneralInfo>(result.ToString().Replace("char", "Char"));
                    ////////var options = new Dictionary<string, object>{
                    ////////                    {"recognize_granularity", "small"},
                    ////////                    {"vertexes_location", "true"}
                    ////////                };
                    ////////string apiName = "";
                    ////////try
                    ////////{
                    ////////    apiName = ConfigurationManager.AppSettings["CallAPIName"].ToString();
                    ////////}
                    ////////catch (Exception ex)
                    ////////{ }
                    ////////DESHelper dESHelper = new DESHelper();
                    ////////OCR clientOCR = new OCR(dESHelper.DecryptString(ConfigurationManager.AppSettings["APIKey"].ToString()), dESHelper.DecryptString(ConfigurationManager.AppSettings["SecretKey"].ToString()));
                    ////////var result = clientOCR.Accurate(apiName, image, options);
                    //////////反序列化
                    ////////resultImgGeneral = JsonConvert.DeserializeObject<ImgGeneralInfo>(result.ToString().Replace("char", "Char"));
                }
                catch (Exception ex)
                { }
                while (!isInitCompleted && countWhile < 10)
                {
                    System.Threading.Thread.Sleep(100);
                    countWhile++;
                }
                if (resultImgGeneral != null && resultImgGeneral.words_result_num > 0)
                {
                    List<WordInfo> listUnValidInfos = new List<WordInfo>();
                    foreach (var item in resultImgGeneral.words_result)
                    {
                        string lineWord = "";
                        List<Rect> rects = new List<Rect>();
                        foreach (var charInfo in item.Chars)
                        {
                            lineWord += charInfo.Char;
                            rects.Add(new Rect() { X = charInfo.location.left * xScale, Y = charInfo.location.top * yScale, Width = charInfo.location.width * xScale, Height = charInfo.location.height * yScale });
                        }
                        var listUnChekedWordInfo = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(lineWord);
                        foreach (var itemInfo in listUnChekedWordInfo)
                        {
                            listUnValidInfos.Add(new WordInfo() { UnValidText = itemInfo.Name, AllText = lineWord, Rects = rects });
                            MatchCollection mc = Regex.Matches(lineWord, itemInfo.Name, RegexOptions.IgnoreCase);
                            if (mc.Count > 0)
                            {
                                foreach (Match m in mc)
                                {
                                    var infoResult = listResult.FirstOrDefault(x => x.Name == itemInfo.Name);
                                    if (infoResult == null)
                                    {
                                        itemInfo.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { TypeTextFrom = "Img", InLineText = lineWord });
                                        itemInfo.ErrorTotalCount++;
                                        listResult.Add(itemInfo);
                                    }
                                    else
                                    {
                                        infoResult.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { TypeTextFrom = "Img", InLineText = lineWord });
                                        infoResult.ErrorTotalCount++;
                                    }
                                }
                            }
                        }
                    }
                    string desiredFolderName = CheckWordTempPath + " \\" + System.IO.Path.GetFileNameWithoutExtension(filePath) + System.IO.Path.GetExtension(filePath).Replace(".", "") + "-Img\\";
                    if (!Directory.Exists(desiredFolderName))
                    {
                        Directory.CreateDirectory(desiredFolderName);
                    }
                    //////string txtResultName = desiredFolderName + "UnCheckedResult.txt";
                    //////foreach (var item in listUnValidInfos)
                    //////{
                    //////    FileOperateHelper.WriteTxt(txtResultName, item.UnValidText);
                    //////}
                    var list = CheckWordUtil.CheckWordHelper.GetUnValidRects(listUnValidInfos);
                    foreach (var item in list)
                    {
                        try
                        {
                            Dispatcher.Invoke(new Action(() => {
                                WordOverlay wordBoxOverlay = new WordOverlay(item);
                                var overlay = new Border()
                                {
                                    Style = (System.Windows.Style)this.Resources["HighlightedWordBoxHorizontalLine"]
                                };
                                overlay.SetBinding(Border.MarginProperty, wordBoxOverlay.CreateWordPositionBinding());
                                overlay.SetBinding(Border.WidthProperty, wordBoxOverlay.CreateWordWidthBinding());
                                overlay.SetBinding(Border.HeightProperty, wordBoxOverlay.CreateWordHeightBinding());
                                TextOverlay.Children.Add(overlay);
                            }));
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (listUnValidInfos.Count > 0)
                    {
                        System.Threading.Thread.Sleep(50);
                        SavePic(desiredFolderName + System.IO.Path.GetFileName(filePath));
                    }
                }
            }
            catch (Exception ex)
            { }
            return listResult;
        }

        private void ReturnBackBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<HideDetailWindowEvent>().Publish(true);
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("MainSet");
        }
        /// <summary>
        /// 点击检查按钮检查数据
        /// </summary>
        public void DealCheckBtnData(ObservableCollection<ChekedWordSettingsInfo> pathsInfos)
        {
            try
            {
                IsCancelDeal = false;
                DealDataResultList = new ObservableCollection<MyFolderDataViewModel>();
                try
                {
                    FileOperateHelper.DeleteFolder(CheckWordTempPath);
                    if (!Directory.Exists(CheckWordTempPath))
                    {
                        Directory.CreateDirectory(CheckWordTempPath);
                    }
                }
                catch (Exception ex)
                { }
                foreach (var item in pathsInfos.Where(x => x.IsChecked))
                {
                    if (IsCancelDeal)
                        return;
                    for (int i = 0; i < item.FilePathsList.Count; i++)
                    {
                        if (IsCancelDeal)
                            return;
                        item.CurrentIndex = i + 1;
                        //System.Threading.Thread.Sleep(1000);
                        DealMyPathsDataSource(item.FilePathsList[i]);
                    }
                    item.IsCheckedFinished = true;
                }
                System.Threading.ThreadStart start = delegate ()
                {
                    System.Threading.Thread.Sleep(500);
                    UtilSystemVar.IsDealingData = false;
                    EventAggregatorRepository.EventAggregator.GetEvent<SendDealDataStateToSeachTxTEvent>().Publish(true);
                    EventAggregatorRepository.EventAggregator.GetEvent<DealCheckBtnDataFinishedEvent>().Publish(true);
                };
                System.Threading.Thread t = new System.Threading.Thread(start);
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            { }
        }
        private void CancelDealCheckBtnData(bool b)
        {
            if (b)
            {
                IsCancelDeal = true;
                UtilSystemVar.IsDealingData = false;
                EventAggregatorRepository.EventAggregator.GetEvent<SendDealDataStateToSeachTxTEvent>().Publish(true);
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
                loginInOutInfo.UrlStr = ConfigurationManager.AppSettings["UrlStr"].ToString();
                loginInOutInfo.Token = UtilSystemVar.UserToken;
                try
                {
                    //保存用户登录信息到本地
                    string loginInOutInfos = string.Format(@"{0}\LoginInOutInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    DataParse.WriteToXmlPath(JsonConvert.SerializeObject(loginInOutInfo), loginInOutInfos);
                }
                catch (Exception ex)
                { }
                string json = JsonConvert.SerializeObject(loginInOutInfo);
                mService.ClientSendMessage(json);
            }
            catch (Exception ex)
            { }
        }
        private void SendDealDataStateToSeachTxT(bool b)
        {
            try
            {
                CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                commonExchangeInfo.Code = "DealDataProcessingState";
                DealDataProcessingStateInfo infoDeal = new DealDataProcessingStateInfo();
                infoDeal.IsDealingData = UtilSystemVar.IsDealingData;
                commonExchangeInfo.Data = JsonConvert.SerializeObject(infoDeal);
                string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                Win32Helper.SendMessage("BrowseSearchTXT", jsonData);
            }
            catch (Exception ex)
            { }
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
            try
            {
                System.Diagnostics.Process.Start("http://ciniuwang.com/pay");
            }
            catch (Exception ex)
            { }
        }
    }
}
