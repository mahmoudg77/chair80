﻿using Chair80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chair80Admin.Controllers
{
    public abstract class ClientApiController<T> : ApiController where T:class
    {
        
    }
}
