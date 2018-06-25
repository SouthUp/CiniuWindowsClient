using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClientCheckWordModel;

namespace CheckWordUtil
{
    public class APIService
    {
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string LoginIn(string userName,string password)
        {
            #region 不调用接口假数据
            if (!UtilSystemVar.IsCallWebApi)
            {
                return "seicjoe5rp6wkkba0sxox3oa3";
            }
            #endregion
            string resultToken = "";
            try
            {
                string apiName = "token";
                UserLoginRequest user = new UserLoginRequest() { username = userName, password = password };
                string json = JsonConvert.SerializeObject(user);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json);
                UserLoginResponse resultInfo = JsonConvert.DeserializeObject<UserLoginResponse>(resultStr);
                resultToken = resultInfo.sessionToken;
            }
            catch (Exception ex)
            { }
            return resultToken;
        }
        /// <summary>
        /// 获取OCR分析结果
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetOCRResultByToken(string token, byte[] image)
        {
            string result = "";
            try
            {
                string apiName = "ocr";
                OCRRequest ocrRequest = new OCRRequest();
                ocrRequest.image = System.Convert.ToBase64String(image);
                string json = JsonConvert.SerializeObject(ocrRequest);
                result = HttpHelper.HttpUrlSend(apiName, "POST", json, token);
            }
            catch (Exception ex)
            { }
            return result;
        }
    }
}
