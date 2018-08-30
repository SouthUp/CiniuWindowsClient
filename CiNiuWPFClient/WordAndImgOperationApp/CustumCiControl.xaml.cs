using CheckWordEvent;
using CheckWordModel;
using CheckWordUtil;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// CustumCiControl.xaml 的交互逻辑
    /// </summary>
    public partial class CustumCiControl : UserControl
    {
        CustumCiControlViewModel viewModel = new CustumCiControlViewModel();
        public CustumCiControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            EventAggregatorRepository.EventAggregator.GetEvent<ReturnToCustumCiViewEvent>().Subscribe(ReturnToCustumCiView);
        }
        private void ReturnToCustumCiView(bool b)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    ContentGrid.Children.Clear();
                    viewModel.CustumCiGridVisibility = Visibility.Visible;
                    viewModel.ContentGridVisibility = Visibility.Collapsed;
                }));
            }
            catch (Exception ex)
            { }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void EditCustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    EditCustumCiControl editCustumCiControl = new EditCustumCiControl();
                    ContentGrid.Children.Add(editCustumCiControl);
                    viewModel.CustumCiGridVisibility = Visibility.Collapsed;
                    viewModel.ContentGridVisibility = Visibility.Visible;
                }
                catch (Exception ex)
                { }
            }));
        }

        private async void SureToCustumCiTiaoBtn_Click(object sender, RoutedEventArgs e)
        {
            //调用接口添加自建词库
            Task<bool> task = new Task<bool>(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                bool b = false;
                try
                {
                    APIService service = new APIService();
                    b = service.AddCustumCiTiaoByToken(UtilSystemVar.UserToken, viewModel.SearchText, viewModel.DiscriptionSearchText);
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex)
                { }
                if (b)
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                }
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                return b;
            });
            task.Start();
            await task;
            if (task.Result)
            {
                ShowTipsInfo("添加自建词条成功");
            }
            else
            {
                ShowTipsInfo("添加自建词条失败");
            }
        }
        private void ShowTipsInfo(string tipsInfo)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    this.viewModel.MessageTipInfo = tipsInfo;
                    viewModel.MessageTipVisibility = Visibility.Visible;
                    Task task = new Task(() => {
                        System.Threading.Thread.Sleep(2000);
                        viewModel.MessageTipVisibility = Visibility.Collapsed;
                    });
                    task.Start();
                }));
            }
            catch (Exception ex)
            { }
        }

        private void DownLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.ciniuwang.com/files/批量导入词条模板.xlsx");
            }
            catch (Exception ex)
            { }
        }
        private async void ImportCustumCiBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Excel文件(*.xlsx)|*.xlsx";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = ofd.FileName;
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                Task<bool> taskCheckFile = new Task<bool>(() => {
                    bool b = CheckFile(strFileName);
                    return b;
                });
                taskCheckFile.Start();
                await taskCheckFile;
                //检验加载文件格式
                if (taskCheckFile.Result)
                {
                    int totalCount = 0;
                    int errorCount = 0;
                    //调用接口添加自建词库
                    Task<bool> task = new Task<bool>(() => {
                        ShowTipsInfo("正在批量上传，请稍后再上传");
                        EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                        try
                        {
                            Aspose.Cells.Workbook workbookName = new Aspose.Cells.Workbook(strFileName);
                            string sheetName = workbookName.Worksheets[0].Name;
                            Aspose.Cells.Cells cellsName = workbookName.Worksheets[0].Cells;
                            int minDataColumn = cellsName.MinDataColumn;
                            int maxDataColumn = cellsName.MaxDataColumn;
                            int minDataRow = cellsName.MinDataRow;
                            int maxDataRow = cellsName.MaxDataRow;
                            //导入数据
                            for (int i = minDataRow + 1; i < maxDataRow + 1; i++)
                            {
                                try
                                {
                                    string name = cellsName[i, minDataColumn].StringValue.Trim();
                                    string comment = cellsName[i, maxDataColumn].StringValue.Trim();
                                    if(!string.IsNullOrEmpty(name))
                                    {
                                        totalCount++;
                                        APIService service = new APIService();
                                        bool b = service.AddCustumCiTiaoByToken(UtilSystemVar.UserToken, name, comment);
                                        if (!b)
                                        {
                                            errorCount++;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                        catch (Exception ex)
                        { }
                        bool bResult = false;
                        if (totalCount != 0 && errorCount == 0)
                        {
                            bResult = true;
                            EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                        }
                        EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                        return bResult;
                    });
                    task.Start();
                    await task;
                    if (task.Result)
                    {
                        ShowTipsInfo("批量导入词条成功");
                    }
                    else
                    {
                        if (totalCount == errorCount)
                        {
                            ShowTipsInfo("批量导入词条失败");
                        }
                        else
                        {
                            ShowTipsInfo("批量导入词条" + (totalCount- errorCount) + "条成功，" + errorCount + "条失败");
                        }
                    }
                }
                else
                {
                    ShowTipsInfo("选择导入的文件与模板不匹配");
                }
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
            }
        }
        private bool CheckFile(string fileName)
        {
            bool result = false;
            try
            {
                Aspose.Cells.Workbook workbookName = new Aspose.Cells.Workbook(fileName);
                int sheetCount = workbookName.Worksheets.Count;
                if (sheetCount == 1)
                {
                    string sheetName = workbookName.Worksheets[0].Name;
                    if (sheetName == "批量导入词条模板")
                    {
                        Aspose.Cells.Cells cellsName = workbookName.Worksheets[0].Cells;
                        int minDataColumn = cellsName.MinDataColumn;
                        int maxDataColumn = cellsName.MaxDataColumn;
                        if (cellsName[0, minDataColumn].StringValue.Trim() == "词条" &&
                            cellsName[0, maxDataColumn].StringValue.Trim() == "解读")
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return result;
        }
    }
}
