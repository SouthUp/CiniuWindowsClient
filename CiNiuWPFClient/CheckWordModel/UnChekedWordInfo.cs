using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class UnChekedWordInfo : ViewModelBase
    {
        private string id = "";
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }
        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        private bool isCustumCi = false;
        public bool IsCustumCi
        {
            get { return isCustumCi; }
            set
            {
                isCustumCi = value;
                RaisePropertyChanged("IsCustumCi");
            }
        }
        private ObservableCollection<UnChekedDetailWordInfo> _unChekedWordDetailInfos = new ObservableCollection<UnChekedDetailWordInfo>();
        public ObservableCollection<UnChekedDetailWordInfo> UnChekedWordDetailInfos
        {
            get { return _unChekedWordDetailInfos; }
            set
            {
                _unChekedWordDetailInfos = value;
                RaisePropertyChanged("UnChekedWordDetailInfos");
            }
        }
    }
}
