using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            InitData();
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
                viewModel = new MyControlViewModel();
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

            }
            catch (Exception ex)
            { }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var workSheet = (Worksheet)workBook.ActiveSheet;
            string workBookName = workBook.Name;
            string workSheetName = workSheet.Name;
            int MaxRow = GetMaxRow(workSheet);
            int MaxColumn = GetMaxColumn(workSheet);
            for (int i = 1; i <= MaxRow; i++)
            {
                for (int j = 1; j <= MaxColumn; j++)
                {
                    string str = CellGetStringValue(workSheet, i, j);
                    if (!string.IsNullOrEmpty(str) &&str.Contains(InputBox.Text))
                    {
                        Range rangeStyle = (Range)(workSheet.Cells[i, j]);
                        if (rangeStyle != null)
                        {
                            rangeStyle.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
                            rangeStyle.Select();
                        }
                    }
                }
            }
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
        public static string CellGetStringValue(Worksheet theSheet, int row, int column)
        {
            var result = string.Empty;
            try
            {
                if (theSheet != null)
                {
                    var rng = theSheet.Cells[row, column] as Range;
                    if (rng != null)
                        result = (string)rng.Text;
                }
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
    }
}
