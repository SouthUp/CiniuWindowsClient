using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordUtil
{
    public sealed class HashHelper
    {
        /// <summary>
        ///  计算指定文件的SHA1值
        /// </summary>
        /// <param name="fileName">指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public static String ComputeSHA1(String fileName)
        {
            String hashSHA1 = String.Empty;
            try
            {
                //检查文件是否存在，如果文件存在则进行计算，否则返回空值
                if (System.IO.File.Exists(fileName))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        //计算文件的SHA1值
                        System.Security.Cryptography.SHA1 calculator = System.Security.Cryptography.SHA1.Create();
                        Byte[] buffer = calculator.ComputeHash(fs);
                        calculator.Clear();
                        //将字节数组转换成十六进制的字符串形式
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            stringBuilder.Append(buffer[i].ToString("x2"));
                        }
                        hashSHA1 = stringBuilder.ToString();
                    }//关闭文件流
                }
            }
            catch (Exception ex)
            { }
            return hashSHA1;
        }
        /// <summary>
        ///  计算指定Str的SHA1值
        /// </summary>
        /// <param name="str">待计算的字符串</param>
        /// <returns>返回值的字符串形式</returns>
        public static String ComputeSHA1ByStr(string str)
        {
            String hashSHA1 = String.Empty;
            try
            {
                //计算文件的SHA1值
                System.Security.Cryptography.SHA1 calculator = System.Security.Cryptography.SHA1.Create();
                Byte[] buffer = calculator.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
                calculator.Clear();
                //将字节数组转换成十六进制的字符串形式
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    stringBuilder.Append(buffer[i].ToString("x2"));
                }
                hashSHA1 = stringBuilder.ToString();
            }
            catch (Exception ex)
            { }
            return hashSHA1;
        }
    }
}
