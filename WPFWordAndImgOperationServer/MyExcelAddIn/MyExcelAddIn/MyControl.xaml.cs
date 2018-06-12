using CheckWordModel;
using CheckWordUtil;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
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
        Dictionary<string, List<UnChekedWordInfo>> CurrentImgsDictionary = new Dictionary<string, List<UnChekedWordInfo>>();
        MyControlViewModel viewModel = new MyControlViewModel();
        // 保存修改过的Range和之前的背景色，以便于恢复
        private List<Range> rangeSelectLists = new List<Range>();
        private List<dynamic> rangeBackColorSelectLists = new List<dynamic>();
        //保存当前要修改的Range的行和列
        private List<Range> rangeCurrentDealingLists = new List<Range>();
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
                // 清除文档中的高亮显示
                ClearMark();
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
            viewModel.IsBusyVisibility = Visibility.Visible;
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
            viewModel.IsBusyVisibility = Visibility.Hidden;
        }
        private static int GetMaxRow(Worksheet workSheet)
        {
            int result = 1;
            try
            {
                //result = ((Range)(workSheet.Cells[workSheet.Rows.Count, 1])).End[XlDirection.xlUp].Row;
                result = workSheet.UsedRange.Rows.Count;
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
                //result = ((Range)(workSheet.Cells[1, workSheet.Columns.Count])).End[XlDirection.xlToLeft].Column;
                result = workSheet.UsedRange.Columns.Count;
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
            rangeCurrentDealingLists = new List<Range>();
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
                                        lock (lockObject)
                                        {
                                            rangeCurrentDealingLists.Add(item);
                                        }
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
            GetPicsFromExcel();
            List<string> listHashs = new List<string>();
            DirectoryInfo dirDoc = new DirectoryInfo(savePathGetImage);
            var filePicInfos = dirDoc.GetFiles();
            foreach (var picInfo in filePicInfos)
            {
                if (picInfo.FullName.Contains("jpg"))
                {
                    string hashPic = HashHelper.ComputeSHA1(picInfo.FullName);
                    listHashs.Add(hashPic);
                    if (!CurrentImgsDictionary.ContainsKey(hashPic))
                    {
                        var listResult = AutoExcutePicOCR(picInfo.FullName);
                        CurrentImgsDictionary.Add(hashPic, listResult);
                    }
                }
            }
            foreach (string key in CurrentImgsDictionary.Keys)
            {
                if (!listHashs.Contains(key))
                {
                    CurrentImgsDictionary.Remove(key);
                }
            }
            foreach (var SelectUnCheckWord in listUnCheckWords)
            {
                var itemInfo = viewModel.UncheckedWordLists.FirstOrDefault(x => x.Name == SelectUnCheckWord.Name);
                Dispatcher.Invoke(new System.Action(() =>
                {
                    if (itemInfo == null)
                    {
                        viewModel.UncheckedWordLists.Add(SelectUnCheckWord);
                    }
                    else
                    {
                        itemInfo.Children.Clear();
                        foreach (var item in SelectUnCheckWord.Children)
                        {
                            itemInfo.Children.Add(item);
                        }
                        itemInfo.WarningCount = itemInfo.Children.Count;
                    }
                }));
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
            //渲染高亮
            foreach (var item in rangeCurrentDealingLists)
            {
                var itemInfo = rangeSelectLists.FirstOrDefault(x => x.Row == item.Row && x.Column == item.Column);
                if (itemInfo == null)
                {
                    rangeSelectLists.Add(item);
                    rangeBackColorSelectLists.Add(item.Interior.Color);
                    item.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                }
            }
            for (int i = 0; i < rangeSelectLists.Count; i++)
            {
                var itemInfo = rangeCurrentDealingLists.FirstOrDefault(x => x.Row == rangeSelectLists[i].Row && x.Column == rangeSelectLists[i].Column);
                if (itemInfo == null)
                {
                    Dispatcher.Invoke(new System.Action(() =>
                    {
                        rangeSelectLists[i].Interior.Color = rangeBackColorSelectLists[i];
                        rangeSelectLists.RemoveAt(i);
                        rangeBackColorSelectLists.RemoveAt(i);
                    }));
                    i--;
                }
            }
            Dispatcher.Invoke(new System.Action(() =>
            {
                viewModel.WarningTotalCount = 0;
                foreach (var item in viewModel.UncheckedWordLists)
                {
                    viewModel.WarningTotalCount += item.WarningCount;
                }
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
                rangeSelectLists = new List<Range>();
                rangeBackColorSelectLists = new List<dynamic>();
            }
            catch (Exception ex)
            { }
        }
        private string CheckWordTempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTempExcel";
        string savePathGetImage = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\MyExcelAddIn\\";
        /// <summary>
        /// 提取图片
        /// </summary>
        private void GetPicsFromExcel()
        {
            try
            {
                var workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
                var workSheet = (Worksheet)workBook.ActiveSheet;
                
                FileOperateHelper.DeleteFolder(savePathGetImage);
                if (!Directory.Exists(savePathGetImage))
                {
                    Directory.CreateDirectory(savePathGetImage);
                }
                for (int i = 1; i <= workSheet.Shapes.Count; i++)
                {
                    var pic = workSheet.Shapes.Item(i);
                    if (pic != null && pic.Type == Microsoft.Office.Core.MsoShapeType.msoPicture)
                    {
                        pic.Copy();
                        System.Drawing.Image image = null;
                        Dispatcher.Invoke(new System.Action(() =>
                        {
                            image = System.Windows.Forms.Clipboard.GetImage();
                        }));
                        if (image != null)
                        {
                            image.Save(savePathGetImage + pic.Name + ".jpg");
                        }
                        Dispatcher.Invoke(new System.Action(() =>
                        {
                            System.Windows.Forms.Clipboard.Clear();
                        }));
                    }
                }
            }
            catch (Exception ex)
            { }
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
                Dispatcher.Invoke(new System.Action(() =>
                {
                    if (ImgGrid.ActualWidth > 0 && ImgGrid.ActualHeight > 0)
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
        /// <summary>
        /// ORC自动分析图片
        /// </summary>
        /// <param name="filePath"></param>
        private List<UnChekedWordInfo> AutoExcutePicOCR(string filePath)
        {
            List<UnChekedWordInfo> listResult = new List<UnChekedWordInfo>();
            try
            {
                countWhile = 0;
                isInitCompleted = false;
                Dispatcher.Invoke(new System.Action(() => {
                    //清除框选
                    TextOverlay.Children.Clear();
                    //生成绑定图片
                    bitmap = Util.GetBitmapImage(filePath);
                    img.Width = bitmap.PixelWidth;
                    img.Height = bitmap.PixelHeight;
                    img.Source = bitmap;
                }));
                ImgGeneralInfo resultImgGeneral = null;
                try
                {
                    var image = File.ReadAllBytes(filePath);
                    var options = new Dictionary<string, object>{
                                        {"recognize_granularity", "small"},
                                        {"vertexes_location", "true"}
                                    };
                    string apiName = "";
                    try
                    {
                        apiName = ConfigurationManager.AppSettings["CallAPIName"].ToString();
                    }
                    catch (Exception ex)
                    { }
                    OCR clientOCR = new OCR(ConfigurationManager.AppSettings["APIKey"].ToString(), ConfigurationManager.AppSettings["SecretKey"].ToString());
                    var result = clientOCR.Accurate(apiName, image, options);
                    //反序列化
                    resultImgGeneral = JsonConvert.DeserializeObject<ImgGeneralInfo>(result.ToString().Replace("char", "Char"));
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
                    string desiredFolderName = CheckWordTempPath + " \\" + Guid.NewGuid().ToString() + "\\";
                    if (!Directory.Exists(desiredFolderName))
                    {
                        Directory.CreateDirectory(desiredFolderName);
                    }
                    var list = CheckWordHelper.GetUnValidRects(listUnValidInfos);
                    foreach (var item in list)
                    {
                        try
                        {
                            Dispatcher.Invoke(new System.Action(() => {
                                WordOverlay wordBoxOverlay = new WordOverlay(item);
                                var overlay = new System.Windows.Controls.Border()
                                {
                                    Style = (System.Windows.Style)this.Resources["HighlightedWordBoxHorizontalLine"]
                                };
                                overlay.SetBinding(System.Windows.Controls.Border.MarginProperty, wordBoxOverlay.CreateWordPositionBinding());
                                overlay.SetBinding(System.Windows.Controls.Border.WidthProperty, wordBoxOverlay.CreateWordWidthBinding());
                                overlay.SetBinding(System.Windows.Controls.Border.HeightProperty, wordBoxOverlay.CreateWordHeightBinding());
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
    }
}
