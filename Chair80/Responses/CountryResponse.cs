using Chair80.DAL;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Responses
{
    public class CountryResponse
    {
       public int id { get; set; }
       public List<trans> name { get; set; }

    }
}