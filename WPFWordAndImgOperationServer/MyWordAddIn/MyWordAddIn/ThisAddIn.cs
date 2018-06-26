using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using Microsoft.Office.Core;
using System.Windows.Forms;
using System.Drawing;
using CheckWordEvent;
using System.Runtime.InteropServices;
using MyWordAddIn.Hook;

namespace MyWordAddIn
{
    public partial class ThisAddIn
    {
        ////////屏蔽右键菜单，快捷键和替换词
        ////////KeyboardHook hook;
        // 定义一个任务窗体 
        //////internal Microsoft.Office.Tools.CustomTaskPane myControlTaskPane;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            ////////屏蔽右键菜单，快捷键和替换词
            ////////hook = new KeyboardHook();
            ////////hook.InitHook();
            #region 加载自定义控件(WinForm)
            //////// ExcelHelp 是一个自定义控件类 
            //////myControlTaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(new TaskPaneWpfControlHost(), "违禁词查询");
            //////// 使任务窗体可见 
            //////myControlTaskPane.Visible = true;
            //////// 通过DockPosition属性来控制任务窗体的停靠位置， 
            //////// 设置为 MsoCTPDockPosition.msoCTPDockPositionRight这个代表停靠到右边，这个值也是默认值 
            //////myControlTaskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
            #endregion
            ////////屏蔽右键菜单，快捷键和替换词
            ////////this.Application.WindowBeforeRightClick += new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(Application_WindowBeforeRightClick);
            EventAggregatorRepository.EventAggregator.GetEvent<OpenMyFloatingPanelEvent>().Subscribe(OpenMyFloatingPanel);
        }
        void Application_WindowBeforeRightClick(Word.Selection Sel, ref bool Cancel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Sel.Range.Text) && Sel.Range.Text.Length > 0)
                {
                    //添加右键菜单
                    RemoveAndAddRightBtns(true);
                }
                else
                {
                    //添加右键菜单
                    RemoveAndAddRightBtns();
                }
                //////Office.CommandBarButton addBtn = (Office.CommandBarButton)Application.CommandBars.FindControl(Office.MsoControlType.msoControlButton, missing, "SearchSynonym", false);
                //////if (addBtn != null)
                //////{
                //////    addBtn.Enabled = false;
                //////    addBtn.Click -= new Office._CommandBarButtonEvents_ClickEventHandler(_FindRepalceWordBtn_Click);
                //////    if (!string.IsNullOrWhiteSpace(Sel.Range.Text) && Sel.Range.Text.Length > 1)
                //////    {
                //////        //添加右键菜单
                //////        RemoveAndAddRightBtns("SearchSynonym",true);
                //////        addBtn.Enabled = true;
                //////        addBtn.Click += new Office._CommandBarButtonEvents_ClickEventHandler(_FindRepalceWordBtn_Click);
                //////    }
                //////    else
                //////    {
                //////        //添加右键菜单
                //////        RemoveAndAddRightBtns("SearchSynonym");
                //////    }
                //////}
            }
            catch (Exception ex)
            { }
        }
        void _FindRepalceWordBtn_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OpenMyFloatingPanel();
        }
        /// <summary>
        /// 打开同义词替换窗体
        /// </summary>
        void OpenMyFloatingPanel()
        {
            try
            {
                Point currentPos = GetPositionForShowing(this.Application.Selection);
                FloatingPanel wpfHost = new FloatingPanel();
                MyWordTipsControl myWordTips = new MyWordTipsControl(wpfHost);
                wpfHost.MyWordTipsWPFHost.HostContainer.Children.Add(myWordTips);

                wpfHost.Location = currentPos;
                wpfHost.ShowDialog();
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 获取Range位置
        /// </summary>
        /// <param name="Sel"></param>
        /// <returns></returns>
        private static Point GetPositionForShowing(Word.Selection Sel)
        {
            int left = 0;
            int top = 0;
            int width = 0;
            int height = 0;
            Globals.ThisAddIn.Application.ActiveDocument.ActiveWindow.GetPoint(out left, out top, out width, out height, Sel.Range);

            Point currentPos = new Point(left, top);
            if (Screen.PrimaryScreen.Bounds.Height - top > 200)
            {
                currentPos.Y += 25;
            }
            else
            {
                currentPos.Y -= 100;
            }
            return currentPos;
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            try
            {
                CheckWordUtil.FileOperateHelper.DeleteFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\CheckWordResultTempWord");
                CheckWordUtil.FileOperateHelper.DeleteFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\MyWordAddIn\\");
                ////////屏蔽右键菜单，快捷键和替换词
                ////////hook.UnHook();
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 删除并添加右键菜单
        /// </summary>
        private void RemoveAndAddRightBtns(bool isEnable = false)
        {
            try
            {
                string nameMenue = "SearchSynonym";
                Microsoft.Office.Core.CommandBar mzBar = Application.CommandBars["Text"];//word文档已有的右键菜单Text
                Microsoft.Office.Core.CommandBarControls bars = mzBar.Controls;
                foreach (Microsoft.Office.Core.CommandBarControl temp_contrl in bars)
                {
                    string t = temp_contrl.Tag;
                    //如果已经存在就删除
                    if (t == nameMenue)
                    {
                        temp_contrl.Delete(true);
                    }
                }
                object missing = Type.Missing;
                Microsoft.Office.Core.CommandBarButton addBtn = (Microsoft.Office.Core.CommandBarButton)Application.CommandBars["Text"].Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlButton, missing, missing, missing, false);
                // 开始一个新Group，即在我们添加的Menu前加一条分割线
                addBtn.BeginGroup = true;
                // 为按钮设置Tag
                addBtn.Tag = nameMenue;
                // 添加按钮上的文字
                addBtn.Caption = "查找替换词";
                // 将按钮初始设为不激活状态
                addBtn.Enabled = isEnable;
                addBtn.Click += new Office._CommandBarButtonEvents_ClickEventHandler(_FindRepalceWordBtn_Click);
            }
            catch (Exception ex)
            { }
        }
        private void OpenMyFloatingPanel(bool isOpen)
        {
            try
            {
                if (!string.IsNullOrEmpty(Globals.ThisAddIn.Application.Selection.Text.Replace("\r", "").Replace("\n", "")))
                {
                    OpenMyFloatingPanel();
                }
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
