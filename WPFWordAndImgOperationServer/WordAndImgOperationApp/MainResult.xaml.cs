using CheckWordEvent;
using CheckWordModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WordAndImgOperationApp
{
    /// <summary>
    /// MainResult.xaml 的交互逻辑
    /// </summary>
    public partial class MainResult : UserControl
    {
        MainResultViewModel viewModel = new MainResultViewModel();
        public MainResult(List<MyFolderDataViewModel> dealDataResultList)
        {
            InitializeComponent();
            viewModel.DealDataResultList = new ObservableCollection<MyFolderDataViewModel>(dealDataResultList);
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<CloseDetailWindowFinishedEvent>().Subscribe(CloseDetailWindowFinished);
        }
        private void CloseDetailWindowFinished(bool b)
        {
            SystemVar.MyDetailWindow = null;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (viewModel.DealDataResultList.Count == 0)
            {
                viewModel.EmptyWindowVisibility = Visibility.Visible;
            }
            if (SystemVar.MyDetailWindow == null)
            {
                SystemVar.MyDetailWindow= new DetailWindow();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var myFolderDataViewModel = btn.Tag as MyFolderDataViewModel;
                try
                {
                    System.Diagnostics.Process.Start(myFolderDataViewModel.FilePath); //打开此文件。
                }
                catch(Exception ex)
                { }
            }
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var myFolderDataViewModel = btn.Tag as MyFolderDataViewModel;
                try
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<SetDetailWindowTopmostEvent>().Publish(true);
                    SystemVar.MyDetailWindow.SetMyFolderDataViewModel(myFolderDataViewModel);
                }
                catch (Exception ex)
                { }
            }
        }
    }
}
