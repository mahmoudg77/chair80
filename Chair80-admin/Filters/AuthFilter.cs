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
    public class AuthFilter : ActionFilterAttribute
    {
        
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            Users user = new Users();

            bool Auth = true;
            if (!actionContext.Request.Headers.Contains("AUTH_KEY"))
            {
                Auth = false;
                //return;
            }
            else
            {
            var USER_ID = actionContext.Request.Headers.GetValues("AUTH_KEY");
                string key = USER_ID.First();
                Guid token;
                if (!Guid.TryParse(key, out token))
                {
                    Auth = false;
                }
                else
                {

                    Sessions sessions = new Sessions(Guid.Parse(key));
                    user = new Users(sessions.Entity.user_id);
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
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<int>(ResultType.fail, "API_ERROR_FORBIDDEN"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });

            }

            string actionName = actionContext.ActionDescriptor.ActionName;
           
            string controllername = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;



            if (!user.Allow(controllername, actionName))
            {

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult<int>(ResultType.fail, "API_ERROR_UNAUTHORIZED"))),//Json("{'type':0,'message':'result.error.E403'}"),
                    ReasonPhrase = "Critical Exception",

                });

            }

            try
            {
              
            }
            catch (Exception)
            {


            }

        
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

        }





    }

}