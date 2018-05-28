using CheckWordModel;
using CheckWordUtil;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWordAddIn
{
    public class MyWordTipsControlViewModel : NotificationObject
    {
        private ObservableCollection<ReplaceWordInfo> replaceWordLists = new ObservableCollection<ReplaceWordInfo>();
        public ObservableCollection<ReplaceWordInfo> ReplaceWordLists
        {
            get { return replaceWordLists; }
            set
            {
                replaceWordLists = value;
                RaisePropertyChanged("ReplaceWordLists");
            }
        }
        public void InitData(string name)
        {
            ObservableCollection<ReplaceWordInfo> replaceWordInfos = new ObservableCollection<ReplaceWordInfo>(CheckWordHelper.GetReplaceWordInfos(name));
            for (int i = 0; i < replaceWordInfos.Count; i++)
            {
                replaceWordInfos[i].Index = i + 1;
            }
            ReplaceWordLists = replaceWordInfos;
        }
    }
}
