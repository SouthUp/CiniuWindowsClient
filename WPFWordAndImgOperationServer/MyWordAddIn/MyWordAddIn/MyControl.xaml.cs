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
                        System.Threading.Thread tGetUncheckedWord = new System.Threading.Thread(GetUncheckedWordLists);
                        tGetUncheckedWord.IsBackground = true;
                        tGetUncheckedWord.Start();
                    }
                    catch (Exception ex)
                    { }
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
                IsChecking = true;
                //检测整个文档
                string textResult = "";
                if (document.Words.Count > 0)
                {
                    for (int i = 1; i < document.Words.Count; i++)
                    {
                        textResult += document.Words[i].Text;
                    }
                }
                var listUnChekedWord = CheckWordHelper.GetUnChekedWordInfoList(textResult).ToList();
                List<string> listStrs = new List<string>();
                foreach (var item in listUnChekedWord)
                {
                    listStrs.Add(item.Name);
                }
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    FindTextAndHightLight(listStrs);
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
                }));
            }
            catch (Exception ex)
            {
                lock (lockObject)
                {
                    DateTime typeDequeue = queue.Dequeue();
                    IsChecking = false;
                }
            }
        }
        private void ListBox_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        //////private void ListBox2_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        //////{
        //////    e.Handled = true;
        //////}
        /// <summary>
        /// 查找文本并高亮显示
        /// </summary>
        /// <param name="strFind"></param>
        private void FindTextAndHightLight(List<string> listStrs)
        {
            //////ObservableCollection<UnChekedDetailWordInfo> listUnCheckDetailWords = new ObservableCollection<UnChekedDetailWordInfo>();
            ObservableCollection<UnChekedWordInfo> listUnCheckWords = new ObservableCollection<UnChekedWordInfo>();
            // 清楚文档中的高亮显示
            ClearMark();
            rangeSelectLists = new List<Range>();
            rangeBackColorSelectLists = new List<WdColorIndex>();
            foreach (var strFind in listStrs)
            {
                UnChekedWordInfo SelectUnCheckWord = new UnChekedWordInfo() {Name = strFind };
                // 按段落检索
                Document currentDocument = Globals.ThisAddIn.Application.ActiveDocument;
                if (currentDocument.Paragraphs != null &&
                    currentDocument.Paragraphs.Count != 0)
                {
                    foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in currentDocument.Paragraphs)
                    {
                        MatchCollection mc = Regex.Matches(paragraph.Range.Text, strFind, RegexOptions.IgnoreCase);
                        if (mc.Count > 0)
                        {
                            foreach (Match m in mc)
                            {
                                try
                                {
                                    int startIndex = paragraph.Range.Start + m.Index;
                                    int endIndex = paragraph.Range.Start + m.Index + m.Length;
                                    Range keywordRange = currentDocument.Range(startIndex, endIndex);
                                    rangeSelectLists.Add(keywordRange);
                                    rangeBackColorSelectLists.Add(keywordRange.HighlightColorIndex);
                                    keywordRange.HighlightColorIndex = WdColorIndex.wdYellow;
                                    //////keywordRange.Font.Underline = WdUnderline.wdUnderlineWavyDouble;
                                    //////document.Comments.Add(range, "违禁词");
                                    SelectUnCheckWord.Children.Add(new UnChekedWordInfo() { Name = paragraph.Range.Text, Range = paragraph.Range, UnCheckWordRange= keywordRange });
                                    SelectUnCheckWord.Initialize();
                                    //////UnChekedDetailWordInfo SelectUnCheckDetailWord = new UnChekedDetailWordInfo() { Name = strFind, UnCheckWordRange = keywordRange};
                                    //////if (listUnCheckDetailWords.FirstOrDefault(x => x.Name == SelectUnCheckDetailWord.Name && x.UnCheckWordRange.Start == SelectUnCheckDetailWord.UnCheckWordRange.Start) == null)
                                    //////{
                                    //////    listUnCheckDetailWords.Add(SelectUnCheckDetailWord);
                                    //////}
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    if (listUnCheckWords.FirstOrDefault(x => x.Name == SelectUnCheckWord.Name) == null)
                    {
                        listUnCheckWords.Add(SelectUnCheckWord);
                    }
                }
            }
            //////foreach (var SelectUnCheckDetailWord in listUnCheckDetailWords)
            //////{
            //////    var itemInfo = viewModel.UncheckedWordDetailLists.FirstOrDefault(x => x.Name == SelectUnCheckDetailWord.Name && x.UnCheckWordRange.Start == SelectUnCheckDetailWord.UnCheckWordRange.Start);
            //////    if (itemInfo == null)
            //////    {
            //////        viewModel.UncheckedWordDetailLists.Add(SelectUnCheckDetailWord);
            //////    }
            //////}
            //////for (int i = 0; i < viewModel.UncheckedWordDetailLists.Count; i++)
            //////{
            //////    var itemInfo = listUnCheckDetailWords.FirstOrDefault(x => x.Name == viewModel.UncheckedWordDetailLists[i].Name && x.UnCheckWordRange.Start == viewModel.UncheckedWordDetailLists[i].UnCheckWordRange.Start);
            //////    if (itemInfo == null)
            //////    {
            //////        viewModel.UncheckedWordDetailLists.RemoveAt(i);
            //////        i--;
            //////    }
            //////}
            //////viewModel.WarningDetailCount = viewModel.UncheckedWordDetailLists.Count;
            foreach (var SelectUnCheckWord in listUnCheckWords)
            {
                var itemInfo = viewModel.UncheckedWordLists.FirstOrDefault(x => x.Name == SelectUnCheckWord.Name);
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
            }
            for (int i = 0; i < viewModel.UncheckedWordLists.Count; i++)
            {
                var itemInfo = listUnCheckWords.FirstOrDefault(x => x.Name == viewModel.UncheckedWordLists[i].Name);
                if (itemInfo == null)
                {
                    viewModel.UncheckedWordLists.RemoveAt(i);
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
                        ////////paragraph.Range.HighlightColorIndex = WdColorIndex.wdNoHighlight;
                        ////////paragraph.Range.Font.Underline = WdUnderline.wdUnderlineNone;
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
        #region 其他方法
        private void CallGetPositionForViewDemo()
        {
            //////// 获取上下文信息
            //////// 获取前两个单词的位置（如果有）
            //////startIndex = GetStartPositionForView(paragraph, m, startIndex);
            //////// 获取后两个单词的位置（如果有）
            //////endIndex = GetEndPositionForView(paragraph, m, endIndex);
            //////// 在ListView中展示检索的关键字以及其上下文
            //////Range range = currentDocument.Range(startIndex, endIndex);
            //////string searchResultInText = range.Text;
        }
        private static int GetEndPositionForView(Microsoft.Office.Interop.Word.Paragraph paragraph, Match m, int endIndex)
        {
            string suffixPart = paragraph.Range.Text.Substring(m.Index + m.Length);
            MatchCollection suffixMC = Regex.Matches(suffixPart, "\\s");
            if (suffixMC.Count >= 3)
            {
                endIndex = endIndex + suffixMC[2].Index;
            }
            else
            {
                if (suffixMC.Count >= 2)
                {
                    endIndex = endIndex + suffixMC[1].Index;
                }
                else if (suffixMC.Count >= 1)
                {
                    endIndex = endIndex + suffixMC[0].Index;
                }
            }
            return endIndex;
        }

        private static int GetStartPositionForView(Microsoft.Office.Interop.Word.Paragraph paragraph, Match m, int startIndex)
        {
            string prefixPart = paragraph.Range.Text.Substring(0, m.Index);
            MatchCollection preficMC = Regex.Matches(prefixPart, "\\s");
            if (preficMC.Count >= 3)
            {
                startIndex = paragraph.Range.Start + preficMC[preficMC.Count - 3].Index;
            }
            else
            {
                if (preficMC.Count >= 2)
                {
                    startIndex = paragraph.Range.Start + preficMC[preficMC.Count - 2].Index;
                }
                else if (preficMC.Count >= 1)
                {
                    startIndex = paragraph.Range.Start + preficMC[preficMC.Count - 1].Index;
                }
            }
            return startIndex;
        }
        //private void FindText(string strFind)
        //{
        //    int intFound = 0;
        //    object missing = Type.Missing;
        //    ////高亮查询显示
        //    Microsoft.Office.Interop.Word.Selection currentselect = document.Content.Sections.Application.Selection;
        //    currentselect.Find.ClearFormatting();
        //    currentselect.Find.Text = strFind;//查询的文字
        //    currentselect.Find.Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue;
        //    bool findover = currentselect.Find.Execute(ref missing, ref missing,
        //    ref missing, ref missing,
        //                   ref missing, ref missing,
        //                   ref missing, ref missing,
        //                   ref missing, ref missing,
        //                   ref missing, ref missing,
        //                   ref missing, ref missing,
        //                   ref missing);
        //    ////查找总个数
        //    Microsoft.Office.Interop.Word.Range rng = document.Content;
        //    rng.Find.ClearFormatting();
        //    rng.Find.Forward = true;
        //    rng.Find.Text = strFind;
        //    rng.Find.Execute(
        //        ref missing, ref missing, ref missing, ref missing, ref missing,
        //        ref missing, ref missing, ref missing, ref missing, ref missing,
        //        ref missing, ref missing, ref missing, ref missing, ref missing);

        //    while (rng.Find.Found)
        //    {
        //        intFound++;
        //        rng.Find.Execute(
        //            ref missing, ref missing, ref missing, ref missing, ref missing,
        //            ref missing, ref missing, ref missing, ref missing, ref missing,
        //            ref missing, ref missing, ref missing, ref missing, ref missing);
        //    }
        //    //MessageBox.Show("找到的词共有: " + intFound.ToString());
        //}
        #endregion
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
        //////private void UnCheckDetailWordGrid_MouseDown(object sender, MouseButtonEventArgs e)
        //////{
        //////    Grid grid = sender as Grid;
        //////    UnChekedDetailWordInfo unChekedDetailWordInfo = grid.Tag as UnChekedDetailWordInfo;
        //////    unChekedDetailWordInfo.IsSelected = true;
        //////    unChekedDetailWordInfo.UnCheckWordRange.Select();
        //////    foreach (var item in viewModel.UncheckedWordDetailLists)
        //////    {
        //////        if (item.Name != unChekedDetailWordInfo.Name || unChekedDetailWordInfo.UnCheckWordRange.Start != item.UnCheckWordRange.Start)
        //////        {
        //////            item.IsSelected = false;
        //////        }
        //////    }
        //////}
        //////private void DetailBtn_Checked(object sender, RoutedEventArgs e)
        //////{
        //////    viewModel.DetailVisibility = Visibility.Visible;
        //////    viewModel.SummaryVisibility = Visibility.Collapsed;
        //////}

        //////private void SummaryBtn_Checked(object sender, RoutedEventArgs e)
        //////{
        //////    viewModel.SummaryVisibility = Visibility.Visible;
        //////    viewModel.DetailVisibility = Visibility.Collapsed;
        //////}
    }
}
