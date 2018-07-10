using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckWordEvent;
using Microsoft.Office.Tools.Ribbon;
using Newtonsoft.Json;
using WPFClientCheckWordModel;

namespace MyExcelAddIn
{
    public partial class MyRibbon
    {
        private void MyRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Subscribe(SetOpenMyControlEnable);
        }
        private void SetOpenMyControlEnable(bool isEnable)
        {
            try
            {
                if (isEnable)
                {
                    CheckWordBtn.Checked = false;
                }
                else
                {
                    CheckWordBtn.Checked = true;
                }
            }
            catch (Exception ex)
            { }
        }
        private void CheckWordBtn_Click(object sender, RibbonControlEventArgs e)
        {
            if (CheckWordBtn.Checked)
            {
                CheckWordBtn.Checked = true;
                EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Publish(true);
            }
            else
            {
                CheckWordBtn.Checked = false;
                EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Publish(false);
            }
            try
            {
                AddInStateInfo addInStateInfo = new AddInStateInfo();
                addInStateInfo.IsOpen = CheckWordBtn.Checked;
                //保存用户操作信息到本地
                string addInStateInfos = string.Format(@"{0}\ExcelAddInStateInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                CheckWordUtil.DataParse.WriteToXmlPath(JsonConvert.SerializeObject(addInStateInfo), addInStateInfos);
            }
            catch (Exception ex)
            { }
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<MarkUnCheckWordEvent>().Publish(true);
        }
    }
}
