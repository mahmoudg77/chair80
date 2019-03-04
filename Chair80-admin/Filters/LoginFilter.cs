using Chair80Admin.BLL.Security;
using Chair80Admin.Libs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Chair80Admin.Filters
{
    public class LoginFilter : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var USER_ID = HttpContext.Current.Request.Headers.GetValues("AUTH_KEY");

            //// For Debug
            //if (actionContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "user" && actionContext.ActionDescriptor.ActionName.ToLower()=="logout")
            //{
            //    var i = "";
            //}
            bool Auth = true;
            if (USER_ID == null  )
            {
                Auth = false;
                //return;
            }
            else
            {
                string key =USER_ID.First();
                Guid token;
                if (!Guid.TryParse(key, out token))
                {
                    Auth = false;
                }
                else
                {

                    Sessions sessions = new Sessions(Guid.Parse(key));

                    if (
                        (sessions == null || sessions.Entity == null || sessions.Entity.end_time != null ||
                        sessions.Entity.sec_users == null) ||
                        sessions.Entity.sec_users.tbl_accounts.is_deleted == true)
                    {

                        Auth = false;
                    }
                }
            }

            if (!Auth)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<object>(ResultType.fail, USER_ID, "API_ERROR_FORBIDDEN"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });
            }


        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Trace.WriteLine(string.Format("Action Method {0} executed at {1}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), "Web API Logs");
        }

        
        
    }

    
   
   
}
