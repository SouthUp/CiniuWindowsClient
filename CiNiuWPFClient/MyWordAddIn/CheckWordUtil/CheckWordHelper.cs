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
        public static List<WordModel> WordModels = new List<WordModel>();
        /// <summary>
        /// 获取文本中包含的违禁词集合
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<UnChekedWordInfo> GetUnChekedWordInfoList(string text, string typeInfo = "Word")
        {
            List<UnChekedWordInfo> result = new List<UnChekedWordInfo>();
            try
            {
                bool isGetAllWord = true;
                string isGetAllWordsInfo = "";
                if (typeInfo == "Word")
                {
                    isGetAllWordsInfo = string.Format(@"{0}\IsWordGetAllWordsInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                }
                else
                {
                    isGetAllWordsInfo = string.Format(@"{0}\IsExcelGetAllWordsInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                }
                var ui = DataParse.ReadFromXmlPath<string>(isGetAllWordsInfo);
                if (ui != null && ui.ToString() != "")
                {
                    try
                    {
                        var isGetAllWords = JsonConvert.DeserializeObject<IsGetAllWordsInfo>(ui.ToString());
                        if (isGetAllWords != null)
                        {
                            isGetAllWord = isGetAllWords.IsGetAllWords;
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                if (isGetAllWord || WordModels.Count == 0)
                {
                    string myWordModelsInfo = string.Format(@"{0}\MyWordModelsInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    var uiMyWords = CheckWordUtil.DataParse.ReadFromXmlPath<string>(myWordModelsInfo);
                    if (uiMyWords != null && uiMyWords.ToString() != "")
                    {
                        try
                        {
                            WordModels = JsonConvert.DeserializeObject<List<WordModel>>(uiMyWords.ToString());
                            IsGetAllWordsInfo info = new IsGetAllWordsInfo();
                            info.IsGetAllWords = false;
                            DataParse.WriteToXmlPath(JsonConvert.SerializeObject(info), isGetAllWordsInfo);
                        }
                        catch
                        { }
                    }
                }
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
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
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
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
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
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return subIndex;
        }
    }
}
