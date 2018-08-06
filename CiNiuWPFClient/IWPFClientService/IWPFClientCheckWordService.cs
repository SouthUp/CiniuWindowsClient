using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using WPFClientCheckWordModel;

namespace IWPFClientService
{
    /// <summary>
    ///  检查违禁词服务类接口
    /// </summary>
    [ServiceContract]
    public interface IWPFClientCheckWordService
    {
        /// <summary>
        ///  检查违禁词
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "CheckWord", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string CheckWord(CheckWordRequestInfo info);
    }
}
