using Chari80.DAL;
using Chari80.DAL;
using Chari80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80.Responses
{
   
    public class LoginResponse
    {
        public tbl_accounts account { get; set; }
        public Guid token { get; set; }
        public string[] roles { get; set; }

    }
}