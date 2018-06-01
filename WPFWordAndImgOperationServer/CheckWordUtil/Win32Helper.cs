using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordUtil
{
    public class Win32Helper
    {
        public const int WM_COPYDATA = 0x004A;

        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilter(uint msg, int flags);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        /// <summary>
        /// 定义用户要传递的消息的数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;//字符串长度
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//字符串
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd,                   //目标窗体句柄
        int Msg,                       //WM_COPYDATA
        int wParam,                //自定义数值
        ref CopyDataStruct lParam             //结构体
        );
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="windowName">window的title，建议加上GUID，不会重复</param>
        /// <param name="strMsg">要发送的字符串</param>
        public static void SendMessage(string windowName, string strMsg)
        {
            if (strMsg == null) return;
            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != IntPtr.Zero)
            {
                CopyDataStruct cds;
                cds.dwData = IntPtr.Zero;
                cds.lpData = strMsg;
                //注意：长度为字节数
                cds.cbData = System.Text.Encoding.Default.GetBytes(strMsg).Length + 1;
                // 消息来源窗体
                int fromWindowHandler = 0;
                SendMessage(hwnd, WM_COPYDATA, fromWindowHandler, ref cds);
            }
        }
        public static void ShowHideWindow(string windowName)
        {
            IntPtr intptr = FindWindow(null, windowName);
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
            }
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        public static bool IsFileOpen(string path)
        {
            bool result = false;
            try
            {
                string vFileName = path;
                IntPtr vHandle = _lopen(vFileName, OF_READWRITE | OF_SHARE_DENY_NONE);//windows Api上面有定义扩展方法
                if (vHandle == HFILE_ERROR)
                {
                    result = true;//文件被占用  
                }
                CloseHandle(vHandle);
            }
            catch (Exception ex)
            { }
            return result;
        }
    }
}
