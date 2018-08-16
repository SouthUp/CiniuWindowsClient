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
using WPFClientCheckWordModel;
using Newtonsoft.Json;
using CheckWordUtil;

namespace MyExcelAddIn
{
    public partial class ThisAddIn
    {
        bool isCloseDoc = false;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                HostSystemVar.CurrentImgsDictionary = new Dictionary<string, List<CheckWordModel.UnChekedWordInfo>>();
                Globals.ThisAddIn.Application.SheetActivate += Application_SheetActivate;
                Globals.ThisAddIn.Application.WorkbookActivate += Application_WorkbookActivate;
                EventAggregatorRepository.EventAggregator.GetEvent<SetMyControlVisibleEvent>().Subscribe(SetMyControlVisible);
                Globals.ThisAddIn.Application.WorkbookBeforeClose += Application_WorkbookBeforeClose; ;
                CreateCiNiuTaskPane();
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }

        private void Application_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            isCloseDoc = true;
        }

        private void Application_WorkbookActivate(Excel.Workbook Wb)
        {
            try
            {
                CreateCiNiuTaskPane();
            }
            catch (Exception ex)
            { }
        }
        private void CreateCiNiuTaskPane()
        {
            RemoveCiNiuTaskPanes();
            try
            {
                APIService service = new APIService();
                bool isOpen = service.GetCurrentAddIn("Excel");
                if (isOpen)
                {
                    CreateMyControlCustomTaskPane();
                }
                else
                {
                    EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(true);
                }
            }
            catch
            { }
        }
        private void RemoveCiNiuTaskPanes()
        {
            try
            {
                for (int i = Globals.ThisAddIn.CustomTaskPanes.Count; i > 0; i--)
                {
                    Microsoft.Office.Tools.CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes[i - 1];
                    if (ctp.Title == "违禁词检查")
                    {
                        Globals.ThisAddIn.CustomTaskPanes.Remove(ctp);
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void CreateMyControlCustomTaskPane()
        {
            try
            {
                var wpfHost = new TaskPaneWpfControlHost();
                MyControl wpfControl = new MyControl();
                wpfHost.WpfElementHost.HostContainer.Children.Add(wpfControl);
                var taskPane = this.CustomTaskPanes.Add(wpfHost, "违禁词检查");
                taskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
                taskPane.VisibleChanged += TaskPane_VisibleChanged;
                taskPane.Visible = true;
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
        /// <summary>
        /// Sheet表单切换事件
        /// </summary>
        /// <param name="Sh"></param>
        private void Application_SheetActivate(object Sh)
        {
            try
            {
                CreateCiNiuTaskPane();
            }
            catch (Exception ex)
            { }
        }

        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                var customTaskPane = sender as Microsoft.Office.Tools.CustomTaskPane;
                if (!isCloseDoc)
                {
                    try
                    {
                        AddInStateInfo addInStateInfo = new AddInStateInfo();
                        addInStateInfo.IsOpen = customTaskPane.Visible;
                        //保存用户操作信息到本地
                        string addInStateInfos = string.Format(@"{0}\ExcelAddInStateInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                        CheckWordUtil.DataParse.WriteToXmlPath(JsonConvert.SerializeObject(addInStateInfo), addInStateInfos);
                    }
                    catch (Exception ex)
                    {
                        CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                    }
                    if (customTaskPane.Visible == false)
                    {
                        RemoveCiNiuTaskPanes();
                        EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(true);
                    }
                    else
                    {
                        EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(false);
                    }
                }
                else
                {
                    isCloseDoc = false;
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
        private void SetMyControlVisible(bool isVisible)
        {
            try
            {
                if (isVisible)
                {
                    CreateMyControlCustomTaskPane();
                }
                else
                {
                    RemoveCiNiuTaskPanes();
                }
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
                Globals.ThisAddIn.Application.SheetActivate -= Application_SheetActivate;
                Globals.ThisAddIn.Application.WorkbookActivate -= Application_WorkbookActivate;
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
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
