using WPFClientCheckWordModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using CheckWordModel.Communication;

namespace WPFClientCheckWordUtil
{
    public class CheckWordHelper
    {
        public static List<WordModel> WordModels = new List<WordModel>();
        /// <summary>
        /// 获取所有校验数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static List<WordModel> GetAllCheckWordByToken(string token)
        {
            WordModels = new List<WordModel>();
            try
            {
                #region 假数据
                WordModels.Add(new WordModel { ID = "1", Name = "第一", IsCustumCi = true, IsMinGanCi = false, UserName = "18310330593", Discriptions = new List<CommonNameModel> { new CommonNameModel { discription = "违反广告法第五条" } } });
                WordModels.Add(new WordModel { ID = "2", Name = "最", IsCustumCi = false, IsMinGanCi = false, UserName = "admin", Discriptions = new List<CommonNameModel> { new CommonNameModel { discription = "违反广告法第四条" } } });
                WordModels.Add(new WordModel { ID = "3", Name = "冠军", IsCustumCi = false, IsMinGanCi = false, UserName = "admin", Discriptions = new List<CommonNameModel> { new CommonNameModel { discription = "违反广告法第三条" } } });
                #endregion
                //string apiName = "word";
                //string resultStr = HttpHelper.HttpUrlSend(apiName, "GET", token);
                //GetAllWordsInfoResponse resultInfo = JsonConvert.DeserializeObject<GetAllWordsInfoResponse>(resultStr);
                //var listDBWords = resultInfo.data;
                //if (listDBWords != null)
                //{
                //    foreach (var item in listDBWords)
                //    {
                //        WordModel word = new WordModel();
                //        word.ID = item.code;
                //        word.Name = item.name;
                //        word.IsCustumCi = item.iscustumci;
                //        word.IsMinGanCi = item.isminganci;
                //        word.UserName = item.username;
                //        word.Discriptions = item.discriptions;
                //        WordModels.Add(word);
                //    }
                //}
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                WordModels = new List<WordModel>();
            }
            if (WordModels.Count > 0)
            {
                try
                {
                    CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                    commonExchangeInfo.Code = "HideNotifyMessageView";
                    commonExchangeInfo.Data = "4003";
                    string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                    WPFClientCheckWordUtilWin32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                }
                catch
                { }
            }
            else
            {
                try
                {
                    CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                    commonExchangeInfo.Code = "ShowNotifyMessageView";
                    commonExchangeInfo.Data = "4003";
                    string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                    WPFClientCheckWordUtilWin32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                }
                catch
                { }
            }
            return WordModels;
        }
        /// <summary>
        /// 获取文本中包含的违禁词集合
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<WordModel> GetUnChekedWordInfoList(string text)
        {
            List<WordModel> result = new List<WordModel>();
            try
            {
                if (WordModels.Count == 0 && !string.IsNullOrEmpty(SystemVar.UserToken))
                {
                    WordModels = CheckWordHelper.GetAllCheckWordByToken(SystemVar.UserToken);
                }
            }
            catch (Exception ex)
            { }
            try
            {
                foreach (var item in WordModels)
                {
                    if (text.Contains(item.Name))
                    {
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            { }
            return result;
        }
    }
    public class HttpHelper
    {
        public static string HttpUrlSend(string apiName, string method, string token = "")
        {
            string urlStr = SystemVar.UrlStr + apiName;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlStr);
            req.Method = method;
            if (!string.IsNullOrEmpty(token))
            {
                req.Headers.Add("X-LC-Session", token);
            }
            req.ContentType = "application/json;charset=UTF-8";
            req.Timeout = 5000;
            try
            {
                using (WebResponse res = req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                    {
                        string strResult = sr.ReadToEnd();
                        return strResult;
                    }
                }
            }
            catch (WebException ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError("状态码：" + ex.Status + "异常信息:" + ex.Message);
                return ex.Message;
            }
        }
    }
}
