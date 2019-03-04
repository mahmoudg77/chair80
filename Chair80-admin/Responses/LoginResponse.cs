using Chair80Admin.DAL;
using Chair80Admin.DAL;
using Chair80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80Admin.Responses
{
   
    public class LoginResponse
    {
        public tbl_accounts account { get; set; }
        public Guid token { get; set; }
        public string[] roles { get; set; }

    }
}