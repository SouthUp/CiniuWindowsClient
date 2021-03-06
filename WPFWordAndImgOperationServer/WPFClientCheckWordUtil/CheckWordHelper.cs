﻿using WPFClientCheckWordModel;
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
        public static List<ReplaceWordModel> ReplaceWordModels = new List<ReplaceWordModel>();
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
                string apiName = "word";
                string resultStr = HttpHelper.HttpUrlSend(apiName, "GET", token);
                GetAllWordsInfoResponse resultInfo = JsonConvert.DeserializeObject<GetAllWordsInfoResponse>(resultStr);
                var listDBWords = resultInfo.data;
                if (listDBWords != null)
                {
                    foreach (var item in listDBWords)
                    {
                        WordModel word = new WordModel();
                        word.ID = item.code;
                        word.Name = item.name;
                        word.SourceDBs = item.type;
                        if (word.SourceDBs != null && word.SourceDBs.Count > 0)
                        {
                            word.SourceDB = word.SourceDBs.First().name;
                        }
                        word.NameTypes = item.category;
                        if (word.NameTypes != null && word.NameTypes.Count > 0)
                        {
                            word.NameType = word.NameTypes.First().name;
                        }
                        WordModels.Add(word);
                    }
                }
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
        /// 获取所有校验数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<WordModel> GetAllCheckWord(string path)
        {
            WordModels = new List<WordModel>();
            try
            {
                XDocument xdoc = XDocument.Load(path);
                var dataInfo = from query in xdoc.Descendants("AreaList")
                               select new WordModel
                               {
                                   ID = (string)query.Element("ID"),
                                   Name = (string)query.Element("Name")
                               };
                return dataInfo.ToList();
            }
            catch (Exception ex)
            {
                return new List<WordModel>();
            }
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
        /// <summary>
        /// 获取text是不是违禁词
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static WordModel GetUnChekedWordInfo(string word)
        {
            WordModel result = null;
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
                result = WordModels.FirstOrDefault(x =>x.Name == word);
            }
            catch (Exception ex)
            { }
            return result;
        }
        /// <summary>
        /// 获取所有替换词数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<ReplaceWordModel> GetReplaceWords(string path)
        {
            ReplaceWordModels = new List<ReplaceWordModel>();
            try
            {
                XDocument xdoc = XDocument.Load(path);
                var dataInfo = from query in xdoc.Descendants("AreaList")
                               select new ReplaceWordModel
                               {
                                   ID = (string)query.Element("ID"),
                                   ReplaceWordInfos = (from queryRaplace in query.Element("RaplaceList").Descendants("Name")
                                                       select new ReplaceWordInfo
                                                       {
                                                           Name = (string)queryRaplace.Value
                                                       }).ToList()
                               };
                return dataInfo.ToList();
            }
            catch (Exception ex)
            {
                return new List<ReplaceWordModel>();
            }
        }
        /// <summary>
        /// 获取违禁词的同义词
        /// </summary>
        /// <param name="name">违禁词</param>
        /// <returns></returns>
        public static List<ReplaceWordInfo> GetReplaceWordInfos(string name)
        {
            name = name.Replace("\r", "").Replace("\n", ""); ;
            List<ReplaceWordInfo> result = new List<ReplaceWordInfo>();
            try
            {
                var item = WordModels.FirstOrDefault(x => x.Name == name);
                if (item != null)
                {
                    result = ReplaceWordModels.FirstOrDefault(x => x.ID == item.ID).ReplaceWordInfos;
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
