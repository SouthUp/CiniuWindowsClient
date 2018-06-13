using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
    }
}
