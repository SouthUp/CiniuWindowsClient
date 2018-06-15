using IWPFClientService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WPFClientCheckWordModel;
using WPFClientCheckWordUtil;

namespace WPFClientService
{
    /// <summary>
    ///  WCF消息通信服务类接口
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MessageService : IMessageService
    {
        private bool isServerRun = true;
        public List<ICallBackServices> ListClient = new List<ICallBackServices>();
        private static MessageService instance = null;
        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static MessageService GetInstance()
        {
            if (instance == null)
            {
                instance = new MessageService();
            }
            return instance;
        }
        /// <summary>
        /// 客户端上线
        /// </summary>
        public void Register(string name)
        {
            try
            {
                ICallBackServices client = OperationContext.Current.GetCallbackChannel<ICallBackServices>();
                MessageService.GetInstance().ListClient.Add(client);
                OperationContext.Current.Channel.Closed += new EventHandler(Channel_Closed);
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 客户端下线
        /// </summary>
        public void Leave(string name)
        {
            try
            {
                OperationContext.Current.Channel.Close();
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 客户端发送消息
        /// </summary>
        /// <param name="message"></param>
        public void ClientSendMessage(string message)
        {
            try
            {
                ICallBackServices client = OperationContext.Current.GetCallbackChannel<ICallBackServices>();
                LoginInOutInfo loginInOutInfo = JsonConvert.DeserializeObject<LoginInOutInfo>(message);
                if (loginInOutInfo.Type == "LoginIn")
                {
                    SystemVar.UrlStr = loginInOutInfo.UrlStr;
                    if (CheckWordHelper.WordModels.Count == 0)
                    {
                        CheckWordHelper.WordModels = CheckWordHelper.GetAllCheckWordByToken(loginInOutInfo.Token);
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 客户端关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Channel_Closed(object sender, EventArgs e)
        {
            try
            {
                ICallBackServices client = sender as ICallBackServices;
                MessageService.GetInstance().ListClient.Remove(client);
            }
            catch (Exception ex)
            { }
        }
    }
}
