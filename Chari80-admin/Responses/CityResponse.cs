﻿using Chari80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chari80.Responses
{
    public class CityResponse
    {
        public int id { get; set; }
        public int country_id { get; set; }

        public List<trans> name { get; set; }

    }
}