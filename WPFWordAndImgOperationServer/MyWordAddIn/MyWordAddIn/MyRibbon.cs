using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckWordEvent;
using Microsoft.Office.Tools.Ribbon;

namespace MyWordAddIn
{
    public partial class MyRibbon
    {
        private void MyRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            //////btnCheckWord.Enabled = false;
            EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Subscribe(SetOpenMyControlEnable);
            EventAggregatorRepository.EventAggregator.GetEvent<SetOpenWordsDBEnableEvent>().Subscribe(SetOpenWordsDBEnable);
            EventAggregatorRepository.EventAggregator.GetEvent<SetOpenSynonymDBEnableEvent>().Subscribe(SetOpenSynonymDBEnable);
        }
        private void SetOpenMyControlEnable(bool isEnable)
        {
            try
            {
                //////btnCheckWord.Enabled = isEnable;
                if(isEnable)
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
        private void SetOpenWordsDBEnable(bool isEnable)
        {
            try
            {
                ViolateDBBtn.Enabled = isEnable;
            }
            catch (Exception ex)
            { }
        }
        private void SetOpenSynonymDBEnable(bool isEnable)
        {
            try
            {
                SynonymDBBtn.Enabled = isEnable;
            }
            catch (Exception ex)
            { }
        }
        private void btnCheckWord_Click(object sender, RibbonControlEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Publish(true);
        }

        private void ViolateDBBtn_Click(object sender, RibbonControlEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetMyWordsDBVisibleEvent>().Publish(true);
        }

        private void SynonymDBBtn_Click(object sender, RibbonControlEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetMySynonymDBVisibleEvent>().Publish(true);
        }

        private void CheckWordBtn_Click(object sender, RibbonControlEventArgs e)
        {
            if(CheckWordBtn.Checked)
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
