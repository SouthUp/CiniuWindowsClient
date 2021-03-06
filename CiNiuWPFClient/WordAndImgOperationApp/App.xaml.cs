﻿using CheckWordUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            try
            {
                WordAndImgAppInfo info = new WordAndImgAppInfo();
                info.Path = AppDomain.CurrentDomain.BaseDirectory + "WordAndImgOperationApp.exe";
                string infos = string.Format(@"{0}\WordAndImgAppInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                DataParse.WriteToXmlPath(JsonConvert.SerializeObject(info), infos);
            }
            catch (Exception ex)
            { }
            try
            {
                UtilSystemVar.UrlStr = ConfigurationManager.AppSettings["UrlStr"].ToString() + ConfigurationManager.AppSettings["APIVersion"].ToString() +"/";
            }
            catch
            { }
            BootStrapper();
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
        private void BootStrapper()
        {
            string strpath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            System.Reflection.Assembly myDllAssembly = System.Reflection.Assembly.LoadFile(strpath);
            System.Type type = myDllAssembly.GetType("WordAndImgOperationApp.MetroBootstrapper");
            object instance = Activator.CreateInstance(type);
            MethodInfo curMethod = type.GetMethod("Run", new Type[] { });
            object result = curMethod.Invoke(instance, null);
        }
    }
}
