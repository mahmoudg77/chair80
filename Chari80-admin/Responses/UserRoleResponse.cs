using Chari80Admin.DAL;
using Chari80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80Admin.Responses
{
    public class UserRoleResponse
    {
		 
        public int id { get; set; }
        public string email { get; set; }
        public List<DAL.sec_users_roles> roles { get; set; }


    }
}