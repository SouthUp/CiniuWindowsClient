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
                            unChekedWordInfo.IsCustumCi = item.IsCustumCi;
                            unChekedWordInfo.IsMinGanCi = item.IsMinGanCi;
                            unChekedWordInfo.UserName = item.UserName;
                            foreach (var discriptionInfo in item.Discriptions)
                            {
                                UnChekedDetailWordInfo unChekedDetailWordInfo = new UnChekedDetailWordInfo();
                                unChekedDetailWordInfo.Discription = discriptionInfo.discription;
                                unChekedWordInfo.UnChekedWordDetailInfos.Add(unChekedDetailWordInfo);
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
        /// <summary>
        /// 获取所有验证不通过区域集合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Rect> GetUnValidRects(List<WordInfo> list)
        {
            List<Rect> result = new List<Rect>();
            try
            {
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        List<int> subIndex = GetStrIndexsFromAllText(item);
                        foreach (var index in subIndex)
                        {
                            Rect rect = new Rect();
                            rect.X = item.Rects[index].X - 2;
                            rect.Y = item.Rects[index].Y - 2;
                            double widthRect = 0;
                            double heightRect = 0;
                            widthRect = item.Rects[index + item.UnValidText.Length - 1].Width + item.Rects[index + item.UnValidText.Length - 1].X - item.Rects[index].X;
                            for (int i = 0; i < item.UnValidText.Length; i++)
                            {
                                if (item.Rects[i].Height > heightRect)
                                {
                                    heightRect = item.Rects[i].Height;
                                }
                            }
                            rect.Width = widthRect + 4;
                            rect.Height = heightRect + 4;
                            result.Add(rect);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>
        /// 获取特定字符串在整个字符串位置集合
        /// </summary>
        /// <param name="wordInfo"></param>
        /// <returns></returns>
        private static List<int> GetStrIndexsFromAllText(WordInfo wordInfo)
        {
            List<int> subIndex = new List<int>();
            try
            {
                if (wordInfo != null && !string.IsNullOrEmpty(wordInfo.AllText) && !string.IsNullOrEmpty(wordInfo.UnValidText))
                {
                    int ii = wordInfo.AllText.IndexOf(wordInfo.UnValidText);
                    while (ii >= 0 && ii < wordInfo.AllText.Length)
                    {
                        subIndex.Add(ii);
                        ii = wordInfo.AllText.IndexOf(wordInfo.UnValidText, ii + 1);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return subIndex;
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
