using Chari80Admin.DAL;
using Chari80Admin.DAL;
using Chari80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80Admin.Responses
{
   
    public class LoginResponse
    {
        public tbl_accounts account { get; set; }
        public Guid token { get; set; }
        public string[] roles { get; set; }

    }
}