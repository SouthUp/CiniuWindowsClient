using CheckWordEvent;
using CheckWordModel.Communication;
using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// MainSet.xaml 的交互逻辑
    /// </summary>
    public partial class MainSet : UserControl
    {
        private static List<string> FilePathsList = new List<string>();
        List<string> listClass = new List<string>() { ".png", ".jpg", ".jpeg", ".doc", ".docx" };
        MainSetViewModel viewModel = new MainSetViewModel();
        public MainSet()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<DealCheckBtnDataFinishedEvent>().Subscribe(DealCheckBtnDataFinished);
        }
        private void DealCheckBtnDataFinished(bool b)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<InitContentGridViewEvent>().Publish("MainResult");
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.InitData();
        }

        private void CircleCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!UtilSystemVar.IsDealingData)
            {
                if (viewModel.ChekedWordSettingsInfos.Where(x => x.IsChecked).ToList().Count > 0)
                {
                    UtilSystemVar.IsDealingData = true;
                    EventAggregatorRepository.EventAggregator.GetEvent<SendDealDataStateToSeachTxTEvent>().Publish(true);
                    viewModel.AddTaskBtnIsEnabled = false;
                    viewModel.CancelBtnVisibility = Visibility.Visible;
                    viewModel.CheckBtnVisibility = Visibility.Collapsed;
                    foreach (var item in viewModel.ChekedWordSettingsInfos)
                    {
                        FilePathsList = new List<string>();
                        if (File.Exists(item.FileFullPath))
                        {
                            if (listClass.Contains(System.IO.Path.GetExtension(item.FileFullPath))
                                && !item.FileFullPath.Contains("~$"))
                            {
                                FilePathsList.Add(item.FileFullPath);
                            }
                        }
                        else if (Directory.Exists(item.FileFullPath))
                        {
                            DirectoryInfo dir = new DirectoryInfo(item.FileFullPath);
                            GetAllFiles(dir);
                        }
                        item.FilePathsList = FilePathsList;
                        item.TotalCount = FilePathsList.Count;
                        item.IsChecking = true;
                    }
                    Task.Run(() => {
                        EventAggregatorRepository.EventAggregator.GetEvent<DealCheckBtnDataEvent>().Publish(viewModel.ChekedWordSettingsInfos);
                    });
                }
            }
        }
        private void GetAllFiles(DirectoryInfo dir)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                if (listClass.Contains(System.IO.Path.GetExtension(fi.FullName))
                    && !fi.FullName.Contains("~$"))
                {
                    FilePathsList.Add(fi.FullName);
                }
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d);
            }
        }
        private void CircleCancelCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<CancelDealCheckBtnDataEvent>().Publish(true);
            UtilSystemVar.IsDealingData = false;
            EventAggregatorRepository.EventAggregator.GetEvent<SendDealDataStateToSeachTxTEvent>().Publish(true);
            viewModel.AddTaskBtnIsEnabled = true;
            viewModel.CancelBtnVisibility = Visibility.Collapsed;
            viewModel.CheckBtnVisibility = Visibility.Visible;
            foreach (var item in viewModel.ChekedWordSettingsInfos)
            {
                item.IsChecking = false;
            }
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string pathSelect = m_Dialog.SelectedPath.Trim();
            var item = viewModel.ChekedWordSettingsInfos.FirstOrDefault(x =>x.FileFullPath == pathSelect);
            if(item ==null)
            {
                viewModel.ChekedWordSettingsInfos.Add(new CheckWordModel.ChekedWordSettingsInfo() {FileFullPath = pathSelect });
                SetCheckBtnEnableAndSavePathsToXml();
            }
        }

        private void DeletePathButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var chekedWordSettingsInfo = btn.Tag as CheckWordModel.ChekedWordSettingsInfo;
                viewModel.ChekedWordSettingsInfos.Remove(chekedWordSettingsInfo);
                SetCheckBtnEnableAndSavePathsToXml();
            }
        }
        private void SetCheckBtnEnableAndSavePathsToXml()
        {
            try
            {
                viewModel.SetIsCircleCheckBtnEnable();
                //保存路径设置信息到本地
                string chekedWordSettingsInfo = string.Format(@"{0}ChekedWordSettings\ChekedWordSettings.xml", AppDomain.CurrentDomain.BaseDirectory);
                DataParse.WriteToXmlPath(Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.ChekedWordSettingsInfos.ToList()), chekedWordSettingsInfo);
            }
            catch (Exception ex)
            { }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var chekedWordSettingsInfo = grid.Tag as CheckWordModel.ChekedWordSettingsInfo;
                if (viewModel.AddTaskBtnIsEnabled)
                {
                    chekedWordSettingsInfo.IsCanDelete = true;
                }
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var chekedWordSettingsInfo = grid.Tag as CheckWordModel.ChekedWordSettingsInfo;
                if (viewModel.AddTaskBtnIsEnabled)
                {
                    chekedWordSettingsInfo.IsCanDelete = false;
                }
            }
        }
    }
}
