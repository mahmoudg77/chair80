using Chari80Admin.DAL;
using Chari80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80Admin.Responses
{
    public class CountryResponse
    {
       public int id { get; set; }
       public List<trans> name { get; set; }

    }
}