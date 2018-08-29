using CheckWordModel.Communication;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFClientCheckWordModel;

namespace CheckWordUtil
{
    public class Util
    {
        /// <summary>
        /// 保存图片到文件
        /// </summary>
        /// <param name="image">图片数据</param>
        /// <param name="filePath">保存路径</param>
        public static void SaveImageToFile(BitmapSource image, string filePath)
        {
            try
            {
                BitmapEncoder encoder = GetBitmapEncoder(filePath);
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }

        /// <summary>
        /// 根据文件扩展名获取图片编码器
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>图片编码器</returns>
        private static BitmapEncoder GetBitmapEncoder(string filePath)
        {
            var extName = Path.GetExtension(filePath).ToLower();
            if (extName.Equals(".png"))
            {
                return new PngBitmapEncoder();
            }
            else
            {
                return new JpegBitmapEncoder();
            }
        }
        public static Byte[] GetBytesByPicture(string picPath)
        {
            try
            {
                byte[] data = System.IO.File.ReadAllBytes(picPath);
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static BitmapImage GetBitmapImage(string picPath)
        {
            BitmapImage image = new BitmapImage();
            try
            {
                var bytes = GetBytesByPicture(picPath);
                if (bytes != null)
                {
                    MemoryStream byteStream = new MemoryStream(bytes);
                    image.BeginInit();
                    image.StreamSource = byteStream;
                    image.EndInit();
                }
            }
            catch (Exception ex)
            { }
            return image;
        }
        public static bool GetIsUserLogin()
        {
            bool result = false;
            try
            {
                string loginInOutInfos = string.Format(@"{0}\LoginInOutInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                var ui = DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                if (ui != null && ui.ToString() != "")
                {
                    try
                    {
                        var loginInOutInfo = JsonConvert.DeserializeObject<LoginInOutInfo>(ui.ToString());
                        if (loginInOutInfo != null && loginInOutInfo.Type == "LoginIn")
                        {
                            var proc = System.Diagnostics.Process.GetProcessesByName("WordAndImgOperationApp");
                            if (proc != null && proc.Length == 1)
                                result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        CheckWordUtil.Log.TextLog.SaveError(ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
            return result;
        }
        public static void CallWordAndImgApp()
        {
            try
            {
                var proc = System.Diagnostics.Process.GetProcessesByName("WordAndImgOperationApp");
                if (proc != null && proc.Length == 1)
                {
                    CommonExchangeInfo commonExchangeInfo = new CommonExchangeInfo();
                    commonExchangeInfo.Code = "ShowWordAndImgOperationApp";
                    string jsonData = JsonConvert.SerializeObject(commonExchangeInfo); //序列化
                    Win32Helper.SendMessage("WordAndImgOperationApp", jsonData);
                }
                else
                {
                    string appPath = "";
                    string loginInOutInfos = string.Format(@"{0}\WordAndImgAppInfo.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WordAndImgOCR\\LoginInOutInfo\\");
                    var ui = DataParse.ReadFromXmlPath<string>(loginInOutInfos);
                    if (ui != null && ui.ToString() != "")
                    {
                        try
                        {
                            var wordAndImgAppInfo = JsonConvert.DeserializeObject<WordAndImgAppInfo>(ui.ToString());
                            if (wordAndImgAppInfo != null)
                            {
                                appPath = wordAndImgAppInfo.Path;
                            }
                        }
                        catch
                        { }
                    }
                    if (!string.IsNullOrEmpty(appPath) && File.Exists(appPath))
                    {
                        var info = new System.Diagnostics.ProcessStartInfo(appPath);
                        info.WorkingDirectory = appPath.Substring(0, appPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                        System.Diagnostics.Process.Start(info);
                    }
                }
            }
            catch (Exception ex)
            {
                CheckWordUtil.Log.TextLog.SaveError(ex.Message);
            }
        }
    }
}
