using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO.Ports;
using System.Xml;
using IWPFClientService;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using WPFClientCheckWordModel;
using WPFClientCheckWordUtil;

namespace WPFClientService
{
    /// <summary>
    ///  检查违禁词服务类
    /// </summary>
    public class WPFClientCheckWordService : IWPFClientCheckWordService
    {
        /// <summary>
        ///  检查违禁词
        /// </summary>
        public string CheckWord(CheckWordRequestInfo info)
        {
            CheckWordResponseResult result = new CheckWordResponseResult();
            if (IsUserLogin())
            {
                try
                {
                    var listUnChekedWord = CheckWordHelper.GetUnChekedWordInfoList(info.Text).ToList();
                    result.Result = true;
                    result.UncheckWordModels = listUnChekedWord;
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }
            }
            else
            {
                result.Message = "LoginOut";
            }
            return JsonConvert.SerializeObject(result);
        }
        private static bool IsUserLogin()
        {
            bool result = false;
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
                            SystemVar.UserToken = loginInOutInfo.Token;
                            result = true;
                        }
                    }
                    catch(Exception ex)
                    {
                        WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                    }
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
