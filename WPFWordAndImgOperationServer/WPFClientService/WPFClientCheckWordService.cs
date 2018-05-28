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
            if(SystemVar.IsLoginIn)
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
        /// <summary>
        ///  检查是否是违禁词
        /// </summary>
        public string CheckOneWord(CheckWordRequestInfo info)
        {
            CheckOnlyWordResponseResult result = new CheckOnlyWordResponseResult();
            if (SystemVar.IsLoginIn)
            {
                try
                {
                    var infoWord = CheckWordHelper.GetUnChekedWordInfo(info.Text);
                    result.Result = true;
                    result.WordInfo = infoWord;
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
        /// <summary>
        ///  获取替换词
        /// </summary>
        public string GetReplaceWord(ReplaceWordRequestInfo info)
        {
            ReplaceWordResponseResult result = new ReplaceWordResponseResult();
            if (SystemVar.IsLoginIn)
            {
                try
                {
                    var listReplaceWord = CheckWordHelper.GetReplaceWordInfos(info.Text).ToList();
                    result.Result = true;
                    result.ReplaceWordModels = listReplaceWord;
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
    }
}
