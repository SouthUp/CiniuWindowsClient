using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientCheckWordModel
{
    public class UserRegisterRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string code { get; set; }
        public string sale { get; set; }
    }
}
