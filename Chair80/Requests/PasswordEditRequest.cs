using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80.Requests
{
    public class PasswordEditRequest : APIRequest
    {
        public string current { get; set; }
        public string password { get; set; }
        public string cpassword { get; set; }
     }
}