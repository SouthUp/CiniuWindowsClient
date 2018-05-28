using CheckWordModel.Communication;
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
using static CheckWordUtil.Win32Helper;

namespace BrowseSearchTXT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool IsCancelDeal = false;
        private static bool IsInputCheckGridVisible = false;
        private static bool IsDataProcessResultVisible = false;
        private static List<string> FilePathsList = new List<string>();
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
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,(Action)(() => { Keyboard.Focus(SearchTextBox); }));
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            IsCancelDeal = false;
            IsInputCheckGridVisible = viewModel.InputCheckGridVisibility == Visibility.Visible;
            IsDataProcessResultVisible = viewModel.DataProcessResultGridVisibility == Visibility.Visible;
            DragTipGrid.Visibility = Visibility.Collapsed;
            FilePathsList = new List<string>();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (var path in ((System.Array)e.Data.GetData(DataFormats.FileDrop)))
                {
                    if (File.Exists(path.ToString()))
                    {
                        if (listClass.Contains(System.IO.Path.GetExtension(path.ToString())))
                        {
                            FilePathsList.Add(path.ToString());
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
                    viewModel.CheckResultText = "未发现能检查的文件";
                    viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                    viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                    viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                    viewModel.CommonCheckResultVisibility = Visibility.Visible;

                    viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                    viewModel.DataProcessGridVisibility = Visibility.Collapsed;
                    viewModel.DataProcessResultGridVisibility = Visibility.Visible;
                }
            }
        }
        private void GetAllFiles(DirectoryInfo dir)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                if (listClass.Contains(System.IO.Path.GetExtension(fi.FullName)))
                {
                    FilePathsList.Add(fi.FullName);
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
            DragTipGrid.Visibility = Visibility.Collapsed;
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            DragTipGrid.Visibility = Visibility.Visible;
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExcuteSearchResult(SearchTextBox.Text);
            }
        }
        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TitleLogoVisibility = Visibility.Visible;
            viewModel.InputCheckGridVisibility = Visibility.Visible;
            viewModel.ReturnBtnVisibility = Visibility.Collapsed;
            viewModel.DataProcessGridVisibility = Visibility.Collapsed;
            viewModel.DataProcessResultGridVisibility = Visibility.Collapsed;
            SearchTextBox.Text = "";
        }
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                if(!IsCancelDeal)
                {
                    CopyDataStruct cds = (CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                    System.Windows.Threading.Dispatcher x = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                    System.Threading.ThreadStart start = delegate ()
                    {
                        string jsonData = cds.lpData;
                        var result = JsonConvert.DeserializeObject<CommonExchangeInfo>(jsonData);
                        if (result.Code == "ExchangeBrowseTxTProcessing")
                        {
                            string data = result.Data;
                            var exchangeBrowseTxTProcessingInfo = JsonConvert.DeserializeObject<ExchangeBrowseTxTProcessingInfo>(data);
                            //处理数据
                            viewModel.CurrentProcessingInfo = exchangeBrowseTxTProcessingInfo;
                            if (viewModel.CurrentProcessingInfo.IsDealFinished)
                            {
                                System.Threading.Thread.Sleep(500);
                                viewModel.DataProcessGridVisibility = Visibility.Collapsed;
                                viewModel.DataProcessResultGridVisibility = Visibility.Visible;
                                viewModel.TitleLogoVisibility = Visibility.Collapsed;
                                viewModel.InputCheckGridVisibility = Visibility.Collapsed;
                                viewModel.ReturnBtnVisibility = Visibility.Visible;
                                if (viewModel.CurrentProcessingInfo.UnCheckWordsCount == 0)
                                {
                                    viewModel.CheckResultText = "未发现违禁词";
                                    viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                                    viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                                    viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                                    viewModel.CommonCheckResultVisibility = Visibility.Visible;
                                }
                                else
                                {
                                    viewModel.CheckResultText =
                                    viewModel.CurrentProcessingInfo.TotalCount + "个文件中有" + viewModel.CurrentProcessingInfo.UnCheckWordsCount + "个违禁词";
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
                        }
                    };
                    System.Threading.Thread t = new System.Threading.Thread(start);
                    t.IsBackground = true;
                    t.Start();
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
                try
                {
                    //处理逻辑
                    var resultInfo = CheckWordUtil.CheckWordHelper.GetOneWordInfo(inputTxt);
                    viewModel.CurrentWordInfo = resultInfo;
                    if (viewModel.CurrentWordInfo.IsUnCheckWord)
                    {
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
            }
            else
            {
                viewModel.CheckResultText = "未发现违禁词";
                viewModel.TongJiCheckResultVisibility = Visibility.Collapsed;
                viewModel.SinggleWordCheckResultVisibility = Visibility.Collapsed;
                viewModel.SinggleWordCheckResultNoUncheckVisibility = Visibility.Collapsed;
                viewModel.CommonCheckResultVisibility = Visibility.Visible;
            }
            viewModel.TitleLogoVisibility = Visibility.Collapsed;
            viewModel.InputCheckGridVisibility = Visibility.Collapsed;
            viewModel.ReturnBtnVisibility = Visibility.Visible;
            viewModel.DataProcessResultGridVisibility = Visibility.Visible;
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
        }

        private void MenuHide_Click(object sender, RoutedEventArgs e)
        {
            CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
            commonExchangeInfo.Code = "ExchangeBrowseTxTHide";
            string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
            SendMessage("WordAndImgOperationApp", jsonData);
        }
    }
}
