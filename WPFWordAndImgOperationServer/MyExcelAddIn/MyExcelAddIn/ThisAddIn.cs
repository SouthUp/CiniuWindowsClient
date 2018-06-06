using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using MyWordAddIn;
using Microsoft.Office.Core;
using CheckWordEvent;

namespace MyExcelAddIn
{
    public partial class ThisAddIn
    {
        MyControl wpfControl;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            var wpfHost = new TaskPaneWpfControlHost();
            var wpfTaskPane = new TaskPaneWpfControl();
            wpfControl = new MyControl();
            wpfTaskPane.TaskPaneContent.Children.Add(wpfControl);
            wpfHost.WpfElementHost.HostContainer.Children.Add(wpfTaskPane);
            var taskPane = this.CustomTaskPanes.Add(wpfHost, "违禁词检查");
            taskPane.Visible = true;
            taskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
            taskPane.VisibleChanged += TaskPane_VisibleChanged;
            HostSystemVar.CustomTaskPane = taskPane;
            EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Subscribe(SetMyControlVisible);
        }
        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            if (HostSystemVar.CustomTaskPane.Visible == false)
            {
                EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(true);
            }
            else
            {
                EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(false);
            }
        }
        private void SetMyControlVisible(bool isVisible)
        {
            try
            {
                HostSystemVar.CustomTaskPane.Visible = isVisible;
            }
            catch (Exception ex)
            { }
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            try
            {
                EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Publish(false);
            }
            catch (Exception ex)
            { }
        }

        #region VSTO 生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
