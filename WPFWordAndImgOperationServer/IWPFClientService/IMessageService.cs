using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace IWPFClientService
{
    /// <summary>
    ///  WCF消息通信服务类接口
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallBackServices))]
    public interface IMessageService
    {
        /// <summary>
        /// 客户端上线
        /// </summary>
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        void Register(string name);

        /// <summary>
        /// 客户端下线
        /// </summary>
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        void Leave(string name);

        /// <summary>
        /// 客户端发送消息
        /// </summary>
        /// <param name="message">消息内容</param>
        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void ClientSendMessage(string message);
    }
    public interface ICallBackServices
    {
        /// <summary>
        /// 服务像客户端发送信息(异步)
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void SendMessage(string str);
    }
}
