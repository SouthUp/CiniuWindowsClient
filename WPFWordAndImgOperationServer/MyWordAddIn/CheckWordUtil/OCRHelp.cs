using System.Collections.Generic;
using System.Text;
using Baidu.Aip;
using Newtonsoft.Json.Linq;

namespace CheckWordUtil
{
    /// <summary>
    /// 文字识别
    /// </summary>
    public class OCR : AipServiceBase
    {
        private const string AccurateURL =
            "https://aip.baidubce.com/rest/2.0/ocr/v1/";
        public OCR(string apiKey, string secretKey) : base(apiKey, secretKey)
        {

        }
        protected AipHttpRequest DefaultRequest(string uri)
        {
            return new AipHttpRequest(uri)
            {
                Method = "POST",
                BodyType = AipHttpRequest.BodyFormat.Formed,
                ContentEncoding = Encoding.UTF8
            };
        }

        /// <summary>
        /// 通用文字识别（高精度，含位置信息版）
        /// </summary>
        /// <param name="image">二进制图像数据</param>
        /// <param name="options"> 可选参数对象，key: value都为string类型，可选的参数包括 </param>
        /// <return>JObject</return>
        public JObject Accurate(string apiName, byte[] image, Dictionary<string, object> options = null)
        {
            if (apiName != "general")
            {
                apiName = "accurate";
            }
            var aipReq = DefaultRequest(AccurateURL + apiName);
            
            CheckNotNull(image, "image");
            aipReq.Bodys["image"] = System.Convert.ToBase64String(image);
            PreAction();
            if (options != null)
                foreach (var pair in options)
                    aipReq.Bodys[pair.Key] = pair.Value;
            return PostAction(aipReq);
        }
    }
}