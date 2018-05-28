using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WPFClientCheckWordUtil;
using WPFClientService;

namespace ConsoleWPFClientServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "WPF服务程序";
                ConsoleWin32Helper.SetTitle("WPF服务程序");
                ConsoleWin32Helper.ShowHideWindow();
                ServiceHost hostHeart = new ServiceHost(typeof(WPFClientCheckWordService));
                hostHeart.Open();
                ServiceHost hostMessage = new ServiceHost(typeof(MessageService));
                hostMessage.Open();
                try
                {
                    string pathDir = SystemVar.FolderPath;
                    if (!Directory.Exists(pathDir))
                    {
                        Directory.CreateDirectory(pathDir);
                    }
                    string xmlPath = pathDir + "Resources/CheckWordDataSet.xml";
                    if (File.Exists(xmlPath))
                    {
                        WPFClientCheckWordUtil.CheckWordHelper.WordModels = WPFClientCheckWordUtil.CheckWordHelper.GetAllCheckWord(xmlPath);
                    }
                    string xmlPathReplace = pathDir + "Resources/ReplaceWordDataSet.xml";
                    if (File.Exists(xmlPathReplace))
                    {
                        WPFClientCheckWordUtil.CheckWordHelper.ReplaceWordModels = WPFClientCheckWordUtil.CheckWordHelper.GetReplaceWords(xmlPathReplace);
                    }
                }
                catch (Exception ex)
                { }
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine("服务开始运行");
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.Read();
            }
            catch (Exception ex)
            { }
        }
    }
    class ConsoleWin32Helper
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private static string _Title;
        public static void SetTitle(string p_strTitle)
        {
            _Title = p_strTitle;
        }
        public static void ShowHideWindow()
        {
            IntPtr intptr = FindWindow(null, _Title);
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
            }
        }
    }
}
