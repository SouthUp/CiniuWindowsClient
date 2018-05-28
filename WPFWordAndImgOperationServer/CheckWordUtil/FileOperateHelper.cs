using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordUtil
{
    public class FileOperateHelper
    {
        /// <summary>
        /// 写入文本内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Content"></param>
        public static void WriteTxt(string path, string Content)
        {
            try
            {
                if (!File.Exists(path))
                {
                    StreamWriter sw = File.CreateText(path);
                    sw.WriteLine(Content);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    return;
                }
                else
                {
                    FileStream fs = new FileStream(path, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(Content);
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 读取文本内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> ReadTxt(string path)
        {
            List<string> result = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!result.Contains(line.ToString()))
                        result.Add(line.ToString());
                }
            }
            catch (Exception ex)
            { }
            return result;
        }
        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            try
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);//直接删除其中的文件 
                    }
                    else
                    {
                        DeleteFolder(d);////递归删除子文件夹
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// C#按创建时间排序（顺序）
        /// </summary>
        /// <param name="arrFi">待排序数组</param>
        public static void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });
        }
    }
}
