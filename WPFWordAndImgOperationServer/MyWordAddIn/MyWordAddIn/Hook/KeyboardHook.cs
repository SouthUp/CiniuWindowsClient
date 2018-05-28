using CheckWordEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MyWordAddIn.Hook
{
    public class KeyboardHook
    {
        #region (invokestuff)
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int code, HookProcKeyboard func, IntPtr hInstance, uint threadID);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        #region constans
        private const int WH_KEYBOARD = 2;
        private const int HC_ACTION = 0;
        #endregion

        delegate int HookProcKeyboard(int code, IntPtr wParam, IntPtr lParam);
        private HookProcKeyboard KeyboardProcDelegate = null;
        private IntPtr khook;
        bool doing = false;

        public void InitHook()
        {
            uint id = GetCurrentThreadId();
            //init the keyboard hook with the thread id of the Visual Studio IDE   
            this.KeyboardProcDelegate = new HookProcKeyboard(this.KeyboardProc);
            khook = SetWindowsHookEx(WH_KEYBOARD, this.KeyboardProcDelegate, IntPtr.Zero, id);
        }

        public void UnHook()
        {
            if (khook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(khook);
            }
        }

        private int KeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (code != HC_ACTION)
                {
                    return CallNextHookEx(khook, code, wParam, lParam);
                }
                if ((int)wParam == (int)Keys.OemSemicolon && ((int)lParam & (int)Keys.Alt) != 0)
                {
                    if (!doing)
                    {
                        doing = true;
                        EventAggregatorRepository.EventAggregator.GetEvent<OpenMyFloatingPanelEvent>().Publish(true);
                        doing = false;
                    }
                }
            }
            catch
            {
            }

            return CallNextHookEx(khook, code, wParam, lParam);
        }
    }
}
