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
            try
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
                Globals.ThisAddIn.Application.SheetActivate += Application_SheetActivate;
                Globals.ThisAddIn.Application.WorkbookActivate += Application_WorkbookActivate;
                EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Subscribe(SetMyControlVisible);
            }
            catch (Exception ex)
            { }
        }

        private void Application_WorkbookActivate(Excel.Workbook Wb)
        {
            try
            {
                if (wpfControl != null && HostSystemVar.CustomTaskPane.Visible)
                {
                    wpfControl.InitData();
                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Sheet表单切换事件
        /// </summary>
        /// <param name="Sh"></param>
        private void Application_SheetActivate(object Sh)
        {
            try
            {
                if (wpfControl != null && HostSystemVar.CustomTaskPane.Visible)
                {
                    wpfControl.InitData();
                }
            }
            catch (Exception ex)
            { }
        }

        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (HostSystemVar.CustomTaskPane.Visible == false)
                {
                    wpfControl.CloseDetector();
                    EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(true);
                }
                else
                {
                    wpfControl.StartDetector();
                    EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(false);
                }
            }
            catch (Exception ex)
            { }
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
                CheckWordUtil.FileOperateHelper.DeleteFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTempExcel");
                CheckWordUtil.FileOperateHelper.DeleteFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\MyExcelAddIn\\");
                EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Publish(false);
                Globals.ThisAddIn.Application.SheetActivate -= Application_SheetActivate;
                Globals.ThisAddIn.Application.WorkbookActivate -= Application_WorkbookActivate;
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
