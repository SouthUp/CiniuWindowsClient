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

namespace MyWordAddIn
{
    public class TextChangeDetector
    {
        public Word.Application Application;
        private BackgroundWorker bg;

        public delegate void TextChangeHandler(object sender, TextChangedEventArgs e);
        public event TextChangeHandler OnTextChanged;

        public TextChangeDetector(Word.Application app)
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
                    if (OnTextChanged != null)
                    {
                        OnTextChanged(this, new TextChangedEventArgs("Text"));
                    }
                    break;
            }
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Word.Application wordApp = e.Argument as Word.Application;
            BackgroundWorker bg = sender as BackgroundWorker;
            int countWordsLast = 0;
            while (true)
            {
                try
                {
                    if (Application.Documents.Count > 0)
                    {
                        if (Application.ActiveDocument.Words.Count > 0)
                        {
                            int countWords = Application.ActiveDocument.Words.Count;
                            if (countWords != countWordsLast)
                            {
                                bg.ReportProgress(50, "");
                                countWordsLast = countWords;
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
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

    public class TextChangedEventArgs : EventArgs
    {
        public string Letter;
        public TextChangedEventArgs(string letter)
        {
            this.Letter = letter;
        }
    }
}
