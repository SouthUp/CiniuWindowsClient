using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckWordControl.Notify
{
    /// <summary>
    ///  屏幕宽高
    /// </summary>
    public static class Screen
    {
        /// <summary>
        ///  工作区宽度
        /// </summary>
        public static double ScreenWidth
        {
            get { return SystemParameters.WorkArea.Width; }
        }

        /// <summary>
        ///  工作区高度
        /// </summary>
        public static double ScreenHeight
        {
            get { return SystemParameters.WorkArea.Height; }
        }
    }
}
