﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class VersionResponse
    {
        public DateTime time { get; set; }
        public string latestClient { get; set; }
        public string minimumApi { get; set; }
        public List<string> descriptionInfos { get; set; }
    }
}
