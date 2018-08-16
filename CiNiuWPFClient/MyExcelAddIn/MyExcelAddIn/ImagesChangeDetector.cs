using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel;
using MyExcelAddIn;
using Microsoft.Office.Interop.Excel;

namespace MyExcelAddIn
{
    public class ImagesChangeDetector
    {
        private BackgroundWorker bg;

        public delegate void ImagesChangeHandler(object sender, ImagesChangedEventArgs e);
        public event ImagesChangeHandler OnImagesChanged;

        public ImagesChangeDetector()
        {

        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            if(bg == null)
            {
                bg = new BackgroundWorker();
                bg.WorkerReportsProgress = true;
                bg.WorkerSupportsCancellation = true;
                bg.ProgressChanged += bg_ProgressChanged;
                bg.DoWork += bg_DoWork;
            }
            bg.RunWorkerAsync();
        }

        private void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 50: //change
                    if (OnImagesChanged != null)
                    {
                        OnImagesChanged(this, new ImagesChangedEventArgs("Image"));
                    }
                    break;
            }
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bg = sender as BackgroundWorker;
            int countPicsLast = 0;
            bool isUserLogin = CheckWordUtil.Util.GetIsUserLogin();
            while (true)
            {
                try
                {
                    int countPics = 0;
                    var workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
                    var workSheet = (Worksheet)workBook.ActiveSheet;
                    for (int i = 1; i <= workSheet.Shapes.Count; i++)
                    {
                        var pic = workSheet.Shapes.Item(i);
                        if (pic != null && pic.Type == Microsoft.Office.Core.MsoShapeType.msoPicture)
                        {
                            countPics++;
                        }
                    }
                    if (countPics != countPicsLast)
                    {
                        bg.ReportProgress(50, "");
                        countPicsLast = countPics;
                    }
                    else
                    {
                        bool isLogin = CheckWordUtil.Util.GetIsUserLogin();
                        if (isLogin != isUserLogin)
                        {
                            bg.ReportProgress(50, "");
                            isUserLogin = isLogin;
                        }
                    }
                }
                catch (Exception ex)
                { }
                if (bg.CancellationPending)
                {
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (bg != null && !bg.CancellationPending)
            {
                bg.CancelAsync();
            }
        }
    }

    public class ImagesChangedEventArgs : EventArgs
    {
        public string Letter;
        public ImagesChangedEventArgs(string letter)
        {
            this.Letter = letter;
        }
    }
}
