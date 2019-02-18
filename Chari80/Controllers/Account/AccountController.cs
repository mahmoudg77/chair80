using Chari80.Filters;
using Chari80.Libs;
using Chari80.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Chari80.Controllers.Account
{
    [AppFilter]
    public class AccountController : ApiController
    {
        [LoginFilter]
        [Route("Account/actAsDriver")]
        public APIResult<bool> actAsDriver(ActAsDriverRequest request)
        {
            var v=request.isValid();
            if (v.type == 0)
            {
                return new APIResult<bool>(ResultType.fail, false, v.message);
            }

            //if(request.CurrentUser(HttpContext.Current.Request).)
            return new APIResult<bool>(ResultType.success, true);
        }
        [LoginFilter]
        [Route("Account/actAsOwner")]
        public APIResult<bool> actAsOwner()
        {
            return new APIResult<bool>(ResultType.success, true);
        }
    }
}
