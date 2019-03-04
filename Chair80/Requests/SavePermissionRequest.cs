using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class SavePermissionRequest : APIRequest
    {
        
        public int role_id { get; set; }
        public string screen { get; set; }
        public List<string> methods { get; set; }

    }
}