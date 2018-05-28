using WPFClientCheckWordModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPFClientCheckWordUtil
{
    public class CheckWordHelper
    {
        public static List<WordModel> WordModels = new List<WordModel>();
        public static List<ReplaceWordModel> ReplaceWordModels = new List<ReplaceWordModel>();
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
                foreach(var item in WordModels)
                {
                    var defaultObj = result.FirstOrDefault(x => x.Name == item.Name);
                    if (text.Contains(item.Name) && defaultObj == null)
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
}
