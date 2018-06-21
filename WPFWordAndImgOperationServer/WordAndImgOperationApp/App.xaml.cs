using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WPFClientCheckWordModel;

namespace WordAndImgOperationApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            var proc = System.Diagnostics.Process.GetProcessesByName("WordAndImgOperationApp");
            //两个进程的话就杀掉一个
            if (proc.Length > 1)
            {
                Application.Current.Dispatcher.Invoke((Action)(() => Application.Current.Shutdown()));
                return;
            }
            StartService();
            try
            {
                WordAndImgAppInfo info = new WordAndImgAppInfo();
                info.Path = AppDomain.CurrentDomain.BaseDirectory + "WordAndImgOperationApp.exe";
                string infos = string.Format(@"{0}\WordAndImgAppInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(info), infos);
            }
            catch (Exception ex)
            { }
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
        private void StartService()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("ConsoleWPFClientServer");
                foreach (var p in processes)
                {
                    p.Kill();
                }
                string exePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                    "ConsoleWPFClientServer.exe");
                var info = new System.Diagnostics.ProcessStartInfo(exePath);
                info.UseShellExecute = true;
                info.WorkingDirectory = exePath.Substring(0, exePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                System.Diagnostics.Process.Start(info);
            }
            catch (Exception ex)
            { }
        }
    }
}
