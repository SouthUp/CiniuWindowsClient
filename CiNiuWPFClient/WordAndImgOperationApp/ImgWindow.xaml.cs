using CheckWordEvent;
using CheckWordModel;
using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// ImgWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImgWindow : Window
    {
        MyFolderDataViewModel myFolder;
        ImgWindowViewModel viewModel = new ImgWindowViewModel();
        public ImgWindow(MyFolderDataViewModel myFolder)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.myFolder = myFolder;
        }
        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
            InitData();
        }

        private void MaxBtn_Checked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void MaxBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            xScale = img.ActualWidth / bitmap.PixelWidth;
            yScale = img.ActualHeight / bitmap.PixelHeight;
            try
            {
                if (myFolder.CheckResultInfo == "1")
                {
                    Dispatcher.Invoke(new Action(() => {
                        TextOverlay.Children.Clear();
                    }));
                    if (myFolder.ResultImgGeneral != null && myFolder.ResultImgGeneral.words_result_num > 0)
                    {
                        List<WordInfo> listUnValidInfos = new List<WordInfo>();
                        foreach (var item in myFolder.ResultImgGeneral.words_result)
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
                            }
                        }
                        var list = CheckWordUtil.CheckWordHelper.GetUnValidRects(listUnValidInfos);
                        foreach (var item in list)
                        {
                            try
                            {
                                Dispatcher.Invoke(new Action(() => {
                                    WordOverlay wordBoxOverlay = new WordOverlay(item);
                                    var overlay = new Border()
                                    {
                                        Style = (System.Windows.Style)this.Resources["HighlightedWordBoxHorizontalLine"]
                                    };
                                    overlay.SetBinding(Border.MarginProperty, wordBoxOverlay.CreateWordPositionBinding());
                                    overlay.SetBinding(Border.WidthProperty, wordBoxOverlay.CreateWordWidthBinding());
                                    overlay.SetBinding(Border.HeightProperty, wordBoxOverlay.CreateWordHeightBinding());
                                    TextOverlay.Children.Add(overlay);
                                }));
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        double xScale = 1;
        double yScale = 1;
        BitmapImage bitmap = null;
        private async void InitData()
        {
            try
            {
                Task task = new Task(() => {
                    Dispatcher.Invoke(new Action(() => {
                        //生成绑定图片
                        bitmap = Util.GetBitmapImageForBackUp(myFolder.FilePath);
                        img.Source = bitmap;
                    }));
                });
                task.Start();
                await task;
            }
            catch (Exception ex)
            { }
            viewModel.BusyWindowVisibility = Visibility.Collapsed;
        }
    }
}
