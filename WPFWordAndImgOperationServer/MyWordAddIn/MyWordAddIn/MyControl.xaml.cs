using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MyWordAddIn
{
    /// <summary>
    /// MyControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyControl : UserControl
    {
        MyControlViewModel viewModel = new MyControlViewModel();
        // 保存修改过的Range和之前的背景色，以便于恢复
        private List<Range> rangeSelectLists = new List<Range>();
        private List<WdColorIndex> rangeBackColorSelectLists = new List<WdColorIndex>();
        //文本改变检测
        TextChangeDetector detector;
        Microsoft.Office.Interop.Word.Application Application = Globals.ThisAddIn.Application;
        Document document = Globals.ThisAddIn.Application.ActiveDocument;
        public MyControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartDetector();
            System.Threading.Thread tGetUncheckedWord = new System.Threading.Thread(GetUncheckedWordLists);
            tGetUncheckedWord.IsBackground = true;
            tGetUncheckedWord.Start();
        }
        private void detector_OnTextChanged(object sender, TextChangedEventArgs e)
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
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            detector.OnTextChanged -= detector_OnTextChanged;
            detector.Stop();
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
            if (detector == null)
                detector = new TextChangeDetector(Application);
            detector.OnTextChanged += detector_OnTextChanged;
            detector.Start();
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
                List<Microsoft.Office.Interop.Word.Paragraph> ParagraphDataList = new List<Microsoft.Office.Interop.Word.Paragraph>();
                //检测整个文档
                foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in document.Paragraphs)
                {
                    ParagraphDataList.Add(paragraph);
                }
                FindTextAndHightLight(ParagraphDataList);
            }
            catch (Exception ex)
            { }
        }
        private void ListBox_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        /// <summary>
        /// 查找文本并高亮显示
        /// </summary>
        /// <param name="strFind"></param>
        private void FindTextAndHightLight(List<Microsoft.Office.Interop.Word.Paragraph> ParagraphDataList)
        {
            ObservableCollection<UnChekedWordInfo> listUnCheckWords = new ObservableCollection<UnChekedWordInfo>();
            // 清除文档中的高亮显示
            ClearMark();
            rangeSelectLists = new List<Range>();
            rangeBackColorSelectLists = new List<WdColorIndex>();
            //处理段落违禁词查找
            try
            {
                int ParagraphCount = document.Paragraphs.Count;
                System.Threading.Tasks.Parallel.For(0, ParagraphCount, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = 10 }, (i, state) =>
                {
                    if (ParagraphDataList.Skip(i).Take(1).ToList().Count > 0)
                    {
                        var paragraph = ParagraphDataList.Skip(i).Take(1).ToList().First();
                        if (paragraph != null)
                        {
                            var listUnChekedWord = CheckWordHelper.GetUnChekedWordInfoList(paragraph.Range.Text).ToList();
                            if (listUnChekedWord != null && listUnChekedWord.Count > 0)
                            {
                                foreach (var strFind in listUnChekedWord.Select(x => x.Name).ToList())
                                {
                                    UnChekedWordInfo SelectUnCheckWord = new UnChekedWordInfo() { Name = strFind };
                                    MatchCollection mc = Regex.Matches(paragraph.Range.Text, strFind, RegexOptions.IgnoreCase);
                                    if (mc.Count > 0)
                                    {
                                        foreach (Match m in mc)
                                        {
                                            try
                                            {
                                                int startIndex = paragraph.Range.Start + m.Index;
                                                int endIndex = paragraph.Range.Start + m.Index + m.Length;
                                                Range keywordRange = document.Range(startIndex, endIndex);
                                                rangeSelectLists.Add(keywordRange);
                                                rangeBackColorSelectLists.Add(keywordRange.HighlightColorIndex);
                                                keywordRange.HighlightColorIndex = WdColorIndex.wdYellow;
                                                SelectUnCheckWord.Children.Add(new UnChekedWordInfo() { Name = paragraph.Range.Text, Range = paragraph.Range, UnCheckWordRange = keywordRange });
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
                                            foreach (var item in SelectUnCheckWord.Children)
                                            {
                                                infoExist.Children.Add(item);
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
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.UncheckedWordLists.Add(SelectUnCheckWord);
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        itemInfo.Children.Clear();
                    }));
                    foreach (var item in SelectUnCheckWord.Children)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            itemInfo.Children.Add(item);
                        }));
                    }
                    itemInfo.WarningCount = itemInfo.Children.Count;
                }
            }
            for (int i = 0; i < viewModel.UncheckedWordLists.Count; i++)
            {
                var itemInfo = listUnCheckWords.FirstOrDefault(x => x.Name == viewModel.UncheckedWordLists[i].Name);
                if (itemInfo == null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.UncheckedWordLists.RemoveAt(i);
                    }));
                    i--;
                }
            }
            viewModel.WarningCount = viewModel.UncheckedWordLists.Count;
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
        private void UnCheckWordGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            UnChekedWordInfo unChekedWordInfo = grid.Tag as UnChekedWordInfo;
            unChekedWordInfo.IsSelected = !unChekedWordInfo.IsSelected;
        }
        private void UnCheckWordChildrenGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            UnChekedWordInfo unChekedWordInfo = grid.Tag as UnChekedWordInfo;
            unChekedWordInfo.IsSelected = true;
        }

        private void UnCheckWordChildrenGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            UnChekedWordInfo unChekedWordInfo = grid.Tag as UnChekedWordInfo;
            unChekedWordInfo.IsSelected = false;
            unChekedWordInfo.UnCheckWordRange.Select();
        }
    }
}
