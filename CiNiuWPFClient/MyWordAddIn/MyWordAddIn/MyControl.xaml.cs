using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using CheckWordEvent;
using CheckWordModel;
using CheckWordModel.Communication;
using CheckWordUtil;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;

namespace MyWordAddIn
{
    /// <summary>
    /// MyControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyControl : UserControl
    {
        List<UnChekedWordInfo> listUnCheckWordsImages = new List<UnChekedWordInfo>();
        private ConcurrentBag<UnChekedWordParagraphInfo> HasUnChenckedWordsParagraphs = new ConcurrentBag<UnChekedWordParagraphInfo>();
        Dictionary<string, List<UnChekedWordInfo>> CurrentWordsDictionary = new Dictionary<string, List<UnChekedWordInfo>>();
        List<UnChekedWordInfo> listUnCheckWords = new List<UnChekedWordInfo>();
        MyControlViewModel viewModel = new MyControlViewModel();
        // 保存修改过的Range和之前的背景色，以便于恢复
        private ConcurrentBag<Range> rangeSelectLists = new ConcurrentBag<Range>();
        //文本改变检测
        TextChangeDetector detector;
        //图片改变检测
        ImagesChangeDetector detectorImages;
        Microsoft.Office.Interop.Word.Application Application = Globals.ThisAddIn.Application;
        bool isFirst = true;
        public MyControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<MarkUnCheckWordEvent>().Subscribe(MarkUnCheckWord);
        }
        private void MarkUnCheckWord(bool b)
        {
            try
            {
                foreach (var item in rangeSelectLists)
                {
                    item.HighlightColorIndex = WdColorIndex.wdYellow;
                }
            }
            catch (Exception ex)
            { }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartDetector();
        }
        private void detector_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!queue.Contains("Text"))
                {
                    queue.Enqueue("Text");
                }
            }
            catch (Exception ex)
            { }
        }
        private void detector_OnImagesChanged(object sender, ImagesChangedEventArgs e)
        {
            try
            {
                if (!queue.Contains("Images"))
                {
                    queue.Enqueue("Images");
                }
            }
            catch (Exception ex)
            { }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseDetector();
        }
        public static Thread tDetector;
        private static object lockObject = new Object();
        private static Queue<string> queue = new Queue<string>();
        private static bool IsChecking = false;
        /// <summary>
        /// 执行检测任务
        /// </summary>
        private void ExcuteQueue()
        {
            while (true)
            {
                try
                {
                    if (queue.Count > 0 && !IsChecking)
                    {
                        try
                        {
                            string typeDequeue = "";
                            lock (lockObject)
                            {
                                IsChecking = true;
                                try
                                {
                                    typeDequeue = queue.Dequeue();
                                }
                                catch
                                { }
                            }
                            if (typeDequeue == "Text")
                            {
                                GetUncheckedWordLists();
                            }
                            else if (typeDequeue == "Images")
                            {
                                OnlyExcutePicture();
                            }
                            lock (lockObject)
                            {
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
                catch (Exception ex)
                { }
            }
        }
        /// <summary>
        /// 开始实时检测功能
        /// </summary>
        public void StartDetector()
        {
            try
            {
                IsChecking = false;
            }
            catch
            { }
            try
            {
                if (detector == null)
                    detector = new TextChangeDetector(Application);
                detector.OnTextChanged += detector_OnTextChanged;
                detector.Start();
                if (detectorImages == null)
                    detectorImages = new ImagesChangeDetector(Application);
                detectorImages.OnImagesChanged += detector_OnImagesChanged;
                detectorImages.Start();
                if (tDetector == null)
                {
                    tDetector = new System.Threading.Thread(ExcuteQueue);
                    tDetector.IsBackground = true;
                    tDetector.Start();
                }
            }
            catch (Exception ex)
            { }
            try
            {
                if (isFirst)
                {
                    isFirst = false;
                    if (!queue.Contains("Images"))
                    {
                        queue.Enqueue("Images");
                    }
                }
            }
            catch
            { }
        }
        /// <summary>
        /// 关闭实时检测功能
        /// </summary>
        public void CloseDetector()
        {
            try
            {
                if (detector != null)
                {
                    detector.OnTextChanged -= detector_OnTextChanged;
                    detector.Stop();
                }
                if (detectorImages != null)
                {
                    detectorImages.OnImagesChanged -= detector_OnImagesChanged;
                    detectorImages.Stop();
                }
                if (tDetector != null)
                {
                    tDetector.Abort();
                    tDetector = null;
                }
            }
            catch (Exception ex)
            { }
            try
            {
                IsChecking = false;
            }
            catch
            { }
        }
        /// <summary>
        /// 获取查到的违禁字列表
        /// </summary>
        private void GetUncheckedWordLists()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    viewModel.IsBusyVisibility = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            { }
            try
            {
                if (Util.GetIsUserLogin())
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.IsUnLogin = false;
                    }));
                    FindTextAndHightLight();
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.IsUnLogin = true;
                    }));
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.WarningTotalCount = 0;
                        viewModel.UncheckedWordLists.Clear();
                    }));
                }
            }
            catch (Exception ex)
            { }
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    viewModel.IsBusyVisibility = Visibility.Hidden;
                }));
            }
            catch (Exception ex)
            { }
        }
        private static AutoResetEvent myEvent = new AutoResetEvent(false);
        private int countDealParagraph = 0;
        /// <summary>
        /// 查找文本并高亮显示
        /// </summary>
        /// <param name="strFind"></param>
        private void FindTextAndHightLight()
        {
            listUnCheckWords = new List<UnChekedWordInfo>();
            rangeSelectLists = new ConcurrentBag<Range>();
            HasUnChenckedWordsParagraphs = new ConcurrentBag<UnChekedWordParagraphInfo>();
            //线程池处理数据
            ThreadPool.SetMaxThreads(500, 500);
            countDealParagraph = Application.ActiveDocument.Paragraphs.Count;
            //检测整个文档
            foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in Application.ActiveDocument.Paragraphs)
            {
                ThreadPool.QueueUserWorkItem(DealSingleParagraph, paragraph);
            }
            myEvent.WaitOne();
            foreach (var ItemInfo in HasUnChenckedWordsParagraphs)
            {
                Microsoft.Office.Interop.Word.Paragraph paragraph = ItemInfo.Paragraph;
                var listUnChekedWord = ItemInfo.UnChekedWordLists;
                foreach (var strFind in listUnChekedWord.ToList())
                {
                    UnChekedWordInfo SelectUnCheckWord = new UnChekedWordInfo() { ID = strFind.ID, Name = strFind.Name, UnChekedWordDetailInfos = strFind.UnChekedWordDetailInfos };
                    MatchCollection mc = Regex.Matches(paragraph.Range.Text, strFind.Name, RegexOptions.IgnoreCase);
                    if (mc.Count > 0)
                    {
                        foreach (Match m in mc)
                        {
                            try
                            {
                                int startIndex = paragraph.Range.Start + m.Index;
                                int endIndex = paragraph.Range.Start + m.Index + m.Length;
                                Range keywordRange = Application.ActiveDocument.Range(startIndex, endIndex);
                                rangeSelectLists.Add(keywordRange);
                                SelectUnCheckWord.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { InLineText = paragraph.Range.Text, UnCheckWordRange = keywordRange });
                                SelectUnCheckWord.ErrorTotalCount++;
                            }
                            catch (Exception ex)
                            { }
                        }
                        var infoExist = listUnCheckWords.AsParallel().FirstOrDefault(x => x.Name == SelectUnCheckWord.Name);
                        if (infoExist == null)
                        {
                            listUnCheckWords.Add(SelectUnCheckWord);
                        }
                        else
                        {
                            foreach (var item in SelectUnCheckWord.UnChekedWordInLineDetailInfos)
                            {
                                infoExist.UnChekedWordInLineDetailInfos.Add(item);
                                infoExist.ErrorTotalCount++;
                            }
                        }
                    }
                }
            }
            APIService service = new APIService();
            bool isCheckPicInDucument = service.GetIsCheckPicAddIn();
            if (isCheckPicInDucument)
            {
                foreach (var item in listUnCheckWordsImages.ToList())
                {
                    var infoExist = listUnCheckWords.AsParallel().FirstOrDefault(x => x.Name == item.Name);
                    if (infoExist == null)
                    {
                        UnChekedWordInfo unChekedWordInfoNoExist = new UnChekedWordInfo();
                        unChekedWordInfoNoExist.ID = item.ID;
                        unChekedWordInfoNoExist.Name = item.Name;
                        unChekedWordInfoNoExist.ErrorTotalCount = item.ErrorTotalCount;
                        unChekedWordInfoNoExist.Range = item.Range;
                        unChekedWordInfoNoExist.UnCheckWordRange = item.UnCheckWordRange;
                        unChekedWordInfoNoExist.UnChekedWordInLineDetailInfos = new ObservableCollection<UnChekedInLineDetailWordInfo>(item.UnChekedWordInLineDetailInfos.ToList());
                        unChekedWordInfoNoExist.UnChekedWordDetailInfos = new ObservableCollection<UnChekedDetailWordInfo>(item.UnChekedWordDetailInfos.ToList());
                        unChekedWordInfoNoExist.TypeTextFrom = item.TypeTextFrom;
                        unChekedWordInfoNoExist.IsSelected = item.IsSelected;
                        listUnCheckWords.Add(unChekedWordInfoNoExist);
                    }
                    else
                    {
                        UnChekedWordInfo unChekedWordInfoExist = new UnChekedWordInfo();
                        unChekedWordInfoExist.ID = infoExist.ID;
                        unChekedWordInfoExist.Name = infoExist.Name;
                        unChekedWordInfoExist.ErrorTotalCount = infoExist.ErrorTotalCount;
                        unChekedWordInfoExist.Range = infoExist.Range;
                        unChekedWordInfoExist.UnCheckWordRange = infoExist.UnCheckWordRange;
                        unChekedWordInfoExist.UnChekedWordInLineDetailInfos = new ObservableCollection<UnChekedInLineDetailWordInfo>(infoExist.UnChekedWordInLineDetailInfos.ToList());
                        unChekedWordInfoExist.UnChekedWordDetailInfos = new ObservableCollection<UnChekedDetailWordInfo>(infoExist.UnChekedWordDetailInfos.ToList());
                        unChekedWordInfoExist.TypeTextFrom = infoExist.TypeTextFrom;
                        unChekedWordInfoExist.IsSelected = infoExist.IsSelected;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            foreach (var detail in item.UnChekedWordInLineDetailInfos.ToList())
                            {
                                unChekedWordInfoExist.UnChekedWordInLineDetailInfos.Add(detail);
                                unChekedWordInfoExist.ErrorTotalCount++;
                            }
                            listUnCheckWords.Remove(infoExist);
                            listUnCheckWords.Add(unChekedWordInfoExist);
                        }));
                    }
                }
            }
            foreach (var SelectUnCheckWord in listUnCheckWords)
            {
                var itemInfo = viewModel.UncheckedWordLists.AsParallel().FirstOrDefault(x => x.Name == SelectUnCheckWord.Name);
                if (itemInfo == null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.UncheckedWordLists.Add(SelectUnCheckWord);
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        itemInfo.UnChekedWordInLineDetailInfos = SelectUnCheckWord.UnChekedWordInLineDetailInfos;
                        itemInfo.ErrorTotalCount = SelectUnCheckWord.ErrorTotalCount;
                    }));
                }
            }
            for (int i = 0; i < viewModel.UncheckedWordLists.Count; i++)
            {
                var itemInfo = listUnCheckWords.AsParallel().FirstOrDefault(x => x.Name == viewModel.UncheckedWordLists[i].Name);
                if (itemInfo == null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.UncheckedWordLists.RemoveAt(i);
                    }));
                    i--;
                }
            }
            int countTotal = 0;
            Parallel.ForEach(viewModel.UncheckedWordLists, (item, loopState) =>
            {
                lock (lockObject)
                {
                    countTotal += item.ErrorTotalCount;
                }
            });
            Dispatcher.Invoke(new Action(() =>
            {
                viewModel.WarningTotalCount = countTotal;
            }));
        }
        /// <summary>
        /// 解析处理段落
        /// </summary>
        /// <param name="ParagraphDataList"></param>
        private void DealSingleParagraph(object obj)
        {
            try
            {
                Microsoft.Office.Interop.Word.Paragraph paragraph = obj as Microsoft.Office.Interop.Word.Paragraph;
                if (paragraph != null)
                {
                    string text = paragraph.Range.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        List<UnChekedWordInfo> listUnChekedWord = new List<UnChekedWordInfo>();
                        string hashWord = HashHelper.ComputeSHA1ByStr(text);
                        try
                        {
                            if (!CurrentWordsDictionary.ContainsKey(hashWord))
                            {
                                listUnChekedWord = CheckWordHelper.GetUnChekedWordInfoList(text).ToList();
                                if (listUnChekedWord != null)
                                {
                                    try
                                    {
                                        CurrentWordsDictionary.Add(hashWord, listUnChekedWord);
                                    }
                                    catch
                                    { }
                                }
                            }
                            else
                            {
                                listUnChekedWord = CurrentWordsDictionary[hashWord];
                            }
                        }
                        catch (Exception ex)
                        {
                            CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                        }
                        if (listUnChekedWord != null && listUnChekedWord.Count > 0)
                        {
                            HasUnChenckedWordsParagraphs.Add(new UnChekedWordParagraphInfo { Paragraph = paragraph, UnChekedWordLists = listUnChekedWord });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            try
            {
                // 以原子操作的形式递减指定变量的值并存储结果。
                if (Interlocked.Decrement(ref countDealParagraph) == 0)
                {
                    // 将事件状态设置为有信号，从而允许一个或多个等待线程继续执行。
                    myEvent.Set();
                }
            }
            catch
            { }
        }
        /// <summary>
        /// 解析图片OCR
        /// </summary>
        private void OnlyExcutePicture()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    viewModel.IsBusyVisibility = Visibility.Visible;
                }));
                listUnCheckWordsImages = new List<UnChekedWordInfo>();
            }
            catch (Exception ex)
            { }
            if (Util.GetIsUserLogin())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    viewModel.IsUnLogin = false;
                }));
                try
                {
                    APIService service = new APIService();
                    bool isCheckPicInDucument = service.GetIsCheckPicAddIn();
                    if (isCheckPicInDucument)
                    {
                        List<ImagesDetailInfo> ImagesDetailInfos = GetImagesFromWord();
                        foreach (var item in ImagesDetailInfos)
                        {
                            string hashPic = HashHelper.ComputeSHA1(item.ImgResultPath);
                            if (!HostSystemVar.CurrentImgsDictionary.ContainsKey(hashPic))
                            {
                                var listResult = AutoExcutePicOCR(item.ImgResultPath, item.UnCheckWordRange);
                                if (listResult != null)
                                {
                                    listUnCheckWordsImages.AddRange(listResult.ToList());
                                    HostSystemVar.CurrentImgsDictionary.Add(hashPic, listResult.ToList());
                                }
                            }
                            else
                            {
                                listUnCheckWordsImages.AddRange(HostSystemVar.CurrentImgsDictionary[hashPic].ToList());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                }
                GetUncheckedWordLists();
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    viewModel.IsUnLogin = true;
                }));
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.WarningTotalCount = 0;
                        viewModel.UncheckedWordLists.Clear();
                    }));
                }
                catch (Exception ex)
                { }
                Dispatcher.Invoke(new Action(() =>
                {
                    viewModel.IsBusyVisibility = Visibility.Hidden;
                }));
            }
        }
        private void UnCheckWordGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                UnChekedWordInfo unChekedWordInfo = grid.Tag as UnChekedWordInfo;
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
        private void UnCheckWordChildrenGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                UnChekedInLineDetailWordInfo unChekedWordInfo = grid.Tag as UnChekedInLineDetailWordInfo;
                if (unChekedWordInfo.TypeTextFrom == "Text")
                {
                    unChekedWordInfo.UnCheckWordRange.Select();
                }
                else
                {
                    try
                    {
                        if (unChekedWordInfo.UnCheckWordRange != null)
                            unChekedWordInfo.UnCheckWordRange.Select();
                    }
                    catch (Exception ex)
                    { }
                    if (File.Exists(unChekedWordInfo.ImgResultPath))
                    {
                        CheckWordControl.ImageDetailForm imageDetailForm = new CheckWordControl.ImageDetailForm();
                        CheckWordControl.ImageDetailControl imageDetailControl = new CheckWordControl.ImageDetailControl(unChekedWordInfo.ImgResultPath);
                        imageDetailForm.WpfElementHost.HostContainer.Children.Add(imageDetailControl);
                        imageDetailForm.ShowDialog();
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
        private async void InLineDetailNameBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                UnChekedWordInfo unChekedWordInfo = btn.Tag as UnChekedWordInfo;
                viewModel.SelectedUnChekedWordInfo = unChekedWordInfo;
                viewModel.IsDetailInfoPopWindowOpen = true;
                if (unChekedWordInfo.UnChekedWordDetailInfos.Count == 0 && !string.IsNullOrEmpty(unChekedWordInfo.ID))
                {
                    List<UnChekedDetailWordInfo> _detailInfos = new List<UnChekedDetailWordInfo>();
                    //查询违禁词描述
                    System.Threading.Tasks.Task taskGetWordDiscribe = new System.Threading.Tasks.Task(() => {
                        APIService serviceApi = new APIService();
                        _detailInfos = serviceApi.GetWordDiscribeLists(unChekedWordInfo.ID);
                    });
                    taskGetWordDiscribe.Start();
                    await taskGetWordDiscribe;
                    unChekedWordInfo.UnChekedWordDetailInfos = new ObservableCollection<UnChekedDetailWordInfo>(_detailInfos);
                    viewModel.SelectedUnChekedWordInfo = unChekedWordInfo;
                }
            }
        }
        private string CheckWordTempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTempWord";
        string savePathGetImage = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\MyWordAddIn\\";
        /// <summary>
        /// 提取图片
        /// </summary>
        private List<ImagesDetailInfo> GetImagesFromWord()
        {
            List<ImagesDetailInfo> result = new List<ImagesDetailInfo>();
            try
            {
                FileOperateHelper.DeleteFolder(savePathGetImage);
                if (!Directory.Exists(savePathGetImage))
                {
                    Directory.CreateDirectory(savePathGetImage);
                }
                object xx = null;
                string ctype = "";
                bool hasPic = false;
                int index = 1;
                foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in Application.ActiveDocument.Paragraphs)
                {
                    foreach (InlineShape ils in paragraph.Range.InlineShapes)
                    {
                        if (ils != null)
                        {
                            if (ils.Type == WdInlineShapeType.wdInlineShapePicture)
                            {
                                //ils.Range.Copy();
                                //System.Drawing.Image image = null;
                                //Dispatcher.Invoke(new Action(() =>
                                //{
                                //    image = System.Windows.Forms.Clipboard.GetImage();
                                //}));
                                //if (image != null)
                                //{
                                //    image.Save(savePathGetImage + "照片-" + index + ".jpg");
                                //    result.Add(new ImagesDetailInfo() { ImgResultPath = savePathGetImage + "照片-" + index + ".jpg", UnCheckWordRange = ils.Range });
                                //    index++;
                                //}
                                //Dispatcher.Invoke(new Action(() =>
                                //{
                                //    System.Windows.Forms.Clipboard.Clear();
                                //}));
                                try
                                {
                                    if (!hasPic)
                                    {
                                        Dispatcher.Invoke(new System.Action(() =>
                                        {
                                            if (Clipboard.ContainsFileDropList())
                                            {
                                                ctype = "FileDrop";
                                                xx = Clipboard.GetFileDropList();
                                            }
                                            else
                                            {
                                                xx = Clipboard.GetDataObject();
                                            }
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                { }
                                hasPic = true;
                                try
                                {
                                    ils.Select();
                                    IDataObject ido = null;
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        try
                                        {
                                            Application.Selection.CopyAsPicture();
                                            ido = Clipboard.GetDataObject();
                                        }
                                        catch
                                        { }
                                    }));
                                    if (ido != null && ido.GetDataPresent(DataFormats.Bitmap))
                                    {
                                        System.Windows.Interop.InteropBitmap bmp = (System.Windows.Interop.InteropBitmap)ido.GetData(DataFormats.Bitmap);
                                        if (bmp != null)
                                        {
                                            if (!Directory.Exists(savePathGetImage))
                                            {
                                                Directory.CreateDirectory(savePathGetImage);
                                            }
                                            Util.SaveImageToFile(bmp.Clone(), savePathGetImage + "照片-" + index + ".jpg");
                                            result.Add(new ImagesDetailInfo() { ImgResultPath = savePathGetImage + "照片-" + index + ".jpg", UnCheckWordRange = ils.Range });
                                            index++;
                                        }
                                    }
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
                if (hasPic)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        System.Windows.Forms.Clipboard.Clear();
                    }));
                    if (xx != null)
                    {
                        System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() => {
                            System.Threading.Thread.Sleep(200);
                            try
                            {
                                Dispatcher.Invoke(new System.Action(() =>
                                {
                                    if (ctype == "FileDrop")
                                    {
                                        StringCollection stringCollection = (StringCollection)xx;
                                        Clipboard.SetFileDropList(stringCollection);
                                    }
                                    else
                                    {
                                        Clipboard.SetDataObject(xx);
                                    }
                                }));
                            }
                            catch (Exception ex)
                            { }
                        });
                        task.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return result;
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
        private List<UnChekedWordInfo> AutoExcutePicOCR(string filePath, Range range)
        {
            List<UnChekedWordInfo> listResult = new List<UnChekedWordInfo>();
            try
            {
                try
                {
                    APIService service = new APIService();
                    bool isNetWrong = false;
                    var userStateInfos = service.GetUserStateByToken(ref isNetWrong);
                    if (!userStateInfos)
                    {
                        if (!isNetWrong)
                        {
                            try
                            {
                                CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                                commonExchangeInfo.Code = "ShowNotifyMessageView";
                                commonExchangeInfo.Data = "500";
                                string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                                Win32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                            }
                            catch
                            { }
                        }
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                countWhile = 0;
                isInitCompleted = false;
                Dispatcher.Invoke(new Action(() => {
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
                    //集成云处理OCR
                    APIService service = new APIService();
                    string fileName = "";
                    try
                    {
                        fileName = Application.ActiveDocument.Name;
                    }
                    catch
                    { }
                    var result = service.GetOCRResultByToken(image, fileName);
                    if (!string.IsNullOrEmpty(result))
                    {
                        new System.Threading.Tasks.Task(() => {
                            try
                            {
                                APIService serviceUser = new APIService();
                                bool isNetWrong = false;
                                var userStateInfos = serviceUser.GetUserStateByToken(ref isNetWrong);
                                if (!userStateInfos)
                                {
                                    if (!isNetWrong)
                                    {
                                        try
                                        {
                                            CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                                            commonExchangeInfo.Code = "ShowNotifyMessageView";
                                            commonExchangeInfo.Data = "500";
                                            string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                                            Win32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                            catch
                            { }
                        }).Start();
                    }
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
                {
                    CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                }
                while (!isInitCompleted && countWhile < 10)
                {
                    System.Threading.Thread.Sleep(100);
                    countWhile++;
                }
                if (resultImgGeneral != null && resultImgGeneral.words_result_num > 0)
                {
                    string desiredFolderName = CheckWordTempPath + " \\" + Guid.NewGuid().ToString() + "\\";
                    if (!Directory.Exists(desiredFolderName))
                    {
                        Directory.CreateDirectory(desiredFolderName);
                    }
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
                                        itemInfo.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { TypeTextFrom = "Img", UnCheckWordRange = range, InLineText = lineWord, ImgResultPath = desiredFolderName + System.IO.Path.GetFileName(filePath) });
                                        itemInfo.ErrorTotalCount++;
                                        listResult.Add(itemInfo);
                                    }
                                    else
                                    {
                                        infoResult.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { TypeTextFrom = "Img", UnCheckWordRange = range, InLineText = lineWord, ImgResultPath = desiredFolderName + System.IO.Path.GetFileName(filePath) });
                                        infoResult.ErrorTotalCount++;
                                    }
                                }
                            }
                        }
                    }
                    var list = CheckWordHelper.GetUnValidRects(listUnValidInfos);
                    foreach (var item in list)
                    {
                        try
                        {
                            Dispatcher.Invoke(new Action(() => {
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

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Util.CallWordAndImgApp();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsDetailInfoPopWindowOpen = false;
        }
    }
}
