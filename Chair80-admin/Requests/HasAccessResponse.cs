using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80Admin.Requests
{
    public class HasAccessResponse
    {
        public string Screen { get; set; }
        public string Method { get; set; }
        public bool Allow { get; set; }
    }
}