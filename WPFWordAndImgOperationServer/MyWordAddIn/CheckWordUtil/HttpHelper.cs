using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckWordUtil
{
    public class HttpHelper
    {
        public static string UrlStr = ConfigurationManager.AppSettings["UrlStr"].ToString();
        public static string HttpUrlSend(string apiName, string method, string json, string token = "")
        {
            string urlStr = UrlStr + apiName;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlStr);
            req.Method = method;
            byte[] postBytes = Encoding.UTF8.GetBytes(json);
            req.ContentType = "application/json;charset=UTF-8";
            req.ContentLength = postBytes.Length;
            if (!string.IsNullOrEmpty(token))
            {
                req.Headers.Add("X-LC-Session", token);
            }
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
                        return strResult;
                    }
                }
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
        }
        public static string HttpUrlGet(string apiName, string method, string token = "")
        {
            string urlStr = UrlStr + apiName;
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
                return ex.Message;
            }
        }
    }
}
