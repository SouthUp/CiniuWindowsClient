using CheckWordModel;
using CheckWordUtil;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace MyExcelAddIn
{
    /// <summary>
    /// MyControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyControl : UserControl
    {
        MyControlViewModel viewModel = new MyControlViewModel();
        // 保存修改过的Range和之前的背景色，以便于恢复
        private List<Range> rangeSelectLists = new List<Range>();
        private List<dynamic> rangeBackColorSelectLists = new List<dynamic>();
        public MyControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
        /// <summary>
        /// 单元格内容改变事件
        /// </summary>
        /// <param name="Sh"></param>
        /// <param name="Target"></param>
        private void Application_SheetChange(object Sh, Range Target)
        {
            try
            {
                if (queue.Count == 0)
                {
                    queue.Enqueue(DateTime.Now);
                }
            }
            catch (Exception ex)
            { }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartDetector();
        }
        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            try
            {
                viewModel.UncheckedWordLists = new ObservableCollection<UnChekedExcelWordInfo>();
                viewModel.WarningTotalCount = 0;
                viewModel.IsBusyVisibility = Visibility.Hidden;
                Thread tGetUncheckedWord = new Thread(GetUncheckedWordLists);
                tGetUncheckedWord.IsBackground = true;
                tGetUncheckedWord.Start();
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 获取违禁词数据
        /// </summary>
        /// <param name="isInitData">是否初始化数据</param>
        public void GetUncheckedWordLists()
        {
            try
            {
                Dispatcher.Invoke(new System.Action(() =>
                {
                    viewModel.IsBusyVisibility = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            { }
            try
            {
                var workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
                var workSheet = (Worksheet)workBook.ActiveSheet;
                int MaxRow = GetMaxRow(workSheet);
                int MaxColumn = GetMaxColumn(workSheet);
                List<Range> RangeDataList = new List<Range>();
                for (int i = 1; i <= MaxRow; i++)
                {
                    for (int j = 1; j <= MaxColumn; j++)
                    {
                        RangeDataList.Add((Range)(workSheet.Cells[i, j]));
                    }
                }
                FindTextAndHightLight(RangeDataList);
            }
            catch (Exception ex)
            { }
            try
            {
                Dispatcher.Invoke(new System.Action(() =>
                {
                    viewModel.IsBusyVisibility = Visibility.Hidden;
                }));
            }
            catch (Exception ex)
            { }
        }
        private static int GetMaxRow(Worksheet workSheet)
        {
            int result = 1;
            try
            {
                result = ((Range)(workSheet.Cells[workSheet.Rows.Count, 1])).End[XlDirection.xlUp].Row;
            }
            catch (Exception ex)
            { }
            return result;
        }
        private static int GetMaxColumn(Worksheet workSheet)
        {
            int result = 1;
            try
            {
                result = ((Range)(workSheet.Cells[1, workSheet.Columns.Count])).End[XlDirection.xlToLeft].Column;
            }
            catch (Exception ex)
            { }
            return result;
        }
        public static string CellGetStringValue(Range rng)
        {
            var result = string.Empty;
            try
            {
                if (rng != null)
                    result = (string)rng.Text;
            }
            catch (Exception ex)
            { }
            return result;
        }
        public static Thread tDetector;
        private static object lockObject = new Object();
        private static Queue<DateTime> queue = new Queue<DateTime>();
        private static bool IsChecking = false;
        /// <summary>
        /// 执行检测任务
        /// </summary>
        private void ExcuteQueue()
        {
            while (true)
            {
                if (queue.Count > 0 && !IsChecking)
                {
                    try
                    {
                        lock (lockObject)
                        {
                            IsChecking = true;
                        }
                        GetUncheckedWordLists();
                        lock (lockObject)
                        {
                            try
                            {
                                DateTime typeDequeue = queue.Dequeue();
                            }
                            catch
                            { }
                            IsChecking = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            IsChecking = false;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }
        /// <summary>
        /// 开始实时检测功能
        /// </summary>
        public void StartDetector()
        {
            try
            {
                Globals.ThisAddIn.Application.SheetChange -= Application_SheetChange;
                Globals.ThisAddIn.Application.SheetChange += Application_SheetChange;
                if (tDetector == null)
                {
                    tDetector = new Thread(ExcuteQueue);
                    tDetector.IsBackground = true;
                    tDetector.Start();
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 关闭实时检测功能
        /// </summary>
        public void CloseDetector()
        {
            try
            {
                Globals.ThisAddIn.Application.SheetChange -= Application_SheetChange;
                if (tDetector != null)
                {
                    tDetector.Abort();
                    tDetector = null;
                }
            }
            catch (Exception ex)
            { }
        }
        private void UnCheckWordGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                UnChekedExcelWordInfo unChekedWordInfo = grid.Tag as UnChekedExcelWordInfo;
                unChekedWordInfo.IsSelected = !unChekedWordInfo.IsSelected;
                foreach (var item in viewModel.UncheckedWordLists)
                {
                    if (item != unChekedWordInfo)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }
        private void InLineDetailNameBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                UnChekedExcelWordInfo unChekedWordInfo = btn.Tag as UnChekedExcelWordInfo;
                unChekedWordInfo.IsSelected = !unChekedWordInfo.IsSelected;
                foreach (var item in viewModel.UncheckedWordLists)
                {
                    if (item != unChekedWordInfo)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }
        private void listBoxChildren_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
        private void UnCheckWordChildrenGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                UnChekedExcelWordInfo unChekedWordInfo = grid.Tag as UnChekedExcelWordInfo;
                unChekedWordInfo.UnCheckWordRange.Select();
            }
        }
        /// <summary>
        /// 查找文本并高亮显示
        /// </summary>
        private void FindTextAndHightLight(List<Range> RangeDataList)
        {
            ObservableCollection<UnChekedExcelWordInfo> listUnCheckWords = new ObservableCollection<UnChekedExcelWordInfo>();
            // 清除文档中的高亮显示
            ClearMark();
            rangeSelectLists = new List<Range>();
            rangeBackColorSelectLists = new List<dynamic>();
            //处理违禁词查找
            try
            {
                int DealPagesCount = 1;
                if (RangeDataList.Count % 10 > 0)
                {
                    DealPagesCount = RangeDataList.Count / 10 + 1;
                }
                else
                {
                    DealPagesCount = RangeDataList.Count / 10;
                    if (DealPagesCount == 0)
                    {
                        DealPagesCount = 1;
                    }
                }
                Parallel.For(0, DealPagesCount, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (i, state) =>
                {
                    var list = RangeDataList.Skip(i * 10).Take(10).ToList();
                    foreach (var item in list)
                    {
                        string str = CellGetStringValue(item);
                        if (!string.IsNullOrEmpty(str))
                        {
                            var listUnChekedWord = CheckWordHelper.GetUnChekedWordInfoList(str).ToList();
                            if (listUnChekedWord != null && listUnChekedWord.Count > 0)
                            {
                                foreach (var strFind in listUnChekedWord.ToList())
                                {
                                    UnChekedExcelWordInfo SelectUnCheckWord = new UnChekedExcelWordInfo() { Name = strFind.Name, UnChekedWordDetailInfos = strFind.UnChekedWordDetailInfos };
                                    MatchCollection mc = Regex.Matches(str, strFind.Name, RegexOptions.IgnoreCase);
                                    if (mc.Count > 0)
                                    {
                                        rangeSelectLists.Add(item);
                                        rangeBackColorSelectLists.Add(item.Interior.Color);
                                        item.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                                        foreach (Match m in mc)
                                        {
                                            try
                                            {
                                                SelectUnCheckWord.Children.Add(new UnChekedExcelWordInfo() { Name = str, UnCheckWordRange = item });
                                                SelectUnCheckWord.Initialize();
                                            }
                                            catch (Exception ex)
                                            { }
                                        }
                                        var infoExist = listUnCheckWords.FirstOrDefault(x => x.Name == SelectUnCheckWord.Name);
                                        if (infoExist == null)
                                        {
                                            listUnCheckWords.Add(SelectUnCheckWord);
                                        }
                                        else
                                        {
                                            foreach (var itemInfo in SelectUnCheckWord.Children)
                                            {
                                                infoExist.Children.Add(itemInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            { }
            foreach (var SelectUnCheckWord in listUnCheckWords)
            {
                var itemInfo = viewModel.UncheckedWordLists.FirstOrDefault(x => x.Name == SelectUnCheckWord.Name);
                if (itemInfo == null)
                {
                    Dispatcher.Invoke(new System.Action(() =>
                    {
                        viewModel.UncheckedWordLists.Add(SelectUnCheckWord);
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new System.Action(() =>
                    {
                        itemInfo.Children.Clear();
                        foreach (var item in SelectUnCheckWord.Children)
                        {
                            itemInfo.Children.Add(item);
                        }
                        itemInfo.WarningCount = itemInfo.Children.Count;
                    }));
                }
            }
            for (int i = 0; i < viewModel.UncheckedWordLists.Count; i++)
            {
                var itemInfo = listUnCheckWords.FirstOrDefault(x => x.Name == viewModel.UncheckedWordLists[i].Name);
                if (itemInfo == null)
                {
                    Dispatcher.Invoke(new System.Action(() =>
                    {
                        viewModel.UncheckedWordLists.RemoveAt(i);
                    }));
                    i--;
                }
            }
            int countTotal = 0;
            foreach (var item in viewModel.UncheckedWordLists)
            {
                countTotal += item.WarningCount;
            }
            Dispatcher.Invoke(new System.Action(() =>
            {
                viewModel.WarningTotalCount = countTotal;
            }));
        }
        /// <summary>
        /// 清除文档中的高亮显示
        /// </summary>
        private void ClearMark()
        {
            try
            {
                for (int i = 0; i < rangeSelectLists.Count; i++)
                {
                    rangeSelectLists[i].Interior.Color = rangeBackColorSelectLists[i];
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
