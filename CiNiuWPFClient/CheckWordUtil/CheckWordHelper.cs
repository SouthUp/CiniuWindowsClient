using CheckWordModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WPFClientCheckWordModel;

namespace CheckWordUtil
{
    public class CheckWordHelper
    {
        public static List<UnChekedWordInfo> GetUnChekedWordInfoList(string text)
        {
            List<UnChekedWordInfo> result = new List<UnChekedWordInfo>();
            try
            {
                CheckWordRequestInfo info = new CheckWordRequestInfo() { Text = text };
                string json = JsonConvert.SerializeObject(info);
                string resultStr = PostSend("http://localhost:8888/WPFClientCheckWordService/CheckWord", json);
                CheckWordResponseResult resultInfo = JsonConvert.DeserializeObject<CheckWordResponseResult>(resultStr);
                if (resultInfo != null && resultInfo.Result && resultInfo.UncheckWordModels != null)
                {
                    foreach (var item in resultInfo.UncheckWordModels)
                    {
                        var defaultObj = result.FirstOrDefault(x => x.Name == item.Name);
                        if (text.Contains(item.Name) && defaultObj == null)
                        {
                            UnChekedWordInfo unChekedWordInfo = new UnChekedWordInfo();
                            unChekedWordInfo.ID = item.ID;
                            unChekedWordInfo.Name = item.Name;
                            foreach (var dbInfo in item.SourceDBs)
                            {
                                foreach (var typeInfo in item.NameTypes)
                                {
                                    UnChekedDetailWordInfo unChekedDetailWordInfo = new UnChekedDetailWordInfo();
                                    unChekedDetailWordInfo.Name = item.Name;
                                    unChekedDetailWordInfo.SourceDB = dbInfo.name;
                                    unChekedDetailWordInfo.NameType = typeInfo.name;
                                    unChekedDetailWordInfo.SourceDBID = dbInfo.code;
                                    unChekedDetailWordInfo.SourceDBPublishtime = dbInfo.publishtime;
                                    unChekedWordInfo.UnChekedWordDetailInfos.Add(unChekedDetailWordInfo);
                                }
                            }
                            result.Add(unChekedWordInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return result;
        }
        public static string PostSend(string url, string json)
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(json);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json;charset=UTF-8";
            req.ContentLength = postBytes.Length;
            try
            {
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }
                using (WebResponse res = req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                    {
                        string strResult = sr.ReadToEnd();
                        strResult = strResult.Replace("\\\"", "\"");
                        if (strResult.Substring(0, 1) == "\"")
                        {
                            string strRegex = @"^(" + "\"" + ")";
                            strResult = Regex.Replace(strResult, strRegex, "");
                        }
                        if (strResult.Substring(strResult.Length - 1, 1) == "\"")
                        {
                            string strRegex2 = @"(" + "\"" + ")" + "$";
                            strResult = Regex.Replace(strResult, strRegex2, "");
                        }
                        return strResult;
                    }
                }
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
        }
    }
}
