using Chari80.DAL;
using Chari80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80.Requests
{
    public class UserRoleRequests
    {
        public int id { get; set; }
        public string email { get; set; }
        public List<DAL.sec_users_roles> roles { get; set; }


    }
}