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
        Dictionary<Microsoft.Office.Interop.Word.Window, bool> CurrentWindowsDictionary = new Dictionary<Microsoft.Office.Interop.Word.Window, bool>();
        private void MyRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Subscribe(SetOpenMyControlEnable);
            Globals.ThisAddIn.Application.WindowActivate += Application_WindowActivate;
            AddTaskPaneWpfControlHost();
        }

        private void Application_WindowActivate(Microsoft.Office.Interop.Word.Document Doc, Microsoft.Office.Interop.Word.Window Wn)
        {
            try
            {
                var customTaskPanes = Globals.ThisAddIn.CustomTaskPanes.Where(x => x.Title == "违禁词检查").ToList();
                if (customTaskPanes.Count == 0)
                {
                    AddTaskPaneWpfControlHost();
                }
                else
                {
                    bool hasAdd = false;
                    foreach (var item in customTaskPanes)
                    {
                        if (item.Window == Globals.ThisAddIn.Application.ActiveWindow)
                        {
                            hasAdd = true;
                        }
                        else
                        {
                            item.Visible = false;
                        }
                    }
                    if (!hasAdd)
                    {
                        AddTaskPaneWpfControlHost();
                    }
                    else
                    {
                        var item = customTaskPanes.FirstOrDefault(x => x.Window == Globals.ThisAddIn.Application.ActiveWindow);
                        item.Visible = CurrentWindowsDictionary[Globals.ThisAddIn.Application.ActiveWindow];
                        CheckWordBtn.Checked = item.Visible;
                        var taskPaneWpfControlHost = item.Control as TaskPaneWpfControlHost;
                        foreach (var myControl in taskPaneWpfControlHost.WpfElementHost.HostContainer.Children)
                        {
                            var wpfControl = myControl as MyControl;
                            if (CheckWordBtn.Checked)
                            {
                                wpfControl.StartDetector();
                            }
                            else
                            {
                                wpfControl.CloseDetector();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void AddTaskPaneWpfControlHost()
        {
            try
            {
                var wpfHost = new TaskPaneWpfControlHost();
                MyControl wpfControl = new MyControl();
                wpfHost.WpfElementHost.HostContainer.Children.Add(wpfControl);
                var taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(wpfHost, "违禁词检查", Globals.ThisAddIn.Application.ActiveWindow);
                taskPane.Visible = true;
                taskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
                taskPane.VisibleChanged += TaskPane_VisibleChanged;
                CurrentWindowsDictionary.Add(Globals.ThisAddIn.Application.ActiveWindow, true);
                CheckWordBtn.Checked = true;
            }
            catch (Exception ex)
            { }
        }
        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            var customTaskPane = sender as Microsoft.Office.Tools.CustomTaskPane;
            if (customTaskPane.Window == Globals.ThisAddIn.Application.ActiveWindow)
            {
                CurrentWindowsDictionary[Globals.ThisAddIn.Application.ActiveWindow] = customTaskPane.Visible;
                EventAggregatorRepository.EventAggregator.GetEvent<SetOpenMyControlEnableEvent>().Publish(!customTaskPane.Visible);
            }
            if (customTaskPane.Visible == false)
            {
                var taskPaneWpfControlHost = customTaskPane.Control as TaskPaneWpfControlHost;
                foreach (var item in taskPaneWpfControlHost.WpfElementHost.HostContainer.Children)
                {
                    var wpfControl = item as MyControl;
                    wpfControl.CloseDetector();
                }
            }
            else
            {
                var taskPaneWpfControlHost = customTaskPane.Control as TaskPaneWpfControlHost;
                foreach (var item in taskPaneWpfControlHost.WpfElementHost.HostContainer.Children)
                {
                    var wpfControl = item as MyControl;
                    wpfControl.StartDetector();
                }
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
                var customTaskPanes = Globals.ThisAddIn.CustomTaskPanes.Where(x => x.Title == "违禁词检查").ToList();
                if (customTaskPanes.Count > 0)
                {
                    foreach (var item in customTaskPanes)
                    {
                        if (item.Window == Globals.ThisAddIn.Application.ActiveWindow)
                        {
                            item.Visible = CheckWordBtn.Checked;
                            CurrentWindowsDictionary[Globals.ThisAddIn.Application.ActiveWindow] = item.Visible;
                        }
                    }
                }
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
