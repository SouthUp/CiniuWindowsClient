﻿using Newtonsoft.Json;
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
        public static bool IsUrlExist(string URL)
        {
            try
            {
                WebRequest request = WebRequest.Create(URL);
                request.Timeout = 1000;
                WebResponse response = request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
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
                    catch
                    { }
                }
            }
            catch
            { }
            return result;
        }
    }
}
