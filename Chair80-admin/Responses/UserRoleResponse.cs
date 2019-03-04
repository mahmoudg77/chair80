using Chair80Admin.DAL;
using Chair80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80Admin.Responses
{
    public class UserRoleResponse
    {
		 
        public int id { get; set; }
        public string email { get; set; }
        public List<DAL.sec_users_roles> roles { get; set; }


    }
}