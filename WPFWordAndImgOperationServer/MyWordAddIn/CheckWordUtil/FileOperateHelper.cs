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
            catch (Exception)
            { }
        }
    }
}
