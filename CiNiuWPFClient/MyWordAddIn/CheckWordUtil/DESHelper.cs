using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordUtil
{
    public class DESHelper
    {
        //DES加密秘钥，要求为8位  
        private const string desKey = "CiNiu111";
        //默认密钥向量  
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>  
        /// DES加密  
        /// </summary>  
        /// <param name="encryptString">待加密的字符串，未加密成功返回原串</param>  
        /// <returns></returns>  
        public string EncryptString(string encryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(desKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch(Exception ex)
            {
                return encryptString;
            }
        }
        /// <summary>  
        /// DES解密  
        /// </summary>  
        /// <param name="decryptString">待解密的字符串，未解密成功返回原串</param>  
        /// <returns></returns>
        public string DecryptString(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(desKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch(Exception ex)
            {
                return decryptString;
            }
        }
    }
}
