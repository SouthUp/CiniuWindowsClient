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
    /// SettingControl.xaml 的交互逻辑
    /// </summary>
    public partial class SettingControl : UserControl
    {
        SettingControlViewModel viewModel = new SettingControlViewModel();
        public SettingControl()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }
        private async void InitData()
        {
            Task task = new Task(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                try
                {
                    APIService service = new APIService();
                    MySettingInfo settingInfo = service.GetUserSettingByToken(UtilSystemVar.UserToken);
                    if (settingInfo != null)
                    {
                        viewModel.IsCheckPicInDucument = settingInfo.IsCheckPicInDucument;
                        viewModel.IsUseCustumCi = settingInfo.IsUseCustumCi;
                        viewModel.CategoryInfos = new System.Collections.ObjectModel.ObservableCollection<CategorySelectInfo>(settingInfo.CategoryInfos.ToList());
                        EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Publish(new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi, CategoryInfos = viewModel.CategoryInfos.ToList() });
                    }
                    else
                    {
                        string mySettingInfo = string.Format(@"{0}\MySettingInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                        var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(mySettingInfo);
                        if (ui != null && ui.ToString() != "")
                        {
                            try
                            {
                                var mySetting = JsonConvert.DeserializeObject<MySettingInfo>(ui.ToString());
                                if (mySetting != null)
                                {
                                    viewModel.IsCheckPicInDucument = mySetting.IsCheckPicInDucument;
                                    viewModel.IsUseCustumCi = mySetting.IsUseCustumCi;
                                    viewModel.CategoryInfos = new System.Collections.ObjectModel.ObservableCollection<CategorySelectInfo>(settingInfo.CategoryInfos.ToList());
                                }
                            }
                            catch
                            { }
                        }
                    }
                }
                catch (Exception ex)
                { }
                System.Threading.Thread.Sleep(500);
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
            });
            task.Start();
            await task;
        }

        private void ApplySettingBtn_Click(object sender, RoutedEventArgs e)
        {
            //调用接口上传设置
            Task task = new Task(() => {
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = true });
                bool b = false;
                try
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Publish(new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi, CategoryInfos = viewModel.CategoryInfos.ToList() });
                    var info = new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi, CategoryInfos = viewModel.CategoryInfos.ToList() };
                    APIService service = new APIService();
                    b = service.SaveUserSettingByToken(UtilSystemVar.UserToken, info);
                    if (b)
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                    }
                }
                catch (Exception ex)
                { }
                EventAggregatorRepository.EventAggregator.GetEvent<SettingWindowBusyIndicatorEvent>().Publish(new AppBusyIndicator { IsBusy = false });
                if (b)
                {
                    ShowTipsInfo("应用设置成功");
                }
                else
                {
                    ShowTipsInfo("应用设置失败");
                }
            });
            task.Start();
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
    }
}
