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
            List<WordModel> wordModelLists = new List<WordModel>();
            try
            {
                //#region 假数据
                //WordModels.Add(new WordModel { ID = "1", Name = "第一", IsCustumCi = true });
                //WordModels.Add(new WordModel { ID = "2", Name = "最", IsCustumCi = false });
                //WordModels.Add(new WordModel { ID = "3", Name = "冠军", IsCustumCi = false });
                //WordModels.Add(new WordModel { ID = "4", Name = "防晒", IsCustumCi = true });
                //#endregion
                string apiName = "words/word";
                string resultStr = HttpHelper.HttpUrlSend(apiName, "GET", token);
                CommonResponse resultInfo = JsonConvert.DeserializeObject<CommonResponse>(resultStr);
                if (resultInfo != null && resultInfo.state)
                {
                    List<DBWordModel> listDBWords = JsonConvert.DeserializeObject<List<DBWordModel>>(resultInfo.result);
                    if (listDBWords != null)
                    {
                        foreach (var item in listDBWords)
                        {
                            WordModel word = new WordModel();
                            word.ID = item.id;
                            word.Name = item.name;
                            wordModelLists.Add(word);
                        }
                        WordModels = wordModelLists;
                    }
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
            }
            catch (Exception ex)
            {
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
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
