using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.ComponentModel;
using Microsoft.Office.Interop.Word;

namespace MyWordAddIn
{
    public class ImagesChangeDetector
    {
        public Word.Application Application;
        private BackgroundWorker bg;

        public delegate void ImagesChangeHandler(object sender, ImagesChangedEventArgs e);
        public event ImagesChangeHandler OnImagesChanged;

        public ImagesChangeDetector(Word.Application app)
        {
            this.Application = app;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            bg = new BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.WorkerSupportsCancellation = true;
            bg.ProgressChanged += bg_ProgressChanged;
            bg.DoWork += bg_DoWork;
            bg.RunWorkerAsync(this.Application);
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
            Application wordApp = e.Argument as Application;
            BackgroundWorker bg = sender as BackgroundWorker;
            int countPicsLast = 0;
            bool isUserLogin = CheckWordUtil.Util.GetIsUserLogin();
            while (true)
            {
                try
                {
                    if (Application.Documents.Count > 0)
                    {
                        if (Application.ActiveDocument.Paragraphs.Count > 0)
                        {
                            int countPics = 0;
                            foreach (Paragraph paragraph in Application.ActiveDocument.Paragraphs)
                            {
                                foreach (InlineShape ils in paragraph.Range.InlineShapes)
                                {
                                    if (ils != null)
                                    {
                                        if (ils.Type == WdInlineShapeType.wdInlineShapePicture)
                                        {
                                            countPics++;
                                        }
                                    }
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
