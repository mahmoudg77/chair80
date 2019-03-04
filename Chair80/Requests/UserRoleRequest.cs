using Chair80.DAL;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class UserRoleRequests
    {
        public int id { get; set; }
        public string email { get; set; }
        public List<DAL.sec_users_roles> roles { get; set; }


    }
}