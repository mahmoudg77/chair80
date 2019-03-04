using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80Admin.Requests
{
    public class PasswordEditRequest : APIRequest
    {
        public string current { get; set; }
        public string password { get; set; }
        public string cpassword { get; set; }
     }
}