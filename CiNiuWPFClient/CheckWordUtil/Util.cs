using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static BitmapImage GetBitmapImageForBackUp(string picPath)
        {
            BitmapImage image = new BitmapImage();
            try
            {
                if(!System.IO.Path.GetExtension(picPath).ToLower().Contains("doc"))
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
            }
            catch (Exception ex)
            { }
            return image;
        }
    }
}
