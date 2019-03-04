using Chair80.DAL;
using Chair80.DAL;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Responses
{
   
    public class LoginResponse
    {
        public tbl_accounts account { get; set; }
        public Guid token { get; set; }
        public string[] roles { get; set; }

    }
}