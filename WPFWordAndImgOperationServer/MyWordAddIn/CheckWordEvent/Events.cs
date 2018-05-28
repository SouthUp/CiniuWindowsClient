using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace CheckWordEvent
{
    /// <summary>
    ///  应用程序是否繁忙事件
    /// </summary>
    public class AppBusyIndicatorEvent : PubSubEvent<AppBusyIndicator>
    {

    }
    /// <summary>
    ///  设置打开违禁词按钮是否可用事件
    /// </summary>
    public class SetOpenMyControlEnableEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  设置违禁词模块窗体是否可见事件
    /// </summary>
    public class SetMyControlVisibleEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  设置打开违禁词库按钮是否可用事件
    /// </summary>
    public class SetOpenWordsDBEnableEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  设置违禁词库模块窗体是否可见事件
    /// </summary>
    public class SetMyWordsDBVisibleEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  设置打开推荐词库按钮是否可用事件
    /// </summary>
    public class SetOpenSynonymDBEnableEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  设置推荐词库模块窗体是否可见事件
    /// </summary>
    public class SetMySynonymDBVisibleEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  快捷键触发同义词替换事件
    /// </summary>
    public class OpenMyFloatingPanelEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  快捷键触发同义词选择序号事件
    /// </summary>
    public class SendSelectNumberToMyWordTipsEvent : PubSubEvent<int>
    {

    }
    /// <summary>
    ///  快捷键关闭选择替换词窗体事件
    /// </summary>
    public class CloseMyWordTipsEvent : PubSubEvent<bool>
    {

    }
}
