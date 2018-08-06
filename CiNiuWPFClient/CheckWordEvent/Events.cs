﻿using System;
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
    ///  应用程序关闭事件
    /// </summary>
    public class CloseMyAppEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  主程序加载项事件
    /// </summary>
    public class InitContentGridViewEvent : PubSubEvent<string>
    {

    }
    /// <summary>
    /// 登录相关事件
    /// </summary>
    public class LoginInOrOutEvent : PubSubEvent<string>
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
    /// <summary>
    ///  登录窗体加载项事件
    /// </summary>
    public class LoadLoginContentGridViewEvent : PubSubEvent<string>
    {

    }
    /// <summary>
    ///  关闭登录窗体事件
    /// </summary>
    public class CloseLoginWindowViewEvent : PubSubEvent<bool>
    {

    }
}
