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
                        OnTextChanged(this, new TextChangedEventArgs((char)e.UserState));
                    }
                    break;
            }
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Word.Application wordApp = e.Argument as Word.Application;
            BackgroundWorker bg = sender as BackgroundWorker;
            string lastPage = string.Empty;
            while (true)
            {
                try
                {
                    if (Application.Documents.Count > 0)
                    {
                        if (Application.ActiveDocument.Words.Count > 0)
                        {
                            var currentPage = Application.ActiveDocument.Bookmarks["\\Page"].Range.Text;
                            if (currentPage != null && currentPage != lastPage)
                            {
                                var differ = new DiffPlex.Differ();
                                var builder = new DiffPlex.DiffBuilder.InlineDiffBuilder(differ);
                                var difference = builder.BuildDiffModel(lastPage, currentPage);
                                var change = from d in difference.Lines where d.Type != DiffPlex.DiffBuilder.Model.ChangeType.Unchanged select d;
                                if (change.Any())
                                {
                                    string changeLastText = change.Last().Text;
                                    if(!string.IsNullOrEmpty(changeLastText))
                                    {
                                        bg.ReportProgress(50, changeLastText.Last());
                                    }
                                }

                                lastPage = currentPage;
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
        public char Letter;
        public TextChangedEventArgs(char letter)
        {
            this.Letter = letter;
        }
    }
}
