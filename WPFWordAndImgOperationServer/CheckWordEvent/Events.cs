using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using CheckWordModel;

namespace CheckWordEvent
{
    /// <summary>
    ///  应用程序是否繁忙事件
    /// </summary>
    public class AppBusyIndicatorEvent : PubSubEvent<AppBusyIndicator>
    {

    }
    /// <summary>
    ///  加载窗体事件
    /// </summary>
    public class InitContentGridViewEvent : PubSubEvent<string>
    {

    }
    /// <summary>
    /// 点击检查按钮检查数据
    /// </summary>
    public class DealCheckBtnDataEvent : PubSubEvent<ObservableCollection<ChekedWordSettingsInfo>>
    {

    }
    /// <summary>
    /// 取消检查按钮检查数据
    /// </summary>
    public class CancelDealCheckBtnDataEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 检查按钮检查数据完成
    /// </summary>
    public class DealCheckBtnDataFinishedEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 登录相关事件
    /// </summary>
    public class LoginInOrOutEvent : PubSubEvent<string>
    {

    }
    /// <summary>
    /// 关闭详情窗体事件
    /// </summary>
    public class CloseDetailWindowEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 隐藏详情窗体事件
    /// </summary>
    public class HideDetailWindowEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 关闭详情窗体完成事件
    /// </summary>
    public class CloseDetailWindowFinishedEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 设置详情窗体是否置顶事件
    /// </summary>
    public class SetDetailWindowTopmostEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 是否能够打开悬浮窗体事件
    /// </summary>
    public class IsCanOpenSearchPopWindowEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 传递是否在处理数据事件
    /// </summary>
    public class SendDealDataStateToSeachTxTEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    /// 传递NotifyMessage事件
    /// </summary>
    public class SendNotifyMessageEvent : PubSubEvent<string>
    {

    }
    /// <summary>
    /// 检查版本事件
    /// </summary>
    public class CheckVersionMessageEvent : PubSubEvent<bool>
    {

    }
}
