using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckWordEvent;
using Microsoft.Office.Tools.Ribbon;

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
        }
    }
}
