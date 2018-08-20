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
    ///  主窗体显示提示信息事件
    /// </summary>
    public class MainAppShowTipsInfoEvent : PubSubEvent<AppBusyIndicator>
    {

    }
    /// <summary>
    ///  主窗体繁忙事件
    /// </summary>
    public class MainAppBusyIndicatorEvent : PubSubEvent<AppBusyIndicator>
    {

    }
    /// <summary>
    ///  设置窗体繁忙事件
    /// </summary>
    public class SettingWindowBusyIndicatorEvent : PubSubEvent<AppBusyIndicator>
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
    /// <summary>
    ///  设置窗体加载项事件
    /// </summary>
    public class LoadSettingWindowGridViewEvent : PubSubEvent<string>
    {

    }
    /// <summary>
    ///  保存用户设置信息事件
    /// </summary>
    public class WriteToSettingInfoEvent : PubSubEvent<MySettingInfo>
    {

    }
    /// <summary>
    /// 拉取词库事件
    /// </summary>
    public class GetWordsEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  自建词条返回事件
    /// </summary>
    public class ReturnToCustumCiViewEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  关闭设置中的PopGrid事件
    /// </summary>
    public class CloseSettingWindowPopGridViewEvent : PubSubEvent<bool>
    {

    }
    /// <summary>
    ///  设置中的PopGrid删除提示事件
    /// </summary>
    public class SettingWindowShowDeletePopViewEvent : PubSubEvent<CustumCiInfo>
    {

    }
    /// <summary>
    ///  删除自定义词条事件
    /// </summary>
    public class DeleteCustumCiEvent : PubSubEvent<CustumCiInfo>
    {

    }
    /// <summary>
    ///  设置中的PopGrid查看详情事件
    /// </summary>
    public class SettingWindowShowDetailPopViewEvent : PubSubEvent<CustumCiInfo>
    {

    }
    /// <summary>
    ///  设置中的PopGrid编辑事件
    /// </summary>
    public class SettingWindowShowEditPopViewEvent : PubSubEvent<CustumCiInfo>
    {

    }
    /// <summary>
    ///  更新自定义词条事件
    /// </summary>
    public class UpdateCustumCiEvent : PubSubEvent<CustumCiInfo>
    {

    }
}
