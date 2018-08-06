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
        /// 用户注册
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <param name="sale"></param>
        /// <returns></returns>
        public string Register(string userName, string password, string code, string sale, out string message)
        {
            string resultToken = "";
            message = "";
            try
            {
                string apiName = "user";
                UserRegisterRequest user = new UserRegisterRequest() { username = userName, password = password, code = code };
                if (!string.IsNullOrEmpty(sale))
                {
                    user.sale = sale;
                }
                string json = JsonConvert.SerializeObject(user);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json);
                CommonResponse resultResponse = JsonConvert.DeserializeObject<CommonResponse>(resultStr);
                if (resultResponse.state)
                {
                    UserRegisterResponse resultInfo = JsonConvert.DeserializeObject<UserRegisterResponse>(resultResponse.result);
                    resultToken = resultInfo.sessionToken;
                }
                else
                {
                    message = resultResponse.message;
                }
            }
            catch (Exception ex)
            {
                message = "注册用户失败";
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return resultToken;
        }
        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string FindPsw(string userName,string password, string code, out string message)
        {
            string resultToken = "";
            message = "";
            try
            {
                string apiName = "user/password";
                UserFindPswrRequest user = new UserFindPswrRequest() { username= userName, password = password, code = code };
                string json = JsonConvert.SerializeObject(user);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json);
                CommonResponse resultResponse = JsonConvert.DeserializeObject<CommonResponse>(resultStr);
                if (resultResponse.state)
                {
                    UserFindPswrResponse resultInfo = JsonConvert.DeserializeObject<UserFindPswrResponse>(resultResponse.result);
                    resultToken = resultInfo.sessionToken;
                }
                else
                {
                    message = resultResponse.message;
                }
            }
            catch (Exception ex)
            {
                message = "密码修改失败";
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return resultToken;
        }
        /// <summary>
        /// 用户发送验证码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string RegisterSendYZM(string userName, string type = "")
        {
            string result = "";
            try
            {
                string apiName = "user/code";
                if (type == "FindPsw")
                {
                    apiName = "user/pwdcode";
                }
                SendYZMRequest sendYZMRequest = new SendYZMRequest() { username = userName };
                string json = JsonConvert.SerializeObject(sendYZMRequest);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json);
                CommonResponse resultResponse = JsonConvert.DeserializeObject<CommonResponse>(resultStr);
                result = resultResponse.message;
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                result = "发送验证码失败";
            }
            return result;
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
