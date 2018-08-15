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
                    string officeWordVersion = GetOfficeAppVersion("Word");
                    if (string.IsNullOrEmpty(officeWordVersion))
                    {
                        viewModel.HasWordOffice = false;
                    }
                    else
                    {
                        viewModel.HasWordOffice = true;
                        viewModel.WordOfficeVersion = officeWordVersion;
                    }
                    string officeExcelVersion = GetOfficeAppVersion("Excel");
                    if (string.IsNullOrEmpty(officeExcelVersion))
                    {
                        viewModel.HasExcelOffice = false;
                    }
                    else
                    {
                        viewModel.HasExcelOffice = true;
                        viewModel.ExcelOfficeVersion = officeExcelVersion;
                    }
                    if (GetHasOfficeAddIn("Word"))
                    {
                        viewModel.HasWordOfficeAddIn = true;
                    }
                    if (GetHasOfficeAddIn("Excel"))
                    {
                        viewModel.HasExcelOfficeAddIn = true;
                    }
                    APIService service = new APIService();
                    MySettingInfo settingInfo = service.GetUserSettingByToken(UtilSystemVar.UserToken);
                    if (settingInfo != null)
                    {
                        viewModel.IsCheckPicInDucument = settingInfo.IsCheckPicInDucument;
                        viewModel.IsUseCustumCi = settingInfo.IsUseCustumCi;
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
        private string GetOfficeAppVersion(string officeName)
        {
            string officeVersion = "";
            RegistryKey rk;
            if (Environment.Is64BitOperatingSystem)
                rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey akey10 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\" + officeName + @"\InstallRoot\");//查询2010
            RegistryKey akey13 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\" + officeName + @"\InstallRoot\");//查询2013
            RegistryKey akey16 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\" + officeName + @"\InstallRoot\");//查询2016
            if (akey10 != null)
            {
                officeVersion = "2010";
            }
            if (akey13 != null)
            {
                officeVersion = "2013";
            }
            if (akey16 != null)
            {
                officeVersion = "2016";
            }
            return officeVersion;
        }

        private bool GetHasOfficeAddIn(string addInName)
        {
            bool result = false;
            try
            {
                RegistryKey rk;
                if (Environment.Is64BitOperatingSystem)
                    rk = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                else
                    rk = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
                RegistryKey akeyAddIns = rk.OpenSubKey(@"Software\Microsoft\Office\" + addInName + @"\Addins\上海冲南智能科技有限公司.词牛" + addInName + "插件");
                if (akeyAddIns != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            { }
            return result;
        }

        private void ToggleIsCheckPic_Checked(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo();
        }

        private void ToggleIsCheckPic_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo();
        }

        private void ToggleIsUseCustumCi_Checked(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo();
        }

        private void ToggleIsUseCustumCi_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveSettingInfo();
        }
        private void SaveSettingInfo()
        {
            try
            {
                EventAggregatorRepository.EventAggregator.GetEvent<WriteToSettingInfoEvent>().Publish(new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi });
                //调用接口上传设置
                Task task = new Task(() => {
                    try
                    {
                        APIService service = new APIService();
                        service.SaveUserSettingByToken(UtilSystemVar.UserToken, new MySettingInfo { IsCheckPicInDucument = viewModel.IsCheckPicInDucument, IsUseCustumCi = viewModel.IsUseCustumCi });
                    }
                    catch (Exception ex)
                    { }
                });
                task.Start();
            }
            catch (Exception ex)
            { }
        }
    }
}
