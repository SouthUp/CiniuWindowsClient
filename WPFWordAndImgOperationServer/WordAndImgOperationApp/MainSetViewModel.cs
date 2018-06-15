using CheckWordModel;
using CheckWordUtil;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WordAndImgOperationApp
{
    public class MainSetViewModel : NotificationObject
    {
        private Visibility checkBtnVisibility = Visibility.Visible;
        public Visibility CheckBtnVisibility
        {
            get { return checkBtnVisibility; }
            set
            {
                checkBtnVisibility = value;
                RaisePropertyChanged("CheckBtnVisibility");
            }
        }
        private Visibility cancelBtnVisibility = Visibility.Collapsed;
        public Visibility CancelBtnVisibility
        {
            get { return cancelBtnVisibility; }
            set
            {
                cancelBtnVisibility = value;
                RaisePropertyChanged("CancelBtnVisibility");
            }
        }
        private bool _addTaskBtnIsEnabled = true;
        public bool AddTaskBtnIsEnabled
        {
            get { return _addTaskBtnIsEnabled; }
            set
            {
                _addTaskBtnIsEnabled = value;
                RaisePropertyChanged("AddTaskBtnIsEnabled");
            }
        }
        private bool _isCircleCheckBtnEnabled = false;
        public bool IsCircleCheckBtnEnabled
        {
            get { return _isCircleCheckBtnEnabled; }
            set
            {
                _isCircleCheckBtnEnabled = value;
                RaisePropertyChanged("IsCircleCheckBtnEnabled");
            }
        }
        private ObservableCollection<ChekedWordSettingsInfo> chekedWordSettingsInfos = new ObservableCollection<ChekedWordSettingsInfo>();
        public ObservableCollection<ChekedWordSettingsInfo> ChekedWordSettingsInfos
        {
            get { return chekedWordSettingsInfos; }
            set
            {
                chekedWordSettingsInfos = value;
                RaisePropertyChanged("chekedWordSettingsInfos");
            }
        }
        public void InitData()
        {
            string chekedWordSettingsInfo = string.Format(@"{0}ChekedWordSettings\ChekedWordSettings.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\");
            var ui = DataParse.ReadFromXmlPath<string>(chekedWordSettingsInfo);
            if (ui != null && ui.ToString() != "")
            {
                try
                {
                    var list =JsonConvert.DeserializeObject<List<ChekedWordSettingsInfo>>(ui.ToString());
                    ChekedWordSettingsInfos = new ObservableCollection<ChekedWordSettingsInfo>(list);
                    foreach (var item in ChekedWordSettingsInfos)
                    {
                        item.IsChecked = true;
                    }
                    SetIsCircleCheckBtnEnable();
                }
                catch
                { }
            }
            else
            {
                IsCircleCheckBtnEnabled = false;
            }
        }
        public void SetIsCircleCheckBtnEnable()
        {
            if (ChekedWordSettingsInfos.Count > 0)
            {
                IsCircleCheckBtnEnabled = true;
            }
            else
            {
                IsCircleCheckBtnEnabled = false;
            }
        }
    }
}
