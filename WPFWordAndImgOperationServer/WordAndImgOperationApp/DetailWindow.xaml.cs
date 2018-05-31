using CheckWordEvent;
using CheckWordModel;
using CheckWordModel.Communication;
using CheckWordUtil;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Newtonsoft.Json;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using WPFClientCheckWordUtil;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// DetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DetailWindow : Window
    {
        WindowState windowState;
        DetailWindowViewModel viewModel = new DetailWindowViewModel();
        public DetailWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            windowState = this.WindowState;
            EventAggregatorRepository.EventAggregator.GetEvent<CloseDetailWindowEvent>().Subscribe(CloseDetailWindow);
            EventAggregatorRepository.EventAggregator.GetEvent<HideDetailWindowEvent>().Subscribe(HideDetailWindow);
            EventAggregatorRepository.EventAggregator.GetEvent<SetDetailWindowTopmostEvent>().Subscribe(SetDetailWindowTopmost);
        }
        private void CloseDetailWindow(bool b)
        {
            this.Close();
        }
        private void HideDetailWindow(bool b)
        {
            this.Hide();
        }
        private void SetDetailWindowTopmost(bool b)
        {
            Dispatcher.Invoke(new Action(() => {
                this.Topmost = b;
                if (b)
                {
                    this.Show();
                    this.Activate();
                    this.WindowState = windowState;
                }
            }));
        }
        public void SetMyFolderDataViewModel(MyFolderDataViewModel myFolderData)
        {
            viewModel.CurrentMyFolderData = myFolderData;
            foreach (var item in viewModel.CurrentMyFolderData.UnChekedWordInfos)
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
                foreach (var infoDetail in item.UnChekedWordInLineDetailInfos)
                {
                    infoDetail.InLineKeyTextRangeStart = -1;
                }
            }
            LoadData();
        }
        /// <summary>
        /// 加载显示数据
        /// </summary>
        private void LoadData()
        {
            try
            {
                if (viewModel.CurrentMyFolderData != null)
                {
                    if (viewModel.CurrentMyFolderData.TypeSelectFile == SelectFileType.Docx)
                    {
                        OnlyLoadDocx();
                    }
                    else if (viewModel.CurrentMyFolderData.TypeSelectFile == SelectFileType.Img)
                    {
                        OnlyLoadImg();
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void RichEdit_DocumentLoaded(object sender, EventArgs e)
        {
            System.Windows.Threading.Dispatcher x = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            System.Threading.ThreadStart start = delegate ()
            {
                System.Threading.Thread.Sleep(500);
                Dispatcher.Invoke(new Action(() => {
                    try
                    {
                        docViewer.ReadOnly = false;
                        docViewer.HorizontalRuler.Visibility = System.Windows.Visibility.Hidden;
                        docViewer.VerticalRuler.Visibility = System.Windows.Visibility.Hidden;
                        foreach (var searchStr in viewModel.CurrentMyFolderData.UnChekedWordInfos.Select(y => y.Name).ToList())
                        {
                            //string searchStr = string.Join("|", viewModel.CurrentMyFolderData.UnChekedWordInfos.Select(y => y.Name).ToList());
                            DocumentRange[] list = docViewer.Document.FindAll(new Regex(@searchStr));
                            for (int i = 0; i < list.Length; i++)
                            {
                                try
                                {
                                    CharacterProperties cp = docViewer.Document.BeginUpdateCharacters(list[i]);
                                    cp.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff00");
                                    this.docViewer.Document.EndUpdateCharacters(cp);
                                }
                                catch (Exception ex)
                                { }
                                //赋值range
                                var itemDetailInfo = viewModel.CurrentMyFolderData.UnChekedWordInfos.FirstOrDefault(y => y.Name == searchStr).UnChekedWordInLineDetailInfos.Where(z => z.TypeTextFrom == "Text" && z.InLineKeyTextRangeStart == -1).FirstOrDefault();
                                if (itemDetailInfo != null)
                                    itemDetailInfo.InLineKeyTextRangeStart = list[i].Start.ToInt();
                            }
                        }
                        docViewer.ReadOnly = true;
                    }
                    catch (Exception ex)
                    { }
                    viewModel.BusyWindowVisibility = Visibility.Collapsed;
                }));
            };
            System.Threading.Thread t = new System.Threading.Thread(start);
            t.IsBackground = true;
            t.Start();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            docViewer.DocumentLoaded += RichEdit_DocumentLoaded;
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
        }
        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetDetailWindowTopmostEvent>().Publish(false);
            this.WindowState = WindowState.Minimized;
        }
        private void MaxBtn_Checked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
        private void MaxBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<HideDetailWindowEvent>().Publish(true);
        }
        /// <summary>
        /// 仅加载显示
        /// </summary>
        private void OnlyLoadDocx()
        {
            viewModel.BusyWindowVisibility = Visibility.Visible;
            System.Threading.ThreadStart start = delegate ()
            {
                System.Threading.Thread.Sleep(200);
                try
                {
                    viewModel.BusyWindowVisibility = Visibility.Visible;
                    viewModel.PicGridVisibility = Visibility.Collapsed;
                    viewModel.AxFramerControlVisibility = Visibility.Visible;
                    Task task = Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(250);
                        var documentFormat = DocumentFormat.OpenXml;
                        if (System.IO.Path.GetExtension(viewModel.CurrentMyFolderData.FilePath).ToLower() == ".docx")
                        {
                            documentFormat = DocumentFormat.OpenXml;
                        }
                        else if (System.IO.Path.GetExtension(viewModel.CurrentMyFolderData.FilePath).ToLower() == ".doc")
                        {
                            documentFormat = DocumentFormat.Doc;
                        }
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            docViewer.LoadDocument(viewModel.CurrentMyFolderData.FilePath, documentFormat);
                        }));
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() => {
                        viewModel.BusyWindowVisibility = Visibility.Collapsed;
                    }));
                }
            };
            System.Threading.Thread t = new System.Threading.Thread(start);
            t.IsBackground = true;
            t.Start();
        }
        /// <summary>
        /// 仅仅加载显示图片
        /// </summary>
        private void OnlyLoadImg()
        {
            try
            {
                viewModel.PicGridVisibility = Visibility.Visible;
                viewModel.AxFramerControlVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            { }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetDetailWindowTopmostEvent>().Publish(false);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                windowState = this.WindowState;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CloseDetailWindowFinishedEvent>().Publish(true);
            EventAggregatorRepository.EventAggregator.GetEvent<CloseDetailWindowEvent>().Unsubscribe(CloseDetailWindow);
            EventAggregatorRepository.EventAggregator.GetEvent<HideDetailWindowEvent>().Unsubscribe(HideDetailWindow);
            EventAggregatorRepository.EventAggregator.GetEvent<SetDetailWindowTopmostEvent>().Unsubscribe(SetDetailWindowTopmost);
        }

        private void DetailGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var info = grid.Tag as UnChekedWordInfo;
                info.IsChecked = !info.IsChecked;
                foreach (var item in viewModel.CurrentMyFolderData.UnChekedWordInfos)
                {
                    if (item != info)
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(viewModel.CurrentMyFolderData.FilePath); //打开此文件。
            }
            catch (Exception ex)
            { }
        }

        private void InLineGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var info = grid.Tag as UnChekedInLineDetailWordInfo;
                if (info.TypeTextFrom == "Text")
                {
                    try
                    {
                        string searchStr = info.InLineKeyText;
                        ScrollToPosition(info.InLineKeyTextRangeStart);
                        docViewer.Document.Selection = docViewer.Document.CreateRange(info.InLineKeyTextRangeStart, searchStr.Length);
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }
        private void ScrollToPosition(int position)
        {
            try
            {
                docViewer.Document.CaretPosition = docViewer.Document.CreatePosition(position);
                docViewer.ScrollToCaret(0.5f);
            }
            catch (Exception ex)
            { }
        }
        private void ScrollToPosition(DocumentPosition position)
        {
            try
            {
                docViewer.Document.CaretPosition = position;
                docViewer.ScrollToCaret(0.5f);
            }
            catch (Exception ex)
            { }
        }

        private void listBox3_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

        private void InLineDetailNameBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                var info = btn.Tag as UnChekedWordInfo;
                info.IsChecked = !info.IsChecked;
                foreach (var item in viewModel.CurrentMyFolderData.UnChekedWordInfos)
                {
                    if (item != info)
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }
    }
}
