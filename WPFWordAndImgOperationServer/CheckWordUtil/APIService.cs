using CheckWordModel;
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
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
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
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 获取会员状态
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserStateInfos GetUserStateByToken(string token)
        {
            UserStateInfos result = null;
            try
            {
                string apiName = "user";
                string resultStr = HttpHelper.HttpUrlGet(apiName, "GET", token);
                UserStateResponse resultInfo = JsonConvert.DeserializeObject<UserStateResponse>(resultStr);
                if (resultInfo != null)
                {
                    result = new UserStateInfos();
                    result.PointCount = resultInfo.points;
                    result.PicCount = resultInfo.count;
                    result.WordCount = resultInfo.countWord;
                    //////result.Active = resultInfo.vip;
                    //////result.ActiveName = result.Active ? "已购买" : "未购买";
                    //////if (resultInfo.roles != null && resultInfo.roles.Count > 0)
                    //////{
                    //////    foreach (var info in resultInfo.roles)
                    //////    {
                    //////        if (info.expiryTime != null)
                    //////        {
                    //////            if (result.ExpiredDate == null)
                    //////            {
                    //////                result.ExpiredDate = info.expiryTime;
                    //////            }
                    //////            else
                    //////            {
                    //////                if (DateTime.Compare(info.expiryTime, result.ExpiredDate) > 0)
                    //////                {
                    //////                    result.ExpiredDate = info.expiryTime;
                    //////                }
                    //////            }
                    //////        }
                    //////    }
                    //////}
                    //////if (result.ExpiredDate != null)
                    //////{
                    //////    result.ExpiredDateStr = result.ExpiredDate.ToString("yyyy-MM-dd");
                    //////}
                }
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                result = null;
            }
            return result;
        }
        public string GetVersion()
        {
            string version = "";
            try
            {
                string apiName = "version";
                string resultStr = HttpHelper.HttpUrlGet(apiName, "GET");
                VersionResponse resultInfo = JsonConvert.DeserializeObject<VersionResponse>(resultStr);
                if (resultInfo != null)
                {
                    version = resultInfo.version;
                }
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return version;
        }
        public ConsumeResponse GetWordConsume(int count, string token)
        {
            ConsumeResponse result = null;
            try
            {
                string apiName = "consume";
                ConsumeRequest consumeRequest = new ConsumeRequest();
                consumeRequest.count = count;
                string json = JsonConvert.SerializeObject(consumeRequest);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json, token);
                ConsumeResponse resultInfo = JsonConvert.DeserializeObject<ConsumeResponse>(resultStr);
                if (resultInfo != null)
                {
                    result = resultInfo;
                }
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return result;
        }
    }
}
