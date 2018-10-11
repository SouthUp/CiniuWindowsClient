using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperateExcelClient.Model;

namespace OperateExcelClient
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
            { }
            return resultToken;
        }
        /// <summary>
        /// 导入类目数据
        /// </summary>
        /// <param name="info"></param>
        public void ImpWordsCategoryData(WordsCategoryInfo info, string token)
        {
            try
            {
                string apiName = "words/type";
                string json = JsonConvert.SerializeObject(info);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json, token);
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 导入条款
        /// </summary>
        /// <param name="info"></param>
        public void ImpLawClauseData(LawClauseInfo info,string token)
        {
            try
            {
                string apiName = "words/lawclause";
                string json = JsonConvert.SerializeObject(info);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json, token);
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 导入词
        /// </summary>
        /// <param name="info"></param>
        public void ImpWordsData(WordsInfo info, string token)
        {
            try
            {
                string apiName = "words/word";
                string json = JsonConvert.SerializeObject(info);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json, token);
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 导入关系
        /// </summary>
        /// <param name="info"></param>
        public void ImpWordsRelationData(WordsRelationInfo info, string token)
        {
            try
            {
                string apiName = "words/relation";
                string json = JsonConvert.SerializeObject(info);
                string resultStr = HttpHelper.HttpUrlSend(apiName, "POST", json, token);
            }
            catch (Exception ex)
            { }
        }
    }
}
