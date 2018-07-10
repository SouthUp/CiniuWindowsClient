using CheckWordModel.Communication;
using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFClientCheckWordModel;
using static CheckWordUtil.Win32Helper;

namespace BrowseSearchTXT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDealingData = false;
        private static bool IsCancelDeal = false;
        private static bool IsInputCheckGridVisible = false;
        private static bool IsDataProcessResultVisible = false;
        private static List<string> FilePathsList = new List<string>();
        private static List<string> UnCheckFilePathsList = new List<string>();
        private static List<string> UnReadFilePathsList = new List<string>();
        List<string> listClass = new List<string>() { ".png", ".jpg", ".jpeg", ".doc", ".docx" };
        MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(string.Format(@"{0}\SearchPopWindowTipsSettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\")))
            {
                viewModel.HidePopWindowVisibility = Visibility.Visible;
                viewModel.IsPopWindowOpen = true;
                this.AllowDrop = false;
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { Keyboard.Focus(SearchTextBox); }));
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            viewModel.IsDetailPopWindowOpen = false;
            if (this.IsDealingData)
            {
                DragDealingTipGrid.Visibility = Visibility.Collapsed;
                DragTipGrid.Visibility = Visibility.Collapsed;
                return;
            }
            this.IsDealingData = true;
            SendDealDataStateToApp();
            IsCancelDeal = false;
            IsInputCheckGridVisible = viewModel.InputCheckGridVisibility == Visibility.Visible;
            IsDataProcessResultVisible = viewModel.DataProcessResultGridVisibility == Visibility.Visible;
            DragDealingTipGrid.Visibility = Visibility.Collapsed;
            DragTipGrid.Visibility = Visibility.Collapsed;
            FilePathsList = new List<string>();
            UnCheckFilePathsList = new List<string>();
            UnReadFilePathsList = new List<string>();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (var path in ((System.Array)e.Data.GetData(DataFormats.FileDrop)))
                {
                    if (File.Exists(path.ToString()))
                    {
                        if (listClass.Contains(System.IO.Path.GetExtension(path.ToString()).ToLower()))
                        {
                            if (!path.ToString().Contains("~$"))
                            {
                                FilePathsList.Add(path.ToString());
                                if (IsFileOpen(path.ToString()))
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
                    else if(Directory.Exists(path.ToString()))
                    {
                        DirectoryInfo dir = new DirectoryInfo(path.ToString());
                        GetAllFiles(dir);
                    }
                }
                if (FilePathsList.Count > 0)
                {
                    CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                    commonExchangeInfo.Code = "ExchangeBrowseTxTPaths";
                    ExchangeBrowseTxTPathsInfo exchangeBrowseTxTPathsInfo = new ExchangeBrowseTxTPathsInfo();
                    exchangeBrowseTxTPathsInfo.Paths = FilePathsList;
                    commonExchangeInfo.Data = JsonConvert.SerializeObject(exchangeBrowseTxTPathsInfo);
                    string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                    SendMessage("WordAndImgOperationApp", jsonData);
                    viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                    viewModel.DataProcessGridVisibility = Visibility.Visible;
                    viewModel.DataProcessResultGridVisibility = Visibility.Collapsed;
                }
                else
                {
                    if (UnCheckFilePathsList.Count > 0)
                    {
                        viewModel.CheckResultText = UnCheckFilePathsList.Count + "个文件类型不支持.";
                    }
                    else
                    {
                        viewModel.CheckResultText = "未发现支持的文件类型.";
                    }
                    viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                    viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                    viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                    viewModel.CommonCheckResultVisibility = Visibility.Visible;

                    viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                    viewModel.DataProcessGridVisibility = Visibility.Collapsed;
                    viewModel.DataProcessResultGridVisibility = Visibility.Visible;

                    viewModel.TitleLogoVisibility = Visibility.Collapsed;
                    viewModel.ReturnBtnVisibility = Visibility.Visible;

                    this.IsDealingData = false;
                    SendDealDataStateToApp();
                }
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
                        if (IsFileOpen(fi.FullName))
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
        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            DragDealingTipGrid.Visibility = Visibility.Collapsed;
            DragTipGrid.Visibility = Visibility.Collapsed;
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
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
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && !viewModel.IsDetailPopWindowOpen)
            {
                this.DragMove();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExcuteSearchResult(SearchTextBox.Text);
            }
        }
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                CopyDataStruct cds = (CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                string jsonData = cds.lpData;
                var result = JsonConvert.DeserializeObject<CommonExchangeInfo>(jsonData);
                if (result.Code == "ExchangeBrowseTxTReturnBack")
                {
                    if (viewModel.ReturnBtnVisibility == Visibility.Visible && !IsDealingData)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { ReturnBack(); }));
                    }
                }
                else if (result.Code == "DealDataProcessingState")
                {
                    string data = result.Data;
                    var info = JsonConvert.DeserializeObject<DealDataProcessingStateInfo>(data);
                    IsDealingData = info.IsDealingData;
                }
                else if (result.Code == "ExchangeBrowseTxTProcessing")
                {
                    if (!IsCancelDeal)
                    {
                        System.Windows.Threading.Dispatcher x = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                        System.Threading.ThreadStart start = delegate ()
                        {
                            string data = result.Data;
                            var exchangeBrowseTxTProcessingInfo = JsonConvert.DeserializeObject<ExchangeBrowseTxTProcessingInfo>(data);
                            //处理数据
                            viewModel.CurrentProcessingInfo = exchangeBrowseTxTProcessingInfo;
                            if (viewModel.CurrentProcessingInfo.IsDealFinished)
                            {
                                System.Threading.Thread.Sleep(500);
                                this.IsDealingData = false;
                                SendDealDataStateToApp();
                                viewModel.DataProcessGridVisibility = Visibility.Collapsed;
                                viewModel.DataProcessResultGridVisibility = Visibility.Visible;
                                viewModel.TitleLogoVisibility = Visibility.Collapsed;
                                viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                                viewModel.ReturnBtnVisibility = Visibility.Visible;
                                if (viewModel.CurrentProcessingInfo.UnCheckWordsCount == 0)
                                {
                                    if (UnReadFilePathsList.Count > 0)
                                    {
                                        viewModel.CheckResultText = UnReadFilePathsList.Count + "个文件正被编辑，无法读取.";
                                    }
                                    else
                                    {
                                        viewModel.CheckResultText = "未发现违禁词.";
                                    }
                                    viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                                    viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                                    viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                                    viewModel.CommonCheckResultVisibility = Visibility.Visible;
                                }
                                else
                                {
                                    if (UnReadFilePathsList.Count == 0)
                                    {
                                        viewModel.FileReadFailTipsVisibility = Visibility.Collapsed;
                                    }
                                    else if (UnReadFilePathsList.Count == 1)
                                    {
                                        viewModel.FileReadFailTipsVisibility = Visibility.Visible;
                                        viewModel.FileReadFailTips = System.IO.Path.GetFileNameWithoutExtension(UnReadFilePathsList.FirstOrDefault());
                                        viewModel.FileReadFailTipsExtention = System.IO.Path.GetExtension(UnReadFilePathsList.FirstOrDefault());
                                    }
                                    else
                                    {
                                        viewModel.FileReadFailTipsVisibility = Visibility.Visible;
                                        viewModel.FileReadFailTips = UnReadFilePathsList.Count + "个文件";
                                        viewModel.FileReadFailTipsExtention = "";
                                    }
                                    viewModel.CheckResultText =
                                    "共检查" + viewModel.CurrentProcessingInfo.TotalCount + "个文件,发现" + viewModel.CurrentProcessingInfo.UnCheckWordsCount + "个违禁词";
                                    viewModel.TongJiCheckResultVisibility = Visibility.Visible;
                                    viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                                    viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                                    viewModel.CommonCheckResultVisibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                viewModel.DataProcessGridVisibility = Visibility.Visible;
                                viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                                viewModel.DataProcessResultGridVisibility = Visibility.Collapsed;
                            }
                        };
                        System.Threading.Thread t = new System.Threading.Thread(start);
                        t.IsBackground = true;
                        t.Start();
                    }
                }
            }
            return hwnd;
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

        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            ExcuteSearchResult(SearchTextBox.Text);
        }
        /// <summary>
        /// 执行检索查词
        /// </summary>
        /// <param name="inputTxt"></param>
        private void ExcuteSearchResult(string inputTxt)
        {
            if (!string.IsNullOrEmpty(inputTxt))
            {
                int countWords = 0;
                countWords = inputTxt.Count();
                try
                {
                    string token = "";
                    try
                    {
                        string loginInOutInfos = string.Format(@"{0}\LoginInOutInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                        var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                        if (ui != null && ui.ToString() != "")
                        {
                            try
                            {
                                var loginInOutInfo = JsonConvert.DeserializeObject<LoginInOutInfo>(ui.ToString());
                                if (loginInOutInfo != null && loginInOutInfo.Type == "LoginIn")
                                {
                                    token = loginInOutInfo.Token;
                                }
                            }
                            catch
                            { }
                        }
                    }
                    catch (Exception ex)
                    { }
                    APIService service = new APIService();
                    var userStateInfos = service.GetUserStateByToken(token);
                    if (userStateInfos != null)
                    {
                        if (userStateInfos.WordCount < countWords)
                        {
                            try
                            {
                                CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                                commonExchangeInfo.Code = "ShowNotifyMessageView";
                                commonExchangeInfo.Data = "500";
                                string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                                SendMessage("WordAndImgOperationApp", jsonData);
                            }
                            catch
                            { }
                        }
                        else
                        {
                            ConsumeResponse consume = service.GetWordConsume(countWords, token);
                            if (consume != null)
                            {
                                try
                                {
                                    //处理逻辑
                                    var resultInfo = CheckWordUtil.CheckWordHelper.GetUnChekedWordInfoList(inputTxt);
                                    viewModel.CurrentWordInfo.Name = inputTxt;
                                    viewModel.CurrentWordInfoResults = new System.Collections.ObjectModel.ObservableCollection<CheckWordModel.UnChekedWordInfo>(resultInfo);
                                    if (viewModel.CurrentWordInfoResults.Count > 0)
                                    {
                                        viewModel.HasUnChekedWordInfoCount = viewModel.CurrentWordInfoResults.Count;
                                        viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                                        viewModel.SinggleWordCheckResultVisibility = Visibility.Visible;
                                        viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                                        viewModel.CommonCheckResultVisibility = Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                                        viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                                        viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Visible;
                                        viewModel.CommonCheckResultVisibility = Visibility.Collapsed;
                                    }
                                }
                                catch (Exception ex)
                                { }
                                viewModel.TitleLogoVisibility = Visibility.Collapsed;
                                viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                                viewModel.ReturnBtnVisibility = Visibility.Visible;
                                viewModel.DataProcessResultGridVisibility = Visibility.Visible;
                            }
                            else
                            {
                                try
                                {
                                    CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                                    commonExchangeInfo.Code = "ShowNotifyMessageView";
                                    commonExchangeInfo.Data = "200";
                                    string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                                    SendMessage("WordAndImgOperationApp", jsonData);
                                }
                                catch
                                { }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                            commonExchangeInfo.Code = "ShowNotifyMessageView";
                            commonExchangeInfo.Data = "200";
                            string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                            SendMessage("WordAndImgOperationApp", jsonData);
                        }
                        catch
                        { }
                    }
                }
                catch
                { }
            }
        }

        private void GoLookBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
            commonExchangeInfo.Code = "ExchangeBrowseTxTGoLook";
            string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
            SendMessage("WordAndImgOperationApp", jsonData);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsCancelDeal = true;
            CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
            commonExchangeInfo.Code = "ExchangeBrowseTxTCancelDeal";
            string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
            SendMessage("WordAndImgOperationApp", jsonData);
            viewModel.DataProcessGridVisibility = Visibility.Collapsed;
            viewModel.InputCheckGridVisibility = IsInputCheckGridVisible ? Visibility.Visible : Visibility.Collapsed;
            viewModel.DataProcessResultGridVisibility = IsDataProcessResultVisible ? Visibility.Visible : Visibility.Collapsed;
            this.IsDealingData = false;
            SendDealDataStateToApp();
        }

        private void MenuHide_Click(object sender, RoutedEventArgs e)
        {
            if (IsDealingData)
            {
                CancelBtn_Click(null, null);
            }
            CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
            commonExchangeInfo.Code = "ExchangeBrowseTxTHide";
            string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
            SendMessage("WordAndImgOperationApp", jsonData);
        }
        private void ReturnBack()
        {
            if (IsDealingData)
            {
                CancelBtn_Click(null, null);
            }

            viewModel.IsDetailPopWindowOpen = false;

            viewModel.TitleLogoVisibility = Visibility.Visible;
            viewModel.InputCheckGridVisibility = Visibility.Visible;
            viewModel.ReturnBtnVisibility = Visibility.Collapsed;
            viewModel.DataProcessGridVisibility = Visibility.Collapsed;
            viewModel.DataProcessResultGridVisibility = Visibility.Collapsed;
            SearchTextBox.Text = "";
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { Keyboard.Focus(SearchTextBox); }));
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Task task = new Task(() => {
                System.Threading.Thread.Sleep(150);
                if (e.LeftButton == MouseButtonState.Released)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { ReturnBack(); }));
                }
            });
            task.Start();
        }
        private void SendDealDataStateToApp()
        {
            try
            {
                CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                commonExchangeInfo.Code = "DealDataProcessingState";
                DealDataProcessingStateInfo infoDeal = new DealDataProcessingStateInfo();
                infoDeal.IsDealingData = this.IsDealingData;
                commonExchangeInfo.Data = JsonConvert.SerializeObject(infoDeal);
                string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                SendMessage("WordAndImgOperationApp", jsonData);
            }
            catch (Exception ex)
            { }
        }

        private void HidePopWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            this.AllowDrop = true;
            viewModel.HidePopWindowVisibility = Visibility.Collapsed;
            viewModel.IsPopWindowOpen = false;
            try
            {
                SearchPopSettingInfo searchPopSettingInfo = new SearchPopSettingInfo();
                searchPopSettingInfo.IsSearchPopStateOpen = false;
                //保存用户登录信息到本地
                string searchPopSettingInfos = string.Format(@"{0}\SearchPopWindowTipsSettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(searchPopSettingInfo), searchPopSettingInfos);
            }
            catch (Exception ex)
            { }
        }
        private void GoDetailBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Top < 420)
            {
                GoDetailPopup.VerticalOffset = -15;
            }
            else
            {
                GoDetailPopup.VerticalOffset = 10;
            }
            viewModel.IsDetailPopWindowOpen = true;
            try
            {
                foreach (var item in viewModel.CurrentWordInfoResults)
                {
                    foreach (var infoDetail in item.UnChekedWordDetailInfos)
                    {
                        if (!string.IsNullOrEmpty(infoDetail.SourceDBID))
                        {
                            infoDetail.SourceDBImgPath = AppDomain.CurrentDomain.BaseDirectory + "Resources/DBTypeLogo/" + infoDetail.SourceDBID + ".png";
                        }
                        else
                        {
                            infoDetail.SourceDBImgPath = AppDomain.CurrentDomain.BaseDirectory + "Resources/DBTypeLogo/Default.png";
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
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
    }
}
