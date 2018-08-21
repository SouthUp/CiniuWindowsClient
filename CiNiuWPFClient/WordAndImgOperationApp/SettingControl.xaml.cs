﻿using CheckWordEvent;
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
                        SaveSettingInfo();
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
        private void CategoryToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo(true);
        }

        private void CategoryToggleBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo(true);
        }
        private void SaveSettingInfo(bool isNeedGetWords = false)
        {
            try
            {
                EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Publish(new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi, CategoryInfos = viewModel.CategoryInfos.ToList() });
                //调用接口上传设置
                Task task = new Task(() => {
                    try
                    {
                        var info = new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi, CategoryInfos = viewModel.CategoryInfos.ToList() };
                        APIService service = new APIService();
                        service.SaveUserSettingByToken(UtilSystemVar.UserToken, info);
                        if(isNeedGetWords)
                        {
                            EventAggregatorRepository.EventAggregator.GetEvent<GetWordsEvent>().Publish(true);
                        }
                    }
                    catch (Exception ex)
                    { }
                });
                task.Start();
            }
            catch (Exception ex)
            { }
        }

        private void ToggleIsUseCustumCi_Click(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo(true);
        }

        private void ToggleIsCheckPic_Click(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo();
        }
    }
}
