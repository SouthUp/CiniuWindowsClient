using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckWordEvent;
using Microsoft.Office.Core;
using Microsoft.Office.Tools.Ribbon;

namespace MyWordAddIn
{
    public partial class MyRibbon
    {
        private void MyRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            Globals.ThisAddIn.Application.WindowActivate += Application_WindowActivate;
            EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Subscribe(SetOpenMyControlEnable);
        }
        private void Application_WindowActivate(Microsoft.Office.Interop.Word.Document Doc, Microsoft.Office.Interop.Word.Window Wn)
        {
            try
            {
                string fileName = Globals.ThisAddIn.Application.ActiveDocument.Name;
                var customTaskPanes = Globals.ThisAddIn.CustomTaskPanes.Where(x => x.Title == "违禁词检查").ToList();
                if (customTaskPanes.Count == 0)
                {
                    AddTaskPaneWpfControlHost(fileName);
                }
                else
                {
                    bool hasAdd = false;
                    foreach (var item in customTaskPanes)
                    {
                        if (item.Control.Tag.ToString() == fileName)
                        {
                            hasAdd = true;
                        }
                    }
                    if (!hasAdd)
                    {
                        AddTaskPaneWpfControlHost(fileName);
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void AddTaskPaneWpfControlHost(string fileName)
        {
            try
            {
                var wpfHost = new TaskPaneWpfControlHost();
                var wpfTaskPane = new TaskPaneWpfControl();
                MyControl wpfControl = new MyControl();
                wpfTaskPane.TaskPaneContent.Children.Add(wpfControl);
                wpfHost.WpfElementHost.HostContainer.Children.Add(wpfTaskPane);
                wpfHost.Tag = fileName;
                var taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(wpfHost, "违禁词检查");
                taskPane.Visible = true;
                taskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
                taskPane.VisibleChanged += TaskPane_VisibleChanged;
            }
            catch (Exception ex)
            { }
        }
        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            var customTaskPane = sender as Microsoft.Office.Tools.CustomTaskPane;
            if (customTaskPane.Visible == false)
            {
                var taskPaneWpfControlHost = customTaskPane.Control as TaskPaneWpfControlHost;
                foreach (var item in taskPaneWpfControlHost.WpfElementHost.HostContainer.Children)
                {
                    var wpfTaskPane = item as TaskPaneWpfControl;
                    foreach (var itemInfo in wpfTaskPane.TaskPaneContent.Children)
                    {
                        var wpfControl = itemInfo as MyControl;
                        wpfControl.CloseDetector();
                    }
                }
                EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(true);
            }
            else
            {
                var taskPaneWpfControlHost = customTaskPane.Control as TaskPaneWpfControlHost;
                foreach (var item in taskPaneWpfControlHost.WpfElementHost.HostContainer.Children)
                {
                    var wpfTaskPane = item as TaskPaneWpfControl;
                    foreach (var itemInfo in wpfTaskPane.TaskPaneContent.Children)
                    {
                        var wpfControl = itemInfo as MyControl;
                        wpfControl.StartDetector();
                    }
                }
                EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(false);
            }
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
            try
            {
                string fileName = Globals.ThisAddIn.Application.ActiveDocument.Name;
                var customTaskPanes = Globals.ThisAddIn.CustomTaskPanes.Where(x => x.Title == "违禁词检查").ToList();
                if (customTaskPanes.Count > 0)
                {
                    foreach (var item in customTaskPanes)
                    {
                        if (item.Control.Tag.ToString() == fileName)
                        {
                            item.Visible = CheckWordBtn.Checked;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
