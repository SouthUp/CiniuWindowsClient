using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CheckWordUtil
{
    public class DataParse
    {

        public static object ReadFromXmlPath<T>(string varRevXmlString)
        {
            try
            {
                XmlSerializer tmpObjSerializer = new XmlSerializer(typeof(T));
                Stream tmpObjStream = new FileStream(varRevXmlString, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                T tmpObj = (T)tmpObjSerializer.Deserialize(tmpObjStream);
                tmpObjStream.Close();
                return tmpObj;
            }
            catch
            {
                return null;
            }
        }

        public static void WriteToXmlPath<T>(T varObj, string varXmlPath)
        {
            try
            {
                string tmpXmlString = string.Empty;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                var path = varXmlPath.Substring(0, varXmlPath.LastIndexOf("\\", StringComparison.Ordinal));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (Stream stream = new FileStream(varXmlPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    xmlSerializer.Serialize(stream, varObj);
                    stream.Close();
                }
            }
            catch
            {

            }
        }

        public static object ReadFromXmlContent<T>(string varRevXmlString)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(varRevXmlString)))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    return (T)ser.Deserialize(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static string WriteToXmlContent<T>(T varObj)
        {
            try
            {
                string tmpXmlString = string.Empty;
                XmlSerializer tmpXmlSerializer = new XmlSerializer(typeof(T));
                using (MemoryStream tmpMemStream = new MemoryStream())
                {
                    tmpXmlSerializer.Serialize(tmpMemStream, varObj);
                    tmpXmlString = Encoding.UTF8.GetString(tmpMemStream.ToArray());
                }
                return tmpXmlString;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
