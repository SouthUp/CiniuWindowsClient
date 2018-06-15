using System;
using System.Collections.Concurrent;
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
using CheckWordModel;
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
        ConcurrentBag<UnChekedWordInfo> listUnCheckWords = new ConcurrentBag<UnChekedWordInfo>();
        Dictionary<string, List<UnChekedWordInfo>> CurrentImgsDictionary = new Dictionary<string, List<UnChekedWordInfo>>();
        MyControlViewModel viewModel = new MyControlViewModel();
        // 保存修改过的Range和之前的背景色，以便于恢复
        private List<Range> rangeSelectLists = new List<Range>();
        private List<WdColorIndex> rangeBackColorSelectLists = new List<WdColorIndex>();
        //文本改变检测
        TextChangeDetector detector;
        //图片改变检测
        ImagesChangeDetector detectorImages;
        Microsoft.Office.Interop.Word.Application Application = Globals.ThisAddIn.Application;
        public MyControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartDetector();
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
            detector.OnTextChanged -= detector_OnTextChanged;
            detector.Stop();
            detectorImages.OnImagesChanged -= detector_OnImagesChanged;
            detectorImages.Stop();
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
        }
        /// <summary>
        /// 开始实时检测功能
        /// </summary>
        public void StartDetector()
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
        /// <summary>
        /// 关闭实时检测功能
        /// </summary>
        public void CloseDetector()
        {
            try
            {
                detector.OnTextChanged -= detector_OnTextChanged;
                detector.Stop();
                detectorImages.OnImagesChanged -= detector_OnImagesChanged;
                detectorImages.Stop();
                if (tDetector != null)
                {
                    tDetector.Abort();
                    tDetector = null;
                }
            }
            catch (Exception ex)
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
                if (Util.IsUrlExist("http://localhost:8888/"))
                {
                    FindTextAndHightLight();
                }
                else
                {
                    if (rangeSelectLists.Count > 0)
                    {
                        for (int i = 0; i < rangeSelectLists.Count; i++)
                        {
                            rangeSelectLists[i].HighlightColorIndex = rangeBackColorSelectLists[i];
                        }
                        rangeSelectLists = new List<Range>();
                        rangeBackColorSelectLists = new List<WdColorIndex>();
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.WarningTotalCount = 0;
                        viewModel.UncheckedWordLists.Clear();
                        CurrentImgsDictionary = new Dictionary<string, List<UnChekedWordInfo>>();
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
        /// <summary>
        /// 查找文本并高亮显示
        /// </summary>
        /// <param name="strFind"></param>
        private void FindTextAndHightLight()
        {
            listUnCheckWords = new ConcurrentBag<UnChekedWordInfo>();
            // 清除文档中的高亮显示
            ClearMark();
            rangeSelectLists = new List<Range>();
            rangeBackColorSelectLists = new List<WdColorIndex>();
            List<Microsoft.Office.Interop.Word.Paragraph> ParagraphDataList = new List<Microsoft.Office.Interop.Word.Paragraph>();
            //检测整个文档
            foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in Application.ActiveDocument.Paragraphs)
            {
                ParagraphDataList.Add(paragraph);
                if (ParagraphDataList.Count >= 200)
                {
                    //处理段落违禁词查找
                    DealParagraph(ParagraphDataList);
                    ParagraphDataList = new List<Microsoft.Office.Interop.Word.Paragraph>();
                }
            }
            //处理段落违禁词查找
            DealParagraph(ParagraphDataList);
            foreach (var Value in CurrentImgsDictionary.Values)
            {
                foreach (var item in Value)
                {
                    var infoExist = listUnCheckWords.AsParallel().FirstOrDefault(x => x.Name == item.Name);
                    if (infoExist == null)
                    {
                        listUnCheckWords.Add(item);
                    }
                    else
                    {
                        foreach (var detail in item.UnChekedWordInLineDetailInfos)
                        {
                            infoExist.UnChekedWordInLineDetailInfos.Add(detail);
                            infoExist.ErrorTotalCount++;
                        }
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
            //回复非违禁词背景色
            for (int i = 0; i < rangeOtherHighlightColorLists.Count; i++)
            {
                try
                {
                    rangeOtherHighlightColorLists[i].HighlightColorIndex = rangeBackOtherHighlightColorLists[i];
                }
                catch (Exception ex)
                { }
            }
        }
        /// <summary>
        /// 解析处理段落
        /// </summary>
        /// <param name="ParagraphDataList"></param>
        private void DealParagraph(List<Microsoft.Office.Interop.Word.Paragraph> ParagraphDataList)
        {
            try
            {
                int ParagraphCount = ParagraphDataList.Count;
                Parallel.For(0, ParagraphCount, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (i, state) =>
                {
                    if (ParagraphDataList.Skip(i).Take(1).ToList().Count > 0)
                    {
                        var paragraph = ParagraphDataList.Skip(i).Take(1).ToList().First();
                        if (paragraph != null)
                        {
                            var listUnChekedWord = CheckWordHelper.GetUnChekedWordInfoList(paragraph.Range.Text).ToList();
                            if (listUnChekedWord != null && listUnChekedWord.Count > 0)
                            {
                                foreach (var strFind in listUnChekedWord.ToList())
                                {
                                    UnChekedWordInfo SelectUnCheckWord = new UnChekedWordInfo() { Name = strFind.Name, UnChekedWordDetailInfos = strFind.UnChekedWordDetailInfos };
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
                                                lock (lockObject)
                                                {
                                                    rangeSelectLists.Add(keywordRange);
                                                    rangeBackColorSelectLists.Add(keywordRange.HighlightColorIndex);
                                                }
                                                keywordRange.HighlightColorIndex = WdColorIndex.wdYellow;
                                                SelectUnCheckWord.UnChekedWordInLineDetailInfos.Add(new UnChekedInLineDetailWordInfo() { InLineText = paragraph.Range.Text, UnCheckWordRange = keywordRange });
                                                SelectUnCheckWord.ErrorTotalCount++;
                                            }
                                            catch (Exception ex)
                                            { }
                                        }
                                        lock (lockObject)
                                        {
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
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            { }
            try
            {
                List<ImagesDetailInfo> ImagesDetailInfos = GetImagesFromWord();
                List<string> listHashs = new List<string>();
                foreach (var item in ImagesDetailInfos)
                {
                    string hashPic = HashHelper.ComputeSHA1(item.ImgResultPath);
                    listHashs.Add(hashPic);
                    if (!CurrentImgsDictionary.ContainsKey(hashPic))
                    {
                        var listResult = AutoExcutePicOCR(item.ImgResultPath, item.UnCheckWordRange);
                        CurrentImgsDictionary.Add(hashPic, listResult);
                    }
                }
                string[] keyArr = CurrentImgsDictionary.Keys.ToArray<string>();
                for (int p = keyArr.Count() - 1; p > -1; p--)
                {
                    if (!listHashs.Contains(keyArr[p]))
                    {
                        CurrentImgsDictionary.Remove(keyArr[p]);
                    }
                }
            }
            catch (Exception ex)
            { }
            GetUncheckedWordLists();
        }
        // 保存非违禁词带有背景色的Range和之前的背景色，以便于恢复
        private List<Range> rangeOtherHighlightColorLists = new List<Range>();
        private List<WdColorIndex> rangeBackOtherHighlightColorLists = new List<WdColorIndex>();
        /// <summary>
        /// 清除文档中的高亮显示
        /// </summary>
        private void ClearMark()
        {
            try
            {
                rangeOtherHighlightColorLists = new List<Range>();
                rangeBackOtherHighlightColorLists = new List<WdColorIndex>();
                for (int i = 0; i < rangeSelectLists.Count; i++)
                {
                    rangeSelectLists[i].HighlightColorIndex = rangeBackColorSelectLists[i];
                }
                Document currentDocument = Globals.ThisAddIn.Application.ActiveDocument;
                if (currentDocument.Paragraphs != null &&
                    currentDocument.Paragraphs.Count != 0)
                {
                    foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in currentDocument.Paragraphs)
                    {
                        if(paragraph.Range.HighlightColorIndex != WdColorIndex.wdNoHighlight
                            && paragraph.Range.HighlightColorIndex != WdColorIndex.wdAuto)
                        {
                            GetOtherHighlightColorLists(paragraph.Range);
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void GetOtherHighlightColorLists(Range range)
        {
            try
            {
                for (int i = 1; i < range.Words.Count; i++)
                {
                    var word = range.Words[i];
                    if (word.HighlightColorIndex != WdColorIndex.wdYellow
                        && word.HighlightColorIndex != WdColorIndex.wdNoHighlight
                        && word.HighlightColorIndex != WdColorIndex.wdAuto)
                    {
                        if (word.Text.Length > 1)
                        {
                            int startIndex = word.Start;
                            int endIndex = word.End;
                            var currentDocument = Globals.ThisAddIn.Application.ActiveDocument;
                            for (int ii = startIndex; ii < endIndex; ii++)
                            {
                                var rangeCurrent = currentDocument.Range(ii, ii + 1);
                                if (rangeCurrent.HighlightColorIndex != WdColorIndex.wdYellow
                                    && rangeCurrent.HighlightColorIndex != WdColorIndex.wdNoHighlight
                                    && rangeCurrent.HighlightColorIndex != WdColorIndex.wdAuto)
                                {
                                    rangeOtherHighlightColorLists.Add(rangeCurrent);
                                    rangeBackOtherHighlightColorLists.Add(rangeCurrent.HighlightColorIndex);
                                }
                            }
                        }
                        else
                        {
                            rangeOtherHighlightColorLists.Add(word);
                            rangeBackOtherHighlightColorLists.Add(word.HighlightColorIndex);
                        }
                    }
                    word.HighlightColorIndex = WdColorIndex.wdNoHighlight;
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
                    if (unChekedWordInfo.UnCheckWordRange != null)
                        unChekedWordInfo.UnCheckWordRange.Select();
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
        private void InLineDetailNameBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                UnChekedWordInfo unChekedWordInfo = btn.Tag as UnChekedWordInfo;
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
        private string CheckWordTempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTemp";
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
                int index = 1;
                foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in Application.ActiveDocument.Paragraphs)
                {
                    foreach (InlineShape ils in paragraph.Range.InlineShapes)
                    {
                        if (ils != null)
                        {
                            if (ils.Type == WdInlineShapeType.wdInlineShapePicture)
                            {
                                ils.Range.Copy();
                                System.Drawing.Image image = null;
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    image = System.Windows.Forms.Clipboard.GetImage();
                                }));
                                if (image != null)
                                {
                                    image.Save(savePathGetImage + "照片-" + index + ".jpg");
                                    result.Add(new ImagesDetailInfo() { ImgResultPath = savePathGetImage + "照片-" + index + ".jpg", UnCheckWordRange = ils.Range });
                                    index++;
                                }
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    System.Windows.Forms.Clipboard.Clear();
                                }));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
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
    }
}
