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
        /// 获取OCR分析结果
        /// </summary>
        /// <returns></returns>
        public string GetOCRResultByToken(byte[] image,string fileName, string taskId = "")
        {
            string result = "";
            try
            {
                string token = "";
                string urlStr = "";
                try
                {
                    string loginInOutInfos = string.Format(@"{0}\LoginInOutInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                    if (ui != null && ui.ToString() != "")
                    {
                        try
                        {
                            var loginInOutInfo = JsonConvert.DeserializeObject<LoginInOutInfo>(ui.ToString());
                            if (loginInOutInfo != null && loginInOutInfo.Type == "LoginIn")
                            {
                                token = loginInOutInfo.Token;
                                urlStr = loginInOutInfo.UrlStr;
                            }
                        }
                        catch(Exception ex)
                        {
                            CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                }
                string url = urlStr + "ocr";
                OCRRequest ocrRequest = new OCRRequest();
                ocrRequest.image = System.Convert.ToBase64String(image);
                ocrRequest.fileName = fileName;
                ocrRequest.taskId = taskId;
                string json = JsonConvert.SerializeObject(ocrRequest);
                result = HttpHelper.HttpUrlSend(url, "POST", json, token);
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 获取会员状态
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool GetUserStateByToken()
        {
            bool result = false;
            try
            {
                string token = "";
                string urlStr = "";
                try
                {
                    string loginInOutInfos = string.Format(@"{0}\LoginInOutInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                    if (ui != null && ui.ToString() != "")
                    {
                        try
                        {
                            var loginInOutInfo = JsonConvert.DeserializeObject<LoginInOutInfo>(ui.ToString());
                            if (loginInOutInfo != null && loginInOutInfo.Type == "LoginIn")
                            {
                                token = loginInOutInfo.Token;
                                urlStr = loginInOutInfo.UrlStr;
                            }
                        }
                        catch (Exception ex)
                        {
                            CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                }
                string url = urlStr + "user";
                string resultStr = HttpHelper.HttpUrlGet(url, "GET", token);
                UserStateResponse resultInfo = JsonConvert.DeserializeObject<UserStateResponse>(resultStr);
                if (resultInfo != null)
                {
                    if(resultInfo.count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 获取插件状态
        /// </summary>
        /// <returns></returns>
        public bool GetCurrentAddIn(string type)
        {
            bool result = true;
            try
            {
                string fileName = type + "AddInStateInfo.xml";
                string addInStateInfos = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\" + fileName);
                var ui = CheckWordUtil.DataParse.ReadFromXmlPath<string>(addInStateInfos);
                if (ui != null && ui.ToString() != "")
                {
                    try
                    {
                        var addInStateInfo = JsonConvert.DeserializeObject<AddInStateInfo>(ui.ToString());
                        if (addInStateInfo != null)
                        {
                            result = addInStateInfo.IsOpen;
                        }
                    }
                    catch (Exception ex)
                    {
                        CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return result;
        }
    }
}
