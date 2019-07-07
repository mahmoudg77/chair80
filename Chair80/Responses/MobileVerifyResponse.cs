using Chair80.DAL;
using Chair80.DAL;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Responses
{
   
    public class MobileVerifyResponse
    {
        public bool is_verified { get; set; }
        public Guid verification_id { get; set; }

    }
}