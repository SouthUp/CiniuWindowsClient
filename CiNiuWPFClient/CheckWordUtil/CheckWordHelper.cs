using CheckWordModel;
using CheckWordModel.Communication;
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
        public static List<WordModel> WordModels = new List<WordModel>();
        /// <summary>
        /// 获取所有校验数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static void GetAllCheckWordByToken(string token)
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
                string resultStr = HttpHelper.HttpUrlGet(apiName, "GET", token);
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
                    }
                    try
                    {
                        CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                        commonExchangeInfo.Code = "HideNotifyMessageView";
                        commonExchangeInfo.Data = "4003";
                        string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                        Win32Helper.SendMessage("WordAndImgOperationApp", jsonData);
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
                        Win32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                wordModelLists = new List<WordModel>();
                WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                try
                {
                    CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                    commonExchangeInfo.Code = "ShowNotifyMessageView";
                    commonExchangeInfo.Data = "4003";
                    string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                    Win32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                }
                catch
                { }
            }
            WordModels = wordModelLists;
            new Task(() => {
                try
                {
                    string myWordModelsInfo = string.Format(@"{0}\MyWordModelsInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    //保存用户设置信息到本地
                    DataParse.WriteToXmlPath(JsonConvert.SerializeObject(WordModels), myWordModelsInfo);
                }
                catch (Exception ex)
                {
                    WPFClientCheckWordUtil.Log.TextLog.SaveError(ex.Message);
                }
            }).Start();
        }
        public static List<UnChekedWordInfo> GetUnChekedWordInfoList(string text)
        {
            List<UnChekedWordInfo> result = new List<UnChekedWordInfo>();
            try
            {
                try
                {
                    if (WordModels.Count == 0 && !string.IsNullOrEmpty(UtilSystemVar.UserToken))
                    {
                        GetAllCheckWordByToken(UtilSystemVar.UserToken);
                    }
                }
                catch (Exception ex)
                { }
                foreach (var item in WordModels)
                {
                    if (text.Contains(item.Name))
                    {
                        var defaultObj = result.FirstOrDefault(x => x.Name == item.Name);
                        if (text.Contains(item.Name) && defaultObj == null)
                        {
                            UnChekedWordInfo unChekedWordInfo = new UnChekedWordInfo();
                            unChekedWordInfo.ID = item.ID;
                            unChekedWordInfo.Name = item.Name;
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
    }
}
