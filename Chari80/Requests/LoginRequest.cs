using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80.Requests
{
    public class LoginRequest: APIRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public Nullable<bool> remember { get; set; }
    }
}