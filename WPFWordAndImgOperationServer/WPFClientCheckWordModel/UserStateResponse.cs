﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class UserStateResponse
    {
        public int points { get; set; }
        public List<UserStateInfo> roles { get; set; }
        public int count { get; set; }
        public int countWord { get; set; }
        public bool vip { get; set; }
    }
}
