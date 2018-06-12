using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckWordModel
{
    public class ImgGeneralInfo
    {
        public int words_result_num { get; set; }
        public List<ImgGeneralDetailInfo> words_result { get; set; }
        public Int64 log_id { get; set; }
    }
    public class ImgGeneralDetailInfo
    {
        public List<VertexesLocationInfo> vertexes_location { get; set; }
        public LocationInfo  location { get; set; }
        public string words { get; set; }
        public List<CharsInfo> Chars { get; set; }
    }
    public class VertexesLocationInfo
    {
        public int x { get; set; }
        public int y { get; set; }
    }
    public class LocationInfo
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    public class CharsInfo
    {
        public LocationInfo location { get; set; }
        public string Char { get; set; }
    }
}
