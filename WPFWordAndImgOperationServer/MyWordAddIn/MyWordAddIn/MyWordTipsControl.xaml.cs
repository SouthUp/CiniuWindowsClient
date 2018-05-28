using CheckWordEvent;
using CheckWordModel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MyWordAddIn
{
    /// <summary>
    /// MyWordTipsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyWordTipsControl : UserControl
    {
        Hook.KeyboardHook2 hook;
        bool isClosed = false;
        Selection Selection = Globals.ThisAddIn.Application.Selection;
        FloatingPanel MyFloatingPanel;
        MyWordTipsControlViewModel viewModel = new MyWordTipsControlViewModel();
        public MyWordTipsControl(FloatingPanel floatingPanel)
        {
            InitializeComponent();
            MyFloatingPanel = floatingPanel;
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<SendSelectNumberToMyWordTipsEvent>().Subscribe(SendSelectNumberToMyWordTips);
            EventAggregatorRepository.EventAggregator.GetEvent<CloseMyWordTipsEvent>().Subscribe(CloseMyWordTips);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            hook = new Hook.KeyboardHook2();
            hook.InitHook();
            listBox.AddHandler(ListBox.MouseWheelEvent, new MouseWheelEventHandler(ReplaceWordScrollViewer_MouseWheel), true);
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() =>
            {
                viewModel.InitData(Selection.Text);
            });
            await task;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            hook.UnHook();
            isClosed = true;
            viewModel.ReplaceWordLists = new System.Collections.ObjectModel.ObservableCollection<ReplaceWordInfo>();
        }
        private void CloseMyWordTips(bool b)
        {
            if (!isClosed)
                MyFloatingPanel.Close();
        }
        /// <summary>
        /// 接收数字快捷键指令
        /// </summary>
        /// <param name="num"></param>
        private void SendSelectNumberToMyWordTips(int num)
        {
            if(!isClosed)
            {
                int index = 1;
                if (num == 49 || num == 97)
                {
                    index = 1;
                }
                else if(num == 50 || num == 98)
                {
                    index = 2;
                }
                else if (num == 51 || num == 99)
                {
                    index = 3;
                }
                else if (num == 52 || num == 100)
                {
                    index = 4;
                }
                else if (num == 53 || num == 101)
                {
                    index = 5;
                }
                else if (num == 54 || num == 102)
                {
                    index = 6;
                }
                else if (num == 55 || num == 103)
                {
                    index = 7;
                }
                else if (num == 56 || num == 104)
                {
                    index = 8;
                }
                else if (num == 57 || num == 105)
                {
                    index = 9;
                }
                if (index <= viewModel.ReplaceWordLists.Count)
                {
                    ReplaceWordInfo info = viewModel.ReplaceWordLists.FirstOrDefault(x => x.Index == index);
                    if (info != null)
                    {
                        Selection.Range.Text = info.Name;
                        MyFloatingPanel.Close();
                    }
                }
            }
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            MyFloatingPanel.Close();
        }

        private void listBox_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void ReplaceWordScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ItemsControl items = (ItemsControl)sender;
            ScrollViewer scroll = FindVisualChild<ScrollViewer>(items);
            if (scroll != null)
            {
                int d = e.Delta;
                if (d > 0)
                {
                    scroll.LineLeft();
                }
                if (d < 0)
                {
                    scroll.LineRight();
                }
            }
        }
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            ReplaceWordInfo info = grid.Tag as ReplaceWordInfo;
            Selection.Range.Text = info.Name;
            MyFloatingPanel.Close();
        }
    }
}
