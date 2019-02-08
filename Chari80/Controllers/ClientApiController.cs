using Chari80.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chari80.Controllers
{
    public abstract class ClientApiController<T> : ApiController where T:class
    {
        
    }
}
